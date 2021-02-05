using UnityEngine;

public class Tunnel : MonoBehaviour
{
	[SerializeField] private float delta = 0.3f;
	[SerializeField] private Transform topOfStairs = default;
	[SerializeField] private Transform bottomOfStairs = default;

	public Vector3 GetTunnelPoint()
	{
		var player = PlayerManager.CurrentPlayer;

		if( player.transform.position.y > transform.position.y + delta )
			return bottomOfStairs.position;
		else
			return topOfStairs.position;
	}
}
