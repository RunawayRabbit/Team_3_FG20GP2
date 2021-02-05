using System.Collections;
using Freya;
using UnityEngine;

public class MoonPuzzle : MonoBehaviour
{
	private static readonly int MoonPhase = Shader.PropertyToID( "_MoonPhase" );

	[SerializeField] private RotateCircle ourCircle;

	[SerializeField] private AudioData waterLevelChangedSound;
	//[SerializeField] private float[] moonPhases = new float[8];

	private float _currentMoonPhase;
	private float _moonPhaseVel;
	private float _targetPhase;
	private int _currentPhase;

	private void OnEnable()
	{
		_currentMoonPhase = RenderSettings.skybox.GetFloat( MoonPhase );
		_targetPhase      = _currentMoonPhase;
		_moonPhaseVel     = 0;

		_currentPhase     = GlobalState.ringPositions[0];

		ourCircle.OnSegmentChanged += OurCircleOnOnSegmentChanged;

		ourCircle.isLocked = false;
	}

	private void OnDisable() { ourCircle.OnSegmentChanged -= OurCircleOnOnSegmentChanged; }

	private void MoveTheGoddamnMoon( int fromSegment, int toSegment )
	{
		//@NOTE: This is all kinds of scuffed, but deadline is in 3 hours so..
		//@NOTE: Clockwise and Anticlockwise are mislabeled here! NO TIME TO REFACTOR

		if( fromSegment == -1 ) fromSegment = RotateCircle.GetDefaultPosition( 0 );

		var deltas = new[]
		{
			0.05f, // 0->1
			0.25f, // 1->2
			0.15f, // 2->3
			0.05f, // 3->4
			0.05f, // 4->5
			0.25f, // 5->6
			0.15f, // 6->7
			0.05f, // 7->0
		};

		// Clockwise, incrementing
		int   clockwise         = fromSegment;
		float clockwiseDistance = 0.0f;

		while( clockwise != toSegment )
		{
			clockwiseDistance += deltas[clockwise];
			clockwise         =  Mathfs.Mod( clockwise + 1, 8 );
		}

		// Anticlockwise, decrementing
		int   anticlockwise         = fromSegment;
		float anticlockwiseDistance = 0.0f;

		while( anticlockwise != toSegment )
		{
			anticlockwise         =  Mathfs.Mod( anticlockwise - 1, 8 );
			anticlockwiseDistance += deltas[anticlockwise];
		}

		if( clockwiseDistance < anticlockwiseDistance )
		{
			// move clockwise
			_targetPhase = _currentMoonPhase + clockwiseDistance;
		}
		else
		{
			// move anticlockwise
			_targetPhase = _currentMoonPhase - anticlockwiseDistance;
		}
	}

	private void Update()
	{
		_currentMoonPhase = Mathfs.SmoothDamp( _currentMoonPhase, _targetPhase, ref _moonPhaseVel, 1.0f, 0.7f );
		RenderSettings.skybox.SetFloat( MoonPhase, _currentMoonPhase );
	}

	private void OurCircleOnOnSegmentChanged( int segment )
	{
		if( segment == _currentPhase ) return;

		// Move the moon regardless
		MoveTheGoddamnMoon( _currentPhase, segment );

		// Check if the water level actually changed.
		var waterLevel = WaterLevelFromSegment( segment );

		if( waterLevel != GlobalState.WaterLevel )
		{
			// Change the water level
			GlobalState.WaterLevel = waterLevel;

			// Animate!
			ourCircle.isLocked = true;
			var ourCurrentSource = SFXManager.PlaySoundAt( waterLevelChangedSound, transform.position );
			StartCoroutine( WaitForSound( ourCurrentSource ) );
		}

		// Change our current phase to reflect the move
		_currentPhase = segment;
	}

	private IEnumerator WaitForSound( AudioSource ourCurrentSource )
	{
		float duration = ourCurrentSource.clip.length * 0.25f;

		yield return new WaitForSeconds( duration );

		ourCircle.isLocked = false;
	}

	private int WaterLevelFromSegment( int segment )
	{
		int[] segmentToWaterLevel =
		{
			1, // right
			2,
			3, // top
			2,
			1, // left
			0,
			0, // bottom
			0,
		};

		return segmentToWaterLevel[segment];
	}
}
