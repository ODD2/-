using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemBase
{
    //用於識別道具 判斷道具欄該顯示哪張圖片
    public int id;
    //初始化道具
    public virtual void Initial() { }


    public abstract void Use(Character _player);

    public abstract void ItemEffect(Character _player);

    public abstract bool IsUsable();

    public abstract bool IsGarbage();


}

