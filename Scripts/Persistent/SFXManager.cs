using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class SFXManager : MonoBehaviour
{
	private static SFXManager _instance = null;

	public enum ClipCategory
	{
		ChangeArea,

		// Main Hub
		BookInteract,
		PlantHarvested,

		// Library
		CrystalPickup,

		// Cauldron
		IngredientBurn,
		IngredientGood,

		// Observatory
		ObservatoryTock,
		ObservatoryTick,
		CorrectZodiac,
		ZodiacMoved,
	}

	[SerializeField] private SFXManagerProfile _profile;
	private readonly List<AudioSource> _sourcePool = new List<AudioSource>();

	private void Awake()
	{
		if( _instance == null ) _instance = this;

		if( !_profile )
		{
#if UNITY_EDITOR
			_profile = AssetDatabase.LoadAssetAtPath<SFXManagerProfile>(
				"Assets/Audio/AudioDefinitions/SFXManagerProfile.asset" );
#else
			Debug.LogWarning( "SFXManagerProfile not set in SFXManager. Is the PersistentScene loaded?" );
#endif
		}
	}

	public static AudioSource PlaySoundAt( ClipCategory category,
										   Vector3      position,
										   float        volume = 1.0f,
										   float        delay  = 0.0f )
	{
		if( !_instance )
		{
			CreateInstance();

			return null;
		}

		// Select the correct data source.
		AudioData audioData = _instance.CategoryToAudioData( category );

		return PlaySoundAt( audioData, position, volume, delay );
	}

	private static void CreateInstance()
	{
		Debug.LogWarning( "Persistent Scene isn't loaded! Creating a temporary SFXManager.." );

		new GameObject( "TEMP_SFXManager" ).AddComponent<SFXManager>();
	}

	public static AudioSource PlaySoundAt( AudioData audioData,
										   Vector3   position,
										   float     volume = 1.0f,
										   float     delay  = 0.0f )
	{
		AudioSource audioSource;

		lock(_instance)
		{
			if( _instance._sourcePool.Count > 0 )
			{
				int lastElement = _instance._sourcePool.Count - 1;
				audioSource = _instance._sourcePool[lastElement];
				_instance._sourcePool.Remove( audioSource );

			}
			else
				audioSource = GenerateNewAudioSource();
		}

		var trans = audioSource.transform;
		trans.position = position;

		if( delay == 0.0f )
		{
			_instance.StartCoroutine( PlaySoundAndReturnToPool( audioSource, audioData, volume ) );
		}
		else
		{
			_instance.StartCoroutine( PlaySoundDelayed( audioSource, audioData, volume, delay ) );
		}

		return audioSource;
	}

	private static IEnumerator PlaySoundAndReturnToPool( AudioSource audioSource, AudioData audioData, float volume )
	{
		float duration = audioData.PlayOn( audioSource, volume );

		yield return new WaitForSeconds( duration );

		_instance._sourcePool.Add( audioSource );
	}

	private static IEnumerator PlaySoundDelayed( AudioSource audioSource,
												 AudioData   AudioData,
												 float       volume,
												 float       delay )
	{
		yield return new WaitForSeconds( delay );
		yield return PlaySoundAndReturnToPool( audioSource, AudioData, volume );
	}

	private static AudioSource GenerateNewAudioSource()
	{
		var GO = new GameObject( $"SFX {Random.Range( 1, 99 )}" );
		//var GO = new GameObject( "SFX" );
		GO.transform.parent = _instance.transform;
		var source = GO.AddComponent<AudioSource>();

		return source;
	}

	private AudioData CategoryToAudioData( ClipCategory category )
	{
		AudioData data =  category switch
		{
			ClipCategory.ChangeArea     => _profile.changeArea,
			ClipCategory.BookInteract   => _profile.bookInteract,
			ClipCategory.PlantHarvested => _profile.plantHarvested,

			// Library
			ClipCategory.CrystalPickup => _profile.crystalPickup,

			// Cauldron
			ClipCategory.IngredientBurn => _profile.ingredientBurn,
			ClipCategory.IngredientGood => _profile.ingredientGood,

			// Observatory
			ClipCategory.ObservatoryTock => _profile.observatoryTock,
			ClipCategory.ObservatoryTick => _profile.observatoryTick,
			ClipCategory.CorrectZodiac   => _profile.correctZodiac,
			ClipCategory.ZodiacMoved     => _profile.zodiacMoved,

			_ => null,
		};

		if(data == null)
			Debug.LogWarning( $"No AudioData set up for {category}!" );

		return data;
	}
}
