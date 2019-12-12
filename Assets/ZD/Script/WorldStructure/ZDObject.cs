using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Photon.Pun;
using Photon;
using Photon.Realtime;
using ZoneDepict.Rule;
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

        protected float ObjectTypeDepth = 0.0f;
        public ETypeZDO[] Types;
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
            //Forced Update to set object depth with type depth;
            SetObjectDepthWithTypeDepth();
        }

        protected void Update()
        {
            ZDMap.UpdateLocation(this);
            //Update Z axis to correct the in block layers.
            if (transform.hasChanged)
            {
                SetObjectDepthWithTypeDepth();
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

        protected void SetObjectDepthWithTypeDepth()
        {
            Vector3 NewPos = transform.position;
            NewPos.z = transform.position.y + ObjectTypeDepth;
            transform.position = NewPos;
        }
    }
}

