using UnityEngine;

public class InteractableSceneTransition : Interactable
{
	[SerializeField] private RoomManager.Room room = default;

	[SerializeField] private InventoryItem key;
	[SerializeField] private ScriptableInteractionText doorIsLockedText;

	private float lastTransition = 0;
	private const float spamClickProtection = 3.0f;

	public override void Interact()
	{
		if( key != null && !GlobalState.observatoryDoorUnlocked )
		{
			if( !TryUnlockDoor() ) return;
		}

		if( Time.time - lastTransition < spamClickProtection )
		{
			return;
		}

		lastTransition = Time.time;

		// Start doing the cool wipe animation
		var position = transform.position;

		TransitionFader.WipeToBlack( position, RoomManager.animationTime );
		SFXManager.PlaySoundAt( SFXManager.ClipCategory.ChangeArea, position );
		RoomManager.TransitionToRoom( room );
	}

	private bool TryUnlockDoor()
	{
		if( !InventoryManager.Contains( key ) )
		{
			// We don't have the key
			TextParent.SpawnText( doorIsLockedText, transform.position + Vector3.up );

			return false;
		}
		else
		{
			InventoryManager.Remove( key );
			GlobalState.MakeThisNotSpawn( key );
			GlobalState.observatoryDoorUnlocked = true;

			return true;
		}
	}

	private void OnEnable()
	{
		RoomManager.RegisterTransition( this, room );
	}

	private void OnDisable()
	{
		RoomManager.UnregisterTransition( this, room );
	}

	public Transform GetExitPoint()
	{
		return base.StandPoint;
	}

	public string GetSceneName()
	{
		return gameObject.scene.name;
	}
}
