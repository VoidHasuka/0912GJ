using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager
{
    private AudioSource waveAudioSource;

    private AudioClip waveClipA;
    private AudioClip waveClipB;
    private AudioClip waveClipC;
    public void Init()
    {
        //waveAudioSource = new AudioSource();
        //waveAudioSource.loop = false;

        GameObject audioSource = new GameObject("AudioSource");
        waveAudioSource = audioSource.AddComponent<AudioSource>();
        waveAudioSource.loop = false;


        waveClipA = Resources.Load<AudioClip>("Audio/A");
        waveClipB = Resources.Load<AudioClip>("Audio/B");
        waveClipC = Resources.Load<AudioClip>("Audio/C");
    }

    public void PlayWaveAudio(WaveType waveType)
    {
        AudioClip toBePlayClip = null;
        switch (waveType)
        {
            case WaveType.A:
                toBePlayClip = waveClipA;
                break;
            case WaveType.B:
                toBePlayClip= waveClipB;
                break;
            case WaveType.C:
                toBePlayClip = waveClipC;
                break;
            default:
                break;
        }
        waveAudioSource.clip = toBePlayClip;
        waveAudioSource.Play();
    }
}
