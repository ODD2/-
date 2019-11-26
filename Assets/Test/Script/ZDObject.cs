using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace ZoneDepict
{
    public class ZDObject : MonoBehaviourPun
    {
        public bool Registered { get; set; } = false;
        public int MapLocX { get; set; } = 0;
        public int MapLocY { get; set; } = 0;
    }
}

