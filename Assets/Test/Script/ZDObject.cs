using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon;
using Photon.Realtime;

namespace ZoneDepict
{
    public class ZDObject : MonoBehaviourPunCallbacks
    {
        protected void Start()
        {
            ZDMap.Register(this);
            if (!Registered)
            {
                Debug.Log("Error! Cannot Register ZDObject, Object Out of Bound!");
            }
        }

        protected void Update()
        {
            if (Registered)
            {
                ZDMap.UpdateLocation(this);
            }
        }

        protected void OnDestroy()
        {
            ZDMap.UnRegister(this);
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.Log("asdfasdfasdf");
        }


        public bool Registered { get; set; } = false;
        public bool ValidInMap { get; set; } = false;
        public int MapLocX { get; set; } = -1;
        public int MapLocY { get; set; } = -1;
    }
}

