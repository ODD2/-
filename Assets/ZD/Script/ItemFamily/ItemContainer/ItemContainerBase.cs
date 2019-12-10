﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZoneDepict;
using Photon.Pun;
using ZoneDepict.Rule;

public abstract class ItemContainerBase : StationaryMapObject, IADamageObject, IPunObservable
{
    #region Field
    public float Durability;
    //[SerializeField]
    public AudioClip BrokenAudio;
    [SerializeField]
    protected GameObject BrokenEffect;
    protected SpriteRenderer spriteRenderer;
    protected Animator animator;
    #endregion

    #region Unity
    protected new void Start()
    {
        base.Start();
        //Setup Depth
        Vector3 NewPos = transform.position;
        NewPos.z = (int)TypeDepth.ItemContainer;
        transform.position = NewPos;
        //Setup Component
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        Durability = 10;
    }
    protected new void Update()
    {
        base.Update();
    }

    protected new void OnDestroy()
    {
        base.OnDestroy();
    }
    #endregion

    #region Photun View
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

    }
    #endregion

    #region Item Container Interface
    public virtual void Hurt(float damaged)
    {
        if (photonView.IsMine && Durability > float.Epsilon)
        {
            Durability -= damaged;
            if (Durability < 1.0)
            {
                Broken();
            }
        }
    }
    public virtual void Broken()
    {
        ZDMap.UnRegister(this);
    }
    public virtual void  Destroy()
    {
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
    #endregion

    #region Enumerator
    #endregion

}