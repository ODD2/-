using System;
using UnityEngine;
using ZoneDepict;
using Photon.Pun;
public class SingleDropItem: FloatDropItem
{
    //撿起特效
    public ItemBase AcquireItem;

    public override void Collect(Character Collecter)
    {
        if (collected) return;
        collected = true;
        photonView.RPC("DoCollected", RpcTarget.All);
        Collecter.GetItem(AcquireItem);
    }
}
