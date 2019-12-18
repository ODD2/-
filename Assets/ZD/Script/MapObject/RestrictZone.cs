using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using ExitGames.Client.Photon;
using ZoneDepict;
using ZoneDepict.Rule;
using ZoneDepict.UI;
using ZoneDepict.Map;
using Hashtable = ExitGames.Client.Photon.Hashtable;


class RestrictZone : MonoBehaviourPun , IOnEventCallback, IPunObservable
{
    public bool DoDamage = false;


    public enum RestrictZoneState
    {
        Initializing,
        Available,
    }

    public struct CachedCharacterState
    {
        public Vector3 position;
        public bool InZone;
        public float InZoneSeconds;
    }
    public static RestrictZone Instance;
    public static bool TryInitialized;

    //preperation informations.
    private Transform VisibleArea;
    private Transform StrictArea;
    private Rect StrictAreaSpriteRect;
    private Rect VisibleAreaSpriteRect;
    private Vector2 StrictAreaCachedRatio;
    private Vector2 VisibleAreaCachedRatio;

    //The difference of current scale and the rate to change the scale.
    private Vector3 TargetScale,ScaleChangeRate;

    //Cached Informations.
    private CachedCharacterState cachedState;
    private Vector3 cachedVisibleAreaScale;
    private Character TargetCharacter;

    //Zone Depends
    private bool Shrink;

    void Start()
    {
        if (Instance!=null && photonView.IsMine)PhotonNetwork.Destroy(photonView);
                
        if (!Initialize())
        {
            if (photonView.IsMine)
            {
                TryInitialized = true;
                PhotonNetwork.Destroy(photonView);
            }
        }        
        Instance = this;
        if (ZDGameManager.GetPlayerProps().Script != null) {
            TargetCharacter = ZDGameManager.GetPlayerProps().Script;
            TargetCharacter.LocationChanged += OnPlayerLocationChanged ;
        }
    }

    void FixedUpdate()
    {
        if (ZDGameManager.GetGameState()== ZDGameState.Play)
        {
            if (Shrink)
            {
                Vector3 FramScaleChange = ScaleChangeRate * Time.fixedDeltaTime;
                Vector3 DeltaScale = TargetScale - VisibleArea.localScale;
                if(DeltaScale.magnitude < FramScaleChange.magnitude)
                {
                    Shrink = false;
                    VisibleArea.localScale = TargetScale;
                }
                else
                {
                    VisibleArea.localScale += FramScaleChange;
                }
            }
            if (DoDamage)
            {
                if (GetCachedInRestrict(ZDController.GetTargetCharacter()))
                {
                    cachedState.InZoneSeconds += Time.deltaTime;
                    if(cachedState.InZoneSeconds > ZDGameRule.RestrictZone.HurtThresh)
                    {
                        cachedState.InZoneSeconds = 0;
                        ZDController.GetTargetCharacter().Hurt(10);
                    }
                }
                else if(cachedState.InZoneSeconds.Equals(0))
                {
                    cachedState.InZoneSeconds = 0;
                }
            }
        }
    }

    bool Initialize()
    {
        SpriteRenderer StrictAreaSpriteRender;
        SpriteMask VisibleAreaSpriteMask;
        VisibleArea = transform.Find("VisibleArea");
        StrictArea = transform.Find("StrictArea");      
        if (VisibleArea==null || StrictArea == null) return false;
        StrictAreaSpriteRender = StrictArea.GetComponent<SpriteRenderer>();
        VisibleAreaSpriteMask = VisibleArea.GetComponent<SpriteMask>();
        if (StrictAreaSpriteRender == null || VisibleAreaSpriteMask == null) return false;

        StrictAreaSpriteRect = StrictAreaSpriteRender.sprite.rect;
        VisibleAreaSpriteRect = VisibleAreaSpriteMask.sprite.rect;

        StrictAreaCachedRatio.x = ZDGameRule.UNIT_IN_PIXEL / StrictAreaSpriteRect.width;
        StrictAreaCachedRatio.y = ZDGameRule.UNIT_IN_PIXEL / StrictAreaSpriteRect.height;
        VisibleAreaCachedRatio.x = ZDGameRule.UNIT_IN_PIXEL / VisibleAreaSpriteRect.width;
        VisibleAreaCachedRatio.y = ZDGameRule.UNIT_IN_PIXEL / VisibleAreaSpriteRect.height;

        Vector3 StrictAreaScale = new Vector3( StrictAreaCachedRatio.x* ZDGameRule.MAP_WIDTH_UNIT * 2f,
                                                                          StrictAreaCachedRatio.y* ZDGameRule.MAP_HEIGHT_UNIT * 2f, 0);
        Vector3 VisibleAreaScale = new Vector3(VisibleAreaCachedRatio.x * ZDGameRule.MAP_WIDTH_UNIT * 2f,
                                                                            VisibleAreaCachedRatio.y * ZDGameRule.MAP_HEIGHT_UNIT * 2f, 0);

        StrictArea.localScale = StrictAreaScale;
        VisibleArea.localScale = VisibleAreaScale;
        return true;
    }

    bool GetCachedInRestrict(Character character)
    {
        if ( character == null) return false;
        else if(cachedVisibleAreaScale != VisibleArea.localScale)
        {
            cachedVisibleAreaScale = VisibleArea.localScale;
            cachedState.InZone = CheckPlayerInZone();
        }
        return cachedState.InZone;
    }

    protected void OnPlayerLocationChanged(object sender, ZDObject.LocationChangeArgs args)
    {
        cachedState.InZone = CheckPlayerInZone();
    }

    bool CheckPlayerInZone()
    {
        float DeltaX = Mathf.Abs(TargetCharacter.transform.position.x - transform.position.x) / ZDGameRule.UNIT_IN_WORLD;
        float DeltaY = Mathf.Abs(TargetCharacter.transform.position.y - transform.position.y) / ZDGameRule.UNIT_IN_WORLD;
        float ThreshX = (VisibleArea.localScale.x / VisibleAreaCachedRatio.x) / 2;
        float ThreshY = (VisibleArea.localScale.y / VisibleAreaCachedRatio.y) / 2;
        if (DeltaX > ThreshX || DeltaY > ThreshY)return  true;
        else return false;
    }

    public void ShrinkZone(Vector2 Size,float Speed)
    {
        photonView.RPC("ShrinkZoneRPC", RpcTarget.All , Size, Speed);
    }

    void InternalShrinkZone(Vector2 Size,float Speed)
    {
        TargetScale = (Size * VisibleAreaCachedRatio);
        ScaleChangeRate = (TargetScale - VisibleArea.localScale) / Speed;
        Shrink = true;
    }

    #region Photon RPC
    [PunRPC]
    void ShrinkZoneRPC(Vector2 Size, float Speed)
    {
        InternalShrinkZone(Size, Speed);
    }
    #endregion

    #region Photon Callbacks
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        try
        {
            if (stream.IsWriting)
            {
                stream.SendNext(transform.position);
                //stream.SendNext(VisibleArea.position);
                stream.SendNext(VisibleArea.localScale);
                //stream.SendNext(StrictArea.position);
                //stream.SendNext(StrictArea.localScale);
            }
            else
            {
                transform.position = (Vector3)stream.ReceiveNext();
                //VisibleArea.position = (Vector3)stream.ReceiveNext();
                VisibleArea.localScale = (Vector3)stream.ReceiveNext();
                //StrictArea.position = (Vector3)stream.ReceiveNext();
                //StrictArea.localScale = (Vector3)stream.ReceiveNext();
            }
        }
        catch (Exception)
        {
            Debug.Log("Restrict Zone Error");
        }
        
    }

    public void OnEvent(EventData photonEvent)
    {
        switch ((ZDGameEvent)photonEvent.Code)
        {
            case ZDGameEvent.RestrictAnounce:
                break;
            case ZDGameEvent.RestrictPrepare:
                break;
            case ZDGameEvent.Restrict:
                break;
            case ZDGameEvent.RestrictEnd:
                break;
        }
    }
    #endregion

}
