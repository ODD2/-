using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZoneDepict.Rule;
using ZoneDepict;

public class ZDController : MonoBehaviour
{
    private Character TargetCharacter = null; 
    private bool IsClickOn = false;
    void Start()
    {
        TargetCharacter = GetComponent<Character>();
        if (!TargetCharacter.photonView.IsMine)
        {
            Destroy(this);
        }
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
            Debug.Log("Hit At Position: " + UnitPos.x + "," + UnitPos.y);
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
        Debug.Log("Mouse Down Received:" + TargetCharacter.transform.position.ToString());
        Vector2 ChrPos = TargetCharacter.transform.position;
        if (Mathf.Sqrt(Mathf.Pow(Position.x - ChrPos.x, 2) + Mathf.Pow(Position.y - ChrPos.y, 2)) < ZDGameRule.UnitInWorld)
        {
            IsClickOn = true;
        }
        else
        {
            IsClickOn = false;
        }
    }
    
}
