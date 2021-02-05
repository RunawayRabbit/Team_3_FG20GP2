using UnityEngine;

[CreateAssetMenu( fileName = "Book", menuName = "Inventory Items/Book", order = 1 )]
public class Book : InventoryItem
{
	public int bookNumber;

	public override void OnClick()
	{
		BookManager.OpenBook(bookNumber);
	}
}
