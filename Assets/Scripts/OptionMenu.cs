using UnityEngine.Audio;
using UnityEngine;

public class OptionMenu : MonoBehaviour
{
     public AudioMixer mainMixer;

     public void SetVolume(float volume){
        mainMixer.SetFloat("volume", volume);
     }
     public void SetScreen(bool isFullscreen){
           Screen.fullScreen = isFullscreen;
     }
     public void SetQuality(int qualityIndex){
        QualitySettings.SetQualityLevel(qualityIndex);
     }
}
