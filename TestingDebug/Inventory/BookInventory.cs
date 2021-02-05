
using UnityEngine;

public class BookInventory : MonoBehaviour
{
    [SerializeField] private int number;

    public void OpenBook()
    {
        BookManager.OpenBook(number);
    }
}
