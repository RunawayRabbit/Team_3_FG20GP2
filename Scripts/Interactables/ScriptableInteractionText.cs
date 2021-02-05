using UnityEngine;

[CreateAssetMenu(menuName = "Data Definitions/Interaction Text", fileName = "New Interaction Text")]
public class ScriptableInteractionText : ScriptableObject
{
    public string interactionText;
    public float secondsOnScreen;
}
