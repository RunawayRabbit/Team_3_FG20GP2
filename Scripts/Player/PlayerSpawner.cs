using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class PlayerSpawner : MonoBehaviour
{
	[SerializeField] public Camera roomCamera;
	[SerializeField] public GameObject _playerPrefab;

	private readonly Vector3 _characterBounds = new Vector3( 0.8f, 1.5f, 0.4f );

	private void OnEnable()
	{
		if( PlayerManager.IsLoaded ) return; // PlayerManager handles player spawning if it's active.

		if( roomCamera == null ) Debug.LogWarning( "PlayerSpawner needs to know where the main camera is!" );

		if( !NavMesh.SamplePosition( transform.position, out var Hit, 5.0f, NavMesh.AllAreas ) )
			Debug.LogWarning( $"PlayerSpawner is too far from the navmesh!" );


		var player = Instantiate( _playerPrefab, Hit.position, transform.rotation );
		PlayerManager.CurrentPlayer = player;
	}

#if UNITY_EDITOR
	private void OnDrawGizmos()
	{
		using( new Handles.DrawingScope( Color.green, transform.localToWorldMatrix ) )
		{
			Handles.DrawWireCube( Vector3.up * _characterBounds.y * 0.5f, _characterBounds );

			Handles.DrawWireCube( Vector3.up * _characterBounds.y * 0.8f + Vector3.forward * _characterBounds.z * 0.8f,
								  new Vector3( _characterBounds.x * 0.3f,
											   _characterBounds.y * 0.3f,
											   _characterBounds.z * 0.8f ) );
		}
	}
#endif
}
