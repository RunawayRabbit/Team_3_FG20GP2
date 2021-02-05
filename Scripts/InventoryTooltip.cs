using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [NonSerialized]public TooltipManager tooltipObject;
    [NonSerialized]public string text;

    private void Start()
    {
        tooltipObject = TooltipManager.singleton;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltipObject.UpdateTooltip(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltipObject.UpdateTooltip(this, true);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        tooltipObject.UpdateTooltip(this, true);
    }
}
