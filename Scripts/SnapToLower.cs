using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


[ExecuteAlways]
public class SnapToLower : MonoBehaviour
{
    private RectTransform rectT;
    [SerializeField] Vector2 offset;

    private void OnEnable()
    {
        rectT = transform as RectTransform;
    }

    private void Awake()
    {
        rectT = transform as RectTransform;
    }

    private void Update()
    {
        transform.localPosition = new Vector3(-rectT.sizeDelta.x/2, -rectT.sizeDelta.y/2, transform.localPosition.z) + new Vector3( offset.x, offset.y,0);
    }


}
