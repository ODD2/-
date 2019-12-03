using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZoneDepict;
using Photon.Pun;

public abstract class ItemContainerBase : ZDStaticObstacle, IADamageObject, IPunObservable
{
    #region Private Field
    public float Durability;
    #endregion

    #region Unity
    protected new void Start()
    {
        base.Start();
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
    public abstract void Hurt(float damaged);
    public abstract void Broken();
    #endregion

}