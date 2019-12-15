using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZoneDepict;
using ZoneDepict.Rule;
using Photon.Pun;
using System;

public class DropItem : ZDRegisterObject, IACollectObject
{
    protected bool collected;
    public GameObject PickupEffect;

    // Start is called before the first frame update
    protected new void Start()
    {
        //Setup Object Depth
        ActorType = EActorType.DroppedItem;
        base.Start();
    }

    protected new void OnDestroy()
    {
        base.OnDestroy();
    }

    public virtual void Collect(Character Collecter)
    {

    }

    [PunRPC]
    protected virtual void DoCollected()
    {
        Vector3 pos = new Vector3(transform.position.x, transform.position.y - 0.5f, 0);
        Instantiate(PickupEffect, pos, Quaternion.Euler(-90, 0, 0));
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(photonView);
        }
    }
}
