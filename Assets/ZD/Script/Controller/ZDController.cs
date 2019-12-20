using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using ZoneDepict;
using ZoneDepict.Rule;
using ZoneDepict.UI;
using ZoneDepict.Map;

public class ZDController : MonoBehaviour
{
    public static ZDController Instance;
    
    protected  Character TargetCharacter;
    private Vector2 TouchPosRecord; // To record the attack start pos (to calculate direction)

    public bool IsPhoneTest = false;

    #region Check Bools
    private bool IsMovingCharacter; // To judge if do click/touch on target
    private bool IsActivateAttackCircle;
    private bool IsSelectingAttack;
    private bool IsTouchMove; // To judge if did "Drag" or not
    private bool IsDidMovePhase;
    private bool IsCollectItem;
    #endregion

    #region Fix const
    private const float ClickOnFix = 0.8f; // To fix the radius of "TochOn Circle"
    private const float TouchMoveFix = 1.7f; // To fix what is "Move"
    private const float TapOrSlide = 0.1f; //Greater than TapOrSlide is Slide
    #endregion

    #region UI CallBack Class
    private ZDUI ZDUIClass;
    private BagController BagClass;
    #endregion

    private int Frame;

    void Start()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        TargetCharacter = ZDGameManager.GetPlayerProps().Script;
        ZDUIClass = GameObject.Find("ZDUI").GetComponent<ZDUI>();
        BagClass = GameObject.Find("Item").GetComponent<BagController>();
   
        //StartCoroutine(WaitToActive());
    }

    void Update()
    {
        if(ZDGameManager.GetGameState() == ZDGameState.Play)
        {
            if (IsPhoneTest)
            {
                //Debug.Log(BagClass.GetHover());
                #region Touch Input for Single Touch
                if (Input.touchCount == 1 && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                {
                    if (BagClass.GetFrameBlock())
                    {
                        BagClass.SetBlockFrame(false);
                        return;
                    }

                    Touch TouchTemp = Input.GetTouch(0);
                    
                    // TouchPos is world Position
                    Vector2 TouchPos = Camera.main.ScreenToWorldPoint(TouchTemp.position);
                    // UnitTouchPos is Unit Position
                    Vector2 UnitTouchPos = ZDGameRule.WorldToUnit(TouchPos);
                    Vector2 CharactorPos = new Vector2(TargetCharacter.transform.position.x, TargetCharacter.transform.position.y);

                    List<ZDObject> HitObjects;
                    if (TouchTemp.phase == TouchPhase.Began)
                    {
                        if ((HitObjects = ZDMap.HitAtUnit(UnitTouchPos, EObjectType.ACollect)) != null)
                        {
                            List<ZDObject> HitCharacter = ZDMap.HitAtUnit(UnitTouchPos, EObjectType.Character);
                            bool CharacterOccupy = false;
                            if (HitCharacter != null)
                            {
                                foreach(var obj in HitCharacter)
                                {
                                    if ((Character)obj == TargetCharacter) break;
                                    else CharacterOccupy = true;
                                }
                                if (CharacterOccupy) return;
                            }
                            foreach (var obj in HitObjects)
                            {
                                if (obj is IACollectObject)
                                {
                                    if(TargetCharacter.GetInventory().Count < TargetCharacter.GetInventoryMax())
                                    {
                                        ((IACollectObject)obj).Collect(TargetCharacter);
                                        IsCollectItem = true;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                            return;
                        }
                        else if ((TouchPos - (Vector2)TargetCharacter.transform.position).magnitude < ZDGameRule.UNIT_IN_WORLD * ClickOnFix)
                        {
                            IsMovingCharacter = true;
                        }
                        else
                        {
                            // Activate Attack System
                            IsMovingCharacter = false;
                            if ((HitObjects = ZDMap.HitAtUnit(UnitTouchPos, EObjectType.ACollect)) == null && !BagClass.GetHover())
                            {
                                IsSelectingAttack = true;
                                IsActivateAttackCircle = true;
                                ZDUIClass.SetAttackIndicator(TouchPos);
                                TouchPosRecord = TouchPos;
                            }
                        }
                    }
                    else if (TouchTemp.phase == TouchPhase.Moved || TouchTemp.phase == TouchPhase.Stationary)
                    {
                        IsDidMovePhase = true;
                    }
                    else if (TouchTemp.phase == TouchPhase.Ended)
                    {
                        ZDUIClass.CancelMoveIndicator();
                        if (IsCollectItem)
                        {
                            IsCollectItem = false;
                            return;
                        }
                        // DoMove
                        if ((TouchTemp.deltaPosition.magnitude >= TouchMoveFix) && IsDidMovePhase)
                        {
                            IsTouchMove = true;
                        }
                        else
                        {
                            IsTouchMove = false;
                        }
                        if (IsMovingCharacter && IsTouchMove)
                        {
                            TargetCharacter.InputSprint(TouchPos);
                            IsTouchMove = false;
                        }
                        else if (IsSelectingAttack)
                        {
                            ZDUIClass.CancelAttackIndicator();
                            IsSelectingAttack = false;
                            IsActivateAttackCircle = false;
                            Vector2 TouchDelta = TouchPos - TouchPosRecord;
                            if (TouchDelta.magnitude < TapOrSlide) // This Distance is to judge how is "Tap"
                            {
                                TargetCharacter.InputAttack(TouchPosRecord - (Vector2)TargetCharacter.transform.position, EAttackType.N);
                            }
                            else
                            {
                                TargetCharacter.InputAttack(TouchDelta, ZDGameRule.DirectionToType(TouchDelta));
                            }
                        }
                        // Tap for Normal attack
                        if (Frame <= ZDGameRule.TOUCH_TAP_BOUND_FRAMES && !IsTouchMove && !IsMovingCharacter)
                        {
                            TargetCharacter.InputAttack(TouchPos - CharactorPos, EAttackType.N);

                        }
                        else if (!IsMovingCharacter) // Other Type Attack
                        {
                            Vector2 Direction = TouchPosRecord - CharactorPos;
                            TargetCharacter.InputAttack(Direction, ZDGameRule.DirectionToType(TouchPos - TouchPosRecord));
                        }
                        IsMovingCharacter = false;
                        IsDidMovePhase = false;
                        IsActivateAttackCircle = false;
                        Frame = 0;
                    }


                    if (IsActivateAttackCircle)
                    {
                        Vector2 Direction = TouchPos - TouchPosRecord;
                        ZDUIClass.UpdateAttackCircle(ZDGameRule.DirectionToType(Direction));
                    }

                    if (IsMovingCharacter)
                    {
                        Vector2 Direction = TouchPos - CharactorPos;
                        float Degree = ZDGameRule.QuadAngle(Direction);
                        Vector3 Distance = ZDGameRule.WorldToUnit(TouchPos) - ZDGameRule.WorldToUnit(CharactorPos);
                        ZDUIClass.SetMoveIndicator(TargetCharacter.transform.position, Degree, Distance.magnitude);
                    }
                    else
                    {
                        ZDUIClass.SetAttackOpacity(Frame);
                    }
                }

                #endregion
            }
        }
    }

    void OnDestroy()
    {
        if(Instance == this)
        {
            Instance = null;
        }
    }

    static public Character GetTargetCharacter()
    {
        if (Instance == null || Instance.TargetCharacter == null)
        {
            return null;
        }
        else
        {
            return Instance.TargetCharacter;
        }
    }
}
