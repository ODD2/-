using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using ZoneDepict;

public class RandomItemContainer : ItemContainerBase
{
    //prefab 塞入
    public string[] DropPrefabs;

    //隨機道具的index
    private int randomNum;

    //破壞特效
    public GameObject BrokenEffect;

    public new void Start()
    {
        base.Start();
        Durability = 10.0f;
        //隨機產生道具
        randomNum = Random.Range(0, DropPrefabs.Length);
        Debug.LogFormat("Random type: {0}", randomNum);
    }

    public new void Update()
    {
        base.Update();
    }

    public new void OnDestroy()
    {
        base.OnDestroy();
    }

    public override void Broken()
    {
        //產生道具
        if (photonView.IsMine)
        {
            //Debug.LogFormat("Container broken! Instantiate obj in {0}", transform.position);
            photonView.RPC("PerformBroken", RpcTarget.All);
            PhotonNetwork.InstantiateSceneObject(ZDAssetTable.GetPath(DropPrefabs[randomNum]),
                                                 transform.position,
                                                 Quaternion.identity);
            PhotonNetwork.Destroy(this.gameObject);
        }
    }

    public override void Hurt(float damaged)
    {
        if (photonView.IsMine)
        {
            Durability -= damaged;
            if (Durability < 1.0)
            {
                Broken();
            }
        }
    }

    #region RPCs
    [PunRPC]
    void PerformBroken()
    {
        Instantiate(BrokenEffect, transform.position, Quaternion.identity);
    }
    #endregion
}
