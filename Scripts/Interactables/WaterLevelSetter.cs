using UnityEngine;

public class WaterLevelSetter : Interactable
{
	[SerializeField] private int level;

	public override void Interact() => GlobalState.WaterLevel = level;

}
