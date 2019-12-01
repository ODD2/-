using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZoneDepict.Rule;
using ZoneDepict;
using Photon.Pun; 
public class Ruso : Character
{
    private float[] AttackDamage = { 5, 10, 15, 20 };
    private float AttackRadius;
    private AttackType Type;
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

    #region ZD Input Call
    public override void InputAttack(Vector2 AttackDirection,AttackType Type)
    {
        this.Type = Type;
        if (photonView.IsMine)
        {
            photonView.RPC("RPCAttack", RpcTarget.AllViaServer, AttackDirection, this.Type);
        }
    }

    public override void InputSprint(Vector2 Destination)
    {
        if (photonView.IsMine)
        {
            Sprint(Destination);
        }
    }
    #endregion

    #region Charater Override
    public override void Attack(Vector2 Direction,AttackType Type)
    {
        //Debug.Log("Attack flow is True !");
        AttackRadius = ZDGameRule.QuadRadius(Direction);
        switch (Type)
        {
            case AttackType.N:
                // Play clip and Trigger notify
                AttackEventN(0);
                //Debug.LogFormat("Attack with 'N' at {0}", Direction);
                // Anims[(int)AttackType.N].Play(Anims[(int)AttackType.N].name);
                break;
            case AttackType.A:
                // Play clip and Trigger notify
                //Debug.LogFormat("Attack with 'A' at {0}", Direction);
                //Anims[(int)AttackType.A].Play();
                break;
            case AttackType.B:
                // Play clip and Trigger notify
                //Debug.LogFormat("Attack with 'B' at {0}", Direction);
                //Anims[(int)AttackType.B].Play();
                break;
            case AttackType.R:
                // Play clip and Trigger notify
                //Debug.LogFormat("Attack with 'R' at {0}", Direction);
                //Anims[(int)AttackType.R].Play();
                break;
            case AttackType.Cancel:
                break;
            default:

                break;
        }
    }

    public override void Sprint(Vector2 Destination)
    {
        //Constraint the directions.
        Vector2 Delta = ZDGameRule.QuadrifyDirection(Destination - (Vector2)transform.position);
        // Do movement
        this.transform.position += ZDGameRule.WorldUnify(Delta.x,Delta.y,transform.position.z);
        // MoveEffect Below.
    }

    public override void ApplyDamage(List<List<ZDObject>> Hits,AttackType Type)
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

    #region Attack Notify
    public void AttackEventN(int Phase)
    {
        List<List<ZDObject>> AllHitObject = new List<List<ZDObject>>();
        // Really do attack
        switch (Phase)
        {
            case 0:
                AllHitObject.Add(ZDMap.HitAt(ZDGameRule.RotateVector2(new Vector2(1, 0), AttackRadius), this));
                AllHitObject.Add(ZDMap.HitAt(ZDGameRule.RotateVector2(new Vector2(0, 1), AttackRadius), this));
                AllHitObject.Add(ZDMap.HitAt(ZDGameRule.RotateVector2(new Vector2(0,-1), AttackRadius), this));
                ApplyDamage(AllHitObject,this.Type);
                break;
            case 1:
                break;
        }
        
    }

    // Not implment yet , same as AttackEventN
    public void AttackEventA(int Phase)
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

    public void AttackEventB(int Phase)
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

    public void AttackEventR(int Phase)
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
