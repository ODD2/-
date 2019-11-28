﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ItemSystem
{
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
            Debug.Log("DisposableItme item Effect.");
        }

        public override void Use(Character _player)
        {
            itemAmount -= 1;
        }

        void Start()
        {
            //隨機道具數量
            itemAmount = Random.Range(0, 5);
        }
    }
}