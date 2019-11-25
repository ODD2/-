using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;
public class NetWorkManager : MonoBehaviourPunCallbacks
{
    public GameObject connect;
    public GameObject disconnect;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    public void onClick()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        connect.SetActive(true);
    }
    
}
