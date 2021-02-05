using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeManager : MonoBehaviour
{
    public void ChangeVolume(float value)
    {
        AudioListener.volume = value;
    }
}
