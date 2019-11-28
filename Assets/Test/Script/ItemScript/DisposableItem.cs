using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class DisposableItem : ItemBase
    {
        // 道具數量

        protected int itemAmount;

        public override bool IsGarbage()
        {
            if (itemAmount <= 0)
            {
                return true;
            }
            return false;
        }

        public override bool IsUsable()
        {
            if (itemAmount > 0)
            {
                return true;
            }
            return false;
        }

        public override void ItemEffect(Character _player)
        {
        }

        public override  void Use(Character _player)
        {
           // Debug.Log("Disposableitem use");
            itemAmount -= 1;
            ItemEffect(_player);
           
        }
    }
