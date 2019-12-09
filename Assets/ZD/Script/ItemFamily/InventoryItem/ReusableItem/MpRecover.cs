using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ZoneDepict.Rule;
using ZoneDepict;
public class MpRecover:ReusableItem
{
    public MpRecover()
    {
        id = 1;
        ItemCD = 1.5f;
        MaxAmount = -1;
        Amount = MaxAmount;
    }
    public override void ItemEffect(Character Caller)
    {
        Caller.SetMP(Caller.GetMP() - 10);
        SendEffectEvent("MpRecoveredEffect",Caller.transform.position,Quaternion.identity);
    }
}
