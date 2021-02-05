using UnityEngine;

public class CauldronInteractible : Interactable
{
    [SerializeField] CauldronMixing cauldronMixing;
    public override void Interact()
    {
        InventoryManager.InventoryIsOpen = true;
        cauldronMixing.BeginMixing();
    }

}
