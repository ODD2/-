using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ZoneDepict;
public class BoostAttack : DisposableItem
{
    //初始化道具
    public BoostAttack()
    {
        id = 2;
        ItemCD = 3.0f;
        Amount = Random.Range(1, 2);
    }

    public override void ItemEffect(Character Caller)
    {
        SendEffectEvent("AttackBoostEffect", Caller.transform.position, Quaternion.identity);
    }
   
}

