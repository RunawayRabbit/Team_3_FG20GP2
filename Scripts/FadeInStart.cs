using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInStart : MonoBehaviour
{
    static bool played = false;
    static int i = 0;
    [SerializeField] GameObject image;
    private void Awake()
    {
        if (played) Destroy(gameObject);
        else
        {
            i++;
            if (i == 2)
                played = true;

            image.SetActive(true);
        }
    }
}
