using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
public class Launcher : MonoBehaviourPunCallbacks
{
    public bool RandomEnter;
    public InputField TeamInput;
    // Start is called before the first frame update
    void Start()
	{
        PhotonNetwork.GameVersion = "1";
        PhotonNetwork.ConnectUsingSettings();
    }

	void Update()
	{
	}

    public void JoinRandom()
    {
        
        ExitGames.Client.Photon.Hashtable PlayerProps = new ExitGames.Client.Photon.Hashtable();
        PlayerProps.Add("Team", Convert.ToInt32(TeamInput.text));
        PlayerProps.Add("Alive", true);
        PhotonNetwork.SetPlayerCustomProperties(PlayerProps);
        PhotonNetwork.JoinRandomRoom();
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
        PhotonNetwork.CreateRoom("ASDASD");
    }

    public override void OnJoinedRoom()
    {

        Debug.Log("Join Room Sucessed ! :) ");
        PhotonNetwork.LoadLevel(1);
    }
}
