using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZoneDepict
{
    public class ZDRegisterObject : ZDObject
    {
        bool FirstUpdate = true;
        // Update is called once per frame
        protected new void Update()
        {
            if (FirstUpdate)
            {
                //Register Objects Update Only Once
                base.Update();
                FirstUpdate = false;
            }
            //Do Not Update ZDObject Regulars After The First Update.
        }
    }

}
