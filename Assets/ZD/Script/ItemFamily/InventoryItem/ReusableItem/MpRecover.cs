using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;
using ZoneDepict.Rule;
using ZoneDepict;
using System;
public class MpRecover : ReusableItem
{
    
    public MpRecover()
    {
        id = 1;
        ItemCD = 10.0f;
        MaxAmount = -1;
        Amount = MaxAmount;
    }
    public override void ItemEffect(Character Caller)
    {
       Caller.SetMP(Caller.GetMP() + 30);
        SendEffectEvent("MpRecoveredEffect", Caller.transform.position, Quaternion.identity);              
    }


}
