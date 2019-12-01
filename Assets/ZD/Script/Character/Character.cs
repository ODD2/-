using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ZoneDepict.Rule;
using System;
using ZoneDepict;
// This class is basic of Charater, and all infos are
// in this class
public class Character : ZDObject,IPunObservable,IADamageObject
{

    #region Input Wrappers
    public virtual void InputAttack(Vector2 AttackDirection, AttackType Type)
    {
        
    }

    public virtual void InputSprint(Vector2 Destination)
    {

    }
    #endregion

    #region Character Attributes
    private float HP = 100;
    private float MP = 100;
    private float RegHP;
    private float RegMP;
    private float MaxHP = 100;
    private float MaxMP = 100;
    List<ItemBase> Inventory = new List<ItemBase>();
    #endregion

    #region Getters/Setters
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
        if (NewHP > MaxHP) HP = MaxHP;
        else if (NewHP < 0) HP = 0;
        else HP = NewHP;
    }
    public void SetMP(float NewMP)
    {
        if (NewMP > MaxMP) MP = MaxMP;
        else if (NewMP < 0) MP = 0;
        else MP = NewMP;
    }
    #endregion

    #region Character Interfaces
    // This virtual function
    public virtual void Attack(Vector2 Direction, AttackType Type)
    {
    }

    public virtual void ApplyDamage(List<List<ZDObject>> Hits, AttackType Type)
    {
    }

    public virtual void Sprint(Vector2 Destination)
    {
    }

    public void Hurt(float Damage)
    {
        Debug.LogFormat("Player Received {0} Damage.", Damage);
        if (photonView.IsMine)
        {
            SetHP(HP - Damage);
        }
    }

    public bool UseItem(int InventoryIndex)
    {
        Debug.LogFormat("Character Use Item in Inventory[{0}].", InventoryIndex);
        if (Inventory.Count > InventoryIndex)
        {
            //Use the selected item
            Inventory[InventoryIndex].Use(this);
            if (Inventory[InventoryIndex].IsGarbage())
            {
                //Destroy item if it's a garbage.
                Inventory.RemoveAt(InventoryIndex);
            }
            return true;
        }
        else
        {
            Debug.LogFormat("Cannot Use Item in Inventory Index: {0}",InventoryIndex);
        }
        return false;

    }

    public void GetItem(ItemBase i)
    {
        Debug.LogFormat("Inventory Add: {0} ", i);
        Inventory.Add(i);
    }
    #endregion

    #region UNITY
    protected new  void Start()
    {
        //Calls ZDObject Start()
        base.Start();
    }

    protected new void Update()
    {
        //Calls ZDObject Update()
        base.Update();
    }

    private new void OnDestroy()
    {
        //Calls ZDObject OnDestroy()
        base.OnDestroy();
    }
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

    #region Testing
    public void PrintStatus()
    {
        Debug.LogFormat("HP: {0}\n" +
                        "MP: {1}\n" +
                        "ItemNum: {2}\n",
                        HP,MP,Inventory.Count);
    }
    #endregion
}
