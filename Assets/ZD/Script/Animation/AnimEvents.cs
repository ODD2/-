using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimEvents : MonoBehaviour
{
    void SelfDestroy()
    {
        Destroy(this.gameObject);
    }

    void RandomInterval()
    {
       StartCoroutine(PauseAndPlay());
    }

    #region HELPER
    IEnumerator PauseAndPlay()
    {
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.enabled = false;
            yield return new WaitForSeconds(Random.Range(0.1f, 1.5f));
            animator.enabled = true;
        }
    }
    #endregion
}
