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
    private bool IsTouchHeld = false; // To judge if Touch for some time or not
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
   
    
    // Update is called once per frame
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
        #region Touch Input
        if (Input.touchCount == 1)
        {

            Touch TouchTemp = Input.GetTouch(0);
            Vector2 TouchPos = Camera.main.ScreenToWorldPoint(TouchTemp.position);
            
            if (TouchTemp.phase == TouchPhase.Began)
            {
                if (ZDGameRule.CalculateDistance(TouchPos, TargetCharacter.transform.position) < ZDGameRule.UnitInWorld * ClickOnFix)
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
                if ((ZDGameRule.CalculateDistance(TouchTemp.deltaPosition) >= TouchMoveFix) && IsDidMovePhase)
                {
                    IsTouchMove = true;
                }
                else
                {
                    IsTouchMove = false;
                }
                if (IsHitOn && IsTouchMove) // DoMove
                {
                    TargetCharacter.InputSprint(Camera.main.ScreenToWorldPoint(TouchTemp.position));
                    IsTouchMove = false;
                }
                else if (!IsHitOn)
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
                float Distance = ZDGameRule.CalculateDistance(Direction);
                ZDUIClass.GetComponent<ZDUI>().SetMoveIndicator(TargetCharacter.transform.position, Degree, Distance);
            }
            else
            {
                ZDUIClass.SetAttackOpacity(Frame);
            }
            


        }
        
        
        #endregion
    }



}
