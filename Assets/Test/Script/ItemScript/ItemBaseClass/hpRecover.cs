using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hpRecover : DisposableItem
{
    //初始化道具
    public override void Initial()
    {
        itemAmount = Random.Range(1, 10);
    }
    public override void ItemEffect(Character _player)
    {
        // 讓玩家數值提升...
        Debug.LogFormat("hpEffect(),  remain {0}", itemAmount);
    }

    public override void Use(Character _player)
    {
        itemAmount = Random.Range(10, 20);
        base.Use(_player);     
    }
}

