using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ZoneDepict;
using ZoneDepict.Rule;
using ZoneDepict.Map;

//Characters Derived from this class is restrict to move and attack on the four directions.
public class CrossMoveCharacter : Character
{
    #region Fields
    //Movement
    private bool NewDestination = false;
    protected Vector2 SprintDestination;
    //Attack
    protected float AttackRad;
    #endregion

    #region Character Override
    public override void InputSprint(Vector2 Destination)
    {
        Destination = GetValidDest(Destination);
        float RequireMana = (Destination - (Vector2)transform.position).magnitude / ZDGameRule.UnitInWorld * MoveMana;
        if (photonView.IsMine && !animator.GetCurrentAnimatorStateInfo(0).IsTag("NM") && RequireMana < MP)
        {
            SetMP(MP - RequireMana);
            photonView.RPC("SprintRPC", RpcTarget.All,Destination);
        }
    }

    public override void InputAttack(Vector2 AttackDirection, EAttackType Type)
    { 
        if (photonView.IsMine &&
            !animator.GetCurrentAnimatorStateInfo(0).IsTag("NM") &&
            SkillMana[(int)Type] < MP &&
            !(SkillCD[(int)Type] > float.Epsilon))
        {
            SetMP(MP - SkillMana[(int)Type]);
            SkillCD[(int)Type] = MaxSkillCD[(int)Type];
            photonView.RPC("RPCAttack", RpcTarget.AllViaServer, AttackDirection, Type);
        }
    }

    protected override void Sprint(Vector2 Destination)
    {
        NewDestination = true;
        //Save Destination
        SprintDestination = ZDGameRule.WorldUnify(ZDGameRule.QuadrifyDirection(Destination, transform.position));
        //PlaySound
        if (MoveSound && audioSource) audioSource.PlayOneShot(MoveSound);
    }

    protected override void Attack(Vector2 Direction, EAttackType Type)
    {
        FaceTo(Direction);

        switch (Type)
        {
            case EAttackType.N:
                // Play clip and Trigger notify
                animator.SetTrigger("AttackN");
                break;
            case EAttackType.A:
                animator.SetTrigger("AttackA");
                break;
            case EAttackType.B:
                animator.SetTrigger("AttackB");
                break;
            case EAttackType.R:
                break;
            default:
                break;
        }
    }
    #endregion

    #region UNITY
    protected new void Start()
    {
        base.Start();
        SprintDestination = transform.position;
    }

    protected new void Update()
    {
        base.Update();
       
    }

    protected new void FixedUpdate()
    {
        base.FixedUpdate();
        if (NewDestination)
        {
            Vector2 Delta = SprintDestination - (Vector2)transform.position;
            Velocity = Delta.normalized * MaxVelocity;
            Vector2 MoveDelta = Velocity * Time.deltaTime;
            //Check if we are at destination this frame
            if (Delta.magnitude <= MoveDelta.magnitude)
            {
                transform.position = new Vector3(SprintDestination.x,
                                                 SprintDestination.y,
                                                 transform.position.z);
                Velocity = Vector2.zero;
                NewDestination = false;
                if (audioSource && audioSource.isPlaying) audioSource.Stop();
            }
            //Move toward destination
            else
            {
                Vector3 NewPos = transform.position;
                NewPos.x += MoveDelta.x;
                NewPos.y += MoveDelta.y;
                transform.position = NewPos;
                //Adjust Facing Direction
                if (!MoveDelta.x.Equals(0)) sprite.flipX = MoveDelta.x > 0 ? true : false;
            }

           
        }
    }
    #endregion

    #region Helper Functions
    void FaceTo(Vector2 Direction)
    {
        //Save Attack Direction
        AttackRad = ZDGameRule.QuadRadius(Direction);

        // Set Scale Direction
        if(!Direction.x.Equals(0))sprite.flipX = Direction.x > 0 ? true: false;
        Direction = ZDGameRule.QuadrifyDirection(Direction);
        // Set Animation Direction
        animator.SetInteger("AtkVertical",(int)Direction.y);
        animator.SetInteger("AtkHorizontal",(int)Direction.x);
    }

    Vector2 GetValidDest(Vector2 Destination)
    {
        
        Vector2 PosInUnit = ZDGameRule.WorldToUnit(transform.position);
        Vector2 DesInUnit = ZDGameRule.WorldToUnit(ZDGameRule.QuadrifyDirection(Destination, transform.position));
        for(int i = 0; i < 2; ++i)
        {
            if (!PosInUnit[i].Equals(DesInUnit[i]))
            {
                int Delta = (PosInUnit[i] < DesInUnit[i] ? 1 : -1);
                Vector2 CurPos = PosInUnit;

                while (true)
                {
                    //Move To Currently Checking Position.
                    CurPos[i] += Delta;
                    if (!ZDMap.IsUnitInMap(CurPos) || ZDMap.HitAtUnit(CurPos,EObjectType.Obstacle)!=null)
                    {
                        //Return To Last Position.
                        CurPos[i] -= Delta;
                        return ZDGameRule.UnitToWorld(CurPos);
                    }
                    if (CurPos[i].Equals(DesInUnit[i])) break;
                }
                return ZDGameRule.UnitToWorld(DesInUnit);
            }
        }
        return transform.position;
    }
    #endregion

    #region CrossMoveCharacter Interfaces
    public virtual void AttackEventN(int Phase) { }
    public virtual void AttackEventA(int Phase) { }
    public virtual void AttackEventB(int Phase) { }
    public virtual void AttackEventR(int Phase) { }
    #endregion

    #region PUN RPC
    [PunRPC]
    public virtual void SprintRPC(Vector2 Destination)
    {
        Sprint(Destination);
    }

    [PunRPC]
    public void RPCAttack(Vector2 Direction, EAttackType type)
    {
        Attack(Direction, type);
    }
    #endregion
}
