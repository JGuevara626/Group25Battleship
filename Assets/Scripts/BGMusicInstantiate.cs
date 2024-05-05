using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMusicInstantiate : MonoBehaviour
{
    public AudioSource bgMusic;
    private void Awake()
    {
        bgMusic = GetComponent<AudioSource>();
        if (PlayerPrefs.HasKey("volume"))
        {
            bgMusic.volume = PlayerPrefs.GetFloat("volume");
        }
    }
}
