using UnityEngine;

public static class AudioUtils
{
	private static float twelfthRootOfTwo = Mathf.Pow( 2, 1.0f / 12 );


	public static float DecibelToLinear( float dB )

	{
		if( dB > -80.0f ) return Mathf.Clamp01( Mathf.Pow( 10.0f, dB / 20.0f ) );

		return 0;
	}

	public static float LinearToDecibel( float linear )

	{
		if( linear > 0.0001f ) return Mathf.Clamp( 20.0f * Mathf.Log10( linear ), -80.0f, 0.0f );

		return -80.0f;
	}


	public static float SemitoneToPitch( float semitone )
	{
		return Mathf.Clamp( Mathf.Pow( twelfthRootOfTwo, semitone ), 0f, 4f );
	}

	public static float PitchToSemitone( float pitch ) { return Mathf.Log( pitch, twelfthRootOfTwo ); }

}