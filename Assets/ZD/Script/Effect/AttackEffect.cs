using System;
using System.Collections.Generic;
using UnityEngine;
using ZoneDepict;
using ZoneDepict.Rule;
using ZoneDepict.Map;

public class AttackEffect:EffectBase
{
    public float[] AttackDamages = { 0 };
    [SerializeField]
    Vector2Int[][] HitOffsetsConfig;

    void AttackEvent(int eventNum){
        //Initial Values
        Vector2Int[] HitOffsets = {new Vector2Int(0,0) };
        float HitDamage = 0;
        Vector2 HitOrigin = ZDGameRule.WorldToUnit(transform.position);
        
        if(eventNum < AttackDamages.Length)
        {
            HitDamage = AttackDamages[eventNum];
        }

        if(HitOffsetsConfig != null && eventNum < HitOffsetsConfig.Length)
        {
            HitOffsets = HitOffsetsConfig[eventNum];
        }

        List<ZDObject> HitList = new List<ZDObject>();
        //Get Hitten Objects
        foreach(var offset in HitOffsets)
        {
            Vector2 HitPos = offset + HitOrigin;
            List<ZDObject> HitResult =  ZDMap.HitAtUnit(HitPos, EObjectType.ADamage);
            if (HitResult != null) HitList.AddRange(HitResult);
        }
        //Apply Damages
        if(!HitDamage.Equals(0))
        {
            foreach (var obj in HitList)
            {
                ((IADamageObject)obj).Hurt(HitDamage);
            }
        }
    }
}
