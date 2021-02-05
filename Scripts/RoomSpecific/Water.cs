using System;
using System.Collections;
using Freya;
using UnityEngine;

public class Water : MonoBehaviour
{
	[SerializeField] public float[] levels;
	[SerializeField] public float speed = 1.2f;
	public Vector3 BasePosition { get; private set; }

	private static GameObject _currentWater;
	public static float? GetActiveWaterHeight()
	{
		if( _currentWater != null ) return _currentWater.transform.position.y;
		else return float.NegativeInfinity;
	}

	private void Awake()
	{
		SetPositionAsBase();
		_currentWater = gameObject;
	}

	private void Start()
	{
		int lastSeenAt = GlobalState.lastSeenWaterLevel;
		int current    = GlobalState.WaterLevel;

		if( current == lastSeenAt )
		{
			// Water hasn't moved since the last time the player saw the water, set and forget.
			_currentWaterLevel = current;
			_targetWaterLevel  = current;
		}
		else if( lastSeenAt > current )
		{
			// Water has moved down, animate it so that the player can see.
			_currentWaterLevel = current + 1;
			_targetWaterLevel  = current;
		}
		else
		{
			// Water has moved up, animate it so that the player can see.
			_currentWaterLevel = current - 1;
			_targetWaterLevel  = current;
		}


		transform.position = new Vector3( BasePosition.x, BasePosition.y - levels[_currentWaterLevel], BasePosition.z );
		StartCoroutine( ChangeWaterLevel() );
	}

	public void SetPositionAsBase() { BasePosition = transform.position; }

	private int _currentWaterLevel = 0;
	private int _targetWaterLevel;

	public void SetTargetLevelAndBegin( int newLevel )
	{
		if( _targetWaterLevel == newLevel ) return;

		if( newLevel < 0
			|| newLevel >= levels.Length )
		{
			Debug.LogWarning(
				$"Water level set to a value that is out of bounds! ({newLevel}) Does this water object have all of it's heights defined?" );

			return;
		}

		_targetWaterLevel = newLevel;
		StartCoroutine( ChangeWaterLevel() );
	}

	private IEnumerator ChangeWaterLevel()
	{
		float from = BasePosition.y - transform.position.y;
		float to   = levels[_targetWaterLevel];

		float distance = Mathfs.Abs( from - to );

		if( distance == 0 ) yield break;

		float moveDuration = distance / speed;

		float fractionTimeElapsed = 0.0f;

		while( fractionTimeElapsed < 1.0f )
		{
			fractionTimeElapsed += Time.deltaTime / moveDuration;
			float t = Mathfs.Smoother01( fractionTimeElapsed );

			transform.position = new Vector3(
				BasePosition.x,
				BasePosition.y - Mathfs.Lerp( from, to, t ),
				BasePosition.z );

			yield return new WaitForEndOfFrame();
		}

		_currentWaterLevel = _targetWaterLevel;
		Debug.Log( $"Water level is now {_currentWaterLevel}" );
	}
}
