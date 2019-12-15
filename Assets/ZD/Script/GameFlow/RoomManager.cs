using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using ZoneDepict.Rule;

public class RoomManager : MonoBehaviourPunCallbacks
{

    [Header("PlayerEntry Ref")]
    public GameObject PlayerEntry;
    [Header("Content of ScollerView")]
    public GameObject ScrollContent;
    [Header("View of All Component")]
    public GameObject ControlView;
    [Header("Teams Field")]
    public GameObject TeamA;
    public GameObject TeamB;

    public Button ReadyBut;
    public Text StateTxt;
    

    private GameObject[] CastCharactors;
    
    private string CharacterName;
    private bool ReadyBool = false;
    private Dictionary<string, GameObject> PlayerListEntries;
    //private int TeamNumber
    void Start()
    {
        CastCharactors = new GameObject[ScrollContent.transform.childCount];
        for (int i = 0; i < ScrollContent.transform.childCount; i++)
        {
            string nameTemp = ScrollContent.transform.GetChild(i).gameObject.name;
            CastCharactors[i] = ScrollContent.transform.GetChild(i).gameObject;
            CastCharactors[i].GetComponent<Button>().onClick.AddListener(() => ChooseCharacter(nameTemp));
            
        }
        ReadyBut.onClick.AddListener(() => Ready());
        
        // Join Room
        PhotonNetwork.JoinRandomRoom();
        StartCoroutine(WaitJoinRoom());
    }

    
    void Update()
    {
        if (!ControlView.active)
        {
            StateTxt.text = PhotonNetwork.NetworkClientState.ToString();
            
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log(PhotonNetwork.CurrentRoom.PlayerCount);
            foreach (var p in PhotonNetwork.PlayerList)
            {
                Debug.Log("Name" + p.NickName);
                Debug.Log(p.CustomProperties);
            }
        }
        //debugtxt.text = PhotonNetwork.CurrentRoom.Name;
    }

    IEnumerator WaitJoinRoom()
    {
        yield return new WaitUntil(() => PhotonNetwork.NetworkClientState == ClientState.Joined);
        ControlView.SetActive(true);
    }

    public void ChooseCharacter(string name)
    {
        if (!ReadyBool)
        {
            /*
            * Play choose audio
            */
            CharacterName = name;
            foreach (var obj in CastCharactors)
            {
                if (obj.name == name)
                {
                    obj.transform.GetChild(0).gameObject.SetActive(true);
                }
                else
                {
                    obj.transform.GetChild(0).gameObject.SetActive(false);
                }
            }
        }
    }
    public void Ready()
    {
        if (CharacterName == null) return;
        /*
         *  Play ready but audio
         */
        if(!ReadyBool)
        {
            ReadyBool = true;
            Debug.Log("Confirm with " + CharacterName);
            Hashtable props = new Hashtable
            {
                {"CharacterName", CharacterName},
                {"Ready",true }
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }
        else
        {
            ReadyBool = false;
            Debug.Log("Cancel Ready");
            foreach (var obj in CastCharactors)
            {
                obj.transform.GetChild(0).gameObject.SetActive(false);
            }
            Hashtable props = new Hashtable
            {
                {"CharacterName", ""},
                {"Ready",false }
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }
        
    }
    public void EntryUpdate(Player player)
    {
        foreach (var p in PhotonNetwork.PlayerList)
        {
            if(PlayerListEntries.ContainsKey(p.NickName))
            {
                // Update
                if(p.CustomProperties.ContainsKey("Ready"))
                {
                    if((bool)p.CustomProperties["Ready"])
                    {
                        PlayerListEntries[p.NickName].transform.GetChild(2).GetComponent<Image>().color = Color.red;
                    }
                    else
                    {
                        PlayerListEntries[p.NickName].transform.GetChild(2).GetComponent<Image>().color = Color.white;
                    }
                }
            }
            else
            {
                // Create
                GameObject Entry = Instantiate(PlayerEntry);
                Entry.transform.GetChild(0).GetComponent<Text>().text = p.NickName;
                Entry.name = p.NickName;
                if ((ZDTeams)p.CustomProperties["Team"] == ZDTeams.T0)
                {
                    Entry.transform.SetParent(TeamA.transform);
                    Entry.transform.localScale = new Vector3(0.5f, 0.5f, 1);
                }
                else
                {
                    Entry.transform.SetParent(TeamB.transform);
                    Entry.transform.localScale = new Vector3(0.5f, 0.5f, 1);
                }
                PlayerListEntries.Add(p.NickName, Entry);
            }
            
        }
    }
    public bool CheackAllReady()
    {
        foreach(var p in PhotonNetwork.PlayerList)
        {
            if(p.CustomProperties.ContainsKey("Ready") && !(bool)p.CustomProperties["Ready"])
            {
                return false;
            }
        }
        return true;
    }
    #region PUN CallBack

    public override void OnJoinedRoom()
    {
        int T0 = 0;
        int T1 = 0;
        foreach(var p in PhotonNetwork.PlayerList)
        {
            if (p == PhotonNetwork.LocalPlayer) continue;
            if(p.CustomProperties.ContainsKey("Team"))
            {
                if ((ZDTeams)p.CustomProperties["Team"] == ZDTeams.T0)
                {
                    T0++;
                }
                else if ((ZDTeams)p.CustomProperties["Team"] == ZDTeams.T1)
                {
                    T1++;
                }
                else
                {
                    continue;
                }
            }
        }
        ZDTeams teams = T0 > T1 ? ZDTeams.T1 : ZDTeams.T0;
        Hashtable props = new Hashtable
            {
                {"Team", teams}
            };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);

        if(PlayerListEntries == null)
        {
            PlayerListEntries = new Dictionary<string, GameObject>();
        }
        
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Here");
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsVisible = true;
        roomOptions.IsOpen = true;
        roomOptions.MaxPlayers = 6;

        PhotonNetwork.CreateRoom("DemoRoom", roomOptions, null);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        EntryUpdate(targetPlayer);
        if (CheackAllReady())
        {
            // Battle
            Debug.Log("All Ready");
            PhotonNetwork.LoadLevel("GameGround");
        }
        else
        {
            Debug.Log("Not yet");
        }
    }
    #endregion
}
