
using UnityEngine;

public class InteractableProp : Interactable
{
    [SerializeField] private float radius = 3f;
    [SerializeField] private Vector3 textOffset;
    [SerializeField] private ScriptableInteractionText text;

    public override void Interact()
    {
        if (text != null)
        {
            TextParent.SpawnText(text, transform.position+textOffset);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + textOffset, 0.2f);
    }
}
