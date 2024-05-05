using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioController : MonoBehaviour
{
    public Slider musicSlider;
    public Slider soundfxSlider;
    private float musicVolume = 1f;
    private float soundfxVolume = 1f;
    public AudioSource bgMusic;
    public GameObject bgMusicObj;
    public AudioSource soundfx;
    public GameObject soundfxObj;

    private void Awake()
    {
     bgMusicObj = GameObject.FindGameObjectWithTag("BackgroundMusic");
        bgMusic = bgMusicObj.GetComponent<AudioSource>();

        soundfxObj = GameObject.FindGameObjectWithTag("audioclipS");
        if (soundfxObj != null) { soundfx = soundfxObj.GetComponent<AudioSource>(); } else { soundfx = null; }


        if (PlayerPrefs.HasKey("volume"))
        {
            musicVolume = PlayerPrefs.GetFloat("volume");
            musicSlider.value = musicVolume;
        }
        else
        {
            PlayerPrefs.SetFloat("volume", 1f);
            musicVolume = 1f;
            musicSlider.value = musicVolume;
            bgMusic.volume = musicVolume;
        }

        if (PlayerPrefs.HasKey("soundfxVolume"))
        {
            soundfxVolume = PlayerPrefs.GetFloat("soundfxVolume");
            soundfxSlider.value = soundfxVolume;
        } else
        {
            PlayerPrefs.SetFloat("soundfxVolume", 1f);
            soundfxVolume = 1f;
            soundfxSlider.value = soundfxVolume;
        }   
    }
    private void Update()
    {
        musicVolume = musicSlider.value;
        bgMusic.volume = musicVolume;
        PlayerPrefs.SetFloat("volume", musicVolume);
        if (soundfxObj == null)
        {
            soundfxObj = GameObject.FindGameObjectWithTag("audioclipS");
            soundfxVolume = soundfxSlider.value;
            if(soundfxObj != null)
            {
                soundfx = soundfxObj.GetComponent<AudioSource>();
                soundfx.volume = soundfxVolume;
            }
        }
        else
        {
            soundfxVolume = soundfxSlider.value;
            soundfx.volume = soundfxVolume;
        }
        PlayerPrefs.SetFloat("soundfxVolume", soundfxVolume);
    }
    public void VolumeUpdater(float volume)
    {
        musicVolume = volume;
    }
    public void MusicReset()
    {
        PlayerPrefs.DeleteKey("volume");
        bgMusic.volume = 1;
        musicSlider.value = 1;
        PlayerPrefs.DeleteKey("soundfxVolume");
        soundfx.volume = 1;
        soundfxSlider.value = 1;
    }
}
