using UnityEngine;

[CreateAssetMenu( fileName = "Ingredient", menuName = "Inventory Items/Ingredients", order = 1 )]
public class Ingredient : InventoryItem
{
	public GameObject ingredientPrefab = null;

	public override void OnClick()
	{
		if( CauldronMixing.activeCauldron != null )
		{
			// We're at the cauldron, do the mixing
			CauldronMixing.activeCauldron.AddIngredient( this );
		}
		else
		{
			// Not at the cauldron. Display text maybe?
			Debug.Log( "No cauldron" );
		}
	}
}
