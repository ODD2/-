using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mpRecover:ReusableItem
{
    public mpRecover()
    {
        id = 1;
        ItemCD = 1.5f;
        MaxAmount = -1;
        Amount = MaxAmount;
    }
    public override void ItemEffect(Character Caller)
    {
        Caller.SetMP(Caller.GetMP() - 10);
    }
}
