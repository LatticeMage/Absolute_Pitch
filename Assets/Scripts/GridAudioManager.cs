using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridAudioManager : MonoBehaviour
{
 
    [Range(0, 1)]
    public float volume;

    public List<AudioSource> AudioSources;

    void Start()
    {
        AudioSources = new List<AudioSource>();
    }

    private void Update()
    {
        foreach(AudioSource ad in AudioSources)
            ad.volume = volume;
    }

    public void OnVolumnChange(float vol)
    {
        this.volume = vol;
    }
}
