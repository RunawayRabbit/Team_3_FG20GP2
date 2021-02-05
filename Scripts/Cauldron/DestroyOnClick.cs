
using UnityEngine;
using UnityEngine.SceneManagement;

public class DestroyOnClick : MonoBehaviour
{
    [SerializeField] string sceneToLoad = "EndingPlaceholder";
    private void OnMouseDown()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
