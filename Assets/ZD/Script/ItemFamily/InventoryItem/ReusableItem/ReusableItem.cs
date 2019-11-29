using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReusableItem : ItemBase
{
    //Available Amounts
    protected int Amount;

    protected int MaxAmount;

    public override bool IsGarbage()
    {
        return false;
    }

    public override bool IsUsable()
    {
        if (Time.time > NextValidTime && (MaxAmount == -1 || Amount > 0))
        {
            return true;
        }
        return false;
    }

    public override void ItemEffect(Character Caller)
    {
       
    }

    public override void Use(Character Caller)
    {
        if (IsUsable())
        {
            NextValidTime = Time.time + ItemCD;
            Amount -= 1;
            ItemEffect(Caller);
        }
        Debug.LogFormat("(Item Log)Amount: {0}, NextValidTime: {1}, Time:{2}", Amount, NextValidTime, Time.time);
    }

    public void Refill()
    {
        Amount = MaxAmount;
    }
}
