using System.Collections;
using Freya;
using UnityEngine;
using UnityEngine.UI;

public class TransitionFader : MonoBehaviour
{
	private static TransitionFader Instance;

	private static Camera _cam;

	private Material _tempMaterial;
	private static readonly int CenterX = Shader.PropertyToID( "_CenterX" );
	private static readonly int CenterY = Shader.PropertyToID( "_CenterY" );
	private static readonly int Radius = Shader.PropertyToID( "_Radius" );
	private static readonly int Alpha = Shader.PropertyToID( "_Alpha" );


	private static readonly WaitForEndOfFrame WaitForEndOfFrame = new WaitForEndOfFrame();

	private void Awake()
	{
		Instance      = this;
		_tempMaterial = Instantiate( gameObject.GetComponent<RawImage>().material );
		_cam          = Camera.main;

		gameObject.GetComponent<RawImage>().material = _tempMaterial;
	}

	private void OnEnable() => SetShader( 0.0f, 0.0f, 0.0f, 0.0f );

	private void OnDestroy() => SetShader( 0.0f, 0.0f, 0.0f, 0.0f );

	private void SetShader( float centerX,
							float centerY,
							float radius,
							float alpha )
	{
		_tempMaterial.SetFloat( CenterX, centerX );
		_tempMaterial.SetFloat( CenterY, centerY );
		_tempMaterial.SetFloat( Radius, radius );
		_tempMaterial.SetFloat( Alpha, alpha );
	}

	private static Vector2 FindFurthestCorner( Vector2 coords )
	{
		// Get distance to furthest corner (in pixels)
		float midX = Screen.width * 0.5f;
		float midY = Screen.height * 0.5f;

		Vector2 furthestCorner;

		furthestCorner.x = coords.x < midX ? Screen.width : 0;
		furthestCorner.y = coords.y < midY ? Screen.height : 0;

		return furthestCorner;
	}


#region WipeToBlack

	public static void WipeToBlack( Vector3 position, float time )
	{
		if( Instance == null ) return;

		Vector2 coords         = _cam.WorldToScreenPoint( position );
		Vector2 furthestCorner = FindFurthestCorner( coords );
		float   radius         = (furthestCorner - coords).magnitude;

		// Set up the shader
		Instance.SetShader( coords.x, coords.y, radius, 1.0f );

		Instance.StartCoroutine( _WipeToBlack( position, radius, time ) );
	}

	private static IEnumerator _WipeToBlack( Vector3 position, float startingRadius, float time )
	{
		float elapsed = 0.0f;

		while( elapsed < time )
		{
			Vector2 coords = _cam.WorldToScreenPoint( position );
			Instance._tempMaterial.SetFloat( CenterX, coords.x );
			Instance._tempMaterial.SetFloat( CenterY, coords.y );

			float progress = elapsed / time;
			float t        = 1 - Mathfs.Cos( progress * Mathfs.TAU / 4.0f );

			float radius = Mathfs.Lerp( startingRadius, 0, t );
			Instance._tempMaterial.SetFloat( Radius, radius );

			yield return WaitForEndOfFrame;
			elapsed += Time.deltaTime;
		}

		Instance._tempMaterial.SetFloat( Alpha, 1.0f );
		Instance._tempMaterial.SetFloat( Radius, -1.0f );
	}

#endregion WipeToBlack


#region WipeFromBlack

	public static void WipeFromBlack( Vector3 position, float time )
	{
		if( Instance == null ) return;

		Vector2 coords         = _cam.WorldToScreenPoint( position );
		Vector2 furthestCorner = FindFurthestCorner( coords );
		float   radius         = (furthestCorner - coords).magnitude;

		Instance.SetShader( coords.x, coords.y, 0.0f, 1.0f );

		Instance.StartCoroutine( _WipeFromBlack( position, radius, time ) );
	}

	private static IEnumerator _WipeFromBlack( Vector3 position, float finalRadius, float time )
	{
		float elapsed = 0.0f;

		while( elapsed < time )
		{
			Vector2 coords = _cam.WorldToScreenPoint( position );
			Instance._tempMaterial.SetFloat( CenterX, coords.x );
			Instance._tempMaterial.SetFloat( CenterY, coords.y );

			float progress = elapsed / time;
			float t        = Mathfs.Sin( progress * Mathfs.TAU / 4.0f );

			float radius = Mathfs.Lerp( 0, finalRadius, t );
			Instance._tempMaterial.SetFloat( Radius, radius );

			yield return WaitForEndOfFrame;
			elapsed += Time.deltaTime;
		}

		Instance._tempMaterial.SetFloat( Alpha, 0.0f );
		Instance._tempMaterial.SetFloat( Radius, finalRadius + 1.0f );
	}

#endregion WipeFromBlack


#region FadeToBlack

	public static void FadeToBlack( float time )
	{
		if( Instance == null ) return;
		Instance.SetShader( 0.0f, 0.0f, 0.0f, 0.0f );
		Instance.StartCoroutine( _FadeToBlack( time ) );
	}

	private static IEnumerator _FadeToBlack( float time )
	{
		float elapsed = 0.0f;

		while( elapsed < time )
		{
			float progress = elapsed / time;

			float t = progress < 0.5f ? 4.0f * progress * progress * progress :
						  1.0f - Mathfs.Pow( -2.0f * progress + 2.0f, 3.0f ) / 2.0f;

			Instance._tempMaterial.SetFloat( Alpha, t );

			yield return WaitForEndOfFrame;
			elapsed += Time.deltaTime;
		}

		Instance._tempMaterial.SetFloat( Alpha, 1.0f );
	}

#endregion FadeToBlack


#region FadeFromBlack

	public static void FadeFromBlack( float time )
	{
		if( Instance == null ) return;

		Instance.SetShader( 0.0f, 0.0f, 0.0f, 1.0f );

		Instance.StartCoroutine( _FadeFromBlack( time ) );
	}

	private static IEnumerator _FadeFromBlack( float time )
	{
		float elapsed = 0.0f;

		while( elapsed < time )
		{
			float progress = elapsed / time;

			float t = 1.0f
					  - (progress < 0.5f ? 4.0f * progress * progress * progress :
							 1.0f - Mathfs.Pow( -2.0f * progress + 2.0f, 3.0f ) / 2.0f);

			Instance._tempMaterial.SetFloat( Alpha, t );

			yield return WaitForEndOfFrame;
			elapsed += Time.deltaTime;
		}

		Instance._tempMaterial.SetFloat( Alpha, 0.0f );
	}

#endregion FadeFromBlack


#region WipeToBlackFromCenter

	public static void WipeToBlackFromCenter( float time )
	{
		if( Instance == null ) return;

		Vector2 coords         = new Vector2( Screen.width, Screen.height ) * 0.5f;
		Vector2 furthestCorner = FindFurthestCorner( coords );
		float   radius         = (furthestCorner - coords).magnitude;

		Instance.SetShader( coords.x, coords.y, radius, 1.0f );

		Instance.StartCoroutine( _WipeToBlackFromCenter( radius, time ) );
	}

	private static IEnumerator _WipeToBlackFromCenter( float startingRadius, float time )
	{
		float elapsed = 0.0f;

		while( elapsed < time )
		{
			float progress = elapsed / time;
			float t        = 1 - Mathfs.Cos( progress * Mathfs.TAU / 4.0f );

			float radius = Mathfs.Lerp( startingRadius, 0, t );
			Instance._tempMaterial.SetFloat( Radius, radius );

			yield return WaitForEndOfFrame;
			elapsed += Time.deltaTime;
		}

		Instance._tempMaterial.SetFloat( Alpha, 1.0f );
		Instance._tempMaterial.SetFloat( Radius, -1.0f );
	}

#endregion WipeToBlackFromCenter


#region WipeFromBlackFromCenter

	public static void WipeFromBlackFromCenter( float time )
	{
		if( Instance == null ) return;

		Vector2 coords         = new Vector2( Screen.width, Screen.height ) * 0.5f;
		Vector2 furthestCorner = FindFurthestCorner( coords );
		float   radius         = (furthestCorner - coords).magnitude;

		Instance.SetShader( coords.x, coords.y, 0.0f, 1.0f );

		Instance.StartCoroutine( _WipeFromBlackFromCenter( radius, time ) );
	}

	private static IEnumerator _WipeFromBlackFromCenter( float finalRadius, float time )
	{
		float elapsed = 0.0f;

		while( elapsed < time )
		{
			float progress = elapsed / time;
			float t        = Mathfs.Sin( progress * Mathfs.TAU / 4.0f );

			float radius = Mathfs.Lerp( 0, finalRadius, t );
			Instance._tempMaterial.SetFloat( Radius, radius );

			yield return WaitForEndOfFrame;
			elapsed += Time.deltaTime;
		}

		Instance._tempMaterial.SetFloat( Alpha, 0.0f );
		Instance._tempMaterial.SetFloat( Radius, finalRadius + 1.0f );
	}

#endregion WipeFromBlackFromCenter
}
