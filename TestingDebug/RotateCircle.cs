using System;
using System.Collections;
using Freya;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = Freya.Random;


public class RotateCircle : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	private const float DegreesPerTick = 5.6f;
												// Moon, inner, middle, outer
	private static readonly int[] DefaultPositions = { 6, 4, 3, 6 };

	[SerializeField] private int segments = 8;
	public int CurrentSegment { get; private set; } = 0;
	[SerializeField] private Transform centerTransform;
	[SerializeField] private RotationStyle rotationStyle;
	[SerializeField] private float rotationSpeed = 1;
	[SerializeField] private Image image;

	[SerializeField] private Sprite activeImageAsset;
	[SerializeField] private Sprite inactiveImageAsset;

	[SerializeField] private int ringIndex;

	[SerializeField] private float alphaTest = 0.75f;

	private Vector2 _initialPosition = Vector2.zero;
	private Vector2 _center = default;
	private Quaternion _initialRotation = default;
	private Quaternion _desiredRotation = default;
	private float _timer = 0;
	private bool _playerInteracting = false;

	private float _previousTick;

	private bool _isActive = false;
	public bool isLocked = false;

	private readonly WaitForEndOfFrame _waitForEndOfFrame = new WaitForEndOfFrame();

	public bool isActive
	{
		get => _isActive;

		set
		{
			var handleImage = GetComponent<Image>();

			if( value )
			{
				//Set Active
				_isActive = true;
				if( image ) image.sprite = activeImageAsset;

				if( handleImage ) handleImage.raycastTarget = true;

				if( gameObject.activeInHierarchy
					&& CurrentSegment == -1 )
					StartCoroutine( _RotateIntoPlace( ringIndex ) );
			}
			else
			{
				//Set Inactive
				_isActive = false;
				if( image ) image.sprite = inactiveImageAsset;

				if( handleImage ) handleImage.raycastTarget = false;
			}
		}
	}

	private Func<Quaternion, Quaternion, float, Quaternion> rotationType;
	public event Action<int> OnSegmentChanged;

	public enum RotationStyle
	{
		LERP,
		SLERP,
		ROTATE_TOWARDS
	}

	private void Awake()
	{
		image ??= GetComponent<Image>();

		image.alphaHitTestMinimumThreshold = alphaTest; //Make transparent parts of Image not hit by raycast

		switch( rotationStyle )
		{
			case RotationStyle.LERP:
				rotationType = Quaternion.Lerp;

				break;

			case RotationStyle.SLERP:
				rotationType = Quaternion.Slerp;

				break;

			default:
				rotationType = Quaternion.RotateTowards;

				break;
		}

		CurrentSegment = GlobalState.ringPositions[ringIndex];
	}

	private void Start()
	{
		if( CurrentSegment == -1 )
		{
			SetRandomRotations();

			if( isActive ) StartCoroutine( _RotateIntoPlace( ringIndex ) );
		}
		else
			SetStartRotation();
	}


	private void SetRandomRotations()
	{
		_desiredRotation    = Quaternion.identity;
		transform.rotation = Quaternion.Euler( 0, 0, Random.Range( -120.0f, 120.0f ) );
	}

	private IEnumerator _RotateIntoPlace( int ringIndex )
	{
		int defaultPos = GetDefaultPosition( ringIndex );

		// Prevent re-spinning after we've seen the spin once.
		if( CurrentSegment == -1 ) GlobalState.ringPositions[ringIndex] = defaultPos;

		CurrentSegment = defaultPos;

		float segmentSize = 360f / segments;
		_desiredRotation = Quaternion.Euler( 0, 0, CurrentSegment * segmentSize );

		var fullRotationSpeed = rotationSpeed;
		rotationSpeed = 0;

		while( rotationSpeed < fullRotationSpeed )
		{
			rotationSpeed = Mathfs.MoveTowards( rotationSpeed, fullRotationSpeed, 0.1f * Time.deltaTime );

			yield return _waitForEndOfFrame;
		}

		OnSegmentChanged?.Invoke( CurrentSegment );

		rotationSpeed = fullRotationSpeed;
	}

	public static int GetDefaultPosition( int ringIndex )
	{
		Debug.Assert( ringIndex < DefaultPositions.Length );

		return DefaultPositions[ringIndex];
	}

	private void SetStartRotation()
	{
		float currentRotation = 360f / segments * CurrentSegment;

		_desiredRotation    = Quaternion.Euler( 0, 0, currentRotation );
		transform.rotation = _desiredRotation;
	}


	private void Update()
	{
		if( !_isActive ) return;

		if( !_playerInteracting
			&& transform.rotation != _desiredRotation )
		{
			if( rotationStyle == RotationStyle.ROTATE_TOWARDS ) { _timer = Time.deltaTime * rotationSpeed; }
			else
			{
				_timer += Time.deltaTime * rotationSpeed;
				_timer =  Mathf.Clamp( _timer, 0, 1 );
			}

			transform.rotation = rotationType( transform.rotation, _desiredRotation, _timer );
		}
		else { _timer = 0; }

		PlayClickingSounds();
	}

	private void PlayClickingSounds()
	{
		var currentRotation = transform.rotation.eulerAngles.z;

		if( !(Mathfs.Abs( currentRotation - _previousTick ) > DegreesPerTick) ) return;

		SFXManager.PlaySoundAt( SFXManager.ClipCategory.ObservatoryTick, transform.position );
		_previousTick = currentRotation;
	}

	public void OnBeginDrag( PointerEventData eventData )
	{
		if( !isActive || isLocked ) return;
		_playerInteracting = true;

		_initialRotation =
			transform.rotation; //initial rotation needed to make rotation relative to where the circle was
		_center          = centerTransform.position;
		_initialPosition = (eventData.position - _center).normalized; //relative from inner circle center
	}

	public void OnDrag( PointerEventData eventData )
	{
		if( !isActive || isLocked ) return;
		Vector2 currentPosition = (eventData.position - _center).normalized; //relative from inner circle center

		float previousAngle =
			Mathf.Rad2Deg * Mathf.Atan2( _initialPosition.y, _initialPosition.x ); //how much were we initally offset?
		float newAngle   = Mathf.Rad2Deg * Mathf.Atan2( currentPosition.y, currentPosition.x );
		float difference = previousAngle - newAngle;
		difference = difference < 0 ? difference + 360 : difference; //remap to 0-360

		transform.rotation =
			_initialRotation
			* Quaternion.Euler( 0, 0, -difference ); //negative sign needed for rotation in correct direction
	}

	public void OnEndDrag( PointerEventData eventData )
	{
		if( !isActive || isLocked ) return;
		float currentRot     = transform.rotation.eulerAngles.z;
		float segmentSize    = 360f / segments;
		float percentRotated = currentRot / 360f;
		int   segmentChosen  = Mathf.RoundToInt( segments * percentRotated );

		CurrentSegment =
			segmentChosen > segments - 1 ? segmentChosen - segments : segmentChosen; //safe against overflow
		_desiredRotation   = Quaternion.Euler( 0, 0, segmentChosen * segmentSize );
		_playerInteracting = false;

		OnSegmentChanged?.Invoke( CurrentSegment );

		GlobalState.ringPositions[ringIndex] = CurrentSegment;

		SFXManager.PlaySoundAt( SFXManager.ClipCategory.ObservatoryTock, transform.position );
	}
}
