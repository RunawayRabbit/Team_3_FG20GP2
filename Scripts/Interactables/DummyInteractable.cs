using UnityEngine;

public class DummyInteractable : Interactable
{
    public override void Interact()
    {
        Debug.Log("I'm being interacted with! I don't have to be alone anymore! Yay!");
    }
}
