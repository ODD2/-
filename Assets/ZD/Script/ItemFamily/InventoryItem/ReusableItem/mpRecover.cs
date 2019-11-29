using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mpRecover:ReusableItem
{
    public mpRecover()
    {
        ItemCD = 1.5f;
        MaxAmount = -1;
        Amount = MaxAmount;
    }
    public override void ItemEffect(Character Caller)
    {
        Caller.SetMP(Caller.GetMP() - 10);
    }
}
