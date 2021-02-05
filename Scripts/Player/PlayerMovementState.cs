using UnityEngine;

public partial class PlayerMovement
{
	private interface IPlayerState
	{
		PlayerState Type { get; }

		void Update();

		void OnEnter();

		void OnExit();
	}

	private class PlayerWaiting : IPlayerState
	{
		public PlayerState Type => PlayerState.Waiting;

		private PlayerMovement _player;
		private float idleStart;
		private bool idleAnimationPlaying = false;
		private float timeUntilIdleAnimation;

		public PlayerWaiting( PlayerMovement player, float timeUntilIdleAnimation )
		{
			this._player                = player;
			this.timeUntilIdleAnimation = timeUntilIdleAnimation;
		}

		public void OnEnter()
		{
			if( _player._agent.isStopped ) _player._agent.isStopped = false;
		}

		public void Update()
		{
			if( !idleAnimationPlaying )
			{
				if( Time.time - idleStart > timeUntilIdleAnimation )
				{
					idleAnimationPlaying = true;
					// @TODO: Idle animation stuff goes here
				}
			}
			else
			{
				// @TODO: Check to see if idle animation is done
				/*
				if( animationIsDone )
				{
					idleAnimationPlaying = false;

					idleStart = Time.time;
				}
			*/
			}
		}

		public void OnExit()
		{
			// do nothing
		}
	}

	private class PlayerInteracting : IPlayerState
	{
		public PlayerState Type => PlayerState.Interacting;

		private PlayerMovement _player;

		public PlayerInteracting( PlayerMovement player ) { this._player = player; }

		public void Update()
		{
			if( _player._animator.GetCurrentAnimatorStateInfo( 0 ).IsName( "Idle" ) )
				_player.ChangeState( PlayerState.Waiting );
		}

		public void OnEnter()
		{
			// do nothing
		}

		public void OnExit()
		{
			// do nothing
		}
	}

	private class PlayerMoving : IPlayerState
	{
		public PlayerState Type => PlayerState.Moving;

		private PlayerMovement _player;
		private float turnSpeed;
		private float snapDistance;

		public PlayerMoving( PlayerMovement player, float turnSpeed, float snapDistance )
		{
			this._player      = player;
			this.turnSpeed    = turnSpeed;
			this.snapDistance = snapDistance;
		}

		public void Update()
		{
			var agent    = _player._agent;
			var dest     = _player._dest;
			var animator = _player._animator;

			// NavMeshAgent is busy, don't disturb it!
			if( agent.pathPending ) return;

			float distRemaining = agent.remainingDistance;

			// Are we at our destination?
			if( distRemaining <= snapDistance )
			{
				// Arrive at the destination
				_player.ChangeState( PlayerState.Waiting );
				_player.transform.position = dest;

				// Interact with our target if we have one
				var targetInteractable = _player._targetInteractable;

				if( targetInteractable != null )
				{
					_player.ChangeState( PlayerState.Interacting );
					animator.SetTrigger( "interact" );
					_player._targetRot = targetInteractable.StandPoint.rotation;
					targetInteractable.Interact();

					//@TODO: Animation handling stuff goes here maybe?

					_player._targetInteractable = null;
				}

				_player.transform.rotation =
					Quaternion.RotateTowards( _player.transform.rotation,
											  _player._targetRot,
											  turnSpeed * Time.deltaTime );
			}
			else { _player._targetRot = Quaternion.LookRotation( dest - _player.transform.position ); }
		}

		public void OnEnter()
		{
			// do nothing
		}

		public void OnExit()
		{
			// do nothing
		}
	}

	private class PlayerLocked : IPlayerState
	{
		public PlayerState Type => PlayerState.Locked;

		private PlayerMovement _player;

		public PlayerLocked( PlayerMovement player ) { this._player = player; }

		public void Update() { return; }

		public void OnEnter()
		{
			// do nothing
		}

		public void OnExit()
		{
			// do nothing
		}
	}
}
