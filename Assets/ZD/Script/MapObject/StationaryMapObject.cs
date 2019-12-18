using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZoneDepict;
using ZoneDepict.Rule;
//Using ZD Register Because This ZDObject Is Not Going To Be Moving.(Stationary)
public class StationaryMapObject : ZDRegisterObject
{
    public bool update = true;
    // Start is called before the first frame update
    protected new void Start()
    {
       
        //Setup ZDObjectt Unit World  Scale
        Vector3 NewScale = transform.localScale;
        NewScale.x *= ZDGameRule.UNIT_IN_WORLD;
        NewScale.y *= ZDGameRule.UNIT_IN_WORLD;
        transform.localScale = NewScale;

        //Setup ZDObject Type Depth.
        base.Start();

        //Setup enabled.
        enabled = update;
    }
}
