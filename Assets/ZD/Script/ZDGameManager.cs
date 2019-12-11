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
using ZoneDepict.UI;
using Hashtable =  ExitGames.Client.Photon.Hashtable;

namespace ZoneDepict
{
    public enum ZDGameState
    {
        Initialize,
        Play,
        End,
        Close,
    };

    public enum ZDGameEvent
    {
        SpawnEffect = 80,
        StartGame = 90,
        EndGame = 100,
    }

    public enum ZDTeams
    {
        T0,
        T1,
        Total,
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

    public struct PlayerProps
    {
        public int Team;
        public bool Alive;
        public string CharacterType;
        public GameObject Object;
    }

    public class ZDGameManager : MonoBehaviourPunCallbacks, IOnEventCallback
    {
        private bool HotKey = false;

        #region Feilds
        public static ZDGameState gameState = ZDGameState.Initialize;
        public static ZDGameManager Instance;
        public static PlayerProps playerProps;
        public static Vector2[] TeamSpawnUnit =
        {
            new Vector2(-9,7),
            new Vector2(9,-7),
        };
        private static SpawnObjectConfig[] StaticMapObjectConfigs = {
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
                name = "WoodBox",
                pos = new Vector2[]
                {
                    new Vector2(3,-1),
                    new Vector2(5,3),
                    new Vector2(6,3),
                    new Vector2(8,-2),
                    new Vector2(10,6),
                    new Vector2(-6,8),
                    new Vector2(-6,7),
                    new Vector2(-6,6),
                },
                flipX = true,
                flipY = true,
            },
            new SpawnObjectConfig
            {
                name = "SpawnPoint",
                pos = new Vector2[]
                {
                    new Vector2(-9,7),
                },
                flipX = true,
                flipY = true,
            },
        };

        Dictionary<Player, int> TeamList = new Dictionary<Player, int>();   
        //Component
        protected AudioSource audioSource;
        //Audio Clips
        public AudioClip EndGameMusic;
        //Scene Objects
        public GameObject Lose;
        public GameObject Victory;
        #endregion

        #region Unity
        void Start()
        {
            if (Instance && Instance != this) Destroy(this);
            else Instance = this;

            //Cache Component
            audioSource = GetComponent<AudioSource>();

            //Initialize Game Resources.
            Initialize();
        }

        void Update()
        {
            if(Input.GetKeyDown(KeyCode.G))
            {
                Debug.Log(PhotonNetwork.LocalPlayer.CustomProperties["Team"]);
            }
        }
        #endregion

        #region Game Process
        void Initialize()
        {
            //if (ZDUI.InstanceObject) ZDUI.InstanceObject.SetActive(false);

            //Fetch Player Props;
            SetupPlayerProps();

            // If Master ,Build Network Scene Objects.
            if (PhotonNetwork.IsMasterClient)
            {
                GenerateStaticMapObjects();
            }
            //Create Character
            CreatePlayerCharacter();
            //Record Team 
            BuildTeamList();
            

            //Tell Everyone I'm Ready To Play
            Hashtable props = new Hashtable
            {
                {ZDGameRule.CustomPropsKey.PLAYER_LOADED_LEVEL, true}
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }

        void GameStart()
        {
            //if(ZDUI.InstanceObject)ZDUI.InstanceObject.SetActive(true);
            gameState = ZDGameState.Play;
        }

        void SetupPlayerProps()
        {
            Hashtable PlayerCustomProps = PhotonNetwork.LocalPlayer.CustomProperties;
            if (PlayerCustomProps.ContainsKey("CharacterName"))
            {
                playerProps.CharacterType = (string)PlayerCustomProps["CharacterName"];
            }
            else
            {
                playerProps.CharacterType = "Ruso";
            }
            if (PlayerCustomProps.ContainsKey("Team"))
            {
                int PropsTeam = (int)PlayerCustomProps["Team"];
                playerProps.Team = PropsTeam >= 0 && PropsTeam < TeamSpawnUnit.Length ? (int)PlayerCustomProps["Team"] : 0;
            }
            else
            {
                playerProps.Team = 0;
            }
            if (PlayerCustomProps.ContainsKey("Alive"))
            {
                playerProps.Alive = (bool)PlayerCustomProps["Alive"];
            }
            else
            {
                playerProps.Alive = false;
            }
        }

        void GenerateStaticMapObjects()
        {
            //Check Validation
            if (PhotonNetwork.IsMasterClient && StaticMapObjectConfigs != null)
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
            //Clean Up
            StaticMapObjectConfigs = null;
        }

        void CreatePlayerCharacter()
        {
            //Spawn Character
            playerProps.Object = PhotonNetwork.Instantiate(playerProps.CharacterType,
                                                           TeamSpawnUnit[playerProps.Team] * ZDGameRule.UnitInWorld,
                                                           Quaternion.identity);
            //Setup Camera
            CameraController.SetTarget(playerProps.Object);

        }

        void BuildTeamList()
        {
            //Initialize Current TeamList
            foreach (var player in PhotonNetwork.PlayerList)
            {
                if (player.CustomProperties.ContainsKey("Alive") && (bool)player.CustomProperties["Alive"])
                {
                    int Team = 0;
                    if (player.CustomProperties.ContainsKey("Team"))
                    {
                        int playerTeam = (int)player.CustomProperties["Team"];
                        if (TeamIsValid(playerTeam)) Team = playerTeam;
                    }
                    AddTeam((int)player.CustomProperties["Team"], player);
                }
            }
        }
        #endregion

        #region Helper
        bool TeamIsValid(int Team)
        {
            if (Team >= 0 && Team < (int)ZDTeams.Total) return true;
            return false;
        }

        void AddTeam(int teamID, Player player)
        {
            if (!TeamList.ContainsKey(player)) TeamList[player] = teamID;
        }

        bool IsGameEnd()
        {
            if (TeamList.Count != 0)
            {
                
                var enumerator = TeamList.Values.GetEnumerator();
                enumerator.MoveNext();
                int AliveTeam = enumerator.Current;
                foreach (var player in TeamList)
                {
                    if (player.Value != AliveTeam)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        void CheckGameEnded()
        {
            if ((PhotonNetwork.IsMasterClient && IsGameEnd()) || HotKey) SendGameEvent(ZDGameEvent.EndGame);
        }

        void SendGameEvent(ZDGameEvent SendEvent)
        {
            byte evCode; // Custom Event 1: Used as "MoveUnitsToTargetPosition" event
            object[] content = { }; // Array contains the target position and the IDs of the selected units
            //Setup evCode and Contents
            switch (SendEvent)
            {
                case ZDGameEvent.StartGame:
                    evCode = (int)ZDGameEvent.StartGame;
                    break;
                case ZDGameEvent.EndGame:
                    int WinningTeam = -1;
                    if (TeamList.Count > 0) WinningTeam = TeamList.Values.GetEnumerator().Current;
                    content = new object[] { WinningTeam };
                    evCode = (int)ZDGameEvent.EndGame;
                    break;
                default:
                    return;
            }
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
            SendOptions sendOptions = new SendOptions { Reliability = true };
            PhotonNetwork.RaiseEvent(evCode, content, raiseEventOptions, sendOptions);
            return;

        }

        void InstantiateAtUnit(SpawnObjectConfig target, Vector3 Pos)
        {
            Pos *= ZDGameRule.UnitInWorld;
            string ObjPath = ZDAssetTable.GetPath(target.name);
            PhotonNetwork.InstantiateSceneObject(ObjPath, Pos, Quaternion.identity);
            if (target.flipX || target.flipY)
            {
                if (target.flipX) Pos.x = -Pos.x;
                if (target.flipY) Pos.y = -Pos.y;
                PhotonNetwork.InstantiateSceneObject(ObjPath, Pos, Quaternion.identity);
            }
        }

        protected bool CheckAllPlayerLoadedLevel()
        {
            foreach (Player play in PhotonNetwork.PlayerList)
            {
                object playerLoadedLevel;
                if (play.CustomProperties.TryGetValue(ZDGameRule.CustomPropsKey.PLAYER_LOADED_LEVEL, out playerLoadedLevel))
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

        IEnumerator WaitToRestart()
        {
            yield return new WaitForSeconds(5);
            PhotonNetwork.LoadLevel("GameStartView");
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

            if (changedProps.ContainsKey(ZDGameRule.CustomPropsKey.PLAYER_LOADED_LEVEL))
            {
                if (CheckAllPlayerLoadedLevel() && PhotonNetwork.IsMasterClient)
                {
                    SendGameEvent(ZDGameEvent.StartGame);
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
                case (int)ZDGameEvent.StartGame:
                    GameStart();
                    break;
                case (int)ZDGameEvent.EndGame:
                    if (audioSource.isPlaying) audioSource.Stop();
                    audioSource.PlayOneShot(EndGameMusic);
                    Debug.Log("Game Ended");
                    object[] dataEnd = (object[])photonEvent.CustomData;
                    Debug.Log(dataEnd[0] + " is winner!");
                    Debug.Log((int)PhotonNetwork.LocalPlayer.CustomProperties["Team"]);
                    if ((int)PhotonNetwork.LocalPlayer.CustomProperties["Team"] == (int)dataEnd[0]) Victory.SetActive(true);
                    else Lose.SetActive(true);
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
    }
}