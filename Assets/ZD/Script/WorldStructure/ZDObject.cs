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

