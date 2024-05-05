using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundFxInstantiate : MonoBehaviour
{
    public AudioSource soundfx;
    private void Awake()
    {
        soundfx = GetComponent<AudioSource>();
        if(PlayerPrefs.HasKey("soundfxVolume"))
        {
            soundfx.volume = PlayerPrefs.GetFloat("soundfxVolume");
        }
    }
}
