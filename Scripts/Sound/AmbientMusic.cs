using System;
using System.Collections;
using Freya;
using UnityEngine;

public class AmbientMusic : MonoBehaviour
{
	private AudioSource _sourceA;
	private AudioSource _sourceB;

	[SerializeField] private AudioClip mainHub;
	[SerializeField] private AudioClip library;
	[SerializeField] private AudioClip observatory;

	[SerializeField] private float fadeTime = 2.0f;

	[SerializeField, Range( -20.0f, 0.0f )]
	private float maxVolumeDb = 0.0f;

	private void Awake()
	{
		AudioSource[] sources = GetComponents<AudioSource>();

		Debug.Assert( sources.Length > 1 );

		_sourceA = sources[0];
		_sourceB = sources[1];
	}

	private void Start()
	{
		OnTransitionComplete(RoomManager.Room.MainHub, RoomManager.Room.MainHub);
	}

	private void OnEnable() => RoomManager.OnTransitionComplete += OnTransitionComplete;

	private void OnDisable() => RoomManager.OnTransitionComplete -= OnTransitionComplete;

	private void OnTransitionComplete( RoomManager.Room from, RoomManager.Room to )
	{
		AudioSource current;
		AudioSource next;

		if( _sourceA.isPlaying )
		{
			current = _sourceA;
			next    = _sourceB;
		}
		else
		{
			current = _sourceB;
			next    = _sourceA;
		}


		switch( to )
		{
			case RoomManager.Room.MainHub:
				next.clip = mainHub;

				break;

			case RoomManager.Room.Library:
				next.clip = library;

				break;

			case RoomManager.Room.Observatory:
				next.clip = observatory;

				break;

			default:
				throw new ArgumentOutOfRangeException( nameof(to), to, null );
		}


		next.volume = 0.0f;
		next.time   = current.time;
		next.Play();

		StartCoroutine( CrossfadeAmbient( current, next, fadeTime ) );
	}

	private IEnumerator CrossfadeAmbient( AudioSource current, AudioSource next, float fadeTime )
	{
		float startTime = Time.time;

		while( true )
		{
			float elapsed = Time.time - startTime;

			float volume = Mathf.Lerp( -20.0f, maxVolumeDb, elapsed / fadeTime );

			float linearVolume = AudioUtils.DecibelToLinear( volume );
			current.volume = 1.0f - linearVolume;
			next.volume    = linearVolume;

			if( Mathfs.Approximately( volume, maxVolumeDb ) )
			{
				current.Stop();
				next.volume = AudioUtils.DecibelToLinear( maxVolumeDb );

				break;
			}

			yield return null;
		}
	}
}
