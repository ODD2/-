using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using ZoneDepict.Rule;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class RoomViewManager : MonoBehaviourPunCallbacks
{
    
    public Button ReadyButton;
    public Text DebugTxt;
    private RoomPlayerState State;
    private string CharacterName;
    private int ChooseTimes = 1;
    public RoomPlayerState GetState()
    {
        return State;
    }

    protected void SetState(RoomPlayerState state)
    {
        State = state;
    }

    void Start()
    {
        ReadyButton.onClick.AddListener(() => Ready());
        
        State = RoomPlayerState.Casting;
        
        Hashtable NewState = new Hashtable();
        NewState.Add("RoomState", RoomPlayerState.Casting);
        PhotonNetwork.SetPlayerCustomProperties(NewState);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            foreach (var player in PhotonNetwork.PlayerList)
            {
                Debug.Log("State : " + player.CustomProperties["RoomState"]);
            }
        }
        
    }

    
    private void Ready()
    {
        if(!(ChooseTimes > 0))
        {
            ReadyButton.enabled = false;

            Hashtable NewState = new Hashtable();
            NewState.Add("RoomState", RoomPlayerState.Ready);
            NewState.Add("CharacterName", CharacterName);
            PhotonNetwork.SetPlayerCustomProperties(NewState);
        }
    }

    
    public void ImgOnClick(string name)
    {
        CharacterName = name;
        
    }
    public void ImgReadyLock(Image Img)
    {
        if(ChooseTimes > 0)
        {
            Img.enabled = true;
            Img.color = new Vector4(1, 1, 1, 1);
            ChooseTimes--;
        }
    }

    public bool CheckAllReady()
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if ((RoomPlayerState)player.CustomProperties["RoomState"] != RoomPlayerState.Ready)
            {
                return false;
            }
        }
        return true;
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (CheckAllReady())
        {
            Debug.Log("All Ready");
            PhotonNetwork.LoadLevel("GameGround");
        }
        else
        {
            Debug.Log("Not Yet");
        }
    }

    #region Photon Callback

    #endregion
}
