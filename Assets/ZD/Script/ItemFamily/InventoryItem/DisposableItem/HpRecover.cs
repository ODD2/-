﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ZoneDepict;
using UnityEngine.Events;
public class HpRecover : DisposableItem
{


    //初始化道具
    public HpRecover()
    {
        id = 0;
        ItemCD = 1.0f;
        Amount = Random.Range(1, 3);
    }

    public override void ItemEffect(Character Caller)
    {
        Caller.SetHP(Caller.GetHP() + 20);
        SendEffectEvent("HpRecoveredEffect", Caller.transform.position, Quaternion.identity);

      // Caller.basicValues.SetLastValue("SkillBuff", 1.2f, 5.0f);
    }


}

