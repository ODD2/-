using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackType
{
    N, A, B, R
}
// This class is basic of Charater, and all infos are
// in this class
public class Charater : MonoBehaviour
{
    #region Basic Attributes
    private float HP;
    private float MP;
    private float RegHp;
    private float RegMP;
    private static float MaxHp = 100;
    private static float MaxMp = 100;
    #endregion

    #region Attributes function

    public float GetHP()
    {
        return HP;
    }
    public float GetMP()
    {
        return MP;
    }
    public void SetHP(float NewHP)
    {
        HP = NewHP;
    }
    public void SetMP(float NewMP)
    {
        MP = NewMP;
    }
    public void Attack()
    {

    }
    public void Hurt(float Damage)
    {
        HP -= Damage;
    }
    public void Attack(AttackType Type)
    {
        
    }
    public void Sprint(Vector2 Destination)
    {
       
    }
    public void UseItem(uint ItemID)
    {

    }
    public void GetItem() // GetItem(ItemBase Item)
    {

    }
    #endregion

    
    //

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
