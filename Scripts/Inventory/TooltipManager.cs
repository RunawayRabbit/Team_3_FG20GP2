using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager singleton;
    public GameObject graphics;
    private InventoryTooltip lastCaller;

    private void Awake()
    {
        singleton = this;
    }

    [SerializeField] private TextMeshProUGUI tmpComp;
    void SetText(string text)
    {
        tmpComp.text = text;
    }

    public void UpdateTooltip(InventoryTooltip caller, bool hide = false)
    {
        if (!hide)
        {
            lastCaller = caller;
            graphics.SetActive(true);
            transform.position = caller.transform.position;
            tmpComp.text = caller.text;
        }
        else
        {
            if(lastCaller == caller)
                graphics.SetActive(false);
        }
    }
}
