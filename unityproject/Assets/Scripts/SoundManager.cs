﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioClip[] ballBounces;
    public AudioClip[] crowdSounds;
    public AudioClip[] playerGrunts;
    public AudioClip[] playerServices;
    public AudioClip[] shoeSqueaks;
    public AudioClip[] racquetHits;
    public AudioClip[] racquetSwishes;
    
    public AudioSource ballAudioSource;
    public AudioSource sceneAudioSource;

    public void PlayCrowdSounds()
    {
        sceneAudioSource.PlayOneShot(ChooseRandom(crowdSounds));
    }

    public void PlayGrunt(AudioSource playerAudioSource)
    {
        playerAudioSource.PlayOneShot(ChooseRandom(playerGrunts));
    }

    public void PlayRacquetHit(AudioSource playerAudioSource)
    {
        playerAudioSource.PlayOneShot(ChooseRandom(racquetHits));
    }

    public void PlayRacquetSwish(AudioSource playerAudioSource)
    {
        playerAudioSource.PlayOneShot(ChooseRandom(racquetSwishes));
    }

    public void PlayBallBounce()
    {
        ballAudioSource.PlayOneShot(ChooseRandom(ballBounces));
    }

    public void PlayService(AudioSource playerAudioSource)
    {
        playerAudioSource.PlayOneShot(ChooseRandom(playerServices));
    }

    public void PlayShoeSqueak(AudioSource playerAudioSource)
    {
        playerAudioSource.PlayOneShot(ChooseRandom(shoeSqueaks));
    }

    private static AudioClip ChooseRandom(IReadOnlyList<AudioClip> clips)
    {
        return clips[Random.Range(0, clips.Count - 1)];
    }
}
