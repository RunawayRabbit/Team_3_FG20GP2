using UnityEngine;

public class FogSpin : MonoBehaviour
{
	[SerializeField] private float spinnyRate;

	private Vector3 dir;
	private Transform mainCam;

	private void Start()
	{
		mainCam = Camera.main.transform;
		dir     = Vector3.up;
	}

	private void Update()
	{
		var        lookVector = mainCam.transform.position - transform.position;
		Quaternion spin       = Quaternion.AngleAxis( spinnyRate * Time.deltaTime, lookVector );

		dir = spin * dir;

		transform.rotation = Quaternion.LookRotation( dir, lookVector );
	}
}
