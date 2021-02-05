using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{

    [SerializeField] AudioMixer masterMixer;
    [SerializeField] AudioCategory audioCategory;
    enum AudioCategory { master, sfx, ambience, music }
    float SliderValue
    {
        get => slider.value;
    }

    Slider slider;

    public void SetVolume()
    {
        switch (audioCategory)
        {
            case AudioCategory.master:
                SetMaster(SliderValue);
                break;
            case AudioCategory.sfx:
                SetSfx(SliderValue);
                break;
            case AudioCategory.ambience:
                SetAmbience(SliderValue);
                break;
            case AudioCategory.music:
                SetMusic(SliderValue);
                break;
            default:
                break;
        }
    }

    private void Awake()
    {
        slider = GetComponent<Slider>();
        slider.minValue = -80;
        slider.maxValue = 0; 
    }

    public void SetMaster(float level)
    {
        masterMixer.SetFloat("Master", level);


    }



    public void SetAmbience(float level)
    {
        masterMixer.SetFloat("Ambience", level);


    }



    public void SetSfx(float level)
    {
        masterMixer.SetFloat("Sfx", level);


    }


    public void SetMusic(float level)
    {
        masterMixer.SetFloat("Music", level);


    }

}
