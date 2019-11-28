using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZoneDepict.Rule;

public class ZDController : MonoBehaviour
{
    private Character TargetCharacter = null; 
    private bool IsClickOn = false;
    void Start()
    {
        TargetCharacter = GetComponent<Character>();     
    }
   
    
    // Update is called once per frame
    void Update()
    {
        #region Debugger
        Vector2 AttackDirection;
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
        #endregion
    }
    void OnMouseUp()
    {
        if (IsClickOn)
        {
            TargetCharacter.InputSprint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
    }

    void OnMouseDown()
    {
        Vector2 Position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 ChrPos = TargetCharacter.transform.position;
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
