using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemBase
{
    //用於識別道具 判斷道具欄該顯示哪張圖片
    public int id;
    //Item cooldown in seconds;
    [SerializeField]
    protected float ItemCD;
    protected float NextValidTime;
    //初始化道具

    #region Class Methods
    public ItemBase()
    {
        ItemCD = 1.5f;
    }
    #endregion

    #region ItemBase Interface
    public abstract void Use(Character Caller);

    public abstract void ItemEffect(Character Caller);

    public abstract bool IsUsable();

    public abstract bool IsGarbage();
    #endregion

}

