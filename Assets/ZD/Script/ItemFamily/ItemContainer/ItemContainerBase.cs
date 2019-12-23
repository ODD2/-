using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZoneDepict;
using Photon.Pun;
using ZoneDepict.Rule;
using ZoneDepict.Map;

public abstract class ItemContainerBase : ZDRegisterObject, IADamageObject, IPunObservable
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
        //Setup Component
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        Durability = 20;

        //Setup ZDObjectt Unit World  Scale
        Vector3 NewScale = transform.localScale;
        NewScale.x *= ZDGameRule.UNIT_IN_WORLD;
        NewScale.y *= ZDGameRule.UNIT_IN_WORLD;
        transform.localScale = NewScale;

        //Setup Depth
        ActorType = EActorType.ItemContainer;
        base.Start();
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
            if (Durability < float.Epsilon)
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