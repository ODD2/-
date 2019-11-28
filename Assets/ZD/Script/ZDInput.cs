using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZoneDepict.Rule;

public class ZDInput : MonoBehaviour
{
    private Charater ZDcontroller = null;
    private Transform ZDTransform;
    private Vector2 AttackDirection;
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
    
}
