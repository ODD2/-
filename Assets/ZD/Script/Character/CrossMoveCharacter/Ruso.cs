using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZoneDepict.Rule;
using ZoneDepict;
using Photon.Pun; 
public class Ruso : CrossMoveCharacter
{

    private float[] AttackDamage = { 200, 10, 15, 20 };
    
    #region UNITY
    protected new void Start()
    {
        base.Start();
    }

    protected new void Update()
    {
        base.Update();        
    }
    #endregion

    #region Charater Override
    protected override void ApplyDamage(List<List<ZDObject>> Hits,AttackType Type)
    {
        if (Hits != null)
        {
            foreach (var i in Hits)
            {
                if (i == null) continue;
                foreach (var Obj in i)
                {
                    if (Obj is IADamageObject HitObj)
                    {
                        if(HitObj is Character HitChar && HitChar.TeamID==this.TeamID)
                        {
                            continue;
                        }
                        HitObj.Hurt(AttackDamage[(int)Type]);
                    }
                }
            }
        }
    }
    #endregion

    #region  CrossMoveCharacter Override
    public override void AttackEventN(int Phase)
    {
        List<List<ZDObject>> AllHitObject = new List<List<ZDObject>>();
        // Really do attack
        switch (Phase)
        {
            case 0:
                AllHitObject.Add(ZDMap.HitAt(ZDGameRule.RotateVector2(new Vector2(1, 0), AttackRad), this));
                AllHitObject.Add(ZDMap.HitAt(ZDGameRule.RotateVector2(new Vector2(0, 1), AttackRad), this));
                AllHitObject.Add(ZDMap.HitAt(ZDGameRule.RotateVector2(new Vector2(0,-1), AttackRad), this));
                ApplyDamage(AllHitObject,AttackType.N);
                break;
            case 1:
                break;
        }
        
    }

    public override void AttackEventA(int Phase)
    {
    }

    public override void AttackEventB(int Phase)
    {
    }

    public override void AttackEventR(int Phase)
    {
    }
    #endregion
}
