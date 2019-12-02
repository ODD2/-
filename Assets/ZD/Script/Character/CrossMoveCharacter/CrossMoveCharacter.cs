using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ZoneDepict.Rule;
using ZoneDepict;

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
        if (photonView.IsMine)
        {
            photonView.RPC("SprintRPC", RpcTarget.All, GetValidDest(Destination));
        }
    }

    public override void InputAttack(Vector2 AttackDirection, AttackType Type)
    { 
        if (photonView.IsMine)
        {
            photonView.RPC("RPCAttack", RpcTarget.AllViaServer, AttackDirection, Type);
        }
    }

    protected override void Sprint(Vector2 Destination)
    {
        NewDestination = true;
        //Save Destination
        SprintDestination = ZDGameRule.WorldUnify(ZDGameRule.QuadrifyDirection(Destination, transform.position));
    }

    protected override void Attack(Vector2 Direction, AttackType Type)
    {
        AttackRad = ZDGameRule.QuadRadius(Direction);
        FaceTo(Direction);

        switch (Type)
        {
            case AttackType.N:
                // Play clip and Trigger notify
                animator.SetTrigger("AttackN");
                break;
            case AttackType.A:
                break;
            case AttackType.B:
                break;
            case AttackType.R:
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
        if (NewDestination)
        {
            Vector2 Delta = SprintDestination - (Vector2)transform.position;
            Velocity = Delta.normalized * MaxVelocity;
            Vector2 MoveDelta = Velocity * Time.deltaTime;
            //Check if we are at destination this frame
            if (Delta.magnitude <= MoveDelta.magnitude)
            {
                transform.position = SprintDestination;
                Velocity = Vector2.zero;
                NewDestination = false;
            }
            //Move toward destination
            else
            {
                transform.position += (Vector3)(MoveDelta);
            }

            //Adjust Facing Direction
            Vector3 NewScale = transform.localScale;
            if (MoveDelta.x * transform.localScale.x < 0)
            {
                NewScale.x *= -1;
                transform.localScale = NewScale;
            }
        }
    }
    #endregion

    #region Helper Functions
     void FaceTo(Vector2 Direction)
    {
        if(transform.localScale.x * Direction.x < 0)
        {
            Vector2 NewScale = transform.localScale;
            NewScale.x *= -1;
            transform.localScale = NewScale;
        }
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
                    if (ZDMap.HitAtUnit(CurPos) != null)
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
    public void RPCAttack(Vector2 Direction, AttackType type)
    {
        Attack(Direction, type);
    }
    #endregion
}