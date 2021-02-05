using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
	private Transform _standingSpot;

	public Transform StandPoint
	{
		get
		{
			if( !_standingSpot )
			{
				_standingSpot = transform.Find( "StandingSpot" );

				if( !_standingSpot )
				{
					_standingSpot          = new GameObject( "StandingSpot" ).transform;
					_standingSpot.position = transform.position + Vector3.forward;
					_standingSpot.parent   = gameObject.transform;
				}

				_standingSpot.gameObject.hideFlags = HideFlags.HideInHierarchy;
			}

			return _standingSpot;
		}
		private set => _standingSpot = value;
	}

	public abstract void Interact();
}
