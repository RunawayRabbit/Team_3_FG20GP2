
using UnityEngine;

public class BagIcon : MonoBehaviour
{
    [SerializeField] private GameObject inventoryUI;
    public void ToggleInventory()
    {
        inventoryUI.SetActive(!inventoryUI.activeSelf);
    }
}
