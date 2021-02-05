using UnityEngine;

public abstract class CamController
{
    protected GameObject player;
    protected Vector3 focalPoint;

    public CamController(GameObject playerObject)
    {
        player = playerObject;
    }

    public abstract void Update( GameObject camera );

}
