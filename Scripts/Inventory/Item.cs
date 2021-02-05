using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "OLD Inventory/Item")]
public class Item : ScriptableObject, IDescribable
{
    public Sprite icon = null;
    public string title = "New Item";
    [TextArea] public string description;

    public virtual void Use()
    {
        // use item
        // do some function
    }

    public virtual string GetDescription()
    {
        return string.Format("<b>{0}</b>", title) + string.Format("\n\n{0}", description);
    }
}
