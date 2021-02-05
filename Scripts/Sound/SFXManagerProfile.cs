
using UnityEngine;


//[CreateAssetMenu( fileName = "SFXManagerProfile", menuName = "Data Definitions/SFX Manager Profile", order = 1 )]
public class SFXManagerProfile : ScriptableObject
{
	[SerializeField] public AudioData changeArea = default;

	[SerializeField] public AudioData bookInteract = default;
	[SerializeField] public AudioData plantHarvested = default;

	// Library
	[SerializeField] public AudioData crystalPickup = default;

	// Cauldron
	[SerializeField] public AudioData ingredientBurn = default;
	[SerializeField] public AudioData ingredientGood = default;

	// Observatory
	[SerializeField] public AudioData observatoryTick = default;
	[SerializeField] public AudioData observatoryTock = default;
	[SerializeField] public AudioData correctZodiac = default;
	[SerializeField] public AudioData zodiacMoved = default;
}
