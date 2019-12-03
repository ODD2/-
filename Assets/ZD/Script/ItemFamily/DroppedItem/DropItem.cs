using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZoneDepict;
using Photon.Pun;
public class DropItem : ZDObject, IPunObservable, IACollectObject
{
    bool collected = false;

    public List<ItemBase> contains;

    //撿起特效
    public UnityEngine.GameObject pickedFX;

    // Start is called before the first frame update
    protected new void Start()
    {
        base.Start();
        contains = new List<ItemBase>()
        {
            new hpRecover(), new mpRecover()
        };
    }

    // Update is called once per frame
    protected new void Update()
    {
        base.Update();
    }

    public void Collect(Character Collecter)
    {
       
        if (collected) return;
        collected = true;

       

        Debug.Log("Item Collected By: " + Collecter.name );
        foreach (ItemBase i in contains)
        {
            Collecter.GetItem(i);
        }
        photonView.RPC("OnCollected",RpcTarget.AllViaServer);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

    }

    [PunRPC]
    private void OnCollected(PhotonMessageInfo info)
    {
        Vector3 FXpos = new Vector3(transform.position.x, transform.position.y, transform.position.z - 1);
        PhotonNetwork.InstantiateSceneObject(pickedFX.name, FXpos, Quaternion.identity);
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(photonView);
        }
    }
}
