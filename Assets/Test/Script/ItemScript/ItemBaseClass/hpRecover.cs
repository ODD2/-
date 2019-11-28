using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hpRecover : ReusableItem
{
    public override void ItemEffect(Character _player)
    {
        // 讓玩家數值提升...
        Debug.Log("Player use hpRecover");
    }

}

