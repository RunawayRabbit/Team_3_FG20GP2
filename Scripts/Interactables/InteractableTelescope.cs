using System.Collections;
using Freya;
using UnityEngine;

public class InteractableTelescope : Interactable
{
	[SerializeField] private ObservatoryPuzzle puzzle;
	[SerializeField] private AudioData _audioData;
	[SerializeField] private float startVolume = -40.0f;
	[SerializeField] private float startPitch = -10.0f;
	[SerializeField] private float fadeTime = 3.0f;

	private Coroutine _currentRoutine;

	private AudioSource _source;
	private void Awake()
	{
		_source = GetComponent<AudioSource>();

		_source.spatialBlend = _audioData.spacialBlend;
		if( GlobalState.ObservatoryPowered )
		{
			_source.volume = AudioUtils.DecibelToLinear(_audioData.baseVolumeDB);
			_source.pitch  = AudioUtils.SemitoneToPitch(_audioData.pitchShift);
			_source.Play();
		}
	}

	private IEnumerator AudioFadeIn( )
	{
		float startTime = Time.time;
		_source.volume = AudioUtils.DecibelToLinear( startVolume );
		_source.pitch = startPitch;

		_source.Play();

		while( true )
		{
			float t      = (Time.time - startTime) / fadeTime;
			float volume = Mathf.Lerp( startVolume, _audioData.baseVolumeDB, t );
			float pitch  = Mathf.Lerp( startPitch, _audioData.pitchShift,t );

			float linearVolume = AudioUtils.DecibelToLinear( volume );
			float linearPitch  = AudioUtils.SemitoneToPitch( pitch );
			_source.volume = linearVolume;
			_source.pitch  = linearPitch;

			if( Mathfs.Approximately( volume, _audioData.baseVolumeDB ) )
			{
				_source.volume = AudioUtils.DecibelToLinear(_audioData.baseVolumeDB);
				_source.pitch  = AudioUtils.SemitoneToPitch(_audioData.pitchShift);

				break;
			}

			yield return null;
		}
	}

	private IEnumerator AudioFadeOut( )
	{
		float startTime = Time.time;
		_source.volume = AudioUtils.DecibelToLinear( startVolume );
		_source.pitch  = startPitch;

		while( true )
		{
			float t      = (Time.time - startTime) / fadeTime;
			float volume = Mathf.Lerp( _audioData.baseVolumeDB, startVolume, t );
			float pitch  = Mathf.Lerp( _audioData.pitchShift,startPitch, t );

			float linearVolume = AudioUtils.DecibelToLinear( volume );
			float linearPitch =  AudioUtils.SemitoneToPitch( pitch );

			_source.volume = linearVolume;
			_source.pitch  = linearPitch;

			if( Mathfs.Approximately( volume, startVolume ) )
			{
				_source.volume = AudioUtils.DecibelToLinear(startVolume);
				_source.pitch  = AudioUtils.SemitoneToPitch(startPitch);

				_source.Stop();
				break;
			}

			yield return null;
		}
	}

	private void OnEnable() => GlobalState.onObservatoryPowerToggle += ToggleSound;
	private void OnDisable() => GlobalState.onObservatoryPowerToggle -= ToggleSound;

	private void ToggleSound( bool isOn )
	{
		if(_currentRoutine != null) StopCoroutine(_currentRoutine);

		if( isOn )
		{
			_currentRoutine = StartCoroutine( AudioFadeIn() );
		}
		else
		{
			_currentRoutine = StartCoroutine( AudioFadeOut() );
		}
	}


	public override void Interact()
	{
		puzzle.StartPuzzle();
	}
}
