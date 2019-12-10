using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZoneDepict.Rule;
using ZoneDepict;
using Photon.Pun; 
public class Ruso : CrossMoveCharacter
{
    [SerializeField]
    private GameObject[] HitEffects;
    private float[] AttackDamage = { 5, 10, 15, 20 };
    
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
            foreach (var HitList in Hits)
            {
                if (HitList == null) continue;
                for(int i = 0 ,_i = HitList.Count; i < _i; ++i)
                {
                    var Obj = HitList[i];
                    if (Obj is IADamageObject HitObj)
                    {
                        if (HitObj is Character HitChar && HitChar.TeamID == this.TeamID)
                        {
                            continue;
                        }
                        HitObj.Hurt(AttackDamage[(int)Type]);
                        CreateHitEffectAt(Obj.transform.position);
                    }
                }
            }
        }
    }
    #endregion

    #region Helpers
    void CreateHitEffectAt(Vector3 pos, int nums = 1,bool rand= true,int which= 0)
    {
        pos.z = (int)TypeDepth.Effect;
 
        Quaternion rot = new Quaternion
        {
            eulerAngles = new Vector3(0, 0, Random.Range(0, 180))
        };
        Instantiate(HitEffects[Random.Range(0, HitEffects.Length)],pos,rot);
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
                AllHitObject.Add(ZDMap.HitAt(ZDGameRule.RotateVector2(new Vector2(1, 0), AttackRad), this,ETypeZDO.ADamage));
                AllHitObject.Add(ZDMap.HitAt(ZDGameRule.RotateVector2(new Vector2(0, 1), AttackRad), this, ETypeZDO.ADamage));
                AllHitObject.Add(ZDMap.HitAt(ZDGameRule.RotateVector2(new Vector2(0,-1), AttackRad), this, ETypeZDO.ADamage));
               
                break;
        }
        ApplyDamage(AllHitObject, AttackType.N);
    }

    public override void AttackEventA(int Phase)
    {
        List<List<ZDObject>> AllHitObject = new List<List<ZDObject>>();
        // Really do attack
        switch (Phase)
        {
            case 0:
                AllHitObject.Add(ZDMap.HitAt(ZDGameRule.RotateVector2(new Vector2(1, 0), AttackRad), this, ETypeZDO.ADamage));
                break;
            case 1:
                AllHitObject.Add(ZDMap.HitAt(ZDGameRule.RotateVector2(new Vector2(2, 0), AttackRad), this, ETypeZDO.ADamage));
                AllHitObject.Add(ZDMap.HitAt(ZDGameRule.RotateVector2(new Vector2(2, 1), AttackRad), this, ETypeZDO.ADamage));
                AllHitObject.Add(ZDMap.HitAt(ZDGameRule.RotateVector2(new Vector2(2, -1), AttackRad), this, ETypeZDO.ADamage));
                break;
        }
        ApplyDamage(AllHitObject, AttackType.A);
    }

    public override void AttackEventB(int Phase)
    {
        List<List<ZDObject>> AllHitObject = new List<List<ZDObject>>();
        // Really do attack
        switch (Phase)
        {
            case 0:
                AllHitObject.Add(ZDMap.HitAt(ZDGameRule.RotateVector2(new Vector2(0, 1), AttackRad), this, ETypeZDO.ADamage));
                AllHitObject.Add(ZDMap.HitAt(ZDGameRule.RotateVector2(new Vector2(1, 1), AttackRad), this, ETypeZDO.ADamage));
                AllHitObject.Add(ZDMap.HitAt(ZDGameRule.RotateVector2(new Vector2(1, 0), AttackRad), this, ETypeZDO.ADamage));
                AllHitObject.Add(ZDMap.HitAt(ZDGameRule.RotateVector2(new Vector2(1, -1), AttackRad), this, ETypeZDO.ADamage));
                AllHitObject.Add(ZDMap.HitAt(ZDGameRule.RotateVector2(new Vector2(0, -1), AttackRad), this, ETypeZDO.ADamage));
                AllHitObject.Add(ZDMap.HitAt(ZDGameRule.RotateVector2(new Vector2(-1, -1), AttackRad), this, ETypeZDO.ADamage));
                AllHitObject.Add(ZDMap.HitAt(ZDGameRule.RotateVector2(new Vector2(-1, 0), AttackRad), this, ETypeZDO.ADamage));
                AllHitObject.Add(ZDMap.HitAt(ZDGameRule.RotateVector2(new Vector2(-1, 1), AttackRad), this, ETypeZDO.ADamage));
                break;
        }
        ApplyDamage(AllHitObject, AttackType.B);
    }

    public override void AttackEventR(int Phase)
    {
    }
    #endregion
}
