using System.Collections.Generic;
using Cinemachine;
using Freya;
using UnityEngine;

public class LibBlendCam : MonoBehaviour
{
	private CinemachineMixingCamera _virtualCam;

	[SerializeField] private List<CinemachineTargetGroup> targetGroups;
	[SerializeField] private Transform followTarget = null;
	[SerializeField] private float smoothTime = 1.2f;

	[SerializeField] private float[] breakpoints = new float[6];

	private float _trackingPos;

	void Start()
	{
		_virtualCam = GetComponent<CinemachineMixingCamera>();
		ResetCamWeights();

		TrackPlayer();
	}

	private void TrackPlayer()
	{
		if( PlayerManager.CurrentPlayer )
		{
			followTarget = PlayerManager.CurrentPlayer.transform;
			_trackingPos = breakpoints[0];

			foreach( var targetGroup in targetGroups )
			{
				targetGroup.GetComponent<CinemachineTargetGroup>().AddMember( followTarget, 0.4f, 1 );
			}
		}
	}



	private void ResetCamWeights()
	{
		_virtualCam.m_Weight0 = 0.0f;
		_virtualCam.m_Weight1 = 0.0f;
		_virtualCam.m_Weight2 = 0.0f;
		_virtualCam.m_Weight3 = 0.0f;
		_virtualCam.m_Weight4 = 0.0f;
	}

	void Update()
	{
		if( !followTarget )
		{
			TrackPlayer();

			return;
		}

		ResetCamWeights();

		_trackingPos =
			Mathfs.MoveTowards( _trackingPos, followTarget.transform.position.y, smoothTime * Time.deltaTime );

		// We're above the topmost breakpoint, show topcam only
		if( _trackingPos > breakpoints[0] )
		{
			_virtualCam.m_Weight0 = 1.0f;

			return;
		}

		// Between cam1 and cam2, lerp.
		if( _trackingPos > breakpoints[1] )
		{
			var camWeights = BlendCameras( breakpoints[0], breakpoints[1], _trackingPos );
			_virtualCam.m_Weight0 = camWeights.Item1;
			_virtualCam.m_Weight4 = camWeights.Item2;
			_virtualCam.m_Weight1 = camWeights.Item3;

			return;
		}

		// In cam2.
		if( _trackingPos > breakpoints[2] )
		{
			_virtualCam.m_Weight1 = 1.0f;

			return;
		}

		// Between cam2 and cam3, lerp.
		if( _trackingPos > breakpoints[3] )
		{
			float t = Mathfs.Smoother01(Mathfs.InverseLerp( breakpoints[2], breakpoints[3], _trackingPos ));
			_virtualCam.m_Weight1 = 1.0f -t;
			_virtualCam.m_Weight2 = t;

			return;
		}

		// In cam3.
		if( _trackingPos > breakpoints[4] )
		{
			_virtualCam.m_Weight2 = 1.0f;

			return;
		}

		// Between cam3 and cam4, lerp.
		if( _trackingPos > breakpoints[5] )
		{
			var camWeights = BlendCameras( breakpoints[4], breakpoints[5], _trackingPos );
			_virtualCam.m_Weight2 = camWeights.Item1;
			_virtualCam.m_Weight4 = camWeights.Item2;
			_virtualCam.m_Weight3 = camWeights.Item3;

			return;
		}

		// We're below the bottommost breakpoint, show botcam only
		_virtualCam.m_Weight3 = 1.0f;
	}

	private (float, float, float) BlendCameras( float a, float b, float t )
	{
		float smoothT   = Mathfs.Smooth01( Mathfs.InverseLerpClamped( a, b, t ) ) * 2.0f;
		float firstCam  = Mathfs.Clamp01( 1 - smoothT );
		float blendCam  = Mathfs.Max( 1 - Mathfs.Abs( 1 - smoothT ), 0 );
		float secondCam = Mathfs.Clamp01( smoothT - 1 );

		var result = (firstCam, blendCam, secondCam);

		return result;
	}
}
