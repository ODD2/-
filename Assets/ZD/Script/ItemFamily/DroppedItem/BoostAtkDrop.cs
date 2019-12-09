using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZoneDepict;
using Photon.Pun;
using System;

public class BoostAtkDrop : SingleDropItem
{
    protected new void Start()
    {
        base.Start();
        AcquireItem = new BoostAttack();
    }
}
