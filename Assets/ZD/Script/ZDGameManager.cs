using System;
using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using ZoneDepict.Rule;
using ZoneDepict.Map;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Random = UnityEngine.Random;

namespace ZoneDepict
{

    public enum ZDGameState
    {
        Initialize,
        WaitPlayer,
        OpenGame,
        Play,
        End,
        Close,
        Unknown,
    };

    public enum ZDGameEvent
    {
        RestrictAnounce=70,
        RestrictPrepare,
        Restrict,
        RestrictEnd,
        SpawnEffect = 80,
        OpenGame = 90,
        StartGame,
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

    public struct PlayerProps
    {
        public int Team;
        public bool Alive;
        public string CharacterType;
        public GameObject Object;
        public Character Script;
    }

    public class ZDGameManager : MonoBehaviourPunCallbacks, IOnEventCallback
    {
        //Test Params
        public bool DoRestrictZone;


        #region Feilds
        //Saved Infos
        public static ZDGameManager Instance;
        protected ZDGameState gameState = ZDGameState.Initialize;
        protected PlayerProps playerProps;

        //Spawn Points
        public static Vector2[] TeamSpawnUnit =
        {
            new Vector2(-9,7),
            new Vector2(9,-7),
            //new Vector2(0,0),
            //new Vector2(0,0),
        };

        //Map Object Configs
        private SpawnObjectConfig[] StaticMapObjectConfigs = {
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
            //new SpawnObjectConfig
            //{
            //    name = "SpawnPoint",
            //    pos = new Vector2[]
            //    {
            //        new Vector2(-9,7),
            //    },
            //    flipX = true,
            //    flipY = true,
            //},
        };
        private string ZoneObjectName = ZDAssetTable.GetPath("ZoneRestrict");


        //WaitPlayer State Deps
        bool AllPlayerLoaded;
        bool hasOpenedGame;
        bool hasEndedGame;

        //OpenGame State Deps
        public int CountDownTime = 3;

        //Play State Deps
        const int ZoneWaves = 5;
        const float ZoneRestrictSpeed = 15;
        readonly float[] ZoneInterval = {10 + ZoneRestrictSpeed,
                                         15 + ZoneRestrictSpeed,
                                         15 + ZoneRestrictSpeed,
                                         15 + ZoneRestrictSpeed,
                                         15 + ZoneRestrictSpeed };
        readonly float[] ZoneSizeRate = { 0.95f, 0.85f, 0.7f, 0.5f, 0.3f};
        int CurrentWave;



        //Components
        protected AudioSource audioSource;
        //Audio Clips
        public AudioClip EndGameVictoryMusic;
        public AudioClip EndGameLoseMusic;
        public AudioClip CountDownSingle;
        public AudioClip StartGameMusic;
        //Scene Objects
        [Header("Scene Views")]
        public GameObject Lose;
        public GameObject Victory;
        public GameObject LoadingView;
        public GameObject CountDownView; 

        #endregion

        #region Unity
        void Start()
        {
            if (Instance && Instance != this) Destroy(this);
            else Instance = this;
            
            //Initialize Static Values.
            gameState = ZDGameState.Initialize;
            playerProps = new PlayerProps();

            //Cache Component
            audioSource = GetComponent<AudioSource>();

            //Initialize Game Resources.
            Initialize();
        }

        void FixedUpdate()
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                if ( playerProps.Script!=null)
                    playerProps.Script.Hurt(100);
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                if (playerProps.Script != null)
                    playerProps.Script.SetSoul(5);
            }

            if (PhotonNetwork.IsMasterClient)
            {
                MasterClientRoutine();
            }
            
        }

        void MasterClientRoutine()
        {
            switch (gameState)
            {
                case ZDGameState.WaitPlayer:
                    if (!hasOpenedGame &&
                        AllPlayerLoaded &&
                        CheckZonePrepared())
                    {
                        hasOpenedGame = true;
                        SendGameEvent(ZDGameEvent.OpenGame);
                    }
                break;
                case ZDGameState.Play:
                break;
                    
            }
        }

        void OnDestroy()
        {
            if(Instance == this)
            {
                Instance = null;
                ZDMap.ResetMap();
            }
        }
        #endregion

        #region Game Process Functions
        void Initialize()
        {
            //if (ZDUI.InstanceObject) ZDUI.InstanceObject.SetActive(false);

            //Fetch Player Props;
            SetupPlayerProps();

            // If Master ,Build Network Scene Objects.
            if (PhotonNetwork.IsMasterClient)
            {
                GenerateStaticMapObjects();
                GenerateZone();
            }
            //Create Character
            CreatePlayerCharacter();

            //Tell Everyone I'm Ready To Play
            Hashtable props = new Hashtable
            {
                { CustomPropsKey.PLAYER_LOADED_LEVEL, true}
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);

            //Set Game State To Wait Player
            gameState = ZDGameState.WaitPlayer;
            if (PhotonNetwork.IsMasterClient)
            {
                CheckPlayersLoadedLevel();
            }
        }

        void OpenGame()
        {
            gameState = ZDGameState.OpenGame;
            StartCoroutine(OpenGameRoutine());
        }

        void StartGame()
        {
            gameState = ZDGameState.Play;
        }

        void EndGame(object InData)
        {
            gameState = ZDGameState.End;
            if (audioSource && audioSource.isPlaying) audioSource.Stop();
            
            object[] data = (object[])InData;
            if ((int)PhotonNetwork.LocalPlayer.CustomProperties["Team"] == (int)data[0])
            {
                if(audioSource && EndGameVictoryMusic)
                    audioSource.PlayOneShot(EndGameVictoryMusic);
                Victory.SetActive(true);
            }
            else
            {
                if(audioSource && EndGameLoseMusic)
                    audioSource.PlayOneShot(EndGameLoseMusic);
                Lose.SetActive(true);
            }
                
            // Go back to start view
            StartCoroutine(WaitToRestart());
        }
        #endregion

        #region Initialize Phase Helper
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
                                    SpawnObjectUnitPos(config, new Vector3(tx, ty, 0));
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
                            SpawnObjectUnitPos(config, pos);
                        }
                    }
                }
            }
            //Clean Up
            StaticMapObjectConfigs = null;
        }

        void GenerateZone()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                //int BorderH = (int)(ZDGameRule.MAP_HEIGHT_UNIT * 0.2);
                //int BorderW = (int)(ZDGameRule.MAP_WIDTH_UNIT * 0.2);
                Vector3 RandomZoneLocation = new Vector2((int)Random.Range(0, ZDGameRule.MAP_WIDTH_UNIT),
                                                         (int)Random.Range(0, ZDGameRule.MAP_HEIGHT_UNIT));
                RandomZoneLocation -= (new Vector3((int)ZDGameRule.MAP_WIDTH_UNIT / 2, ZDGameRule.MAP_HEIGHT_UNIT / 2));
                RandomZoneLocation *= ZDGameRule.UNIT_IN_WORLD;
                RandomZoneLocation.z = -2;
                PhotonNetwork.InstantiateSceneObject(ZoneObjectName, RandomZoneLocation, Quaternion.identity);
            }
        }

        void CreatePlayerCharacter()
        {
            //Spawn Character
            object[] CharacterCustomData = { PhotonNetwork.LocalPlayer.NickName };
            playerProps.Object = PhotonNetwork.Instantiate(playerProps.CharacterType,
                                                           TeamSpawnUnit[playerProps.Team] * ZDGameRule.UNIT_IN_WORLD,
                                                           Quaternion.identity,0, CharacterCustomData);
            playerProps.Script = playerProps.Object.GetComponent<Character>();
            //Setup Camera
            CameraController.SetTarget(playerProps.Object);
        }

        void SpawnObjectUnitPos(SpawnObjectConfig target, Vector3 Pos)
        {
            Pos *= ZDGameRule.UNIT_IN_WORLD;
            string ObjPath = ZDAssetTable.GetPath(target.name);
            PhotonNetwork.InstantiateSceneObject(ObjPath, Pos, Quaternion.identity);
            if (target.flipX || target.flipY)
            {
                if (target.flipX) Pos.x = -Pos.x;
                if (target.flipY) Pos.y = -Pos.y;
                PhotonNetwork.InstantiateSceneObject(ObjPath, Pos, Quaternion.identity);
            }
        }
        #endregion

        #region WaitPlayer Phase Helper
        protected void CheckPlayersLoadedLevel()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                AllPlayerLoaded = HasAllPlayerLoadedLevel();
            }
        }

        protected bool HasAllPlayerLoadedLevel()
        {
            foreach (Player play in PhotonNetwork.PlayerList)
            {
                object playerLoadedLevel;
                if (play.CustomProperties.TryGetValue(CustomPropsKey.PLAYER_LOADED_LEVEL, out playerLoadedLevel))
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

        protected bool CheckZonePrepared()
        {
            if (RestrictZone.Instance != null || RestrictZone.TryInitialized) return true;
            return false;
        }
        #endregion

        #region OpenGame Phase Helper
        IEnumerator OpenGameRoutine()
        {
            int CountDown = CountDownTime;
            while(CountDown > 0)
            {
                CountDownView.SetActive(true);
                if (audioSource && CountDownSingle) audioSource.PlayOneShot(CountDownSingle);
                yield return new WaitForSeconds(1);
                CountDown -= 1;
            }
            if (audioSource && StartGameMusic) audioSource.PlayOneShot(StartGameMusic);
            if (PhotonNetwork.IsMasterClient)
            {
                SendGameEvent(ZDGameEvent.StartGame);
                StartCoroutine(ZoneRestrictRoutine());
            }
        }
        #endregion

        #region Play Phase Helper
        public void PlayerCharacterDied()
        {
            if(playerProps.Script &&
               playerProps.Script.photonView.IsMine &&
               playerProps.Script.currentState == CharacterState.Dead)
            {
                Hashtable NewSetting = new Hashtable();
                NewSetting.Add("Alive", false);
                PhotonNetwork.SetPlayerCustomProperties(NewSetting);
                if (PhotonNetwork.IsMasterClient)
                {
                    CheckGameEnded();
                }
            }
        }

        IEnumerator ZoneRestrictRoutine()
        {
            if (RestrictZone.Instance && CurrentWave < ZoneWaves && DoRestrictZone)
            {
                float LeftTime = ZoneInterval[CurrentWave];
                while (LeftTime > 0)
                {
                    yield return new WaitForSeconds(0.1f);
                    if (gameState == ZDGameState.Play)
                    {
                        LeftTime -= 0.1f;
                    }
                }

                if (DoRestrictZone)
                {
                    RestrictZone.Instance.ShrinkZone(new Vector2Int((int)(ZDGameRule.MAP_WIDTH_UNIT * ZoneSizeRate[CurrentWave]),
                                                                    (int)(ZDGameRule.MAP_HEIGHT_UNIT * ZoneSizeRate[CurrentWave])),ZoneRestrictSpeed);
                }
                CurrentWave += 1;
                StartCoroutine(ZoneRestrictRoutine());
            }
        }
        #endregion

        #region End Phase Helper
        IEnumerator WaitToRestart()
        {
            
            yield return new WaitForSeconds(5);
            PhotonNetwork.LeaveRoom();
        }
        #endregion

        #region Game State Helper
        bool  IsGameEnd(out int SurviveTeam)
        {
            SurviveTeam = -1;
            foreach (var player in PhotonNetwork.PlayerList)
            {
                if (player.CustomProperties.ContainsKey("Alive") && (bool)player.CustomProperties["Alive"])
                {
                    int playerTeam = 0;
                    if (player.CustomProperties.ContainsKey("Team"))
                    {
                        playerTeam = (int)player.CustomProperties["Team"];
                    }


                    if (SurviveTeam < 0)
                    {
                        SurviveTeam = playerTeam;
                    }
                    else if (SurviveTeam != playerTeam)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        void CheckGameEnded()
        {
            int SurviveTeam;
            if (!hasEndedGame &&PhotonNetwork.IsMasterClient && IsGameEnd(out SurviveTeam))
            {
                hasEndedGame = true;
                SendGameEvent(ZDGameEvent.EndGame, new object[] { SurviveTeam });
            }
        }
        #endregion

        #region Helper
        bool TeamIsValid(int Team)
        {
            if (Team >= 0 && Team < (int)ZDTeams.Total) return true;
            return false;
        }

        static public PlayerProps GetPlayerProps()
        {
            if(Instance == null)
            {
                return new PlayerProps();
            }
            return Instance.playerProps;
        }

        static public ZDGameState GetGameState()
        {
            if (Instance == null) return ZDGameState.Unknown;
            else return Instance.gameState;
        }
        #endregion

        #region Game Event Helper
        void SendGameEvent(ZDGameEvent SendEvent, object[] content = null)
        {
            byte evCode;
            switch (SendEvent)
            {
                case ZDGameEvent.EndGame:
                    evCode = (int)ZDGameEvent.EndGame;
                    break;
                default:
                    evCode = (byte)SendEvent;
                    break;
            }
            if (content == null) content = new object[] { };
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
            SendOptions sendOptions = new SendOptions { Reliability = true };
            PhotonNetwork.RaiseEvent(evCode, content, raiseEventOptions, sendOptions);
            return;

        }
        #endregion

        #region Photon Callbacks
        public override void OnLeftRoom()
        {
            base.OnLeftRoom();
            PhotonNetwork.Disconnect();
            PhotonNetwork.LoadLevel("GameStartView");
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            
            switch (gameState)
            {
                case ZDGameState.WaitPlayer:
                    if (changedProps.ContainsKey(CustomPropsKey.PLAYER_LOADED_LEVEL))
                    {
                        CheckPlayersLoadedLevel();
                    }
                break;
                case ZDGameState.OpenGame:
                case ZDGameState.Play:
                    if (changedProps.ContainsKey("Alive") &&
                        !(bool)changedProps["Alive"] &&
                        PhotonNetwork.IsMasterClient) CheckGameEnded();
               break;
            }
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                switch (gameState)
                {
                    case ZDGameState.WaitPlayer:
                        CheckPlayersLoadedLevel();
                        break;
                    case ZDGameState.OpenGame:
                    case ZDGameState.Play:
                        CheckGameEnded();
                        break;
                }
            }
        }

        public void OnEvent(EventData photonEvent)
        {
            switch ((ZDGameEvent)photonEvent.Code)
            {
                case ZDGameEvent.OpenGame:
                    Destroy(LoadingView);
                    OpenGame();
                    break;
                case ZDGameEvent.StartGame:
                    StartGame();
                    break;
                case ZDGameEvent.EndGame:
                    EndGame(photonEvent.CustomData);
                    break;
                case ZDGameEvent.SpawnEffect:
                    object[] data = (object[])photonEvent.CustomData;
                    Instantiate(ZDAssetTable.GetObject((string)data[0]),(Vector3)data[1],(Quaternion)data[2]);
                    break;
            }
        }
        #endregion
    }
}
