using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Photon.Pun;
using Photon;
using Photon.Realtime;

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
        }

        protected void Update()
        {
            ZDMap.UpdateLocation(this);
        }

        protected void OnDestroy()
        {
            ZDMap.UnRegister(this);
        }

        public bool IsRegistered()
        {
            return ZDMap.IsRegistered(this);
        }
    }
}

