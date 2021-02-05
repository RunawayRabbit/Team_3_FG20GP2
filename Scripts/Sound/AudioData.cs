using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu( fileName = "NewAudioData", menuName = "Data Definitions/Audio Data", order = 1 )]
public class AudioData : ScriptableObject
{
	[SerializeField] private List<AudioClip> clips;

	// https://tomhazledine.com/what-is-a-decibel-anyway/
	[Range( -80.0f, 20.0f )] public float baseVolumeDB = 0.0f;
	[Range( 0.0f, 1.0f )] public float spacialBlend = 1.0f;
	[Range( -24.0f, 24.0f )] public float pitchShift = 0f;
	[Range( -20.0f, 0.0f )] public float randomVolume = 0f;
	[Range( 0.0f, 12.0f )] public float randomPitch = 0f;


	public float PlayOn( AudioSource source, float volume )
	{
		if( clips.Count == 0 )
		{
			Debug.LogWarning( "No AudioClips are set for this data!" );

			return 0;
		}

		AudioClip clip = GetRandomClip();

		source.clip = clip;
		float adjustedVolDB = baseVolumeDB * volume;

		float vol = AudioUtils.DecibelToLinear(
			Random.Range( adjustedVolDB - randomVolume, adjustedVolDB + randomVolume ) );
		source.volume = vol;

		source.pitch = AudioUtils.SemitoneToPitch( Random.Range( pitchShift - randomPitch, pitchShift + randomPitch ) );

		source.spatialBlend = spacialBlend;

		source.Play();

		return clip.length;
	}

	private AudioClip GetRandomClip()
	{
		int i = 0;

		if( clips.Count > 1 )
		{
			i = Random.Range( 0, clips.Count );
		}

		return clips[i];
	}
}
