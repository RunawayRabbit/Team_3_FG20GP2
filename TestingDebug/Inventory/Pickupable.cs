using System.Collections;
using UnityEngine;

public class Pickupable : Interactable
{
	[SerializeField] public InventoryItem item;

	private bool _wasUsed = false;

	private void Start()
	{
		if( item == null )
		{
			Debug.LogWarning( "I DON'T HAVE AN INVENTORY ITEM DEFINITION", gameObject );

			return;
		}

		if( InventoryManager.Contains( item ) )
		{
			gameObject.SetActive( false );
		}

		if( GlobalState.HasThisDespawned( item ) )
		{
			gameObject.SetActive( false );
		}

		if( !item.spawnedAtStart
			&& !GlobalState.HasThisSpawned( item ) )
		{
			gameObject.SetActive( false );
		}
	}


	public override void Interact()
	{
		if( _wasUsed ) return;

		GlobalState.MakeThisNotSpawn( item );

		InventoryManager.Add( item );
		if( item?.pickupSound != null ) SFXManager.PlaySoundAt( item.pickupSound, transform.position );

		TextParent.SpawnText(item.pickupText, transform.position);

		_wasUsed = true;
		StartCoroutine( AnimateIntoInventory() );
	}

	private IEnumerator AnimateIntoInventory()
	{
		// STUB Do animation
		yield return new WaitForSeconds( 1.0f );
		gameObject.SetActive( false );
	}
}
