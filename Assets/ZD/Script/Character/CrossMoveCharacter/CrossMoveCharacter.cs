using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ZoneDepict.Rule;

public class CrossMoveCharacter : Character
{
    private bool NewDestination = false;

    protected Vector2 SprintDestination;

    public override void InputSprint(Vector2 Destination)
    {
        if (photonView.IsMine)
        {
            photonView.RPC("SprintRPC", RpcTarget.All, Destination);
        }
    }

    protected override void Sprint(Vector2 Destination)
    {
        NewDestination = true;
        //Save Destination
        SprintDestination = ZDGameRule.WorldUnify(ZDGameRule.QuadrifyDirection(Destination, transform.position));
    }

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

    #region RPC
    [PunRPC]
    public virtual void SprintRPC(Vector2 Destination)
    {
        Sprint(Destination);
    }
    #endregion
}