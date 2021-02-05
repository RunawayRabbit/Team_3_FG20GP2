using System.Collections;
using Freya;
using UnityEngine;

public class SkyboxZodiac : MonoBehaviour
{
	private static SkyboxZodiac _instance;

	[SerializeField] private Light moonDir;

	[SerializeField] private float minStarAlpha = 0.15f;
	[SerializeField] private float maxStarAlpha = 0.8f;
	[SerializeField] private float minOutlineAlpha = 0.0f;
	[SerializeField] private float maxOutlineAlpha = 0.6f;

	[SerializeField] private float rotateTime = 1.2f;
	[SerializeField] private float outlineFadeTime = 0.8f;


	[SerializeField] private readonly Vector3 focusSpot = new Vector3( 0.2579362f, -0.3051639f, 0.9167027f );

	[SerializeField] private Vector3[] restingPositions = new Vector3[8];

	private readonly Coroutine[] _coroutines = new Coroutine[8];
	private readonly Vector3[] _positions = new Vector3[8];

	private Vector3 _moonDir;
	private Vector3 _startPos;
	private Vector3 _endPos;

	private int _currentlyShownZodiac = -1;

	private Material _skybox;

	private static readonly int[] Zodiacs = new[]
	{
		Shader.PropertyToID( "_Zodiac0" ),
		Shader.PropertyToID( "_Zodiac1" ),
		Shader.PropertyToID( "_Zodiac2" ),
		Shader.PropertyToID( "_Zodiac3" ),
		Shader.PropertyToID( "_Zodiac4" ),
		Shader.PropertyToID( "_Zodiac5" ),
		Shader.PropertyToID( "_Zodiac6" ),
		Shader.PropertyToID( "_Zodiac7" ),
		Shader.PropertyToID( "_Zodiac8" ),
		Shader.PropertyToID( "_Zodiac9" ),
		Shader.PropertyToID( "_Zodiac10" ),
		Shader.PropertyToID( "_Zodiac11" ),
		Shader.PropertyToID( "_Zodiac12" ),
		Shader.PropertyToID( "_Zodiac13" ),
		Shader.PropertyToID( "_Zodiac14" ),
		Shader.PropertyToID( "_Zodiac15" ),
	};

	private static readonly int moonPhase = Shader.PropertyToID( "_MoonPhase" );

	private void Awake()
	{
		_instance = this;
		_skybox   = RenderSettings.skybox;
		_moonDir  = moonDir.transform.rotation * Vector3.forward;

		_startPos = RotateAroundMoon( focusSpot, -50.0f );
		_endPos   = RotateAroundMoon( focusSpot, 50.0f );

		/*// Normalize the resting positions
		for( int i = 0; i < restingPositions.Length; i++ )
		{
			restingPositions[i] = restingPositions[i].normalized;
		}*/

		_skybox.SetFloat(moonPhase, 0.3f);

		ResetZodiac();
	}

	public static void SetZodiac( int newZodiac )
	{
		if( _instance._currentlyShownZodiac >= 0 ) { _instance.StartRotatingOut( _instance._currentlyShownZodiac ); }

		if( newZodiac >= 0 )
		{
			_instance.StartRotatingIn( newZodiac );
			_instance._currentlyShownZodiac = newZodiac;
		}
		else { _instance._currentlyShownZodiac = -1; }
	}

#if false
	private void Update()
	{
		if( Input.GetKeyDown( KeyCode.A ) ) { SetZodiac( -1 ); }

		if( Input.GetKeyDown( KeyCode.S ) ) { SetZodiac( 0 ); }


		if( Input.GetKeyDown( KeyCode.D ) ) { SetZodiac( 1 ); }


		if( Input.GetKeyDown( KeyCode.F ) ) { SetZodiac( 2 ); }


		if( Input.GetKeyDown( KeyCode.G ) ) { SetZodiac( 3 ); }


		if( Input.GetKeyDown( KeyCode.H ) ) { SetZodiac( 4 ); }


		if( Input.GetKeyDown( KeyCode.J ) ) { SetZodiac( 5 ); }


		if( Input.GetKeyDown( KeyCode.K ) ) { SetZodiac( 6 ); }


		if( Input.GetKeyDown( KeyCode.L ) ) { SetZodiac( 7 ); }
	}
#endif
	private void ResetZodiac()
	{
		for( int i = 0; i < 8; i++ ) { SetZodiac( i, restingPositions[i], 0.15f, 0.0f ); }
	}

	private void OnDisable() { ResetZodiac(); }

	private Vector3 RotateAroundMoon( Vector3 pos, float degrees )
	{
		return Quaternion.AngleAxis( degrees, _moonDir ) * pos;
	}

	private void StartRotatingIn( int index )
	{
		Debug.Assert( index < 8 );

		if( _coroutines[index] != null )
		{
			// We're running something. Big oof.
			StopCoroutine( _coroutines[index] );
		}
		else
		{
			SetZodiac( index, _startPos, minStarAlpha, minOutlineAlpha );
			_positions[index] = _startPos;
		}

		_coroutines[index] = StartCoroutine( RotateIn( index ) );
	}

	private void StartRotatingOut( int index )
	{
		Debug.Assert( index < 8 );

		if( _coroutines[index] != null )
		{
			// We're running something. Big oof.
			StopCoroutine( _coroutines[index] );
		}
		else
		{
			SetZodiac( index, focusSpot, maxStarAlpha, maxOutlineAlpha );
			_positions[index] = focusSpot;
		}


		_coroutines[index] = StartCoroutine( RotateOut( index ) );
	}


	private IEnumerator RotateIn( int index )
	{
		// Rotate into place
		float rotation = -50.0f;
		float rotVel   = 0.0f;
		float starAlpha;
		float maxStarAlphaFirstPass = (minStarAlpha + maxStarAlpha) * 0.5f;

		do
		{
			_positions[index] = RotateAroundMoon( focusSpot, rotation );
			rotation          = Mathfs.SmoothDamp( rotation, 1.0f, ref rotVel, rotateTime );

			float alphaT = Mathfs.InverseLerpClamped( -45.0f, -5.0f, rotation );
			starAlpha = Mathfs.LerpSmooth( minStarAlpha, maxStarAlphaFirstPass, alphaT );

			SetZodiac( index, _positions[index], starAlpha, 0 );

			yield return new WaitForEndOfFrame();
		} while( rotation < 0.0f );


		SetZodiac( index, focusSpot, maxStarAlphaFirstPass, minOutlineAlpha );

		// Fade Alpha In
		float outlineAlpha    = minOutlineAlpha;
		float outlineAlphaVel = 0.0f;
		float starAlphaVel    = 0.0f;

		do
		{
			outlineAlpha = Mathfs.SmoothDamp( outlineAlpha,
											  maxOutlineAlpha * 1.01f,
											  ref outlineAlphaVel,
											  outlineFadeTime );
			starAlpha = Mathfs.SmoothDamp( starAlpha, maxStarAlpha * 1.01f, ref starAlphaVel, outlineFadeTime );
			SetZodiac( index, _positions[index], starAlpha, outlineAlpha );

			yield return new WaitForEndOfFrame();
		} while( outlineAlpha < maxOutlineAlpha );

		SetZodiac( index, _positions[index], maxStarAlpha, maxOutlineAlpha );
	}

	private IEnumerator RotateOut( int index )
	{
		// Rotate away and fade out
		float outlineAlpha    = maxOutlineAlpha;
		float starAlpha       = maxStarAlpha;
		float outlineAlphaVel = 0.0f;
		float starAlphaVel    = 0.0f;

		float rotation = 0.0f;
		float rotVel   = 0.0f;

		do
		{
			_positions[index] = RotateAroundMoon( focusSpot, rotation );
			rotation          = Mathfs.SmoothDamp( rotation, 91.0f, ref rotVel, rotateTime );

			starAlpha = Mathfs.SmoothDamp( starAlpha, minStarAlpha * 1.01f, ref starAlphaVel, outlineFadeTime );

			outlineAlpha = Mathfs.SmoothDamp( outlineAlpha,
											  minOutlineAlpha * 1.01f,
											  ref outlineAlphaVel,
											  outlineFadeTime );

			SetZodiac( index, _positions[index], starAlpha, outlineAlpha );

			yield return new WaitForEndOfFrame();
		} while( rotation < 50.0f );

		SetZodiac( index, restingPositions[index], minStarAlpha, minOutlineAlpha );
	}

	private void SetZodiac( int     number,
							Vector3 position,
							float   starAlpha,
							float   outlineAlpha )
	{
		int starIndex    = number * 2;
		int outlineIndex = starIndex + 1;

		var starPayload    = new Vector4( position.x, position.y, position.z, starAlpha );
		var outlinePayload = new Vector4( position.x, position.y, position.z, outlineAlpha );

		_skybox.SetVector( Zodiacs[starIndex], starPayload );
		_skybox.SetVector( Zodiacs[outlineIndex], outlinePayload );
	}
}
