using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZoneDepict;
using ZoneDepict.Rule;
using ZoneDepict.Map;
using ZoneDepict.Audio;

public class AnimEvents : MonoBehaviour
{
    float PlaySoundVolume = 1.0f;
    void SelfDestroy()
    {
        Destroy(this.gameObject);
    }

    void RandomInterval()
    {
       StartCoroutine(PauseAndPlay());
    }

    void AttackInPosition(float Damage)
    {
        Vector3 UnitPos = ZDGameRule.WorldToUnit(transform.position);
        List<ZDObject> HitList = ZDMap.HitAtUnit(UnitPos, EObjectType.ADamage);
        if(HitList != null)
        {
            foreach(var obj in HitList)
            {
                ((IADamageObject)obj).Hurt(Damage);
            }
        }
    }

    public void SetPlaySoundVolume(float vol)
    {
        PlaySoundVolume = vol;
    }

    public void PlaySound(AudioClip clip)
    {
        ZDAudioSource.PlayAtPoint(clip, transform.position,PlaySoundVolume);
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
