using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ResolutionManager : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown resDropdown;
    private Resolution[] resolutions;
    private List<Resolution> resolutionList;

    private int currentResIndex;

    void Start()
    {
        resolutions = Screen.resolutions;
        resolutionList = new List<Resolution>();

        resDropdown.ClearOptions();

        for (int i = 0; i < resolutions.Length; i++)
        {
            float widthFloat = (float)(resolutions[i].width);
            float heightFloat = (float)(resolutions[i].height);
            if (widthFloat/heightFloat == 16.0/9.0)
            {
                resolutionList.Add(resolutions[i]);
            }
        }

        List<string> options = new List<string>();
        for (int i = 0;i < resolutionList.Count; i++) 
        {
            string resolutionOption = resolutionList[i].width + "x" + resolutionList[i].height;
            options.Add(resolutionOption);
            if (resolutionList[i].width == Screen.width && resolutionList[i].height == Screen.height)
            {
                currentResIndex = i;
            }
        }

        resDropdown.AddOptions(options);
        resDropdown.value = currentResIndex;
        resDropdown.RefreshShownValue();
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutionList[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, true);
    }
}
