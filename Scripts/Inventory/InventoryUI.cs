using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    public Transform itemParent;
    public GameObject inventoryUI;
    public static InventoryUI instance;

    private Inventory inventory;
    private InventorySlot[] slots;

    [SerializeField] private GameObject tooltip;
    private TextMeshProUGUI tooltipText;

    private void Awake()
    {
        if (instance != null)
        {
            return;
        }

        instance = this;
        tooltipText = tooltip.GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Start()
    {
        inventory = Inventory.instance;
        inventory.onItemChangedCallback += UpdateUI;

        slots = itemParent.GetComponentsInChildren<InventorySlot>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            inventoryUI.SetActive(!inventoryUI.activeSelf);
        }
    }

    public void OpenBag()
    {
        inventoryUI.SetActive(!inventoryUI.activeSelf);
    }
    
    void UpdateUI()
    {
        for (int i = 0; 1 < slots.Length; i++)
        {
            if (i < inventory.items.Count)
            {
                slots[i].AddItem(inventory.items[i]);
            }
        }
    }

    public void ShowTooltip(Vector3 position, IDescribable description)
    {
        tooltip.SetActive(true);
        tooltip.transform.position = position;
        tooltipText.text = description.GetDescription();
    }

    public void ShowEmptyTooltip(Vector3 position, IDescribable description)
    {
        tooltip.SetActive(true);
        tooltip.transform.position = position;
        tooltipText.text = string.Format("<b>Empty slot</b>") + string.Format("\n\nThis slot is empty..\nTry another") ;
    }

    public void HideTooltip()
    {
        tooltip.SetActive(false);
    }
}
