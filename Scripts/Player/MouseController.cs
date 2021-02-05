using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class MouseController : MonoBehaviour
{
	[SerializeField] public LayerMask clickables;
	[SerializeField] private float maxRayDistance = 100.0f;
	[SerializeField] private GameObject clickVFX = default;
	[SerializeField] private float clickVFXDuration = 1.5f;

	private readonly List<GameObject> _clickVFXPool = new List<GameObject>();
	public static event Action<Vector3> OnMoveCommand;
	public static event Action<Interactable> OnInteractable;

	public Camera cam;

	private void Start() { cam ??= CameraDirector.Camera; }

	private void Update()
	{
		if( Input.GetMouseButtonDown( 0 ) )
		{
			// Stops us from walking when we click on the inventory icon
			if( EventSystem.current
				&& EventSystem.current.currentSelectedGameObject != null ) { return; }

			var ray = cam.ScreenPointToRay( Input.mousePosition );

			Debug.DrawLine( transform.position, ray.direction * 100.0f, Color.black, 10.0f );

			if( Time.timeScale == 0.0f ) return;

			if( Physics.Raycast( ray, out RaycastHit hit, maxRayDistance, clickables ) )
			{
				if( NavMesh.SamplePosition( hit.point, out var navHit, 2.0f, NavMesh.AllAreas ) )
				{
					SpawnVFX( navHit.position );
				}

				Debug.DrawLine( transform.position, hit.point, Color.red, 10.0f );
				int hitLayer = hit.transform.gameObject.layer;

				if( hitLayer == LayerMask.NameToLayer( "Interactable" ) )
				{
					var interactable = hit.transform.gameObject.GetComponent<Interactable>();
					OnInteractable?.Invoke( interactable );
				}

				else if( hitLayer == LayerMask.NameToLayer( "Walkable" ) )
				{
					var tunnel = hit.transform.gameObject.GetComponent<Tunnel>();

					if( tunnel != null )
						OnMoveCommand?.Invoke( tunnel.GetTunnelPoint() );
					else
						OnMoveCommand?.Invoke( hit.point );
				}
			}
		}
	}

	private void SpawnVFX( Vector3 hitInfoPoint )
	{
		if( clickVFX == null ) return;

		GameObject GO;

		if( _clickVFXPool.Count > 0 )
		{
			int lastElement = _clickVFXPool.Count - 1;
			GO = _clickVFXPool[lastElement];
			_clickVFXPool.RemoveAt( lastElement );
			GO.SetActive( true );
			GO.transform.position = hitInfoPoint;
		}
		else
			GO = Instantiate( clickVFX, hitInfoPoint, Quaternion.identity, gameObject.transform );


		StartCoroutine( DisableAfter( GO, clickVFXDuration ) );
	}

	private IEnumerator DisableAfter( GameObject GO, float timer )
	{
		yield return new WaitForSeconds( timer );
		GO.SetActive( false );
		_clickVFXPool.Add( GO );
	}
}
