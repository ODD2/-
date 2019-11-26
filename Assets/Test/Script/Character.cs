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
        var Destination = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Debug.Log("Mouse Pos:" + Destination.ToString());
    }

    private void OnMouseDown()
    {
        
    }

    private void OnMouseDrag()
    {
        
    }

    private void OnMouseUp()
    {
        var Destination = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Destination.z = 0;
        Debug.Log("Mouse Up Position:" + Destination.ToString());
        Destination = GameSetting.WorldToUnit(Destination);
        Debug.Log("Mouse Up Unit:" + Destination.ToString());
        if (photonView)
        {
            photonView.RPC("TEST", RpcTarget.AllViaServer);
            photonView.RPC("SprintTo", RpcTarget.AllViaServer,(int)Destination.x,(int)Destination.y);
            Debug.Log("Called Rpc");
        }        
    }

    #endregion

    #region Observable
    public void   OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

    }
    #endregion

    #region PUN Callback & RPC
    [PunRPC]
    public void TEST()
    {
        Debug.Log("Received Testa Rpc.");
    }

    [PunRPC]
    public void SprintTo(int x,int y,PhotonMessageInfo info)
    {
        Debug.Log("Rpc Received.");
        if (ZDMap.UpdateLocation(new Vector2(x, y), this))
        {
            transform.position = GameSetting.UnitToWorld(x, y, 0);
        }
    }
    #endregion
}
