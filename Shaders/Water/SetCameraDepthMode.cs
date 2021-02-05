using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCameraDepthMode : MonoBehaviour
{
    [SerializeField] private bool fix;
    private void OnValidate()
    {
        SetCameraDepthTextureMode();
    }

    private void Awake()
    {
        SetCameraDepthTextureMode();
    }

    private void SetCameraDepthTextureMode()
    {
        GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
    }
}
