using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
using ZoneDepict;
using ZoneDepict.Rule;
using ZoneDepict.Map;
using ZoneDepict.Audio;
using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Random = UnityEngine.Random;

public class Ruso : CrossTrackCharacter
{
    public GameObject RusoREffectSample;
    protected const int RusoAttackRRangeWidth = 4;
    protected const int RusoAttackRRangeHeight = 2;

    #region UNITY
    protected new void Start()
    {
        base.Start();
        AttackDamage = new float[] { 15, 25, 35, 30 };
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

    #region Character Override
    public override void InputAttack(Vector2 AttackDirection, EAttackType Type)
    {
        switch (Type)
        {
            case EAttackType.N:
                break;
            default:
                if (Soul == (int)EAttackType.A &&
                    !ZDGameRule.QuadrifyDirection(AttackDirection).y.Equals(0))
                    return;
                break;
        }
        base.InputAttack(AttackDirection, Type);
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
                AllHitObject.Add(ZDMap.HitAt(ZDGameRule.RotateVector2(new Vector2(-2, 2), AttackRad), this, EObjectType.ADamage));
                AllHitObject.Add(ZDMap.HitAt(ZDGameRule.RotateVector2(new Vector2(-1, 2), AttackRad), this, EObjectType.ADamage));
                AllHitObject.Add(ZDMap.HitAt(ZDGameRule.RotateVector2(new Vector2(0, 2), AttackRad), this, EObjectType.ADamage));
                AllHitObject.Add(ZDMap.HitAt(ZDGameRule.RotateVector2(new Vector2(1, 2), AttackRad), this, EObjectType.ADamage));
                AllHitObject.Add(ZDMap.HitAt(ZDGameRule.RotateVector2(new Vector2(2, 2), AttackRad), this, EObjectType.ADamage));

                AllHitObject.Add(ZDMap.HitAt(ZDGameRule.RotateVector2(new Vector2(-2, 1), AttackRad), this, EObjectType.ADamage));
                AllHitObject.Add(ZDMap.HitAt(ZDGameRule.RotateVector2(new Vector2(-1, 1), AttackRad), this, EObjectType.ADamage));
                AllHitObject.Add(ZDMap.HitAt(ZDGameRule.RotateVector2(new Vector2(0, 1), AttackRad), this, EObjectType.ADamage));
                AllHitObject.Add(ZDMap.HitAt(ZDGameRule.RotateVector2(new Vector2(1, 1), AttackRad), this, EObjectType.ADamage));
                AllHitObject.Add(ZDMap.HitAt(ZDGameRule.RotateVector2(new Vector2(2, 1), AttackRad), this, EObjectType.ADamage));

                AllHitObject.Add(ZDMap.HitAt(ZDGameRule.RotateVector2(new Vector2(-2, 0), AttackRad), this, EObjectType.ADamage));
                AllHitObject.Add(ZDMap.HitAt(ZDGameRule.RotateVector2(new Vector2(-1, 0), AttackRad), this, EObjectType.ADamage));
                //AllHitObject.Add(ZDMap.HitAt(ZDGameRule.RotateVector2(new Vector2(0, 0), AttackRad), this, EObjectType.ADamage));
                AllHitObject.Add(ZDMap.HitAt(ZDGameRule.RotateVector2(new Vector2(1, 0), AttackRad), this, EObjectType.ADamage));
                AllHitObject.Add(ZDMap.HitAt(ZDGameRule.RotateVector2(new Vector2(2, 0), AttackRad), this, EObjectType.ADamage));

                AllHitObject.Add(ZDMap.HitAt(ZDGameRule.RotateVector2(new Vector2(-2, -1), AttackRad), this, EObjectType.ADamage));
                AllHitObject.Add(ZDMap.HitAt(ZDGameRule.RotateVector2(new Vector2(-1, -1), AttackRad), this, EObjectType.ADamage));
                AllHitObject.Add(ZDMap.HitAt(ZDGameRule.RotateVector2(new Vector2(0, -1), AttackRad), this, EObjectType.ADamage));
                AllHitObject.Add(ZDMap.HitAt(ZDGameRule.RotateVector2(new Vector2(1, -1), AttackRad), this, EObjectType.ADamage));
                AllHitObject.Add(ZDMap.HitAt(ZDGameRule.RotateVector2(new Vector2(2, -1), AttackRad), this, EObjectType.ADamage));

                AllHitObject.Add(ZDMap.HitAt(ZDGameRule.RotateVector2(new Vector2(-2, -2), AttackRad), this, EObjectType.ADamage));
                AllHitObject.Add(ZDMap.HitAt(ZDGameRule.RotateVector2(new Vector2(-1, -2), AttackRad), this, EObjectType.ADamage));
                AllHitObject.Add(ZDMap.HitAt(ZDGameRule.RotateVector2(new Vector2(0, -2), AttackRad), this, EObjectType.ADamage));
                AllHitObject.Add(ZDMap.HitAt(ZDGameRule.RotateVector2(new Vector2(1, -2), AttackRad), this, EObjectType.ADamage));
                AllHitObject.Add(ZDMap.HitAt(ZDGameRule.RotateVector2(new Vector2(2, -2), AttackRad), this, EObjectType.ADamage));
                break;
        }
        ApplyDamage(AllHitObject, EAttackType.B);
    }

    public override void AttackEventR(int Phase)
    {
        if (photonView.IsMine)
        {
            StartCoroutine(CreateThunder());
        }
    }
    #endregion

    IEnumerator CreateThunder()
    {
        int i = Random.Range(10, 15);
        int x, y;
        while (i > 0)
        {
            --i;
            yield return new WaitForSeconds(Random.Range(0.05f, 0.2f));
            do
            {
                x = Random.Range(-RusoAttackRRangeWidth, RusoAttackRRangeWidth + 1);
                y = Random.Range(-RusoAttackRRangeHeight, RusoAttackRRangeHeight + 1);
            } while (x == 0 && x == y);
            Vector2 UnitPos = (Vector2)ZDGameRule.WorldToUnit(transform.position) + new Vector2(x, y);
            photonView.RPC("CreateAttackREffect", RpcTarget.All, UnitPos);
        }
    }

    [PunRPC]
    void CreateAttackREffect(Vector2 UnitPos)
    {
        if (RusoREffectSample)
        {
            GameObject RusoREffect = Instantiate(RusoREffectSample, UnitPos * ZDGameRule.UNIT_IN_WORLD, Quaternion.identity);
            AttackEffect RusoREffectScript = RusoREffect.GetComponent<AttackEffect>();
            if (RusoREffectScript)
            {
                RusoREffectScript.AttackDamages = new float[] { GetFinalAttackDamage(EAttackType.R) };
            }
        }
    }
}
