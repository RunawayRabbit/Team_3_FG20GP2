using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
	private static PlayerManager Instance { get; set; }

	[SerializeField] private GameObject _playerPrefab = default;

	[SerializeField] private Transform _playerSpawnpoint = default;

	public static GameObject CurrentPlayer { get; set; }

	public static event Action<GameObject> OnPlayerChanged;
	public static bool IsLoaded => Instance != null;

	private void Start()
	{
		Instance = this;
		SpawnPlayerAt( _playerSpawnpoint.position, _playerSpawnpoint.rotation );
	}

	private void OnEnable() { RoomManager.OnTransitionComplete += OnTransitionComplete; }

	private void OnDisable() { RoomManager.OnTransitionComplete -= OnTransitionComplete; }

	private void OnTransitionComplete( RoomManager.Room from, RoomManager.Room to )
	{
		Transform dest = RoomManager.GetExitPosition( to, from );
		TransitionFader.WipeFromBlack( dest.position, RoomManager.animationTime );
		SpawnPlayerAt( dest.position, dest.rotation * Quaternion.AngleAxis( 180.0f, Vector3.up ) );
	}

	private void SpawnPlayerAt( Vector3 destPosition, Quaternion destRotation)
	{
		if( CurrentPlayer ) Destroy( CurrentPlayer );

		CurrentPlayer = Instantiate( _playerPrefab, destPosition, destRotation );

		/*CurrentPlayer.GetComponent<NavMeshAgent>().Warp( destPosition );
		CurrentPlayer.transform.rotation = destRotation;*/

		OnPlayerChanged?.Invoke( CurrentPlayer );
	}

	public static void ChangePlayerState( PlayerMovement.PlayerState state )
	{
		PlayerMovement player;

		if( IsLoaded )
			player = CurrentPlayer.GetComponent<PlayerMovement>();
		else
			player = GameObject.FindWithTag( "Player" ).GetComponent<PlayerMovement>();

		player.ChangeState( state );
	}

}
