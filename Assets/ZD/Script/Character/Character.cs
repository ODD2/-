using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ZoneDepict.Rule;
using System;
using ZoneDepict;
// This class is basic of Charater, and all infos are
// in this class
public class Character : ZDObject,IPunObservable
{

    #region Virtual Function
    public virtual void InputAttack(Vector2 AttackDirection, AttackType Type)
    {
        
    }
    public virtual void InputSprint(Vector2 Destination)
    {

    }
    public virtual void AddDamage(List<List<ZDObject>> Hits, AttackType Type)
    {

    }
    #endregion

    #region Basic Attributes
    private float HP = 100;
    private float MP = 100;
    private float RegHp;
    private float RegMP;
    private const float MaxHp = 100;
    private const float MaxMp = 100;
    
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
    private void SetHP(float NewHP)
    {
        HP = NewHP;
    }
    private void SetMP(float NewMP)
    {
        MP = NewMP;
    }
    // This virtual function
    public virtual void Attack(Vector2 Direction, AttackType Type)
    {
        Debug.Log("Supper !");
    }
  
    public virtual void Sprint(Vector2 Destination)
    {

    }

    public void Hurt(float Damage)
    {
        if (photonView.IsMine)
        {
            HP = Damage > HP ? 0 : HP - Damage;
            Debug.Log("Hurt~~~~~!!!");
        }
    }


    public void UseItem(uint ItemID)
    {

    }
    public void GetItem() // GetItem(ItemBase Item)
    {

    }
    #endregion

    #region UNITY
    private new  void Start()
    {
        // By ZDObj start()
        base.Start();
        
    }

    private new void Update()
    {
        base.Update();
    }

    private new void OnDestroy()
    {
        base.OnDestroy();
    }
    #region PlayerInput

    #endregion
    #endregion

    #region PUN CallBack
    [PunRPC]
    public void RPCAttack(Vector2 Direction,AttackType type)
    {
        Attack(Direction,type);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(HP);
        }
        else if (stream.IsReading)
        {
            transform.position = (Vector3)stream.ReceiveNext();
            HP = (float)stream.ReceiveNext();
        }
    }

    
    #endregion


}
