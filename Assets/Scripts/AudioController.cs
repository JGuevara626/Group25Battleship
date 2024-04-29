using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioController : MonoBehaviour
{
    public Slider musicSlider;
    private float musicVolume = 1f;
    public AudioSource bgMusic;
    public GameObject bgMusicObj;
    public AudioSource soundfx;
    public GameObject soundfxObj;

    private void Start()
    {
        bgMusicObj = GameObject.FindGameObjectWithTag("BackgroundMusic");
        bgMusic = bgMusicObj.GetComponent<AudioSource>();

        soundfxObj = GameObject.FindGameObjectWithTag("audioclipS");
        soundfx = soundfxObj.GetComponent<AudioSource>();

        musicVolume = PlayerPrefs.GetFloat("volume");
        bgMusic.volume = musicVolume;
        musicSlider.value = musicVolume;
    }
    private void Update()
    {
        musicVolume = musicSlider.value;
        bgMusic.volume = musicVolume;
        PlayerPrefs.SetFloat("volume", musicVolume);
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
    }
}
