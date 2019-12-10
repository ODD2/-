using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroyAudioSource : MonoBehaviour
{
    AudioSource audioSource;
    public AudioClip clip;
    public float vol=1.0f;
    public bool loop;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource)
        {
            audioSource.clip = clip;
            audioSource.volume = vol;
            audioSource.loop = loop;
            audioSource.Play();
            enabled = true;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Setup(AudioClip clip, float vol = 1.0f,bool loop = false)
    {
        this.clip = clip;
        this.vol = vol;
        this.loop = loop;
    }

    // Update is called once per frame
    void Update()
    {
        if (!audioSource.isPlaying) Destroy(gameObject);
    }
}
