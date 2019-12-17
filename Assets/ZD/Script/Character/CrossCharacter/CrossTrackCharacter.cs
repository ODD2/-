﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ZoneDepict;
using ZoneDepict.Rule;
using ZoneDepict.Map;

public class CrossTrackCharacter : CrossMoveCharacter
{

    [SerializeField]
    protected GameObject TrackAngleIndicator;
    bool TrackAvailable;
    protected float TrackAngle;

    protected new void Start()
    {
        base.Start();
        if (photonView.IsMine)
        {
            TrackAngleIndicator = Instantiate(ZDAssetTable.GetObject("TrackAngleIndicator"), transform);
            TrackAngleIndicator.SetActive(false);
            TrackAngleIndicator.transform.position += new Vector3(0, ZDGameRule.UnitInWorld * 1.2f, 0);
            StartCoroutine(GenerateNewTrack());
        }
    }

    protected override void Sprint(Vector2 Destination)
    {
        base.Sprint(Destination);
        if (photonView.IsMine && TrackAvailable)
        {
            float SprintAngle = ZDGameRule.QuadAngle(SprintDestination - (Vector2)transform.position);
            if (TrackAngle.Equals(SprintAngle))
            {
                TrackFullFilled();
            }
        }
    }

    private void TrackFullFilled()
    {
        TrackAvailable = false;
        SetSoul(Soul + 1);  
        if (TrackAngleIndicator) TrackAngleIndicator.SetActive(false);
        StartCoroutine(GenerateNewTrack());
    }

    private IEnumerator GenerateNewTrack()
    {
        yield return new WaitForSeconds(ZDGameRule.CrossTrack.NextTrackDelay);
        DisplayNewTrack(ZDGameRule.QuadAngle(Random.Range(0, 359)));
    }

    private void DisplayNewTrack(float angle)
    {
        TrackAvailable = true;
        TrackAngle = angle;
        if (TrackAngleIndicator)
        {
            TrackAngleIndicator.SetActive(true);
            TrackAngleIndicator.transform.rotation = new Quaternion
            {
                eulerAngles = new Vector3(0, 0, angle)
            };
        }
    }
}
