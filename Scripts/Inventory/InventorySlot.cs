using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject ShelfUI;
    
    [SerializeField] private Image icon;

    private Item item;
    
    public void AddItem(Item newItem)
    {
        item = newItem;

        icon.sprite = item.icon;
        icon.enabled = true;
    }

    public void UseItem()
    {
        if (item != null)
        {
            item.Use();
        }
    }

    public void OpenShelf()
    {
        ShelfUI.SetActive(!ShelfUI.activeSelf);
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item != null) // on hover on top of item, show popup tooltip
        {
            InventoryUI.instance.ShowTooltip(transform.position, item);
        }
        // else
        // {
        //     InventoryUI.instance.ShowEmptyTooltip(transform.position, item);
        // }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        InventoryUI.instance.HideTooltip(); // on exiting icon space, hide tooltip 
    }
}
