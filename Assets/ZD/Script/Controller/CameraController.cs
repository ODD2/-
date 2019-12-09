using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    static GameObject target = null;

    public static void SetTarget(GameObject NewTarget)
    {
        target = NewTarget;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (target)
        {
            Vector3 NewPos = target.transform.position;
            NewPos.z = transform.position.z;
            transform.position = NewPos;
        }
    }
}
