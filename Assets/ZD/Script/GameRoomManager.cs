using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using ZoneDepict.Rule;
// This class is handle with Game room
// After u connected sucessed and are in lobby
// u need this class to deal "with RoomThings"
public class GameRoomManager : MonoBehaviourPunCallbacks
{
    private const string RoomName = "RoomName";
    private bool RoomLock_New = false;
    private bool RoomLock_Join = false;
    public Button Create;
    public Button Entry;

    private void JoinRoom()
    {
        Debug.Log("JoingRoom...");
        PhotonNetwork.JoinRoom(RoomName, null);
    }
    private void CreatRoom()
    {
        PhotonNetwork.CreateRoom(RoomName, new RoomOptions { MaxPlayers = ZDGameRule.MAX_PLAYERS },null);
    }

    private void Start()
    {
        Create.onClick.AddListener(() => {
            //PhotonNetwork.JoinRandomRoom();
            //return;
            RoomLock_New = true;
            CreatRoom();
        });
        Entry.onClick.AddListener(() => {
            RoomLock_Join = true;
            JoinRoom();
        });
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.N) && !RoomLock_New)
        {
            //Debug.Log("Creating Room...");
            RoomLock_New = true;
            CreatRoom();
        }
        if (Input.GetKey(KeyCode.R) && !RoomLock_Join)
        {
            //Debug.Log("Join Room...");
            RoomLock_Join = true;
            JoinRoom();
        }

    }
    // Photon CallBack
    public override void OnJoinedRoom()
    {
        
        Debug.Log("Join Room Sucessed ! :) ");
        PhotonNetwork.LoadLevel(1);
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Join Room Failed :( ");
    }
}
