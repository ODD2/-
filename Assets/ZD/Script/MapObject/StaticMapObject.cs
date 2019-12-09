using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZoneDepict;
using ZoneDepict.Rule;
public class StaticMapObject : ZDRegisterObject
{
    public bool update;
    // Start is called before the first frame update
    protected new void Start()
    {
        base.Start();
        Vector3 NewPos = transform.position;
        NewPos.z = (int)TypeDepth.MapObject;
        transform.position = NewPos;
        enabled = update;
        //this.gameObject.SetActive(CanUpdate);
    }
}
