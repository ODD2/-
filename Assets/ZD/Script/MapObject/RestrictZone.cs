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


class RestrictZone : MonoBehaviourPun , IOnEventCallback
{
    public class CachedCharacterState
    {
        public Vector3 position;
        public bool InZone;
    }
    public static RestrictZone Instance;
    public float ShrinkSpeed = 50f;

    private CachedCharacterState cachedState;
    private Transform VisibleArea;
    private Transform StrictArea;
    private Rect StrictAreaSpriteRect;
    private Rect VisibleAreaSpriteRect;
    private Vector2 StrictAreaCachedRatio;
    private Vector2 VisibleAreaCachedRatio;

    private Vector3 DeltaScale,ScaleChangeRate;
    private bool Shrink = false;
    void Start()
    {
        if (Instance) Destroy(this);
        else Instance = this;
        if (!CanInitialize()) enabled = false;
        ShrinkVisible(new Vector2(7, 5));
    }

    void FixedUpdate()
    {
        if (ZDGameManager.gameState == ZDGameState.Play)
        {
            if (Shrink)
            {
                Vector3 FramScaleChange = ScaleChangeRate * Time.deltaTime;
                if(DeltaScale.magnitude < FramScaleChange.magnitude)
                {
                    Shrink = false;
                    VisibleArea.localScale += DeltaScale;
                }
                else
                {
                    VisibleArea.localScale += FramScaleChange;
                    DeltaScale -= FramScaleChange;
                }
            }
            //if (IsInRestrict(ZDController.TargetCharacter))
            //{
            //    ZDController.TargetCharacter.Hurt(10);
            //}
        }
    }

    bool CanInitialize()
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

        StrictAreaCachedRatio.x = ZDGameRule.UnitInPixel / StrictAreaSpriteRect.width;
        StrictAreaCachedRatio.y = ZDGameRule.UnitInPixel / StrictAreaSpriteRect.height;
        VisibleAreaCachedRatio.x = ZDGameRule.UnitInPixel / VisibleAreaSpriteRect.width;
        VisibleAreaCachedRatio.y = ZDGameRule.UnitInPixel / VisibleAreaSpriteRect.height;

        Vector3 StrictAreaScale = new Vector3( StrictAreaCachedRatio.x* ZDGameRule.MAP_WIDTH_UNIT * 2f,
                                                                          StrictAreaCachedRatio.y* ZDGameRule.MAP_HEIGHT_UNIT * 2f, 0);
        Vector3 VisibleAreaScale = new Vector3(VisibleAreaCachedRatio.x * ZDGameRule.MAP_WIDTH_UNIT * 2f,
                                                                            VisibleAreaCachedRatio.y * ZDGameRule.MAP_HEIGHT_UNIT * 2f, 0);

        StrictArea.localScale = StrictAreaScale;
        VisibleArea.localScale = VisibleAreaScale;
        return true;
    }

    bool IsInRestrict(Character character)
    {
        if (character == null) return false;
        else if(cachedState.position != character.transform.position)
        {
            cachedState.position = character.transform.position;
            int deltaUnit = Mathf.FloorToInt(((character.transform.position - transform.position).magnitude) / ZDGameRule.UnitInWorld);
        }
        return cachedState.InZone;
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

    void ShrinkVisible(Vector2 Size)
    {
        Vector3 TargetScale = Size * VisibleAreaCachedRatio;
        DeltaScale = TargetScale - VisibleArea.localScale;
        ScaleChangeRate = (DeltaScale / ShrinkSpeed);
        Shrink = true;
    }
}
