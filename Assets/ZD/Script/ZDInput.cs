using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZoneDepict.Rule;

public class ZDInput : MonoBehaviour
{
    private Charater ZDcontroller = null;
    private Transform ZDTransform;
    private Vector2 AttackDirection;
    private bool IsClickOn = false;
    void Start()
    {
        ZDcontroller = GetComponent<Charater>();
        ZDTransform = GetComponent<Transform>();
        
    }
   
    
    // Update is called once per frame
    void Update()
    {
        //ZDcontroller.transform.position += Vector3.up;
        #region Debuger
        
        if (Input.GetKey(KeyCode.Q))
        {
            AttackDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition) - ZDTransform.position;
            ZDcontroller.InputAttack(AttackDirection, AttackType.N);
        }
        if (Input.GetKey(KeyCode.W))
        {
            AttackDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition) - ZDTransform.position;
            ZDcontroller.InputAttack(AttackDirection, AttackType.A);
        }
        if (Input.GetKey(KeyCode.E))
        {
            AttackDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition) - ZDTransform.position;
            ZDcontroller.InputAttack(AttackDirection, AttackType.B);
        }
        if (Input.GetKey(KeyCode.R))
        {
            AttackDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition) - ZDTransform.position;
            ZDcontroller.InputAttack(AttackDirection, AttackType.R);
            
        }
        #endregion
    }
    void OnMouseUp()
    {
        
        if (IsClickOn)
        {
            ZDcontroller.InputSprint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
    }
    void OnMouseDown()
    {
        Vector2 Position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 ChrPos = ZDTransform.position;
        if (Mathf.Sqrt(Mathf.Pow(Position.x - ChrPos.x, 2) + Mathf.Pow(Position.y - ChrPos.y, 2)) < ZDGameRule.UNITINWORLD)
        {
            IsClickOn = true;
        }
        else
        {
            IsClickOn = false;
        }
    }
    
}
