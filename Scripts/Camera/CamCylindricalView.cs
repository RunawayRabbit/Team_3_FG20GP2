using Cinemachine;
using UnityEngine;

public class CamCylindricalView : CinemachineExtension
{
	[SerializeField] private Transform player = default;
	[SerializeField] private Transform pivot = default;
	[SerializeField] private Vector2 offset = default;
	[SerializeField] private float moveSpeed = 2.0f;

	[SerializeField] private float hackyHeightThreshold = -10.0f;

	private Vector3 _vel;
	private Vector3 _pos;

	private void Start()
	{
		player = PlayerManager.CurrentPlayer.transform;

		if( player == null ) { player = GameObject.FindWithTag( "Player" ).transform; }

		_pos = transform.position;
	}

	protected override void PostPipelineStageCallback( CinemachineVirtualCameraBase vcam,
													   CinemachineCore.Stage        stage,
													   ref CameraState              state,
													   float                        deltaTime )
	{
		if( player == null ) return;

		if( stage == CinemachineCore.Stage.Body )
		{
			float direction = 1.0f;

			if( player.position.y < hackyHeightThreshold ) direction = -1.0f;

			var pivotPos     = pivot.position;
			var offsetVector = (pivotPos - player.position).normalized * direction;
			var radialPos    = pivotPos + (offsetVector * offset.x);
			var newTargetPos = radialPos + Vector3.up * offset.y;

			_pos = Vector3.SmoothDamp( _pos, newTargetPos, ref _vel, moveSpeed );

			state.RawPosition = _pos;
		}
	}
}
