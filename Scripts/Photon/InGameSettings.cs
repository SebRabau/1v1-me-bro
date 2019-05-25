using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class InGameSettings : Photon.MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider audioSlider;
    public PhotonMovement playervar;

    // Start is called before the first frame update
    public void updateSettings()
    {
        //Set Slider
        float val;
        audioMixer.GetFloat("volume", out val);
        audioSlider.value = val;
    }

    public void setVolume(float vol)
    {
        audioMixer.SetFloat("volume", vol);
    }

    public void setSensitivity(float sens)
    {
        playervar.lookSpeed = sens;
    }
   
}
