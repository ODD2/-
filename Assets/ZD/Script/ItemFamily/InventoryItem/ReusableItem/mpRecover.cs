using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class mpRecover:ReusableItem
{
    public mpRecover()
    {
        id = 1;
        ItemCD = 1.5f;
        MaxAmount = -1;
        Amount = MaxAmount;
    }
    public override void ItemEffect(Character Caller)
    {
        Vector3 FXpos = new Vector3(Caller.transform.position.x, Caller.transform.position.y, Caller.transform.position.z - 1);

        PhotonNetwork.Instantiate("mprecover_effect", FXpos,Quaternion.identity);
        Caller.SetMP(Caller.GetMP() - 10);
    }
}
