using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mpRecover:ReusableItem
{

    //初始化道具
    public override void Initial()
    {
        CD = 10;
        itemCD = 5;
        //CD 10 秒 能重複用5次  
    }
    public override void ItemEffect(Character _player)
    {
        // 讓玩家數值提升...
        Debug.LogFormat("mpRecover:ItemEffect , ItemCD: {0} , Nextusetime {1}",itemCD,NextUseTime);
    }

    public override void Use(Character _player)
    {
        base.Use(_player);
    }
}
