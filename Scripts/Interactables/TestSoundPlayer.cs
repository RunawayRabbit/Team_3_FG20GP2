
using UnityEngine;

public class TestSoundPlayer : Interactable
{
	[SerializeField] private SFXManager.ClipCategory sound;
	public override void Interact()
	{
		SFXManager.PlaySoundAt(sound, transform.position);
	}
}
