using UnityEngine;

public class CheatMenu : MonoBehaviour
{
	[SerializeField] private InventoryItem mushroom;
	[SerializeField] private InventoryItem herb;
	[SerializeField] private InventoryItem flower;

	[SerializeField] private InventoryItem crystal;
	[SerializeField] private InventoryItem key;

	[SerializeField] private ScriptableInteractionText cheatText;

	private void Update()
	{
		if( Input.GetKeyDown( KeyCode.Home ) )
		{
			ToggleMenu();
		}
	}

	private void ToggleMenu()
	{
		var menu   = gameObject.transform.GetChild( 0 ).gameObject;
		menu.SetActive(!menu.activeInHierarchy);
	}

	public void CrystalIntoScope()
	{
		GlobalState.observatoryCrystalInserted = true;
		TextParent.SpawnText(cheatText, PlayerManager.CurrentPlayer.transform.position);

		var crystalSocket = GameObject.FindObjectOfType<InteractableCrystalSocket>();
		if( crystalSocket != null ) crystalSocket.SetSocketState( true );
	}

	public void WaterGoUp()
	{
		TextParent.SpawnText(cheatText, PlayerManager.CurrentPlayer.transform.position);

		foreach( var water in FindObjectsOfType<Water>() )
		{
			water.SetTargetLevelAndBegin( GlobalState.WaterLevel-- );
		}
	}

	public void WaterGoDown()
	{
		TextParent.SpawnText(cheatText, PlayerManager.CurrentPlayer.transform.position);

		foreach( var water in FindObjectsOfType<Water>() )
		{
			water.SetTargetLevelAndBegin( GlobalState.WaterLevel++ );
		}
	}

	public void MushroomGoBig()
	{
		GlobalState.MakeThisSpawn(mushroom);
		TextParent.SpawnText(cheatText, PlayerManager.CurrentPlayer.transform.position);
		ToggleIngredient( mushroom );
	}

	public void HerbGoBig()
	{
		GlobalState.MakeThisSpawn(herb);
		TextParent.SpawnText(cheatText, PlayerManager.CurrentPlayer.transform.position);
		ToggleIngredient( herb );
	}
	public void FlowerGoBig()
	{
		GlobalState.MakeThisSpawn(flower);
		TextParent.SpawnText(cheatText, PlayerManager.CurrentPlayer.transform.position);
		ToggleIngredient( flower );
	}

	private void ToggleIngredient( InventoryItem inventoryItem )
	{
		foreach( var pikcupable in FindObjectsOfType<Pickupable>( true ) )
		{
			if(pikcupable.item == inventoryItem)
				pikcupable.gameObject.SetActive(true);
		}
	}

	public void DoorBeOpen()
	{
		GlobalState.observatoryDoorUnlocked = true;
	}

	public void GiefKey()
	{
		GlobalState.MakeThisNotSpawn(key);
		TextParent.SpawnText(cheatText, PlayerManager.CurrentPlayer.transform.position);
		InventoryManager.Add(key);
	}

	public void GiefCrystal()
	{
		GlobalState.MakeThisNotSpawn(crystal);
		TextParent.SpawnText(cheatText, PlayerManager.CurrentPlayer.transform.position);
		InventoryManager.Add(crystal);
	}

}
