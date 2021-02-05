using Freya;
using UnityEngine;

public class HubCamController : CamController
{
	private const float trackingSpeed = 5.0f;
	private Vector3 positionOffset;
	private Vector3 focalPointOffset;

	public HubCamController( GameObject playerObject )
		: base( playerObject )
	{
		Debug.Log( playerObject );
		focalPoint       = playerObject.transform.position + focalPointOffset;
		positionOffset   = Vector3.back * 20.0f + Vector3.up * 3.0f;
		focalPointOffset = Vector3.up * 1.5f;
	}
	public override void Update( GameObject camera )
	{
		Vector3 playerPos          = player.transform.position + focalPointOffset;
		Vector3 focalPointToTarget = playerPos - focalPoint;
		float   focalPointDelta    = Vector3.Dot( playerPos, focalPointToTarget );

		float t = Mathfs.Smooth01(Mathfs.Clamp01( focalPointDelta )) * trackingSpeed * Time.deltaTime;

		focalPoint = Vector3.MoveTowards( focalPoint, playerPos, t );

		var finalPos = focalPoint + positionOffset;
		camera.transform.position = finalPos;
		camera.transform.rotation = Quaternion.LookRotation( playerPos - finalPos, Vector3.up );
	}


}
