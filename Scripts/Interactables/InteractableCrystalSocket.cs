using UnityEngine;

public class InteractableCrystalSocket : Interactable
{
	[SerializeField] private InventoryItem crystal;
	[SerializeField] private GameObject crystalVisual;
	[SerializeField] private Animation door;

	[SerializeField] private ScriptableInteractionText poweredOn;
	[SerializeField] private ScriptableInteractionText poweredOff;
	[SerializeField] private ScriptableInteractionText unpowered;
	[SerializeField] private ScriptableInteractionText crystalInsert;



	private void Start()
	{
		crystalVisual.SetActive( GlobalState.observatoryCrystalInserted );
		door["TelescopeDoorClose"].time = GlobalState.observatoryCrystalInserted ? 5.0f : 0.0f;
	}

	public override void Interact()
	{
		if( GlobalState.observatoryCrystalInserted )
		{
			if( GlobalState.ObservatoryPowered )
			{
				TextParent.SpawnText( poweredOn, transform.position );
			}
			else
			{
				TextParent.SpawnText( poweredOff, transform.position );
			}
		}
		else
		{
			if( InventoryManager.Contains( crystal ) )
			{
				TextParent.SpawnText( crystalInsert, transform.position );

				InventoryManager.Remove( crystal );
				GlobalState.MakeThisNotSpawn( crystal );
				GlobalState.observatoryCrystalInserted = true;

				//@TODO: Play a powering-up sound

				//@TODO: Display powering-up text

				door.Play();

			}
			else
			{
				TextParent.SpawnText( unpowered, transform.position );
			}
		}


		// @TODO: Do some animation when we have that

		SetSocketState( GlobalState.observatoryCrystalInserted );
	}

	public void SetSocketState(bool crystalIsInserted)
	{
		crystalVisual.SetActive( crystalIsInserted );
	}
}
