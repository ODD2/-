using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class Luucher : MonoBehaviourPunCallbacks
{
    
    public Text state;
	public GameObject connect;
	public GameObject disconnect;
    public GameObject room;
    private readonly string connectionStatusMessage = "    Connection Status: ";
    // Start is called before the first frame update
    void Start()
	{
        
	}

	void Update()
	{
		if (Input.touchCount > 0)
		{
			Touch touch = Input.GetTouch(0);
			if (touch.phase == TouchPhase.Began)
			{
				if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
				{
					onClick();

				}
			}
		}

        if (Input.GetKey(KeyCode.P))
        {
            onClick();
        }

        state.text = connectionStatusMessage + PhotonNetwork.NetworkClientState;
	}

	public void onClick()
	{
        PhotonNetwork.GameVersion = "1";
		PhotonNetwork.ConnectUsingSettings();

	}

	public override void OnDisconnected(DisconnectCause cause)
	{
		disconnect.SetActive(true);
	}
	public override void OnConnectedToMaster()
	{
		Debug.Log("Connecting...");
        //PhotonNetwork.JoinRandomRoom();
        
		PhotonNetwork.JoinLobby(TypedLobby.Default);
	}
    
    public override void OnJoinedLobby()
	{
		if (disconnect.activeSelf)
			disconnect.SetActive(true);
		connect.SetActive(true);
       
        //PhotonNetwork.JoinRandomRoom();
		
	}
}
