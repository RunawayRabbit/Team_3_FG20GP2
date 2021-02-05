
using UnityEngine;

public class CameraDirector : MonoBehaviour
{
	private static CameraDirector Instance { get; set; }
	public static bool IsLoaded => Instance != null;

	public static Camera Camera => Instance.mainCam;


	[SerializeField] private Camera mainCam;

	private void Awake()
	{
		Instance = this;
	}

	public static void CutTo( Transform newCam )
	{
		if( Instance == null ) return;
		Instance.mainCam.transform.position = newCam.position;
		Instance.mainCam.transform.rotation = newCam.rotation;
	}
}
