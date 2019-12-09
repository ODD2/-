using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatDropItem : DropItem
{
   
    public float amplitude = 0.1f;
    public float frequency = 1f;

    // Position Storage Variables
    Vector3 posOffset = new Vector3();
    Vector3 tempPos = new Vector3();

    protected new void Start()
    {
        base.Start();
        posOffset = transform.position;
    }

    protected new void Update()
    {
        base.Update();
        // Float up/down with a Sin()
        tempPos = posOffset;
        tempPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude;
        transform.position = tempPos;
    }
}
