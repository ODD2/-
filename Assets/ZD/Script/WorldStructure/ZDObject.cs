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
    public struct VectorI2
    {
        public VectorI2(int x , int y)
        {
            this.x = x;
            this.y = y;
        }

        public int x;
        public int y;
    }

    public class ZDObject : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        protected EActorType ActorType;
        private float ActorTypeDepth = 0.0f;
        public EObjectType[] ObjectTypes;
        public VectorI2[] Terrain;

        protected void Start()
        {
            //If Terrain Unspecified, Set Default Terrain Only To Origin.
            if (Terrain==null || Terrain.Length == 0) Terrain = new VectorI2[] { new VectorI2(0,0) };
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
            ZDMap.UpdateLocation(this);
            //Update Z axis to correct the in block layers.
            if (transform.hasChanged)
            {
                SetActorDepth();
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
    }
}

