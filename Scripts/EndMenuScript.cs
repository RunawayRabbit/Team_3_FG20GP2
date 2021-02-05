using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndMenuScript : MonoBehaviour
{
    public void GoToMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
