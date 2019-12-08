using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;

public class RandomItemContainer : ItemContainerBase
{
    //prefab 塞入
    public UnityEngine.GameObject[] DropPrefabs;

    //隨機道具的index
    private int randomNum;

    //破壞特效
    public UnityEngine.GameObject brokenFX;

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
            Vector3 FXpos = new Vector3(transform.position.x, transform.position.y, transform.position.z-1);
            
            //Debug.LogFormat("Container broken! Instantiate obj in {0}", transform.position);
             PhotonNetwork.InstantiateSceneObject(DropPrefabs[randomNum].name, transform.position, Quaternion.identity);

            PhotonNetwork.InstantiateSceneObject(brokenFX.name, FXpos, Quaternion.identity);
            PhotonNetwork.Destroy(photonView);
        }
    }

    public override void Hurt(float damaged)
    {
        if (photonView.IsMine)
        {
            Durability -= damaged;
            if (Durability < 1.0)
            {
                //Debug.Log("RandomItemContainer Broken!");
                Broken();
            }
        }
    }
}
