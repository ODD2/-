using System.Collections;
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
    [Header("Button to Activate Teaching Page")]
    public Button TeachingButton;
    [Header("Button to Back to StartView")]
    public Button BackButton;
    [Header("To show PhotonNetwork State")]
    public Text DebugText;
    [Header("Views Area")]
    [Header("The View of Connecting")]
    public GameObject Connecting;
    [Header("The View of Teaching")]
    public GameObject Teaching;
    // [0] = BGM , [1] = start , [2] = but sound
    [Header("Audio array = {BGM,Start Audio,But Sound}")]
    public AudioSource[] StartViewAudio;

    private string[] ServersName = { "jp", "asia" ,"eu","us"};
    void Start()
    {
        StartButton.onClick.AddListener(() => StartGame());
        TeachingButton.onClick.AddListener(() => ActivateTeaching());
        BackButton.onClick.AddListener(() => Back());
        
        //StartViewAudio = GetComponents<AudioSource>();
       
    }

    protected void Update()
    {
        DebugText.text = PhotonNetwork.NetworkClientState.ToString();
        if (Input.GetKeyDown(KeyCode.A))
        {
            PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "eu";
            PhotonNetwork.ConnectUsingSettings();

            Debug.Log(PhotonNetwork.CloudRegion);
            Debug.Log(PhotonNetwork.GetPing());
        }
    }

    private void StartGame()
    {
        int Serverindex = ChooseServer.value;
        StartViewAudio[0].Stop();
        StartViewAudio[2].PlayOneShot(StartViewAudio[2].clip);
        StartViewAudio[1].Play();
        /*
         *  Add Player Name by Random
         */
        PhotonNetwork.GameVersion = "1";
        PhotonNetwork.LocalPlayer.NickName = "Player" + UnityEngine.Random.Range(1000, 10000).ToString();
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = ServersName[Serverindex];
        PhotonNetwork.ConnectUsingSettings();
        StartButton.enabled = false;
        Connecting.SetActive(true);
    }

    private void ActivateTeaching()
    {
        StartViewAudio[2].Play();
        Teaching.SetActive(true);
    }
    private void Back()
    {
        StartViewAudio[2].Play();
        Teaching.SetActive(false);
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
        //PhotonNetwork.JoinLobby();
        PhotonNetwork.LoadLevel("GameRoomView");

    }

    public override void OnJoinedLobby()
    {
        Debug.Log("In Lobby ~~");
        Debug.Log(PhotonNetwork.CountOfRooms);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log(roomList[0].CustomProperties);
        Debug.Log("New Room:");
        Debug.Log(roomList[0]);
        
        //Debug.Log();
    }

    #endregion

}
