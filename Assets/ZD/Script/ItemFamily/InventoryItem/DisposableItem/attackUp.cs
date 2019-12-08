using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class attackUp : DisposableItem
{


    //初始化道具
    public attackUp()
    {
        id = 2;
        ItemCD = 3.0f;
        Amount = Random.Range(1, 2);
    }

    public override void ItemEffect(Character Caller)
    {
        Vector3 FXpos = new Vector3(Caller.transform.position.x, Caller.transform.position.y-1.2f, Caller.transform.position.z - 1);
        PhotonNetwork.Instantiate("attackUp_effect", FXpos, Quaternion.Euler(-90, 0, 0));
        
    }
}

