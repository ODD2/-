using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Photon.Pun;
using Photon;
using Photon.Realtime;
using ZoneDepict.Rule;
using ZoneDepict.Map;



namespace ZoneDepict
{
    [Serializable]
    public class ObjectConfig
    {
        public EObjectType ObjectType;
        public Vector2Int[] Terrain;
    }

    public class ZDObject : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        protected EActorType ActorType;
        private float ActorTypeDepth = 0.0f;
        public ObjectConfig[] Configs ;
        public Dictionary<Vector2Int, HashSet<EObjectType>> Regions;

        protected void Start()
        {
            InitializeTerrain();
            ZDMap.Register(this);
            if (!IsRegistered())
            {
                Debug.Log("Error! Cannot Register ZDObject, Object Out of Bound!");
            }

            //Cache ActorTypeDepth
            ActorTypeDepth = ZDGameRule.ActorDepth(ActorType);

            //Forced Update to set object depth with type depth;
            SetActorDepth();
        }

        protected void Update()
        {
            if(transform.hasChanged)
            {
                if (ZDMap.UpdateLocation(this))
                {
                    //Update Z axis to correct the in block layers.
                    SetActorDepth();

                    //
                    if (ZDMap.HitAtObject(this, EObjectType.Shelter) != null) Debug.Log("ENTER SHELTER");
                }
                transform.hasChanged = false;
            }
        }

        protected void OnDestroy()
        {
            ZDMap.UnRegister(this);
        }

        public bool IsRegistered()
        {
            return ZDMap.IsRegistered(this);
        }

        protected void SetActorDepth()
        {
            Vector3 NewPos = transform.position;
            NewPos.z = ZDGameRule.WorldDepth(NewPos.y) + ActorTypeDepth;
            transform.position = NewPos;
        }

        private void InitializeTerrain()
        {
           //If Config is valid, setup region according to it.
            if (Configs != null && Configs.Length != 0)
            {
                Regions = new Dictionary<Vector2Int, HashSet<EObjectType>>();
                //Create Dictionary List
                foreach (var config in  Configs)
                {
                    if (IsPreservedType(config.ObjectType)) continue;
                    else if (config.Terrain == null || config.Terrain.Length == 0)config.Terrain = new Vector2Int[] { new Vector2Int(0, 0) };
                    foreach (var vect in config.Terrain)
                    {
                        if (!Regions.ContainsKey(vect)) Regions.Add(vect, new HashSet<EObjectType>());
                        Regions[vect].Add(config.ObjectType);
                    }
                }
            }
            //If this object has no regions.
            if(Regions == null || Regions.Count == 0)
            {
                //Default region
                Regions = new Dictionary<Vector2Int, HashSet<EObjectType>>
                {
                    {
                        new Vector2Int(0, 0) ,
                        new HashSet<EObjectType>{EObjectType.Transient }
                    }
                };
            }
            //Add Preserved Types.
            AddPreserveToTransient();
            return;
        }

        private bool IsPreservedType(EObjectType Type)
        {
            switch (Type)
            {
                case EObjectType.ACollect:
                    break;
                case EObjectType.ADamage:
                    break;
                case EObjectType.Character:
                    break;
                default:
                    return false;
            }
            return true;
        }

        private void AddPreserveToTransient()
        {
            HashSet<EObjectType> eObjectTypes = new HashSet<EObjectType>();
            if (this is Character) eObjectTypes.Add(EObjectType.Character);
            if(this is IACollectObject) eObjectTypes.Add(EObjectType.ACollect);
            if (this is IADamageObject) eObjectTypes.Add(EObjectType.ADamage);

            foreach(var region in Regions)
            {
                //Obstalce and Transient Parts Forms The Total Body Of An ZDObject.
                if(region.Value.Contains(EObjectType.Obstacle) ||
                    region.Value.Contains(EObjectType.Transient))
                    region.Value.UnionWith(eObjectTypes);
            }
        }
    }
}

