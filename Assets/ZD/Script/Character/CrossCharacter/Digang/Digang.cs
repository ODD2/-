using System.Collections.Generic;
using System;
using System.Collections;
using UnityEngine;
using ZoneDepict;
using ZoneDepict.Rule;
using ZoneDepict.Map;

public class Digang : CrossTrackCharacter
{
    private bool Ulting = false;
    #region UNITY
    protected new void Start()
    {
        base.Start();
        AttackDamage = new float[] { 10, 25, 35, 10 };
    }

    protected new void Update()
    {
        base.Update();
        if (Input.GetKeyDown(KeyCode.Z))
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
                AllHitObject.Add(ZDMap.HitAt(ZDGameRule.RotateVector2(new Vector2(1, 0), AttackRad), this, EObjectType.ADamage));
                AllHitObject.Add(ZDMap.HitAt(ZDGameRule.RotateVector2(new Vector2(0, 1), AttackRad), this, EObjectType.ADamage));
                AllHitObject.Add(ZDMap.HitAt(ZDGameRule.RotateVector2(new Vector2(-1, 0), AttackRad), this, EObjectType.ADamage));
                AllHitObject.Add(ZDMap.HitAt(ZDGameRule.RotateVector2(new Vector2(0, -1), AttackRad), this, EObjectType.ADamage));
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
                AllHitObject.Add(ZDMap.HitAt(ZDGameRule.RotateVector2(new Vector2(1, 1), AttackRad), this, EObjectType.ADamage));
                AllHitObject.Add(ZDMap.HitAt(ZDGameRule.RotateVector2(new Vector2(1, -1), AttackRad), this, EObjectType.ADamage));
                AllHitObject.Add(ZDMap.HitAt(ZDGameRule.RotateVector2(new Vector2(-1, 1), AttackRad), this, EObjectType.ADamage));
                AllHitObject.Add(ZDMap.HitAt(ZDGameRule.RotateVector2(new Vector2(-1, -1), AttackRad), this, EObjectType.ADamage));
                break;
            case 1:
                AllHitObject.Add(ZDMap.HitAt(ZDGameRule.RotateVector2(new Vector2(1, 1), AttackRad), this, EObjectType.ADamage));
                AllHitObject.Add(ZDMap.HitAt(ZDGameRule.RotateVector2(new Vector2(1, -1), AttackRad), this, EObjectType.ADamage));
                AllHitObject.Add(ZDMap.HitAt(ZDGameRule.RotateVector2(new Vector2(-1, 1), AttackRad), this, EObjectType.ADamage));
                AllHitObject.Add(ZDMap.HitAt(ZDGameRule.RotateVector2(new Vector2(-1, -1), AttackRad), this, EObjectType.ADamage));
                AllHitObject.Add(ZDMap.HitAt(ZDGameRule.RotateVector2(new Vector2(2, 2), AttackRad), this, EObjectType.ADamage));
                AllHitObject.Add(ZDMap.HitAt(ZDGameRule.RotateVector2(new Vector2(2, -2), AttackRad), this, EObjectType.ADamage));
                AllHitObject.Add(ZDMap.HitAt(ZDGameRule.RotateVector2(new Vector2(-2, 2), AttackRad), this, EObjectType.ADamage));
                AllHitObject.Add(ZDMap.HitAt(ZDGameRule.RotateVector2(new Vector2(-2, -2), AttackRad), this, EObjectType.ADamage));
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
                AllHitObject.Add(ZDMap.HitAt(new Vector2(1, 0), this, EObjectType.ADamage));
                AllHitObject.Add(ZDMap.HitAt(new Vector2(2, 0), this, EObjectType.ADamage));
                AllHitObject.Add(ZDMap.HitAt(new Vector2(-1, 0), this, EObjectType.ADamage));
                AllHitObject.Add(ZDMap.HitAt(new Vector2(-2, 0), this, EObjectType.ADamage));
                AllHitObject.Add(ZDMap.HitAt(new Vector2(0, 1), this, EObjectType.ADamage));
                AllHitObject.Add(ZDMap.HitAt(new Vector2(0, -1), this, EObjectType.ADamage));
                break;
            case 1:
                AllHitObject.Add(ZDMap.HitAt(new Vector2(0, 2), this, EObjectType.ADamage));
                AllHitObject.Add(ZDMap.HitAt(new Vector2(0, -2), this, EObjectType.ADamage));
                AllHitObject.Add(ZDMap.HitAt(new Vector2(3, 0), this, EObjectType.ADamage));
                AllHitObject.Add(ZDMap.HitAt(new Vector2(3, 1), this, EObjectType.ADamage));
                AllHitObject.Add(ZDMap.HitAt(new Vector2(3, -1), this, EObjectType.ADamage));
                AllHitObject.Add(ZDMap.HitAt(new Vector2(-3, 0), this, EObjectType.ADamage));
                AllHitObject.Add(ZDMap.HitAt(new Vector2(-3, 1), this, EObjectType.ADamage));
                AllHitObject.Add(ZDMap.HitAt(new Vector2(-3, -1), this, EObjectType.ADamage));
                break;
        }
        ApplyDamage(AllHitObject, EAttackType.B);
    }

    public override void AttackEventR(int Phase)
    {
        switch (Phase)
        {
            case 0:
                StartCoroutine(AttackingR());
                break;
            case 1:
                Debug.Log("Hi");
                Ulting = false;
                break;
        }
        
    }

    protected override bool IsInputAttackValid(Vector2 AttackDirection, EAttackType Type)
    {
        return base.IsInputAttackValid(AttackDirection, Type) && !Ulting;
    }

    protected override bool IsMotionInterruptValid()
    {
        return base.IsMotionInterruptValid() && !Ulting;
    }
    #endregion
    IEnumerator AttackingR()
    {
        if (!Ulting)
        {
            Ulting = true;
            while (Ulting &&
                    (animator.GetCurrentAnimatorStateInfo(0).IsName("AttackR_Loop")||
                     animator.GetCurrentAnimatorStateInfo(0).IsName("AttackR_Start")))
            {
                yield return new WaitForSeconds(0.33f);
                List<List<ZDObject>> AllHitObject = new List<List<ZDObject>>();
                AllHitObject.Add(ZDMap.HitAt(new Vector2(0, 1), this, EObjectType.ADamage));
                AllHitObject.Add(ZDMap.HitAt(new Vector2(0, -1), this, EObjectType.ADamage));
                AllHitObject.Add(ZDMap.HitAt(new Vector2(1, 0), this, EObjectType.ADamage));
                AllHitObject.Add(ZDMap.HitAt(new Vector2(-1, 0), this, EObjectType.ADamage));
                AllHitObject.Add(ZDMap.HitAt(new Vector2(1, 1), this, EObjectType.ADamage));
                AllHitObject.Add(ZDMap.HitAt(new Vector2(1, -1), this, EObjectType.ADamage));
                AllHitObject.Add(ZDMap.HitAt(new Vector2(-1, 1), this, EObjectType.ADamage));
                AllHitObject.Add(ZDMap.HitAt(new Vector2(-1, -1), this, EObjectType.ADamage));
                ApplyDamage(AllHitObject, EAttackType.R);
            }
            Ulting = false;
        }
    }
}
