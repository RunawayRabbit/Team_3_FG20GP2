using Freya;
using UnityEngine;

public class CauldronLightFlicker : MonoBehaviour
{
	[SerializeField] private float minIntensity = 1.5f;
	[SerializeField] private float maxIntensity = 2.5f;

	[SerializeField] private float posVariance = 0.5f;
	[SerializeField] private float posSpeed = 0.1f;
	[SerializeField] private float intensitySpeed = 0.1f;

	private Vector3 _rootPosition;

	private Vector3 _targetPos;
	private float _targetIntensity;

	private Light _light;

	private void Awake()
	{
		_rootPosition = transform.position;
		_light         = GetComponent<Light>();

		_targetPos = _rootPosition;
		_targetIntensity = _light.intensity;
	}

	private void Update()
	{
		if( Mathfs.Approximately( _targetIntensity, _light.intensity ) )
		{
			// We reached our intensity, pick a new one
			_targetIntensity = Freya.Random.Range( minIntensity, maxIntensity );
		}

		if( Mathfs.DistanceSquared( _targetPos, transform.position ) < 0.01f )
		{
			// We reached our position, pick a new one
			var tarX = Freya.Random.Range( _rootPosition.x - posVariance, _rootPosition.x + posVariance );
			var tarY = Freya.Random.Range( _rootPosition.y - posVariance, _rootPosition.y + posVariance );
			var tarZ = Freya.Random.Range( _rootPosition.z - posVariance, _rootPosition.z + posVariance );

			_targetPos = new Vector3( tarX, tarY, tarZ ) ;
		}

		_light.intensity = Mathfs.MoveTowards( _light.intensity, _targetIntensity, intensitySpeed * Time.deltaTime );

		_light.transform.position =
			Vector3.MoveTowards( _light.transform.position, _targetPos, posSpeed * Time.deltaTime );
	}
}
