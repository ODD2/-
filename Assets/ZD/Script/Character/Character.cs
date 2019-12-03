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
    protected float RegHP;
    protected float RegMP;
    protected float MaxHP = 100;
    protected float MaxMP = 100;
    protected Vector2 Velocity = new Vector2(0,0);
    protected float MaxVelocity = 30;
    List<ItemBase> Inventory = new List<ItemBase>();
    #endregion

    #region Getters/Setters
    public List<ItemBase>  GetInventory()
    {
        return Inventory;
    }
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
        if (photonView.IsMine)
        {
            SetHP(HP - Damage);
            if (HP.Equals(0))
            {
                Dead();
            }
        }
    }

    public virtual void Dead()
    {
        if (photonView.IsMine)
        {
            Hashtable NewSetting = new Hashtable();
            NewSetting.Add("Alive", false);
            PhotonNetwork.SetPlayerCustomProperties(NewSetting);
            photonView.RPC("DeadRPC", RpcTarget.AllViaServer);
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
    #endregion

    #region UNITY
    protected new  void Start()
    {
        //Calls ZDObject Start()
        base.Start();
        //Cache Components.
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();

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

        if (Velocity.x.Equals(0)) animator.SetInteger("FaceHorizontal", 0);
        else if (Velocity.x > float.Epsilon) animator.SetInteger("FaceHorizontal", 1);
        else animator.SetInteger("FaceHorizontal", -1);

        if (Velocity.y.Equals(0)) animator.SetInteger("FaceVertical", 0);
        else if (Velocity.y > float.Epsilon) animator.SetInteger("FaceVertical", 1);
        else animator.SetInteger("FaceVertical", -1);
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
    public void DeadRPC()
    {
        StartCoroutine(Vanish());
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
    private IEnumerator Vanish()
    {
        Color currentColor = sprite.color;
        while(currentColor.a > 0)
        {
            yield return new WaitForSeconds(0.2f);
            currentColor.a -= 0.1f;
            sprite.color = currentColor;
        }
    }
    #endregion
}
