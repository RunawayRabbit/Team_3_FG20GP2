
using Cinemachine;
using UnityEngine;

public class CamPlanarConfine : CinemachineExtension
{
	[SerializeField] private float camZ = 0;

	protected override void PostPipelineStageCallback( CinemachineVirtualCameraBase vcam,
													   CinemachineCore.Stage        stage,
													   ref CameraState              state,
													   float                        deltaTime )
	{
		if( stage == CinemachineCore.Stage.Body )
		{
			var pos = state.RawPosition;
			pos.z             = camZ;
			state.RawPosition = pos;
		}
	}
}
