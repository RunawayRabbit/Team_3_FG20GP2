using Cinemachine;
using UnityEngine;

public class RoomCamera : MonoBehaviour
{
	private void Awake()
	{
		Camera thisCam = this.GetComponent<Camera>();

		if( CameraDirector.IsLoaded )
		{
			thisCam.enabled = false;

			this.GetComponent<AudioListener>().enabled    = false;
			this.GetComponent<CinemachineBrain>().enabled = false;
			CameraDirector.CutTo( transform );
			gameObject.SetActive( false );
			this.tag = "Untagged";
		}
		else
		{
			var controller = gameObject.AddComponent<MouseController>();
			controller.clickables = LayerMask.GetMask( "Walkable", "Interactable" );
			controller.cam        = thisCam;
			this.tag              = "MainCamera";
		}
	}
}
