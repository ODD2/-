using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hpRecover : DisposableItem
{
    //初始化道具
    public hpRecover()
    {
        ItemCD = 1.0f;
        Amount = Random.Range(1, 2);
    }

    public override void ItemEffect(Character Caller)
    {
        Caller.SetHP(Caller.GetHP() - 10);
    }
}

