using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
// If there is another Server in the future , set the server info here and 
// change server setting in this class
public class StartViewManager : MonoBehaviourPunCallbacks
{
    
    public Dropdown ChooseServer;
    public Button StartButton;
    public Text DebugText;
    public GameObject Connecting;
    
    
    void Start()
    {
        StartButton.onClick.AddListener(() => StartGame());

        
    }

    protected void Update()
    {
        DebugText.text = PhotonNetwork.NetworkClientState.ToString();
    }

    public void StartGame()
    {
        int Serverindex = ChooseServer.value;
        switch (Serverindex)
        {
            case 0:
                PhotonNetwork.GameVersion = "1";
                PhotonNetwork.ConnectUsingSettings();
                break;
            
            default:
                break;
        }
        Connecting.SetActive(true);
        StartCoroutine(WaitConnect());
    }

    public override void OnConnectedToMaster()
    {
        // Connected successed !
    }
    IEnumerator  WaitConnect()
    {
        yield return new WaitUntil(()=> PhotonNetwork.NetworkClientState == ClientState.ConnectedToMaster);
        PhotonNetwork.LoadLevel("GameLobbyView");
    }
}
