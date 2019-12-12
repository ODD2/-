using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZoneDepict;
using ZoneDepict.Rule;

public class Grass : StationaryMapObject
{
    public AudioClip InGrassAudio;
    protected uint InGrassCount;
    protected AudioSource audioSource;



    protected new void Start()
    {
        base.Start();
        StartCoroutine(PauseAndPlay());
        audioSource = GetComponent<AudioSource>();
    }

    protected new void Update()
    {
        base.Update();
    }

    protected void FixedUpdate()
    {
        uint NewCount = 0;
        List<ZDObject> FetchList =  ZDMap.HitAtObject(this, ETypeZDO.Character);
        if (FetchList != null) NewCount = (uint)FetchList.Count;
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
    }

    IEnumerator PauseAndPlay()
    {
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.enabled = false;
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 1.5f));
            animator.enabled = true;
        }
    }
}
