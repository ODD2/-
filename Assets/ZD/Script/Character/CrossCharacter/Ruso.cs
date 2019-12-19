using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ZoneDepict;
using ZoneDepict.Rule;
using ZoneDepict.Map;




public class Ruso : CrossTrackCharacter
{       
    #region UNITY
    protected new void Start()
    {
        base.Start();
        AttackDamage = new float[] { 10, 20, 15, 30 };
        SkillMana = new float[]{ 5, 20, 30, 60 };
        MaxSkillCD = new float[] { 0.25f, 3f, 6f, 10 };
    }

    protected new void Update()
    {
        base.Update();
        if (Input.GetKeyDown(KeyCode.K))
        {
            Hurt(100);
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
                AllHitObject.Add(ZDMap.HitAt(ZDGameRule.RotateVector2(new Vector2(1, 0), AttackRad), this,EObjectType.ADamage));
                break;
        }
        ApplyDamage(AllHitObject, EAttackType.N);
    }

    public override void AttackEventA(int Phase)
    {
        List<List<ZDObject>> AllHitObject = new List<List<ZDObject>>();
        // Really do attack
        switch (Phase)
        {
            case 0:
                AllHitObject.Add(ZDMap.HitAt(ZDGameRule.RotateVector2(new Vector2(1, 0), AttackRad), this, EObjectType.ADamage));
                break;
            case 1:
                AllHitObject.Add(ZDMap.HitAt(ZDGameRule.RotateVector2(new Vector2(2, 0), AttackRad), this, EObjectType.ADamage));
                AllHitObject.Add(ZDMap.HitAt(ZDGameRule.RotateVector2(new Vector2(2, 1), AttackRad), this, EObjectType.ADamage));
                AllHitObject.Add(ZDMap.HitAt(ZDGameRule.RotateVector2(new Vector2(2, -1), AttackRad), this, EObjectType.ADamage));
                break;
        }
        ApplyDamage(AllHitObject, EAttackType.A);
    }

    public override void AttackEventB(int Phase)
    {
        List<List<ZDObject>> AllHitObject = new List<List<ZDObject>>();
        // Really do attack
        switch (Phase)
        {
            case 0:
                AllHitObject.Add(ZDMap.HitAt(ZDGameRule.RotateVector2(new Vector2(0, 1), AttackRad), this, EObjectType.ADamage));
                AllHitObject.Add(ZDMap.HitAt(ZDGameRule.RotateVector2(new Vector2(1, 1), AttackRad), this, EObjectType.ADamage));
                AllHitObject.Add(ZDMap.HitAt(ZDGameRule.RotateVector2(new Vector2(1, 0), AttackRad), this, EObjectType.ADamage));
                AllHitObject.Add(ZDMap.HitAt(ZDGameRule.RotateVector2(new Vector2(1, -1), AttackRad), this, EObjectType.ADamage));
                AllHitObject.Add(ZDMap.HitAt(ZDGameRule.RotateVector2(new Vector2(0, -1), AttackRad), this, EObjectType.ADamage));
                AllHitObject.Add(ZDMap.HitAt(ZDGameRule.RotateVector2(new Vector2(-1, -1), AttackRad), this, EObjectType.ADamage));
                AllHitObject.Add(ZDMap.HitAt(ZDGameRule.RotateVector2(new Vector2(-1, 0), AttackRad), this, EObjectType.ADamage));
                AllHitObject.Add(ZDMap.HitAt(ZDGameRule.RotateVector2(new Vector2(-1, 1), AttackRad), this, EObjectType.ADamage));
                break;
        }
        ApplyDamage(AllHitObject, EAttackType.B);
    }

    public override void AttackEventR(int Phase)
    {
    }
    #endregion
}
