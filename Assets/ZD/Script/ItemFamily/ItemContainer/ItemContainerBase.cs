using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZoneDepict;
using Photon.Pun;
using ZoneDepict.Rule;

public abstract class ItemContainerBase : ZDRegisterObject, IADamageObject, IPunObservable
{
    #region Private Field
    public float Durability;
    #endregion

    #region Unity
    protected new void Start()
    {
        base.Start();
        Durability = 10;
        Vector3 NewPos = transform.position;
        NewPos.z = (int)TypeDepth.ItemContainer;
        transform.position = NewPos;
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
    public abstract void Hurt(float damaged);
    public abstract void Broken();
    #endregion

}