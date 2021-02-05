
using System.Collections.Generic;
using UnityEngine;

public class LibraryCandleVFX : MonoBehaviour
{
	private static List<LibraryCandleVFX> allCandles = new List<LibraryCandleVFX>();

	public GameObject fireVFX;
	public GameObject waterVFX;

	private static float nextCheck;
	private const float timeBetweenUpdates = 2.0f;

	private void OnEnable()
	{
		allCandles.Add(this);
	}

	private void OnDisable()
	{
		allCandles.Remove( this );
	}

	private void Update()
	{
		if( Time.time > nextCheck )
		{
			float waterHeight = Water.GetActiveWaterHeight() ?? float.NegativeInfinity;

			foreach( LibraryCandleVFX candle in allCandles )
			{
				bool isFire = candle.transform.position.y > waterHeight;
				candle.fireVFX.SetActive(isFire);
				candle.waterVFX.SetActive(!isFire);
			}

			nextCheck = Time.time + timeBetweenUpdates;
		}
	}

}
