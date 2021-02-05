using System;
using UnityEngine;

public class BookManager : MonoBehaviour
{
    [SerializeField] private GameObject[] books;

    public static BookManager singleton;

    private void Awake()
    {
        singleton = this;
    }

    public static void OpenBook(int number)
    {
        if( singleton == null ) return;
        if (number < singleton.books.Length)
        {
            if (!singleton.books[number].activeSelf)
            {
                foreach (Transform child in singleton.transform)
                {
                    child.gameObject.SetActive(false);
                }
            }
            singleton.books[number].SetActive(!singleton.books[number].activeSelf);
        }
    }
}
