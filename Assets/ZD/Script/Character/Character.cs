using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
using ZoneDepict;
using ZoneDepict.Rule;
using ZoneDepict.Audio;
using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;

// This class is basic of Charater, and all infos are
// in this class
public class Character : ZDObject, IPunObservable, IADamageObject, IPunInstantiateMagicCallback
{
    #region Components
    protected Animator animator;
    protected SpriteRenderer sprite;
    protected AudioSource audioSource;
    //Sound Effects
    [Header("Test")]
    [Tooltip("Tooltip")]
    public AudioClip DeathSound;
    public AudioClip MoveSound;
    public AudioClip HurtSound;
    #endregion

    #region Input Wrappers
    public virtual void InputAttack(Vector2 AttackDirection, EAttackType Type)
    {
      
    }

    public virtual void InputSprint(Vector2 Destination)
    {

    }
    #endregion

    #region Character Attributes
    public int TeamID { get; protected set; } = 0;
    public CharacterState currentState { get; protected set; } = CharacterState.Alive;

    public CharacterValue basicValues = new CharacterValue();

    protected int Soul = 0;
    protected const int MaxSoul = 5;

    protected Vector2 Velocity = new Vector2(0, 0);
    protected float MaxVelocity = 30;

    List<ItemBase> Inventory = new List<ItemBase>();
    protected int InventoryMax = 3;

    protected float MoveMana = 5.0f;

    protected float[] MaxSkillCD = new float[4];
    protected float[] SkillMana = new float[4];
    protected float[] SkillCD = new float[4];

    #endregion

    #region Getters/Setters
    public List<ItemBase> GetInventory()
    {
        return Inventory;
    }
    public float GetRegHP()
    {
        return basicValues.RegHP;
    }
    public float GetRegMP()
    {
        return basicValues.RegMP;
    }
    public float GetHP()
    {
        return basicValues.HP;
    }
    public void SetHP(float NewHP)
    {
        if (NewHP > basicValues.MaxHP) basicValues.HP = basicValues.MaxHP;
        else if (NewHP < 0) basicValues.HP = 0;
        else basicValues.HP = NewHP;
    }
    public void SetMaxHP(float maxHP)
    {
        basicValues.MaxHP = maxHP;
    }
    public void SetMaxMP(float maxMP)
    {
        basicValues.MaxMP = maxMP;
    }
    public float GetMaxHP()
    {
        return basicValues.MaxHP;
    }
    public float GetMP()
    {
        return basicValues.MP;
    }
    public void SetMP(float NewMP)
    {
        if (NewMP > basicValues.MaxMP) basicValues.MP = basicValues.MaxMP;
        else if (NewMP < 0) basicValues.MP = 0;
        else basicValues.MP = NewMP;
    }
    public float GetMaxMP()
    {
        return basicValues.MaxMP;
    }
    public int GetSoul()
    {
        return Soul;
    }
    public void SetSoul(int NewSoul)
    {
        if (NewSoul > MaxSoul) Soul = MaxSoul;
        else if (NewSoul < 0) Soul = 0;
        else Soul = NewSoul;
    }
    public int GetMaxSoul()
    {
        return MaxSoul;
    }
    public int GetInventoryMax()
    {
        return InventoryMax;
    }
    #endregion

    #region Delegates
    //public class SingleFloatEventArgs : EventArgs
    //{
    //    public SingleFloatEventArgs(float value)
    //    {
    //        this.value = value;
    //    }
    //    public float value;
    //}
    //public event EventHandler<SingleFloatEventArgs> SoulChanged;
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
        if (photonView.IsMine && !GetHP().Equals(0))
        {
            if (animator && animator.GetCurrentAnimatorStateInfo(0).IsTag("NM")) return;
            SetHP(GetHP() - Damage);
            if (GetHP().Equals(0))
            {
                Dead();
            }
            else
            {
                photonView.RPC("DoHurtRpc", RpcTarget.AllViaServer);
            }
        }
    }

    public virtual void Dead()
    {
        if (photonView.IsMine)
        {
            currentState = CharacterState.Dead;
            photonView.RPC("DoDeadRpc", RpcTarget.AllViaServer);
            if (ZDGameManager.Instance) ZDGameManager.Instance.PlayerCharacterDied();
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

        base.Start();
    }

    protected new void Update()
    {
        //Calls ZDObject Update()
        base.Update();
    }

    protected void FixedUpdate()
    {
        if(currentState == CharacterState.Alive &&
           ZDGameManager.GetGameState() == ZDGameState.Play)
        {
            UpdateAnimParams();
            SetHP(GetHP() + Time.fixedDeltaTime * GetRegHP() * 8);
            SetMP(GetMP() + Time.fixedDeltaTime * GetRegMP() * 8);
            for (int i = 0; i < 4; ++i)
            {
                if (SkillCD[i] > float.Epsilon)
                {
                    SkillCD[i] -= (Time.fixedDeltaTime > SkillCD[i]) ? SkillCD[i] : Time.fixedDeltaTime;
                }
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
            stream.SendNext(MaxVelocity);
            stream.SendNext(basicValues.HP);
            stream.SendNext(basicValues.MaxHP);
            stream.SendNext(basicValues.MP);
            stream.SendNext(basicValues.MaxMP);
            stream.SendNext(basicValues.RegHP);
            stream.SendNext(basicValues.RegMP);
            stream.SendNext(basicValues.CDR);
            stream.SendNext(basicValues.ReduceManaCost);
            stream.SendNext(basicValues.AttackBuff);
        }
        else if (stream.IsReading)
        {
            transform.position = (Vector3)stream.ReceiveNext();
            transform.localScale = (Vector3)stream.ReceiveNext();
            MaxVelocity = (float)stream.ReceiveNext();
            basicValues.HP = (float)stream.ReceiveNext();
            basicValues.MaxHP = (float)stream.ReceiveNext();
            basicValues.MP = (float)stream.ReceiveNext();
            basicValues.MaxMP = (float)stream.ReceiveNext();
            basicValues.RegHP = (float)stream.ReceiveNext();
            basicValues.RegMP = (float)stream.ReceiveNext();
            basicValues.CDR = (float)stream.ReceiveNext();
            basicValues.ReduceManaCost = (float)stream.ReceiveNext();
            basicValues.AttackBuff = (float)stream.ReceiveNext();
        }
    }
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        gameObject.name = (string)info.photonView.InstantiationData[0];
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
                        "ItemNum: {2}\n"+
                        "Soul: {3}\n",
                        GetHP(),GetMP(),Inventory.Count,Soul);
    }

    public void PlaySound(AudioClip clip)
    {
        if (audioSource) audioSource.PlayOneShot(clip);
    }
    #endregion

    #region Routines
    #endregion

}
