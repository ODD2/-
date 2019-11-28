using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZoneDepict;

    public abstract class ItemContainerBase : ZDObject 
    {
        // Start is called before the first frame update
    public float Durability;
    public new void Start()
    {
        base.Start();
        Durability = 10;
    }


    // Update is called once per frame
    public new void Update()
    {
        base.Update();
    }
        public abstract void Damaged(float damaged);
        public abstract void Broken();

    }
