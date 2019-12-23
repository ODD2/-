using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZoneDepict.Rule;

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
        Vector3 MainCameraPos = transform.position;
        MainCameraPos.z = (float)GameActorLayers.MainCamera;
        Camera.main.transform.position = MainCameraPos;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (target)
        {
            Vector3 NewPos = target.transform.position;
            NewPos.z = transform.position.z;
            transform.position = NewPos;
        }
    }

    private void OnDestroy()
    {
        target = null;
    }
}
