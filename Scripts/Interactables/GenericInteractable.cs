using UnityEngine;
using UnityEngine.Events;

public class GenericInteractable : Interactable
{
	[SerializeField] private UnityEvent theEvent;


	public override void Interact() { theEvent.Invoke(); }
}
