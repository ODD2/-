﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
using ZoneDepict;
using ZoneDepict.Rule;
using ZoneDepict.Audio;
using Hashtable = ExitGames.Client.Photon.Hashtable;

// This class is basic of Charater, and all infos are
// in this class
public class Character : ZDObject,IPunObservable, IADamageObject
{
    #region Components
    protected Animator animator;
    protected SpriteRenderer sprite;
    protected AudioSource audioSource;
    //Sound Effects
    public AudioClip DeathSound;
    public AudioClip MoveSound;
    #endregion

    //public CharacterValue basicValues = new CharacterValue();
    
   

    #region Input Wrappers
    public virtual void InputAttack(Vector2 AttackDirection, EAttackType Type)
    {
      
    }

    public virtual void InputSprint(Vector2 Destination)
    {

    }
    #endregion

    #region Character Attributes
    public int TeamID {get; protected set; } = 0;
    protected float HP = 100;
    protected float MP = 100;
    protected float RegHP = 0.1f;
    protected float RegMP = 2.9f;
    protected float MaxHP = 100;
    protected float MaxMP = 100;
    protected Vector2 Velocity = new Vector2(0,0);
    protected float MaxVelocity = 30;
    List<ItemBase> Inventory = new List<ItemBase>();
    protected int InventoryMax = 3;
    protected float MoveMana = 5.0f;
    protected float[] MaxSkillCD = new float[4];
    protected float[] SkillMana = new float[4];
    protected float[] SkillCD = new float[4];
    
    #endregion

    #region Getters/Setters
    public List<ItemBase>  GetInventory()
    {
        return Inventory;
    }
    public float GetRegHP()
    {
        return RegHP;
    }
    public float GetRegMP()
    {
        return RegMP;
    }
    public float GetMaxHP()
    {
        return MaxHP;
    }
    public float GetHP()
    {
        return HP;
    }
    public float GetMaxMP()
    {
        return MaxMP;
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
    public int GetInventoryMax()
    {
        return InventoryMax;
    }
    #endregion

    #region Character Interfaces
    // This virtual function
    protected virtual void Attack(Vector2 Direction, EAttackType Type)
    {
    }

    protected virtual void ApplyDamage(List<List<ZDObject>> Hits, EAttackType Type)
    {
    }

    protected virtual void Sprint(Vector2 Destination)
    {
    }

    public virtual void Hurt(float Damage)
    {
        Debug.LogFormat("Player Received {0} Damage.", Damage);
        if (photonView.IsMine && !HP.Equals(0))
        {
            SetHP(HP - Damage);
            if (HP.Equals(0))Dead();
            else photonView.RPC("DoHurtRpc",RpcTarget.AllViaServer);
        }
    }

    public virtual void Dead()
    {
        if (photonView.IsMine)
        {
            Hashtable NewSetting = new Hashtable();
            NewSetting.Add("Alive", false);
            PhotonNetwork.SetPlayerCustomProperties(NewSetting);
            photonView.RPC("DoDeadRpc", RpcTarget.AllViaServer);
        }
    }

    public virtual bool UseItem(int InventoryIndex)
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
        // 重複利用道具不可重複拿
        foreach (ItemBase  item in Inventory)
        {
            if(item.id == i.id )
            {
                if (i.canReuse())
                {
                    return;
                }
                /*else
                {
                    item.Amount += i.Amount;
                    return;
                }*/
                
            }
        }
        Inventory.Add(i);
    }
    #endregion

    #region Character Helpers
    public void Destroy()
    {
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(photonView);
        }
    }
    #endregion

    #region UNITY
    protected new  void Start()
    {

       

        //Cache Components.
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        ZDAudioSource.SetupAudioSource(audioSource);

        //Setup TeamID.
        if (photonView.Owner.CustomProperties.ContainsKey("Team"))
        {
            TeamID = (int)photonView.Owner.CustomProperties["Team"];
        }

        //Setup Depth
        if (photonView.IsMine) ActorType = EActorType.LocalCharacter;
        else ActorType = EActorType.RemoteCharacter;

        //Calls ZDObject Start()
        base.Start();
    }

    protected new void Update()
    {
        //Calls ZDObject Update()
        base.Update();
    }

    protected void FixedUpdate()
    {
        UpdateAnimParams();
        SetHP(GetHP() + Time.deltaTime * GetRegHP() * 8);
        SetMP(GetMP() + Time.deltaTime * GetRegMP() * 8);
        for (int i = 0; i < 4; ++i)
        {
            if (SkillCD[i] > float.Epsilon)
            {
                SkillCD[i] -= Time.deltaTime > SkillCD[i] ? SkillCD[i] : Time.deltaTime;
            }
        }
    }

    private new void OnDestroy()
    {
        //Calls ZDObject OnDestroy()
        base.OnDestroy();
    }

    private void UpdateAnimParams()
    {

        animator.SetBool("Sprinting", Velocity.magnitude > 0);
        if (Velocity.magnitude > 0)
        {
            if (Velocity.x.Equals(0)) animator.SetInteger("AccHorizontal", 0);
            else if (Velocity.x > float.Epsilon) animator.SetInteger("AccHorizontal", 1);
            else animator.SetInteger("AccHorizontal", -1);

            if (Velocity.y.Equals(0)) animator.SetInteger("AccVertical", 0);
            else if (Velocity.y > float.Epsilon) animator.SetInteger("AccVertical", 1);
            else animator.SetInteger("AccVertical", -1);
        }
    }
    #endregion

    #region PUN CallBack
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.localScale);
            stream.SendNext(HP);
            stream.SendNext(MP);
            stream.SendNext(MaxHP);
            stream.SendNext(MaxMP);
            stream.SendNext(MaxVelocity);
        }
        else if (stream.IsReading)
        {
            transform.position = (Vector3)stream.ReceiveNext();
            transform.localScale = (Vector3)stream.ReceiveNext();
            HP = (float)stream.ReceiveNext();
            MP = (float)stream.ReceiveNext();
            MaxHP = (float)stream.ReceiveNext();
            MaxMP = (float)stream.ReceiveNext();
            MaxVelocity = (float)stream.ReceiveNext();
        }
    }
    #endregion

    #region RPC
    [PunRPC]
    public void DoDeadRpc()
    {
        if(animator)animator.SetTrigger("Die");
        ZDAudioSource.PlayAtPoint(DeathSound, transform.position, 1, false);
    }
    [PunRPC]
    public void DoHurtRpc()
    {
        animator.SetTrigger("Hurt");
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

    #region Routines
    #endregion
}
