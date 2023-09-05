using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private Slider musicVSlider;
    [SerializeField] private Slider sfxVSlider;
    void Start()
    {
        musicVSlider.value =  SoundManager.Instance!=null ?  SoundManager.Instance.musicVolume : 0.5f;
        sfxVSlider.value = SoundManager.Instance!=null ?  SoundManager.Instance.sfxVolume : 0.5f;
    }
    public void ChangeBGMVolume()
    {
        float sliderValue = musicVSlider.value;
        //mainAM.SetFloat("musicVolume",
        //    Mathf.Log10(sliderValue) * 20);
        SoundManager.Instance?.SetMusicVolume(sliderValue);
    }
    public void ChangeSFXVolume()
    {
        float sliderValue = sfxVSlider.value;
        //mainAM.SetFloat("sfxVolume",
        //    Mathf.Log10(sliderValue) * 20);
        SoundManager.Instance?.SetSFXVolume(sliderValue);
    }
}
