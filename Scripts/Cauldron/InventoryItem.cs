using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Inventory Items/Generic", order = 1)]
public class InventoryItem : ScriptableObject
{
	public Sprite inventoryIcon = null;
	public SFXManager.ClipCategory pickupSound = default;
	public ScriptableInteractionText pickupText = null;
	public string tooltipText = null;

	[SerializeField, Tooltip( "Is this item supposed to be spawned from the start of the game, or does it spawn during gameplay?" )]
	public bool spawnedAtStart;

	public virtual void OnClick()
	{
		Debug.Log("Inventory Click");
	}
}
