using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReusableItem : ItemBase
{
    //道具CD
    protected  int CD;

    //還能用幾次
    protected  int itemCD;

    //下次可使用時間
    protected float NextUseTime;

    public void Start()
    {
        NextUseTime = 0.0f;

    }
    public override bool IsGarbage()
    {
        if (itemCD <= 0)
        {
            return true;
        }
        return false;
    }

    public override bool IsUsable()
    {
        if(Time.time>NextUseTime && itemCD>0)
        {
            return true;
        }
        return false;
    }

    public override void ItemEffect(Character _player)
    {   

    }

    public override void Use(Character _player)
    {
        if (itemCD>0)
        {
            itemCD--;
            NextUseTime = Time.time + CD;
            ItemEffect(_player);
        }     
    }
}
