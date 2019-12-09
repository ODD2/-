﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class SimpleLauncher
    : MonoBehaviourPunCallbacks
{
    public bool RandomEnter;
    // Start is called before the first frame update
    void Start()
    {
    }

    void Update()
    {
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
    }

    public override void OnConnectedToMaster()
    {
        if (RandomEnter) PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        if (RandomEnter) PhotonNetwork.CreateRoom("ASDASD");
    }

    public override void OnJoinedRoom()
    {
    }
}
