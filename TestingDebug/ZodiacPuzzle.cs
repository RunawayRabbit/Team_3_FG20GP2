using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Freya;
using UnityEngine;
using UnityEngine.UI;

public class ZodiacPuzzle : MonoBehaviour
{
	[SerializeField] private Toggle powerButton = default;

	[SerializeField] private Rect moonPosition;
	[SerializeField] private float moonScale = 1.0f;
	[SerializeField] private Rect starPosition;
	[SerializeField] private float starScale = 1.0f;

	[SerializeField] private RotateCircle moonCircle;
	[SerializeField] private RotateCircle innerCircle;
	[SerializeField] private RotateCircle midCircle;
	[SerializeField] private RotateCircle outerCircle;

	[SerializeField] private CinemachineVirtualCamera moonCam;
	[SerializeField] private CinemachineVirtualCamera starCam;

	[SerializeField] private Image offStateSwitch;
	[SerializeField] private Image onStateSwitch;


	[SerializeField] private List<string> codes;
	[SerializeField] private List<int> codesToZodiac;

	[SerializeField] private List<Ingredient> plants;

	private int[] code = new int[3] { 1, 1, 1 };

	private WaitForEndOfFrame _waitForEndOfFrame = new WaitForEndOfFrame();

	public enum Mode
	{
		Off,
		Moon,
		Stars,
		Busy,
	}

	private Mode _currentMode = Mode.Off;
	private CinemachineBrain _cam;


	private void Awake()
	{
		_cam = Camera.main.GetComponent<CinemachineBrain>();

		powerButton.isOn = GlobalState.ObservatoryPowered;
	}

	public void TogglePower( bool isOn )
	{
		if( isOn && GlobalState.observatoryCrystalInserted )
		{
			ChangeMode( Mode.Stars );

			offStateSwitch.gameObject.SetActive(false);
			onStateSwitch.gameObject.SetActive(true);
			powerButton.isOn = true;
		}

		else
		{
			ChangeMode( Mode.Moon );

			offStateSwitch.gameObject.SetActive(true);
			onStateSwitch.gameObject.SetActive(false);
			powerButton.isOn = false;
		}
	}

	public void ChangeMode( Mode newMode, bool immediate = false )
	{
		if( _cam )
		{
			_cam.m_DefaultBlend = new CinemachineBlendDefinition( CinemachineBlendDefinition.Style.EaseInOut, 1.8f );
		}

		if( _currentMode == Mode.Busy ) return;
		if( _currentMode == newMode ) return;

		Rect  targetRect;
		float targetScale;

		switch( newMode )
		{
			case Mode.Off:

				moonCam.Priority = 0;
				starCam.Priority = 0;

				return;

			case Mode.Moon:
				targetRect       = moonPosition;
				targetScale      = moonScale;
				moonCam.Priority = 100;
				starCam.Priority = 0;

				GlobalState.ObservatoryPowered = false;

				moonCircle.isActive  = true;
				innerCircle.isActive = false;
				midCircle.isActive   = false;
				outerCircle.isActive = false;

				break;

			case Mode.Stars:
				targetRect       = starPosition;
				targetScale      = starScale;
				moonCam.Priority = 0;
				starCam.Priority = 100;

				GlobalState.ObservatoryPowered = true;

				moonCircle.isActive  = false;
				innerCircle.isActive = true;
				midCircle.isActive   = true;
				outerCircle.isActive = true;

				break;

			default:
				return;
		}

		if( immediate )
		{
			RectTransform rectTrans = transform as RectTransform;

			rectTrans.anchorMin = targetRect.min;
			rectTrans.anchorMax = targetRect.max;

			rectTrans.localScale = Vector3.one * targetScale;

			_currentMode = newMode;
		}
		else
		{
			_currentMode = Mode.Busy;
			StartCoroutine( SmoothlyRescale( newMode, targetRect, targetScale ) );
		}
	}

	private IEnumerator SmoothlyRescale( Mode newMode, Rect targetRect, float targetScale )
	{
		RectTransform rectTrans = transform as RectTransform;

		const float deltaT = 1.0f / 1.2f;

		float t = 0.0f;

		Vector2 startRectMin = rectTrans.anchorMin;
		Vector2 startRectMax = rectTrans.anchorMax;
		float   startScale   = rectTrans.localScale.x;

		while( t < 1.0f )
		{
			var left   = Mathfs.Lerp( startRectMin.x, targetRect.xMin, Mathfs.Smooth01( t ) );
			var right  = Mathfs.Lerp( startRectMax.x, targetRect.xMax, Mathfs.Smooth01( t ) );
			var top    = Mathfs.Lerp( startRectMin.y, targetRect.yMin, Mathfs.Smooth01( t ) );
			var bottom = Mathfs.Lerp( startRectMax.y, targetRect.yMax, Mathfs.Smooth01( t ) );

			var scale = Mathfs.Lerp( startScale, targetScale, Mathfs.Smooth01( t ) );

			rectTrans.anchorMax = new Vector2( left, top );
			rectTrans.anchorMin = new Vector2( right, bottom );

			rectTrans.localScale = Vector3.one * scale;

			t += Time.deltaTime * deltaT;

			yield return _waitForEndOfFrame;
		}

		rectTrans.anchorMin = new Vector2( targetRect.xMin, targetRect.yMin );
		rectTrans.anchorMax = new Vector2( targetRect.xMax, targetRect.yMax );

		rectTrans.localScale = Vector3.one * targetScale;

		_currentMode = newMode;
	}

	private void OnEnable()
	{
		innerCircle.OnSegmentChanged += InnerOnOnSegmentChanged;
		midCircle.OnSegmentChanged   += MidOnOnSegmentChanged;
		outerCircle.OnSegmentChanged += OuterOnOnSegmentChanged;

		GlobalState.onObservatoryPowerToggle += TogglePower;

		//@NOTE: VERY shitty way of handling ultrawide
		if( ((float) Screen.width / (float) Screen.height) >= 2.0f )
		{
			moonCam.m_Lens.FieldOfView = 7.63f;
			starCam.m_Lens.FieldOfView = 7.63f;
		}
		else
		{
			moonCam.m_Lens.FieldOfView = 10.0f;
			starCam.m_Lens.FieldOfView = 10.0f;
		}
	}

	private void OnDisable()
	{
		innerCircle.OnSegmentChanged -= InnerOnOnSegmentChanged;
		midCircle.OnSegmentChanged   -= MidOnOnSegmentChanged;
		outerCircle.OnSegmentChanged -= OuterOnOnSegmentChanged;

		GlobalState.onObservatoryPowerToggle -= TogglePower;
	}

	private void OuterOnOnSegmentChanged( int segment ) { RingSegmentChanged( 2, segment ); }

	private void MidOnOnSegmentChanged( int segment ) { RingSegmentChanged( 1, segment ); }

	private void InnerOnOnSegmentChanged( int segment ) { RingSegmentChanged( 0, segment ); }

	private void RingSegmentChanged( int i, int segment )
	{
		//@NOTE: The numbers on the graphic are backwards.
		code[i] = (8 - segment) % 8 + 1;
		string codeString = string.Join( "", code );

		Debug.Log( codeString );

		int index = codes.IndexOf( codeString );

		if( index >= 0 )
		{
			if( index < plants.Count )
			{
				var item = plants[index];

				if( !InventoryManager.Contains( item )
					&& !GlobalState.HasThisSpawned( item ) )
				{
					// We made a new thing, woot
					GlobalState.MakeThisSpawn( item );
					SFXManager.PlaySoundAt( SFXManager.ClipCategory.CorrectZodiac, transform.position );
				}
				else
				{
					// We repeated an old pattern, play a sound, but not as happy :(
					SFXManager.PlaySoundAt( SFXManager.ClipCategory.ZodiacMoved, transform.position );
				}
			}

			SkyboxZodiac.SetZodiac(codesToZodiac[index]);
		}
		else
		{
			// This is an invalid code. Get rid of the zodiac
			SkyboxZodiac.SetZodiac(-1);
		}

	}
}
