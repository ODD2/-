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

    public enum ZDGameEvent
    {
        SpawnEffect = 80,
        EndGame = 100,
    }

    [Serializable]
    public struct SpawnObjectConfig
    {
        public string name;
        public Vector2[] pos;
        public bool line;
        public bool flipX;
        public bool flipY;
    }

    public class ZDGameManager : MonoBehaviourPunCallbacks, IOnEventCallback
    {
        private bool HotKey = false;


        #region Feilds
        ZDGameState gameState = ZDGameState.Initialize;
        public static ZDGameManager Instance;
        public static GameObject PlayerObject { get; set; } = null;
        private static readonly SpawnObjectConfig[] StaticMapObjectConfigs = {
            new SpawnObjectConfig
            {
                name = "Grass",
                pos = new Vector2[]
                {
                    new Vector2(-1,1),
                    new Vector2(-2,2),
                    new Vector2(-1,-1),
                    new Vector2(-2,-2),

                    new Vector2(-5,-1),
                    new Vector2(-6,-2),
                    new Vector2(-2,-5),
                    new Vector2(-3,-5),

                    new Vector2(-9,2),
                    new Vector2(-9,-4),
                    new Vector2(-10,2),
                    new Vector2(-10,1),
                    new Vector2(-10,-3),
                    new Vector2(-10,-5),

                    new Vector2(-1,8),
                    new Vector2(-2,8),
                    new Vector2(0,-8),
                    new Vector2(-1,-8)
                },
                flipX = true,
                flipY = true,
                line = true,
            },
            new SpawnObjectConfig
            {
                name = "Bamboo",
                pos = new Vector2[]
                {
                    new Vector2(-9,-6),
                    new Vector2(-8,-6),
                    new Vector2(-5,-5),
                    new Vector2(-4,-5),
                    new Vector2(-4,7),
                    new Vector2(-3,7),
                    new Vector2(3,-8),
                    new Vector2(4,-8),
                    new Vector2(4,4),
                    new Vector2(5,4),
                    new Vector2(8,5),
                    new Vector2(9,5),
                },
            },
            new SpawnObjectConfig
            {
                name = "Tree",
                pos = new Vector2[]
                {
                    new Vector2(-10,-8),
                    new Vector2(-5,5),
                    new Vector2(4,6),
                    new Vector2(9,7)
                },
            },
            new SpawnObjectConfig
            {
                name = "Stone",
                pos = new Vector2[]
                {
                    new Vector2(-10,0),
                    new Vector2(-10,-2),
                    new Vector2(-6,-6),
                    new Vector2(-6,-7),
                    new Vector2(-5,8),
                    new Vector2(-5,7),
                    new Vector2(-1,4),
                    new Vector2(-1,5),
                },
                line  = true,
                flipX = true,
                flipY = true
            },
            new SpawnObjectConfig
            {
                name = "Stone",
                pos = new Vector2[]
                {
                    new Vector2(0,2),
                    new Vector2(2,0),
                },
                flipY = true,
                flipX = true,
            },
            new SpawnObjectConfig
            {
                name = "Mountain",
                pos = new Vector2[]
                {
                    new Vector2(9,-4)
                },
                flipX = true,
                flipY = true
            },
            new SpawnObjectConfig
            {
                name = "TreasureBox",
                pos = new Vector2[]
                {
                    new Vector2(3,-1),
                    new Vector2(5,3),
                    new Vector2(6,3),
                    new Vector2(8,-2),
                    new Vector2(10,6),
                },
                flipX = true,
                flipY = true,
            },
        };
        Dictionary<Player, int> TeamList = new Dictionary<Player, int>();
        public GameObject Lose;
        public GameObject Victory;
        public GameObject ZDUI;
        private int TeamAlive = -1;
        #endregion
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
            //Initialize();
            gameState = ZDGameState.Prepare;
            // Wait for everybody in
            Hashtable props = new Hashtable
            {
                {ZDGameRule.PLAYER_LOADED_LEVEL, true}
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }

        void Update()
        {
            if(Input.GetKeyDown(KeyCode.G))
            {
                //HotKey = true;
                //CheckGameEnded();
                Debug.Log(PhotonNetwork.LocalPlayer.CustomProperties["Team"]);
            }
            
        }
        #endregion

        #region Game Procedures
        void Initialize()
        {
            // If Master ,Build  Network Scene Objects.
            if (PhotonNetwork.IsMasterClient)
            {
                GenerateStaticMapObjects();
            }
            //Build Common Scene Objects.
            

            //Spawn Player Character.
            Vector3 Position = new Vector3(0, 0, 0);
            Quaternion Rotation = Quaternion.Euler(0, 0, 0);
            if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("CharacterName"))
            {
                string CharacterName = (string)PhotonNetwork.LocalPlayer.CustomProperties["CharacterName"];
                PlayerObject = PhotonNetwork.Instantiate(CharacterName, Position, Rotation, 0);
            }
            else
            {
                PlayerObject = PhotonNetwork.Instantiate("Ruso", Position, Rotation, 0);
            }

            //Setup Camera
            CameraController.SetTarget(PlayerObject);

            //Initialize TeamList
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

            ZDUI.SetActive(true);
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
            if ((PhotonNetwork.IsMasterClient && IsGameEnd()) || HotKey)
            {
                
                SendEndGameEvent();
            }
        }

        private void SendEndGameEvent()
        {
            byte evCode = 100; // Custom Event 1: Used as "MoveUnitsToTargetPosition" event
            object[] content = { TeamAlive }; // Array contains the target position and the IDs of the selected units
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
            SendOptions sendOptions = new SendOptions { Reliability = true };
            PhotonNetwork.RaiseEvent(evCode, content, raiseEventOptions, sendOptions);
        }

        void GenerateStaticMapObjects()
        {
            foreach (var config in StaticMapObjectConfigs)
            {
                if (config.line)
                {
                    int end = config.pos.Length;
                    if (config.pos.Length % 2 != 0) end -= 1;
                    //Iterate In 2 Units
                    for (int i = 0; i < end; i += 2)
                    {
                        int x, _x, dx;
                        int y, _y, dy;
                        x = (int)config.pos[i].x; _x = (int)config.pos[i + 1].x; dx = _x > x ? 1 : -1;
                        y = (int)config.pos[i].y; _y = (int)config.pos[i + 1].y; dy = _y > y ? 1 : -1;
                        for (int tx = x; ; tx += dx)
                        {
                            for (int ty = y; ; ty += dy)
                            {
                                InstantiateAtUnit(config, new Vector3(tx, ty, 0));
                                if (ty == _y) break;
                            }
                            if (tx == _x) break;
                        }
                    }
                }
                else
                {
                    foreach (var pos in config.pos)
                    {
                        InstantiateAtUnit(config, pos);
                    }
                }
            }
        }

        void InstantiateAtUnit(SpawnObjectConfig target, Vector3 Pos)
        {
            Pos *= ZDGameRule.UnitInWorld;
            string ObjPath = ZDAssetTable.GetPath(target.name);
            PhotonNetwork.InstantiateSceneObject(ObjPath,Pos,Quaternion.identity);
            if (target.flipX || target.flipY)
            {
                if (target.flipX) Pos.x = -Pos.x;
                if (target.flipY) Pos.y = -Pos.y;
                PhotonNetwork.InstantiateSceneObject(ObjPath, Pos,Quaternion.identity);
            }
        }

        private bool CheckAllPlayerLoadedLevel()
        {
            foreach (Player play in PhotonNetwork.PlayerList)
            {
                object playerLoadedLevel;

                if (play.CustomProperties.TryGetValue(ZDGameRule.PLAYER_LOADED_LEVEL, out playerLoadedLevel))
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
        #endregion

        #region Coroutines
        IEnumerator WaitToRestart()
        {
            yield return new WaitForSeconds(5);
            PhotonNetwork.LoadLevel("GameStartView");
        }
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

            if (changedProps.ContainsKey(ZDGameRule.PLAYER_LOADED_LEVEL))
            {
                if (CheckAllPlayerLoadedLevel())
                {
                    // Start Game
                    Initialize();
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
            switch (photonEvent.Code)
            {

                case (int)ZDGameEvent.EndGame:
                    Debug.Log("Game Ended");
                    object[] dataEnd = (object[])photonEvent.CustomData;
                    Debug.Log(dataEnd[0] + " is winner!");
                    
                    Debug.Log((int)PhotonNetwork.LocalPlayer.CustomProperties["Team"]);
                    
                    if ((int)PhotonNetwork.LocalPlayer.CustomProperties["Team"] == (int)dataEnd[0])
                    {
                        Victory.SetActive(true);
                    }
                    else
                    {
                        Lose.SetActive(true);
                    }
                    // Go back to start view
                    StartCoroutine(WaitToRestart());
                    break;
                case (int)ZDGameEvent.SpawnEffect:
                    object[] data = (object[])photonEvent.CustomData;
                    Instantiate(ZDAssetTable.GetObject((string)data[0]),(Vector3)data[1],(Quaternion)data[2]);
                    break;
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