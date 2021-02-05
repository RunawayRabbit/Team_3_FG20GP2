using Cinemachine;
using Freya;
using UnityEngine;

public class HubBlendCam : MonoBehaviour
{
	private CinemachineMixingCamera _virtualCam;

	[SerializeField] private Transform followTarget;
	[SerializeField] private CinemachineTargetGroup[] targetGroups;

	[SerializeField] private float minHeight;
	[SerializeField] private float maxHeight;

	private float _blend;
	private float _camVelocity;

	void Start()
	{
		_virtualCam           = GetComponent<CinemachineMixingCamera>();
		_virtualCam.m_Weight0 = 1.0f;
		_virtualCam.m_Weight1 = 0.0f;

		_blend = 0.0f;

		if( PlayerManager.CurrentPlayer )
		{
			followTarget = PlayerManager.CurrentPlayer.transform;
		}

		AddFollowToTargetGroups();
	}

	private void AddFollowToTargetGroups()
	{
		if(followTarget)
		{
			foreach( var targetGroup in targetGroups )
				targetGroup.AddMember( followTarget, 1.0f, 1.0f );
		}
	}

	private void OnEnable() { PlayerManager.OnPlayerChanged += PlayerManagerOnOnPlayerChanged; }
	private void OnDisable() { PlayerManager.OnPlayerChanged -= PlayerManagerOnOnPlayerChanged; }

	private void PlayerManagerOnOnPlayerChanged( GameObject obj )
	{
		followTarget = obj.transform;
		AddFollowToTargetGroups();
	}

	void Update()
	{
		if(followTarget)
		{
			float target = Mathfs.InverseLerp( minHeight, maxHeight, followTarget.position.y );
			//blend = Mathfs.MoveTowards( blend, target, 0.3f * Time.deltaTime );
			_blend = Mathfs.SmoothDamp( _blend, target, ref _camVelocity, 0.4f );

			_virtualCam.m_Weight0 = Mathf.Abs( 1.0f - _blend);
			_virtualCam.m_Weight1 = Mathf.Abs( _blend );
		}
	}
}
