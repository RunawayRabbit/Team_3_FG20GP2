using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    #region Singelton

    public static Inventory instance;

    private void Awake()
    {
        if (instance != null)
        {
            return;
        }

        instance = this;
    }
    
    #endregion

    public delegate void OnItemChanged();

    public OnItemChanged onItemChangedCallback;

    public int bagSize = 4;
    public List<Item> items = new List<Item>();

    public bool Add(Item item)
    {
        if (items.Count >= bagSize)
        {
            return false;
        }
        
        items.Add(item);

        if (onItemChangedCallback != null)
            onItemChangedCallback.Invoke();

        return true;
    }

    // private void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.O))
    //     {
    //         
    //     }
    // }
}
