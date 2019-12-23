using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisposableItem : ItemBase
{
    public override bool canReuse()
    {
        return false;
    }

    public override bool IsGarbage()
    {
        if (!(Amount > 0))
        {
            return true;
        }
        return false;
    }
   
    public override bool IsUsable()
    {
        if (Amount > 0 &&   Time.time > NextValidTime) return true;
        return false;
    }

    public override void ItemEffect(Character Caller)
    {

    }

    public override float ItemState()
    {     
        return 0.0f;
    }

    public override void Use(Character Caller)
    {
        if (IsUsable())
        {
            Amount -= 1;
            NextValidTime = Time.time + ItemCD;
            ItemEffect(Caller);
        }
        //Debug.LogFormat("(Item Log)Amount: {0}, NextValidTime: {1}, Time:{2}", Amount, NextValidTime, Time.time);
    }
}
