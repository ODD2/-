using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using ZoneDepict.Rule;
public class ZDGameManager : MonoBehaviourPunCallbacks
{
    public static ZDGameManager Instance = null;

    #region Unity
    public void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        StartGame();
    }
    #endregion

    void Update()
    {
        
    }
    #region StartCoroutine
    private IEnumerator SpwanBox()
    {
        // Basic Spwan Box function , can set some condition
        // This is random number for waiting Spwan Box
        yield return new WaitForSeconds(10.5f);
        Vector3 Position = new Vector3(0, 0, 0);
        Quaternion Rotation = Quaternion.Euler(0, 0, 0);
        // This can create more infos of this Object 
        object[] instantiationData = { 0, 1 };
        PhotonNetwork.InstantiateSceneObject("Box", Position, Rotation, 0, instantiationData);

    }
    #endregion

    private void StartGame()
    {
        
        //Spwan the player
        Vector3 Position = new Vector3(Random.Range(-2, 2), Random.Range(-2, 2), 0);
        Quaternion Rotation = Quaternion.Euler(0, 0, 0);
        PhotonNetwork.Instantiate("Ruso", Position, Rotation, 0);

        // If ur Server , u have to build box in scene
        if (PhotonNetwork.IsMasterClient)
        {
            //StartCoroutine(SpwanBox());
        }
        
    }
    
    private bool CheckAllPlayerLoadedLevel()
    {
        // This check for everyone is in room
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            object playerLoadedLevel;
            if (p.CustomProperties.TryGetValue(ZDGameRule.PLAYER_LOADED_LEVEL, out playerLoadedLevel))
            {
                if ((bool)playerLoadedLevel)
                {
                    continue;
                }
            }

            return false;
        }

        return true;
    }

}
