using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZoneDepict;
using ZoneDepict.Rule;
public class StationaryMapObject : ZDRegisterObject
{
    public bool update = true;
    // Start is called before the first frame update
    protected new void Start()
    {
        base.Start();
        Vector3 NewPos = transform.position;
        NewPos.z = (int)TypeDepth.MapObject;
        transform.position = NewPos;
        Vector3 NewScale = transform.localScale;
        NewScale.x = ZDGameRule.UnitInWorld;
        NewScale.y = ZDGameRule.UnitInWorld;
        transform.localScale = NewScale;
        enabled = update;
    }
}
