﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System;
using ZoneDepict.Rule;
using Hashtable = ExitGames.Client.Photon.Hashtable;
// If there is another Server in the future , set the server info here and 
// change server setting in this class
public class StartViewManager : MonoBehaviourPunCallbacks
{
    [Header("Set the Server Setting")]
    public Dropdown ChooseServer;
    [Header("Button to Start Game")]
    public Button StartButton;
    [Header("To show PhotonNetwork State")]
    public Text DebugText;
    [Header("The View of Connecting")]
    public GameObject Connecting;
    // [0] = BGM , [1] = start , [2] = but sound
    [Header("Audio array = {BGM,Start Audio,But Sound}")]
    public AudioSource[] StartViewAudio;
    
    
    void Start()
    {
        StartButton.onClick.AddListener(() => StartGame());
        //StartViewAudio = GetComponents<AudioSource>();
    }

    protected void Update()
    {
        DebugText.text = PhotonNetwork.NetworkClientState.ToString();
    }

    public void StartGame()
    {
        
        // int Serverindex = ChooseServer.value;
        int ServerIndex = 0;
        StartViewAudio[0].Stop();
        StartViewAudio[2].PlayOneShot(StartViewAudio[2].clip);
        StartViewAudio[1].Play();
        /*
         *  Add Player Name by Random
         */
        switch (ServerIndex)
        {
            case 0:
                
                PhotonNetwork.GameVersion = "1";
                PhotonNetwork.LocalPlayer.NickName = "Player" + UnityEngine.Random.Range(1000, 10000).ToString();
                PhotonNetwork.ConnectUsingSettings();
                break;
            
            default:
                break;
        }
        StartButton.enabled = false;
        Connecting.SetActive(true);
        //StartCoroutine(WaitConnect());
        //StartCoroutine(WaitAudioStop());
    }


    #region PUN CallBack
    public override void OnConnectedToMaster()
    {
        ExitGames.Client.Photon.Hashtable PlayerProps = new ExitGames.Client.Photon.Hashtable();
        PlayerProps.Add("Alive", true);
        PlayerProps.Add("RoomState", RoomPlayerState.Enter);
        PlayerProps.Add("CharacterName", "");
        PlayerProps.Add("Ready", false);
        PhotonNetwork.SetPlayerCustomProperties(PlayerProps);
        PhotonNetwork.LoadLevel("GameRoomView");

    }

    public override void OnJoinedLobby()
    {
        Debug.Log("In Lobby ~~");
    }

    #endregion

}
