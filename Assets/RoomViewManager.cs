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
    // [0] = BGM , [1] = Get choose but , [2] = get Ready but , [3] = readywaiting
    private AudioSource[] RoomViewAudio;
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
        RoomViewAudio = GetComponents<AudioSource>();
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
            RoomViewAudio[3].Stop();
            RoomViewAudio[2].Play();
            StartCoroutine(ReadyWait());
            ReadyButton.enabled = false;
            State = RoomPlayerState.Ready;
            Hashtable NewState = new Hashtable();
            NewState.Add("RoomState", RoomPlayerState.Ready);
            NewState.Add("CharacterName", CharacterName);
            PhotonNetwork.SetPlayerCustomProperties(NewState);
        }
    }

    
    public void ImgOnClick(string name)
    {
        if(ChooseTimes > 0)
        {
            CharacterName = name;
        }
    }
    public void ImgReadyLock(Image Img)
    {
        if(ChooseTimes > 0)
        {
            RoomViewAudio[1].Play();
            Img.enabled = true;
            Img.color = new Vector4(1, 1, 1, 1);
            ChooseTimes--;
            RoomViewAudio[0].Stop();
            RoomViewAudio[1].Play();
            StartCoroutine(ReadyWait());
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

    IEnumerator ReadyWait()
    {
        yield return new WaitUntil(() => !RoomViewAudio[1].isPlaying);
        RoomViewAudio[3].Play();
    }
}
