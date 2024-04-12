using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsMenuButton : MonoBehaviour
{
    public string settingsMenuSceneName;

    public void OpenSettingsMenu()
    {
        SceneManager.LoadScene("SettingsScreen", LoadSceneMode.Additive);
    }

    public void CloseSettingsMenu()
    {
        SceneManager.UnloadSceneAsync("SettingsScreen");
    }
}
