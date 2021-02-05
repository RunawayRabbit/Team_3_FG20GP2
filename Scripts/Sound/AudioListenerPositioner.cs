
using UnityEngine;

[RequireComponent( typeof(AudioListener))]
public class AudioListenerPositioner : MonoBehaviour
{
	[SerializeField] private float distanceFromPlayer = 1.0f;
	private Transform _mainCam;
	private Transform _player;

	private void Awake()
	{
		_mainCam = Camera.main.transform;
		_player  = transform.parent;

		//NOTE: This is janky but we only do it on scene transitions sooo...?
		foreach( var listener in FindObjectsOfType<AudioListener>(false) )
			listener.enabled = false;

		GetComponent<AudioListener>().enabled = true;
	}

	// @TODO: Should this be done in update instead..?
	private void LateUpdate()
	{
		var playerPos = _player.position;
		transform.position = playerPos + (_mainCam.position - playerPos).normalized * distanceFromPlayer;
	}
}
