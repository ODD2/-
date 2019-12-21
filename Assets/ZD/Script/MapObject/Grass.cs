using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZoneDepict;
using ZoneDepict.Rule;
using ZoneDepict.Map;
using ZoneDepict.Audio;
using Random = UnityEngine.Random;


public class Grass : StationaryMapObject
{
    
    protected uint InGrassCount;
    public AudioClip InGrassAudio;
    protected AudioSource audioSource;
    protected Animator animator;



    protected new void Start()
    {
        ActorCustomDepthShift = (int)ZDGameRule.WorldToUnit(transform.position).x % 2 == 0 ?
                         -1e-5f :1e-5f ;
        base.Start();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        ZDAudioSource.SetupAudioSource(audioSource);
        //StartCoroutine(PauseAndPlay());
    }

    protected new void Update()
    {
        base.Update();
    }

    protected void FixedUpdate()
    {
        uint NewCount = 0;
        List<ZDObject> FetchList =  ZDMap.HitAtObject(this, EObjectType.Character);
        if (FetchList != null) NewCount = (uint)FetchList.Count;
        else NewCount = 0;

        if (NewCount != InGrassCount)
        {
            GrassChangeEffect();
            InGrassCount = NewCount;
        }    
    }

    protected void GrassChangeEffect()
    {
        if (audioSource)
        {
            audioSource.PlayOneShot(InGrassAudio);
        }
        if (animator)
        {
            animator.SetTrigger("Move");
        }
    }

    IEnumerator PauseAndPlay()
    {
       
        if (animator != null)
        {
            animator.enabled = false;
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 1.5f));
            animator.enabled = true;
        }
    }
}
