using Cinemachine;
using UnityEngine;

public class ObsGroupCam : MonoBehaviour
{
	private CinemachineTargetGroup _targetGroup;

	void Start()
	{
		_targetGroup = this.GetComponent<CinemachineTargetGroup>();

		if( PlayerManager.CurrentPlayer )
		{
			var followTarget = PlayerManager.CurrentPlayer.transform;
			_targetGroup.AddMember( followTarget, 1.0f, 1.0f );
		}
	}

	private void OnEnable() { PlayerManager.OnPlayerChanged += PlayerManagerOnOnPlayerChanged; }

	private void OnDisable() { PlayerManager.OnPlayerChanged -= PlayerManagerOnOnPlayerChanged; }

	private void PlayerManagerOnOnPlayerChanged( GameObject obj )
	{
		_targetGroup.AddMember( obj.transform, 1.0f, 1.0f );
	}
}
