using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using ZoneDepict;

public class Character : ZDObject, IPunObservable
{
    #region Private Field
    //private PhotonView photonView;
    #endregion

    #region UNITY
    private void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        Vector3 pos = GameSetting.WorldToUnit(transform.position);
        ZDMap.Register(new Vector2(pos.x,pos.y),this);
    }

    // Update is called once per frame
    void Update()
    {

       
    }

    private void FixedUpdate()
    {
        //var Destination = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //Debug.Log("Mouse Pos:" + Destination.ToString());
    }

    private void OnMouseDown()
    {
        
    }

    private void OnMouseDrag()
    {
        
    }

    private void OnMouseUp()
    {
        //只有擁有者可以操控這隻角色
        if (photonView && photonView.IsMine)
        {
            var Destination = GameSetting.WorldToUnit(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            photonView.RPC("SprintTo", RpcTarget.AllViaServer,Destination);
        }        
    }

    #endregion

    #region Observable
    public void   OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
        }
        else if (stream.IsReading)
        {
            transform.position =  (Vector3)stream.ReceiveNext();
        }
    }
    #endregion

    #region PUN Callback & RPC
    [PunRPC]
    public void TEST()
    {
        Debug.Log("Received Testa Rpc.");
    }

    [PunRPC]
    public void SprintTo(Vector3  UnitLoc,PhotonMessageInfo info)
    {
        //所有人都可以更新他的Map
        if (ZDMap.UpdateLocation(UnitLoc, this))
        {
            //只有擁有者可以更新角色的位置
            if(photonView && photonView.IsMine)
            {
                transform.position = GameSetting.UnitToWorld((int)UnitLoc.x, (int)UnitLoc.y, 0);
            }
        }
    }
    #endregion
}
