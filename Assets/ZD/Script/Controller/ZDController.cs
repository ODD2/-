using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZoneDepict.Rule;
using ZoneDepict.UI;
using ZoneDepict;

public class ZDController : MonoBehaviour
{
    private Character TargetCharacter = null; 
    private bool IsHitOn = false; // To judge if do click/touch on target
    private bool IsTouchMove = false; // To judge if did "Drag" or not
    private bool IsDidMovePhase = false;
    private float ClickOnFix = 0.7f; // To fix the radius of "TochOn Circle"
    private float TouchMoveFix = 1.7f; // To fix what is "Move"
    private List<Touch> ZDTouchs; // To support multiTouchs
    private Vector2 TouchPosRecord;

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
        #region Debugger
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


        if (Input.GetMouseButtonDown(0))
        {
           
            Vector3 UnitPos = ZDGameRule.WorldToUnit(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            //Debug.Log("Hit At Position: " + UnitPos.x + "," + UnitPos.y);
            var TargetList = ZDMap.HitAtUnit(UnitPos);
            if (TargetList != null)
            {
                foreach (var obj in TargetList)
                {
                    if (obj is IACollectObject)
                    {
                        ((IACollectObject)obj).Collect(TargetCharacter);
                    }
                }
            }
            
        }
        #endregion
        #region Touch Input for Single Touch
        if (Input.touchCount == 1)
        {

            Touch TouchTemp = Input.GetTouch(0);
            Vector2 TouchPos = Camera.main.ScreenToWorldPoint(TouchTemp.position);
            
            if (TouchTemp.phase == TouchPhase.Began)
            {
                if ((TouchPos - new Vector2(TargetCharacter.transform.position.x, TargetCharacter.transform.position.y)).magnitude < ZDGameRule.UnitInWorld * ClickOnFix)
                {
                    IsHitOn = true;
                }
                else
                {
                    IsHitOn = false;
                    ZDUIClass.SetAttackIndicator(TouchPos);
                    TouchPosRecord = TouchPos;
                    
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
                if (IsHitOn && IsTouchMove) 
                {
                    TargetCharacter.InputSprint(Camera.main.ScreenToWorldPoint(TouchTemp.position));
                    IsTouchMove = false;
                }
                else if (!IsHitOn) // Other Type Attack
                {
                    Vector2 Direction =  new Vector3(TouchPos.x, TouchPos.y,0) - TargetCharacter.transform.position;
                    TargetCharacter.InputAttack(Direction, ZDGameRule.DirectionToType(TouchPos - TouchPosRecord));
                }
                ZDUIClass.CancelAttackIndicator();
                ZDUIClass.CancelMoveIndicator();
                IsHitOn = false;
                IsDidMovePhase = false;
                Frame = 0;
            }
            if (IsHitOn)
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
        }


        #endregion

        if (Input.GetMouseButtonDown(0))
        {
            
            Vector2 temp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 CharactorPos = new Vector2(TargetCharacter.transform.position.x, TargetCharacter.transform.position.y);
            if ((temp - CharactorPos).magnitude <= ZDGameRule.UnitInWorld * ClickOnFix)
            {
                IsHitOn = true;
            }
            else
            {
                ZDUIClass.SetAttackIndicator(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                TouchPosRecord = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                IsHitOn = false;
            }
            
        }
        if (Input.GetMouseButtonUp(0))
        {
            ZDUIClass.CancelMoveIndicator();
            Vector2 Direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - TargetCharacter.transform.position;
            ZDUIClass.CancelAttackIndicator();
            if (IsHitOn)
            {
                TargetCharacter.InputSprint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                IsHitOn = false;
            }
            else if ((Direction- TouchPosRecord).magnitude < 0.5f) //Tap
            {
                
                TargetCharacter.InputAttack(Direction, AttackType.N);
            }
            else
            {
                TargetCharacter.InputAttack(Direction, ZDGameRule.DirectionToType(Camera.main.ScreenToWorldPoint(Input.mousePosition) - new Vector3(TouchPosRecord.x, TouchPosRecord.y, 0)));
            }
        }
        if (IsHitOn)
        {
            
            Vector2 Direction = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, 0) - TargetCharacter.transform.position;
            float Degree = ZDGameRule.QuadAngle(Direction);
            //Debug.Log(ZDGameRule.QuadrifyDirection(Direction));
            Vector3 Temp = ZDGameRule.WorldToUnit(Camera.main.ScreenToWorldPoint(Input.mousePosition)) - ZDGameRule.WorldToUnit(TargetCharacter.transform.position);
            Vector2 Distance = new Vector2(Temp.x, Temp.y);
            
            ZDUIClass.SetMoveIndicator(TargetCharacter.transform.position, Degree, Distance.magnitude);
        }
        
    }
    
}
