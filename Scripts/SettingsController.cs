using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingsController : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider audioslider;
    public Toggle toggle;
    public Dropdown resDropdown;
    public Dropdown gDropdown;

    Resolution[] resolutions;

    void Start()
    {
        //Set current quality
        int curG;
        curG = QualitySettings.GetQualityLevel();
        gDropdown.value = curG;

        //Set toggle
        toggle.isOn = Screen.fullScreen;

        //Set Slider
        float val;
        audioMixer.GetFloat("volume", out val);
        audioslider.value = val;

        //Setup resolution dropdown
        resolutions = Screen.resolutions;

        resDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentindex = 0;

        for(int i=0; i<resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if(resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
            {
                currentindex = i;
            }
        }

        resDropdown.AddOptions(options);
        resDropdown.value = currentindex;
        resDropdown.RefreshShownValue();
    }

    public void setVolume(float vol)
    {
        audioMixer.SetFloat("volume", vol);
    }

    public void setQuality(int index)
    {
        QualitySettings.SetQualityLevel(index, true);
        Debug.Log("Changing Quality Settings: " + index);
    }

    public void setFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void setResolution(int index)
    {
        Resolution resolution = resolutions[index];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
}
