using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Photon.Pun;
using ZoneDepict;
using ZoneDepict.Rule;
using ZoneDepict.Map;
using Random = UnityEngine.Random;
public class CrossTrackCharacter : CrossMoveCharacter
{
    //TrackCharacter Setup.
    public bool TrackAvailable { get; protected set; }
    public List<float> TrackAngles { get; protected set; } = new List<float>();
    public float TrackRemainTime { get; protected set; }
    public float TrackDurationTime { get; protected set; }

    #region Unity
    protected new void Start()
    {
        base.Start();
        if (photonView.IsMine)
        {
            StartCoroutine(WaitSecondsToGenerateTrack(0));
        }
    }

    protected new void FixedUpdate()
    {
        if (TrackAvailable)
        {
            TrackRemainTime -= Time.fixedDeltaTime;
            if ( TrackRemainTime < 0)
            {
                TrackMissionFailed();
            }
        }
        base.FixedUpdate();
    }
    #endregion

    #region Character Interface
    protected override void Sprint(Vector2 Destination)
    {

        base.Sprint(Destination);
        if (photonView.IsMine && TrackAvailable)
        {
            float SprintAngle = ZDGameRule.QuadAngle(SprintDestination - (Vector2)transform.position);
            if (TrackAngles[0].Equals(SprintAngle))
            {
                TrackFullFilled();
            }
        }
    }

    public override void InputAttack(Vector2 AttackDirection, EAttackType Type)
    {
        switch (Type)
        {
            case EAttackType.N:
                base.InputAttack(AttackDirection, EAttackType.N);
                break;
            default:
                if (Soul > 0)
                {
                    if (Soul > (int)EAttackType.R) base.InputAttack(AttackDirection, EAttackType.R);
                    else base.InputAttack(AttackDirection, (EAttackType)Soul);
                }
                break;
        }
    }

    protected override void Attack(Vector2 Direction, EAttackType Type)
    {
        if (photonView.IsMine)
        {
            //Reduce Soul
            SetSoul(Soul - (int)Type);
        }
        base.Attack(Direction, Type);
    }
    #endregion

    #region Helper Functions
    private void TrackFullFilled()
    {
       
        if(TrackAngles.Count == 1)
        {
            TrackMissionSuccess();
        }
        else
        {
            TrackAngles.RemoveAt(0);
        }
        TrackInfoChanged?.Invoke(this, new TrackInfoChangeArgs(TrackAvailable, TrackAngles[0]));
        //Debug.Break();
    }

    private void TrackMissionSuccess()
    {
        SetSoul(Soul + 1);
        StartCoroutine(WaitSecondsToGenerateTrack(0.0f));
    }

    private void TrackMissionFailed()
    {
        StartCoroutine(WaitSecondsToGenerateTrack(ZDGameRule.CrossTrack.MissionFailedPunish));
    }

    private IEnumerator WaitSecondsToGenerateTrack(float AdditionTime)
    {
        TrackAvailable = false;
        TrackInfoChanged?.Invoke(this, new TrackInfoChangeArgs(TrackAvailable, 0));
        yield return new WaitForSeconds(ZDGameRule.CrossTrack.NextTrackDelay + AdditionTime);
        while (GetSoul() >= GetMaxSoul())
        {
            yield return new WaitForSeconds(2);
        }
        SpawnNewTrackMission();
    }

    private void SpawnNewTrackMission()
    {
        if(GetSoul() < GetMaxSoul())
        {
            TrackAvailable = true;
            TrackDurationTime = (GetMaxSoul() - GetSoul()) * ZDGameRule.CrossTrack.TrackDurationConst;
            TrackRemainTime = TrackDurationTime;
            TrackAngles.Clear();
            for (int i = 0, _i = (GetSoul() + 1) * ZDGameRule.CrossTrack.TrackCountsConst; i < _i; ++i)
            {
                TrackAngles.Add(ZDGameRule.QuadAngle(Random.Range(0, 359)));
            }
            TrackInfoChanged?.Invoke(this, new TrackInfoChangeArgs(TrackAvailable, TrackAngles[0]));
        }
    }
    #endregion

    #region Delegates
    public class TrackInfoChangeArgs : EventArgs
    {
        public TrackInfoChangeArgs(bool HasTrack, float Angle)
        {
            this.HasTrack = HasTrack;
            this.Angle = Angle;
        }
        public bool HasTrack { get; } = false;
        public float Angle { get; } = 0;
    }

    public event EventHandler<TrackInfoChangeArgs> TrackInfoChanged;
    #endregion
}

