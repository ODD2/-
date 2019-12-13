using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using ZoneDepict;
using ZoneDepict.Audio;

public class RandomItemContainer : ItemContainerBase
{
    //prefab 塞入
    public string[] DropPrefabs;

    //隨機道具的index
    private int randomNum;

    public new void Start()
    {
        base.Start();
        Durability = 10.0f;
        //隨機產生道具
        randomNum = Random.Range(0, DropPrefabs.Length);
        Debug.LogFormat("Random type: {0}", randomNum);
    }

    public override void Broken()
    {
        base.Broken();
        //產生道具
        if (photonView.IsMine)
        {
            //Debug.LogFormat("Container broken! Instantiate obj in {0}", transform.position);
            photonView.RPC("PerformBroken", RpcTarget.All);
            PhotonNetwork.InstantiateSceneObject(ZDAssetTable.GetPath(DropPrefabs[randomNum]),
                                                 transform.position,
                                                 Quaternion.identity);
            if (!animator)
            {
                //如果沒有Animator就自行Destroy不然就由Animator來觸發。
                Destroy();
            }
        }
        
    }

    #region RPCs
    [PunRPC]
    void PerformBroken()
    {
        if (BrokenEffect)Instantiate(BrokenEffect, transform.position, Quaternion.identity);
        if (BrokenAudio) ZDAudioSource.PlayAtPoint(BrokenAudio, transform.position, 1.0f);
        if (animator)animator.SetTrigger("Broken");
    }
    #endregion
}
