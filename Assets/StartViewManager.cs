using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System;
using ZoneDepict.Rule;
// If there is another Server in the future , set the server info here and 
// change server setting in this class
public class StartViewManager : MonoBehaviourPunCallbacks
{
    
    public Dropdown ChooseServer;
    public Button StartButton;
    public Text DebugText;
    public GameObject Connecting;
    private AudioSource[] StartViewAudio;
    
    void Start()
    {
        StartButton.onClick.AddListener(() => StartGame());
        StartViewAudio = GetComponents<AudioSource>();
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
        StartViewAudio[1].Play();
        switch (ServerIndex)
        {
            case 0:
                PhotonNetwork.GameVersion = "1";
                PhotonNetwork.ConnectUsingSettings();
                break;
            
            default:
                break;
        }

        StartCoroutine(WaitAudioStop());

    }

    public override void OnConnectedToMaster()
    {
        // Connected successed !
        // If connected successed , plz go to Lobby
        // PhotonNetwork.JoinLobby();
    }

    IEnumerator  WaitConnect()
    {
        yield return new WaitUntil(()=> PhotonNetwork.NetworkClientState == ClientState.ConnectedToMaster);

        //PhotonNetwork.LoadLevel("GameLobbyView");
        // Only for beta demo and add team is random if
        // want do some real team-thing , modify here
        int RandomRoomint = UnityEngine.Random.Range(0, 9999999);
        ExitGames.Client.Photon.Hashtable PlayerProps = new ExitGames.Client.Photon.Hashtable();
        PlayerProps.Add("Team",RandomRoomint);
        PlayerProps.Add("Alive", true);
        PlayerProps.Add("RoomState", RoomPlayerState.Enter);
        PhotonNetwork.SetPlayerCustomProperties(PlayerProps);
        PhotonNetwork.JoinRandomRoom();
        Debug.Log(RandomRoomint);
    }
    IEnumerator WaitAudioStop()
    {
        yield return new WaitUntil(() => !StartViewAudio[1].isPlaying);
        Connecting.SetActive(true);

        StartCoroutine(WaitConnect());
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Join Room Sucessed ! :) ");
        PhotonNetwork.LoadLevel("GameRoomView");
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        PhotonNetwork.CreateRoom("DemoRoom");
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("In Lobby ~~");
    }
    
}
