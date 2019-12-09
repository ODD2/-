using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZoneDepict.Rule;
using ZoneDepict.UI;
using ZoneDepict;

public class ZDController : MonoBehaviour
{
    private Character TargetCharacter = null; 
    private bool IsMovingCharacter = false; // To judge if do click/touch on target
    private bool IsActivateAttackCircle = false;
    private bool IsSelectingAttack = false;
    private bool IsTouchMove = false; // To judge if did "Drag" or not
    private bool IsDidMovePhase = false;
    private float ClickOnFix = 1.2f; // To fix the radius of "TochOn Circle"
    private float TouchMoveFix = 1.7f; // To fix what is "Move"
    private List<Touch> ZDTouchs; // To support multiTouchs
    private Vector2 TouchPosRecord; // To record the attack start pos (to calculate direction)

    private ZDUI ZDUIClass;
    private int Frame;

    void Start()
    {
        TargetCharacter = GetComponent<Character>();
        if (!TargetCharacter.photonView.IsMine)
        {
            Destroy(this);
        }
        ZDUIClass = GameObject.Find("ZDUI").GetComponent<ZDUI>();
    }

    void Update()
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
            TargetCharacter.InputAttack(AttackDirection, AttackType.N);
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            AttackDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition) - TargetCharacter.transform.position;
            TargetCharacter.InputAttack(AttackDirection, AttackType.A);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            AttackDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition) - TargetCharacter.transform.position;
            TargetCharacter.InputAttack(AttackDirection, AttackType.B);
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            AttackDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition) - TargetCharacter.transform.position;
            TargetCharacter.InputAttack(AttackDirection, AttackType.R);
            
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

        #region Touch Input for Single Touch
        if (Input.touchCount == 1)
        {

            Touch TouchTemp = Input.GetTouch(0);
            // TouchPos is world Position
            Vector2 TouchPos = Camera.main.ScreenToWorldPoint(TouchTemp.position);
            
            if (TouchTemp.phase == TouchPhase.Began)
            {
                if ((TouchPos - (Vector2)TargetCharacter.transform.position).magnitude < ZDGameRule.UnitInWorld * ClickOnFix)
                {
                    IsMovingCharacter = true;
                }
                else
                {
                    IsMovingCharacter = false;
                    //
                    List<ZDObject> PosObjects = ZDMap.HitAtUnit(ZDGameRule.WorldToUnit(TouchPos));
                    //Debug.Log("Objs : " + PosObjects.Count);
                    if (PosObjects == null)
                    {
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
            else if (TouchTemp.phase == TouchPhase.Ended )
            {
                
                // Check if really do "Drag"
                if ((TouchTemp.deltaPosition.magnitude >= TouchMoveFix) && IsDidMovePhase)
                {
                    IsTouchMove = true;
                }
                else
                {
                    IsTouchMove = false;
                }
                // Tap for Normal attack
                if (Frame <= ZDGameRule.TOUCH_TAP_BOUND_FRAMES && !IsTouchMove)
                {
                    TargetCharacter.InputAttack(new Vector3(TouchPos.x, TouchPos.y, 0) - TargetCharacter.transform.position, AttackType.N);
                }
                // DoMove
                if (IsMovingCharacter && IsTouchMove) 
                {
                    TargetCharacter.InputSprint(Camera.main.ScreenToWorldPoint(TouchTemp.position));
                    IsTouchMove = false;
                }
                else if (!IsMovingCharacter) // Other Type Attack
                {
                    Vector2 Direction =  new Vector3(TouchPos.x, TouchPos.y,0) - TargetCharacter.transform.position;
                    TargetCharacter.InputAttack(Direction, ZDGameRule.DirectionToType(TouchPos - TouchPosRecord));
                }
                ZDUIClass.CancelAttackIndicator();
                ZDUIClass.CancelMoveIndicator();
                IsMovingCharacter = false;
                IsDidMovePhase = false;
                IsActivateAttackCircle = false;
                Frame = 0;
            }
            if (IsMovingCharacter)
            {
                Vector2 Direction = new Vector3(TouchPos.x, TouchPos.y, 0) - TargetCharacter.transform.position;
                float Degree = ZDGameRule.QuadAngle(Direction);
                Vector3 Temp = ZDGameRule.WorldToUnit(Camera.main.ScreenToWorldPoint(Input.mousePosition)) - ZDGameRule.WorldToUnit(TargetCharacter.transform.position);
                Vector2 Distance = new Vector2(Temp.x, Temp.y);
                ZDUIClass.SetMoveIndicator(TargetCharacter.transform.position, Degree, Distance.magnitude);
            }
            else
            {
                ZDUIClass.SetAttackOpacity(Frame);
            }
            if (IsActivateAttackCircle)
            {
                Vector2 Direction = TouchPos - TouchPosRecord;
                ZDUIClass.UpdateAttackCircle(ZDGameRule.DirectionToType(Direction));
            }
        }


        #endregion

        #region Just for Fun and Debug
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 HitLoc = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 CharactorPos = new Vector2(TargetCharacter.transform.position.x, TargetCharacter.transform.position.y);
            Vector2 UnitLoc = ZDGameRule.WorldToUnit(HitLoc);
            List<ZDObject> HitObjects;
            if ((HitLoc - CharactorPos).magnitude <= ZDGameRule.UnitInWorld * ClickOnFix)
            {
                IsMovingCharacter = true;
            }
            else if ((HitObjects = ZDMap.HitAtUnit(UnitLoc,ETypeZDO.ACollect) )!= null)
            {
                foreach (var obj in HitObjects)
                {
                    if (obj is IACollectObject)
                    {
                        ((IACollectObject)obj).Collect(TargetCharacter);
                    }
                }
            }
            else
            {
                IsSelectingAttack = true;
                Vector2 Tempp = ZDGameRule.WorldToUnit(new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y));
                List<ZDObject> PosObjects = ZDMap.HitAtUnit(Tempp);
                if (PosObjects == null)
                {
                    ZDUIClass.SetAttackIndicator(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                    IsActivateAttackCircle = true;
                }
                
                TouchPosRecord = HitLoc;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {

            Vector2 DropLocation = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            
        
            if (IsMovingCharacter)
            {
                IsMovingCharacter = false;
                ZDUIClass.CancelMoveIndicator();
                TargetCharacter.InputSprint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            }
            else if (IsSelectingAttack) //Tap
            {
                ZDUIClass.CancelAttackIndicator();
                IsSelectingAttack = false;
                IsActivateAttackCircle = false;
                if((DropLocation - TouchPosRecord).magnitude < 1.0f)
                {
                    TargetCharacter.InputAttack(DropLocation, AttackType.N);
                }
                else
                {
                    TargetCharacter.InputAttack(DropLocation, ZDGameRule.DirectionToType(Camera.main.ScreenToWorldPoint(Input.mousePosition) - new Vector3(TouchPosRecord.x, TouchPosRecord.y, 0)));
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

}
