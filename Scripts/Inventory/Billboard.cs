using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform camera;

    private void Awake()
    {
        camera = Camera.main.transform;
    }

    private void LateUpdate()
    {
        transform.LookAt(transform.position + camera.forward);
    }
}
