using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ZoneDepict.Rule;
using System;
using ZoneDepict;
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
    [SerializeField]
    protected AudioClip DeathSound;
    #endregion

    #region Input Wrappers
    public virtual void InputAttack(Vector2 AttackDirection, AttackType Type)
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
    #endregion

    #region Character Interfaces
    // This virtual function
    protected virtual void Attack(Vector2 Direction, AttackType Type)
    {
    }

    protected virtual void ApplyDamage(List<List<ZDObject>> Hits, AttackType Type)
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
        //Calls ZDObject Start()
        base.Start();
        //Cache Components.
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        //Setup Depth
        Vector3 NewPos = transform.position;
        if (photonView.IsMine) NewPos.z = (int)TypeDepth.LocalCharacter;
        else NewPos.z = (int)TypeDepth.RemoteCharacter;
        transform.position = NewPos;

        //Setup TeamID.
        if (photonView.Owner.CustomProperties.ContainsKey("Team"))
        {
            TeamID = (int)photonView.Owner.CustomProperties["Team"];
        }
        
    }

    protected new void Update()
    {
        //Calls ZDObject Update()
        base.Update();
        SetHP(GetHP() + Time.deltaTime * GetRegHP() * 8);
        SetMP(GetMP() + Time.deltaTime * GetRegMP() * 8);
    }

    protected void FixedUpdate()
    {
        UpdateAnimParams();
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
