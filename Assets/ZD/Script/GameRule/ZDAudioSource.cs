using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZoneDepict.Rule;

namespace ZoneDepict.Audio
{
    public class ZDAudioSource
    {
       static public GameObject PlayAtPoint(AudioClip clip, Vector3 pos, float vol=1.0f,bool loop = false)
       {
            GameObject Sample = ZDAssetTable.GetObject("AudioPlayer");
            GameObject AudioPlayer = null;
            if (Sample)
            {
                AutoDestroyAudioSource audioController;
                AudioPlayer = Object.Instantiate(Sample,pos,Quaternion.identity);
                audioController = AudioPlayer.GetComponent<AutoDestroyAudioSource>();
                if (audioController)
                {
                    //找得到就播放聲音
                    audioController.Setup(clip, vol, loop);
                }
                else
                {
                    //找不到Script就刪掉
                    Object.Destroy(AudioPlayer);
                    AudioPlayer = null;
                }
            }
            return AudioPlayer;
       }

        static public void SetupAudioSource(AudioSource audioSource)
        {
            audioSource.spatialBlend = 1;
            audioSource.minDistance = 1;
            audioSource.dopplerLevel = 0;
            audioSource.maxDistance = ZDGameRule.MAX_AUDIO_DISTANCE;
        }
    }
}

