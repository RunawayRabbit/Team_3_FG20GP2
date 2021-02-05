using UnityEngine;

public class CrystalFollower : MonoBehaviour
{
	[SerializeField] private float heightOffset = 1.8f;
	[SerializeField] private float smoothTime = 0.6f;

	private Transform _player;
	private Vector3 _vel;

	private void Start()
	{
		if( PlayerManager.CurrentPlayer )
			_player = PlayerManager.CurrentPlayer.transform;
		else
			_player = GameObject.FindWithTag( "Player" ).transform;
	}

	private void FixedUpdate()
	{
		if( _player )
		{
			var targetPos = new Vector3( transform.position.x,
										 _player.position.y + heightOffset,
										 transform.position.z );

			transform.position = Vector3.SmoothDamp( transform.position, targetPos, ref _vel, smoothTime );
		}
		else if( PlayerManager.CurrentPlayer ) _player = PlayerManager.CurrentPlayer.transform;
	}
}
