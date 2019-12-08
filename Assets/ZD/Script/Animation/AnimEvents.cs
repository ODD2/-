using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimEvents : MonoBehaviour
{
    void SelfDestroy()
    {
        Destroy(this.gameObject);
    }
}
