using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using ZoneDepict.Rule;

enum GameState
{
    Initialize,
    Prepare,
    Play,
    End,
    Close,
};

public class ZDGameManager : MonoBehaviourPunCallbacks
{
    
    public static ZDGameManager Instance = null;
    GameState gameState = GameState.Initialize;
    #region Unity
    public void Awake()
    {
        if (Instance)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        Initialize();
        gameState = GameState.Prepare;
    }

    void Update()
    {

    }
    #endregion

    void Initialize()
    {
        int Team;
        Vector3 Position = new Vector3(0, 0, 0);
        Quaternion Rotation = Quaternion.Euler(0, 0, 0);
        //設定主相機要跟著這隻角色
        if (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Team")) 
        {
            Team = 0;
        }
        else
        {
            Team = (int)PhotonNetwork.LocalPlayer.CustomProperties["Team"];
        }

        CameraController.SetTarget(PhotonNetwork.Instantiate("Ruso", Position, Rotation, 0));

        // If server , build scene objects.
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(SpwanBox());
        }
    }

    void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {

    }

    #region Game Procedures
    private void StartGame()
    {
    }
    #endregion

    #region Coroutines
    private IEnumerator SpwanBox()
    {
        System.Random rnd = new System.Random();
        // Basic Spwan Box function , can set some condition
        // This is random number for waiting Spwan Box
        for(int i = 0; i < 3; ++i)
        {
            yield return new WaitForSeconds(0.5f);
            Vector3 Position = ZDGameRule.WorldUnify(new Vector3(rnd.Next(-4, 4), rnd.Next(-4, 4), 0));
            Quaternion Rotation = Quaternion.Euler(0, 0, 0);
            // This can create more infos of this Object 
            object[] instantiationData = { 0, 1 };
            PhotonNetwork.InstantiateSceneObject("TreasureBox", Position, Rotation, 0, instantiationData);
        }
    }
    #endregion
}
