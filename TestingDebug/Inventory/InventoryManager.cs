using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
	public static InventoryManager singleton;
	[SerializeField] private TextMeshProUGUI tooltipPrefab;
	[SerializeField] private Transform contentParent;

	[SerializeField] private Transform inventoryFrame;

	[SerializeField] private GameObject inventoryUIPrefab;

	public static Dictionary<InventoryItem, GameObject> _inventory = new Dictionary<InventoryItem, GameObject>();

	public static bool InventoryIsOpen
	{
		get =>singleton && singleton.inventoryFrame.gameObject.activeInHierarchy;
		set
		{
			if(singleton)
				singleton.inventoryFrame.gameObject.SetActive(value);
		}
	}

	private void Awake() { singleton = this; }

	public static bool Contains( InventoryItem item )
	{
		if( singleton == null ) return false;
		return _inventory.ContainsKey( item );
	}

	public static void Add( InventoryItem item )
	{
		if( !item )
		{
			Debug.LogWarning( "lol books" );

			return;
		}

		if( !singleton ) return;

		if(_inventory.ContainsKey( item ))
			Debug.LogWarning( $"Inventory already contains {item} and Euan doesn't know what I should do about that." );
		else
		{
			// Spawn the UI item
			GameObject UIObject = Instantiate( singleton.inventoryUIPrefab, singleton.contentParent );
			UIObject.GetComponent<Image>().sprite = item.inventoryIcon;

			UIObject.GetComponent<InventoryTooltip>().text = item.tooltipText;

			// Register the button
			UIObject.GetComponent<Button>().onClick.AddListener( item.OnClick );


			// Add to the "inventory"

			_inventory.Add( item, UIObject );
		}
	}

	public static void Remove( InventoryItem item )
	{
		GameObject UIObject = _inventory[item];

		if( UIObject == null ) return;

		// Unregister the button
		UIObject.GetComponent<Button>().onClick.RemoveAllListeners();

		// Destroy the UI item
		Destroy( UIObject );

		// Remove from the "inventory"
		_inventory.Remove( item );
	}
}
