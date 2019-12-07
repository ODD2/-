using System;
using System.Collections;
using System.Collections.Generic;

using Photon;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using UnityEngine;
using ExitGames.Client.Photon;
using ZoneDepict.Rule;
using Hashtable =  ExitGames.Client.Photon.Hashtable;

namespace ZoneDepict
{
    enum ZDGameState
    {
        Initialize,
        Prepare,
        Play,
        End,
        Close,
    };

    enum ZDGameEvent
    {
        EndGame = 100,
    }

    public class ZDGameManager : MonoBehaviourPunCallbacks, IOnEventCallback
    {
        Dictionary<Player, int> TeamList = new Dictionary<Player, int>();
        public static ZDGameManager Instance = null;
        ZDGameState gameState = ZDGameState.Initialize;
        public static GameObject PlayerObject { get; set; } = null;

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
            gameState = ZDGameState.Prepare;
        }

        void Update()
        {

        }
        #endregion

        #region Game Procedures
        void Initialize()
        {
            //Spawn Player Character.
            int Team;
            Vector3 Position = new Vector3(0, 0, 0);
            Quaternion Rotation = Quaternion.Euler(0, 0, 0);
            if (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Team"))
            {
                Team = 0;
            }
            else
            {
                Team = (int)PhotonNetwork.LocalPlayer.CustomProperties["Team"];
            }
            //Setup Camera
            string CharacterName = (string)PhotonNetwork.LocalPlayer.CustomProperties["CharacterName"];
            PlayerObject = PhotonNetwork.Instantiate(CharacterName, Position, Rotation, 0);
            CameraController.SetTarget(PlayerObject);

            //Update TeamList
            foreach (var player in PhotonNetwork.PlayerList)
            {
                if (player.CustomProperties.ContainsKey("Alive") && (bool)player.CustomProperties["Alive"])
                {
                    if (!player.CustomProperties.ContainsKey("Team"))
                    {
                        AddTeam(0, player);
                    }
                    else
                    {
                        AddTeam((int)player.CustomProperties["Team"], player);
                    }
                }
            }

            // If master , build scene objects.
            if (PhotonNetwork.IsMasterClient)
            {
                StartCoroutine(SpawnBox());
            }
        }

        private void StartGame()
        {
        }
        #endregion

        #region Helper Functions
        void AddTeam(int teamID, Player player)
        {
            if (!TeamList.ContainsKey(player))
            {
                TeamList[player] = teamID;
            }
        }

        bool IsGameEnd()
        {
            int TeamAlive = -1;
            foreach (var player in TeamList)
            {
                if (TeamAlive < 0)
                {
                    TeamAlive = player.Value;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        void CheckGameEnded()
        {
            if (PhotonNetwork.IsMasterClient && IsGameEnd())
            {
                SendEndGameEvent();
            }
        }

        private void SendEndGameEvent()
        {
            byte evCode = 100; // Custom Event 1: Used as "MoveUnitsToTargetPosition" event
            object[] content = new object[] { new Vector3(10.0f, 2.0f, 5.0f), 1, 2, 5, 10 }; // Array contains the target position and the IDs of the selected units
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
            SendOptions sendOptions = new SendOptions { Reliability = true };
            PhotonNetwork.RaiseEvent(evCode, content, raiseEventOptions, sendOptions);
        }
        #endregion

        #region Coroutines
        private IEnumerator SpawnBox()
        {
            System.Random rnd = new System.Random();
            // Basic Spwan Box function , can set some condition
            // This is random number for waiting Spwan Box
            for (int i = 0; i < 3; ++i)
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

        #region Photon Callbacks
        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            if (changedProps.ContainsKey("Alive") && !(bool)changedProps["Alive"])
            {
                if (TeamList.ContainsKey(targetPlayer))
                {
                    TeamList.Remove(targetPlayer);
                    if (PhotonNetwork.IsMasterClient)
                    {
                        CheckGameEnded();
                    }
                }

            }
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            if (newPlayer.CustomProperties.ContainsKey("Alive") && (bool)newPlayer.CustomProperties["Alive"])
            {
                if (!newPlayer.CustomProperties.ContainsKey("Team"))
                {
                    AddTeam(0, newPlayer);
                }
                else
                {
                    AddTeam((int)newPlayer.CustomProperties["Team"], newPlayer);
                }
            }
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            if (TeamList.ContainsKey(otherPlayer))
            {
                TeamList.Remove(otherPlayer);
                if (PhotonNetwork.IsMasterClient)
                {
                    CheckGameEnded();
                }
            }
        }

        public void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code == (int)ZDGameEvent.EndGame)
            {
                Debug.Log("Game Ended");
            }
        }
        #endregion

        #region RPC
        [PunRPC]
        void GameEnded()
        {
            gameState = ZDGameState.End;
        }
        #endregion
    }
}