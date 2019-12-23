using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using ZoneDepict;
public abstract class ItemBase
{
    //用於識別道具 判斷道具欄該顯示哪張圖片
    public int id;
    //Item cooldown in seconds;
    public int Amount;
    [SerializeField]
    public float ItemCD; // 每次使用間隔
    public float NextValidTime;
    //初始化道具

    #region Class Methods
    public ItemBase()
    {
        ItemCD = 1.5f;
    }
    #endregion

    #region ItemBase Interface

    public abstract bool canReuse();

    public abstract void Use(Character Caller);

    public abstract void ItemEffect(Character Caller);

    public abstract bool IsUsable();

    public abstract bool IsGarbage();

    //回傳狀態 可重複利用者回傳剩餘CD 其他回傳數量
    public abstract float ItemState();
    #endregion

    protected void SendEffectEvent(string effectName,Vector3 Pos, Quaternion Rot)
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            byte evCode = (byte)ZDGameEvent.SpawnEffect; 
            object[] content = { effectName, Pos, Rot}; 
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            SendOptions sendOptions = new SendOptions { Reliability = true };
            PhotonNetwork.RaiseEvent(evCode, content, raiseEventOptions, sendOptions);
        }
    }
}

