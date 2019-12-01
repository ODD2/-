using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZoneDepict.Rule;
using ZoneDepict;
using Photon.Pun; 
public class Ruso : CrossMoveCharacter
{
    private float[] AttackDamage = { 5, 10, 15, 20 };
    //private Vector2 AttackDirection;
    public AnimationClip[] Clips;
    public Animator[] Anims;
    
    #region UNITY
    protected new void Start()
    {
        Debug.Log("Debug !!!");
        base.Start();
        #region Animation notify
        // Type N
        AnimationEvent EvtN = new AnimationEvent();
        EvtN.intParameter = 0;
        
        EvtN.time = 0;
        EvtN.functionName = "AttackEventN";
        Clips[(int)AttackType.N] = Anims[(int)AttackType.N].runtimeAnimatorController.animationClips[0];
        Clips[(int)AttackType.N].AddEvent(EvtN);
        // Type A
        AnimationEvent EvtA = new AnimationEvent();
        EvtA.intParameter = 0;
        EvtA.time = 0;
        EvtA.functionName = "AttackEventA";
        Clips[(int)AttackType.A] = Anims[(int)AttackType.N].runtimeAnimatorController.animationClips[0];
        Clips[(int)AttackType.A].AddEvent(EvtN);
        // Type B
        AnimationEvent EvtB = new AnimationEvent();
        EvtB.intParameter = 0;
        EvtB.time = 0;
        EvtB.functionName = "AttackEventB";
        Clips[(int)AttackType.B] = Anims[(int)AttackType.N].runtimeAnimatorController.animationClips[0];
        Clips[(int)AttackType.B].AddEvent(EvtN);
        // Type R
        AnimationEvent EvtR = new AnimationEvent();
        EvtR.intParameter = 0;
        EvtR.time = 0;
        EvtR.functionName = "AttackEventR";
        Clips[(int)AttackType.R] = Anims[(int)AttackType.N].runtimeAnimatorController.animationClips[0];
        Clips[(int)AttackType.R].AddEvent(EvtN);
        #endregion
    }

    protected new void Update()
    {
        base.Update();        
    }
    #endregion

    #region Charater Override
    protected override void ApplyDamage(List<List<ZDObject>> Hits,AttackType Type)
    {
        Debug.Log("Damaged Object Lists: " + Hits.Count);
        if (Hits != null)
        {
            foreach (var i in Hits)
            {
                if (i == null) continue;
                foreach (var Obj in i)
                {
                    if (Obj is IADamageObject)
                    {
                        ((IADamageObject)Obj).Hurt(AttackDamage[(int)Type]);
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
        List<ZDObject>[] HitObject;
        // Really do attack
        switch (Phase)
        {
            case 0:
                break;
            case 1:
                break;
        }
    }

    public override void AttackEventB(int Phase)
    {
        List<ZDObject>[] HitObject;
        // Really do attack
        switch (Phase)
        {
            case 0:
                break;
            case 1:
                break;
        }
    }

    public override void AttackEventR(int Phase)
    {
        List<ZDObject>[] HitObject;
        // Really do attack
        switch (Phase)
        {
            case 0:
                break;
            case 1:
                break;
        }
    }
    #endregion
}
