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
    #endregion

    #region UI CallBack Class
    private ZDUI ZDUIClass;
    private BagController BagClass;
    #endregion

    private List<Touch> ZDTouchs; // To support multiTouchs
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
                    //Debug.Log("NotEveSys");

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
                        else if ((TouchPos - (Vector2)TargetCharacter.transform.position).magnitude < ZDGameRule.UnitInWorld * ClickOnFix)
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
                        Frame++;

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
                            if (TouchDelta.magnitude < 0.1f) // This Distance is to judge how is "Tap"
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
            else
            {
                #region Debugger with hotkeys
                Vector2 AttackDirection;
                if (Input.GetKeyDown(KeyCode.V))
                {
                    TargetCharacter.PrintStatus();
                }

                if (Input.GetKeyDown(KeyCode.Q))
                {
                    AttackDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition) - TargetCharacter.transform.position;
                    TargetCharacter.InputAttack(AttackDirection, EAttackType.N);
                }
                else if (Input.GetKeyDown(KeyCode.W))
                {
                    AttackDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition) - TargetCharacter.transform.position;
                    TargetCharacter.InputAttack(AttackDirection, EAttackType.A);
                }
                else if (Input.GetKeyDown(KeyCode.E))
                {
                    AttackDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition) - TargetCharacter.transform.position;
                    TargetCharacter.InputAttack(AttackDirection, EAttackType.B);
                }
                else if (Input.GetKeyDown(KeyCode.R))
                {
                    AttackDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition) - TargetCharacter.transform.position;
                    TargetCharacter.InputAttack(AttackDirection, EAttackType.R);

                }

                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    TargetCharacter.UseItem(0);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    TargetCharacter.UseItem(1);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    TargetCharacter.UseItem(2);
                }
                #endregion

                #region Just for Fun and Debug
                if (Input.GetMouseButtonDown(0))
                {
                    // World
                    Vector2 HitLoc = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    Vector2 CharactorPos = new Vector2(TargetCharacter.transform.position.x, TargetCharacter.transform.position.y);
                    Debug.Log("ChPos : " + CharactorPos);
                    // Unit
                    Vector2 UnitLoc = ZDGameRule.WorldToUnit(HitLoc);
                    Vector2 UnitCharactorPos = ZDGameRule.WorldToUnit(CharactorPos);

                    List<ZDObject> HitObjects;
                    
                    if ((HitObjects = ZDMap.HitAtUnit(UnitLoc, EObjectType.ACollect)) != null)
                    {
                        Debug.Log("Hit is CollectObj");
                        foreach (var obj in HitObjects)
                        {
                            if (TargetCharacter.GetInventory().Count < TargetCharacter.GetInventoryMax())
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
                    else if ((HitLoc - CharactorPos).magnitude <= ZDGameRule.UnitInWorld * ClickOnFix)
                    {
                        Debug.Log("IsMoving");
                        IsMovingCharacter = true; // Moveing Charactor
                    }

                    else
                    {
                        // Is Activate Attack System
                        Debug.Log("Dis : " + (HitLoc - CharactorPos).magnitude);
                        Debug.Log("Judge : " + ZDGameRule.UnitInWorld * ClickOnFix);
                        if ((HitObjects = ZDMap.HitAtUnit(UnitLoc, EObjectType.ACollect)) == null && !BagClass.GetHover())
                        {

                            IsSelectingAttack = true;
                            ZDUIClass.SetAttackIndicator(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                            IsActivateAttackCircle = true;
                            TouchPosRecord = HitLoc;
                        }

                    }
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    Vector2 CharactorPos = new Vector2(TargetCharacter.transform.position.x, TargetCharacter.transform.position.y);
                    Vector2 DropLocation = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                    if (IsMovingCharacter)
                    {
                        IsMovingCharacter = false;
                        ZDUIClass.CancelMoveIndicator();
                        TargetCharacter.InputSprint(DropLocation);
                    }
                    else if (IsSelectingAttack) //Tap
                    {
                        ZDUIClass.CancelAttackIndicator();
                        IsSelectingAttack = false;
                        IsActivateAttackCircle = false;
                        if ((DropLocation - TouchPosRecord).magnitude < 0.1f) // This Distance is to judge how is "Tap"
                        {
                            TargetCharacter.InputAttack(DropLocation - CharactorPos, EAttackType.N);
                        }
                        else
                        {
                            TargetCharacter.InputAttack(TouchPosRecord - CharactorPos, ZDGameRule.DirectionToType(Camera.main.ScreenToWorldPoint(Input.mousePosition) - new Vector3(TouchPosRecord.x, TouchPosRecord.y, 0)));
                        }
                    }

                }

                if (IsMovingCharacter)
                {

                    Vector2 Direction = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, 0) - TargetCharacter.transform.position;
                    float Degree = ZDGameRule.QuadAngle(Direction);
                    //Debug.Log(ZDGameRule.QuadrifyDirection(Direction));
                    Vector3 Temp = ZDGameRule.WorldToUnit(Camera.main.ScreenToWorldPoint(Input.mousePosition)) - ZDGameRule.WorldToUnit(TargetCharacter.transform.position);
                    Vector2 Distance = new Vector2(Temp.x, Temp.y);

                    ZDUIClass.SetMoveIndicator(TargetCharacter.transform.position, Degree, Distance.magnitude);
                }
                if (IsActivateAttackCircle)
                {
                    Vector2 Direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - new Vector3(TouchPosRecord.x, TouchPosRecord.y, 0);
                    ZDUIClass.UpdateAttackCircle(ZDGameRule.DirectionToType(Direction));
                }
                #endregion
            }

            if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                Debug.Log("Hurt");
                TargetCharacter.Hurt(10);
                //TargetCharacter.SetHP(TargetCharacter.GetHP()-10);
                TargetCharacter.SetMP(TargetCharacter.GetMP() - 10);
            }
            // Update HP/MP
            //ZDUIClass.UpdateHPBar(TargetCharacter.GetMaxHP(), TargetCharacter.GetHP());
            //ZDUIClass.UpdateMPBar(TargetCharacter.GetMaxMP(), TargetCharacter.GetMP());
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
