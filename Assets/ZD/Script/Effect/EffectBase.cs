using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZoneDepict.Rule;

public class EffectBase : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //Setup Effect
        Vector3 NewPos = transform.position;
        NewPos.z = (int)TypeDepth.Effect;
        transform.position = NewPos;
    }
}
