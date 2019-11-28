using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mpRecover:ReusableItem
{

    public override void ItemEffect(Character _player)
    {
        // 讓玩家數值提升...
        Debug.Log("Player use mpRecover");
    }
    public new void Start()
    {
        CD = 10;
        itemCD = 5;
        //CD 10 秒 能重複用5次    
    }
    public override void Use(Character _player)
    {
        
    }
}
