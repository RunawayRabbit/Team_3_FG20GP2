using System;
using System.Collections;
using Freya;
using UnityEngine;
using UnityEngine.AI;

/*
 *	@TODOs:
 *		ejtb: Rotation is hella ugly, probably needs fixing once animations are in. Worst-case
 *	scenario, we'll probably have to set _agent.angularSpeed = 0 as we approach our target and
 *  do the slerp manually? I suggest using
 *		t = Mathfs.Smooth01((_dest - transform.position).sqrMagnitude / arrivalDistanceSq)
 *	or some similar.
 *
 *		ejtb: Replace _isBusy with an enum representing state. Idle, Moving, Interacting are
 *	the ones I'd start with.
 */

[RequireComponent( typeof(NavMeshAgent) )]
public partial class PlayerMovement : MonoBehaviour
{
	public enum PlayerState
	{
		Waiting,
		Interacting,
		Moving,
		Locked,
	}

	private NavMeshAgent _agent;
	private Animator _animator;

	[SerializeField] private float snapDistance = 0.02f;
	[SerializeField] private float interactionRange = 0.1f;
	[SerializeField] private float turnSpeed = 0.1f;
	[SerializeField] private float timeUntilIdleAnimation = 5.0f;

	private Interactable _targetInteractable;

	private Vector3 _dest;
	private Quaternion _targetRot;

	private IPlayerState _state;

	private void Awake()
	{
		_agent    = this.GetComponent<NavMeshAgent>();
		_animator = this.GetComponentInChildren<Animator>();
		_dest     = transform.position; // Stops weird teleporty nonsense

		ChangeState( PlayerState.Waiting );
	}

	public void OnEnable()
	{
		MouseController.OnMoveCommand  += OnMoveCommand;
		MouseController.OnInteractable += OnInteractable;
	}

	public void OnDisable()
	{
		MouseController.OnMoveCommand  -= OnMoveCommand;
		MouseController.OnInteractable -= OnInteractable;
	}

	private void Update()
	{
		float distanceToPosition = (_agent.destination - transform.position).sqrMagnitude;
		bool atPosition = distanceToPosition < 0.05;
		if(atPosition)
			_animator.SetFloat( "agentVelocity", 0);
		else
			_animator.SetFloat( "agentVelocity", _agent.velocity.magnitude );

		_state.Update();
	}

	public void OnMoveCommand( Vector3 dest )
	{
		// @TODO: Should we ignore move commands while interacting?
		if( _state.Type == PlayerState.Locked
			|| _state.Type == PlayerState.Interacting )
			return;

		_targetInteractable = null;
		ChangeState( PlayerState.Moving );

		Debug.DrawLine( transform.position, dest, Color.red, 2.0f );

		if( NavMesh.SamplePosition( dest, out NavMeshHit hit, 2.0f, NavMesh.AllAreas ) )
		{
			MoveToPosition( hit.position );
		}
		else if( _targetInteractable != null ) { Debug.Log( "NavMesh Unreachable. Play `I Can't Reach!" ); }

		Debug.DrawLine( transform.position, _dest, Color.green, 2.0f );
	}

	public void OnInteractable( Interactable interactable )
	{
		if( _state.Type == PlayerState.Locked ) return;

		ChangeState( PlayerState.Moving );
		Vector3 standPoint = interactable.StandPoint.position;

		// Are we out of range of the interactable?
		if( (standPoint - transform.position).sqrMagnitude > interactionRange.Square() )
		{
			MoveToPosition( standPoint );
		}

		_targetInteractable = interactable;
	}

	private void MoveToPosition( Vector3 newDest )
	{
		_dest = newDest;

		_agent.SetDestination( newDest );
		_agent.isStopped = false;
	}


	public void ChangeState( PlayerState state )
	{
		_state?.OnExit();

		_state = state switch
		{
			PlayerState.Waiting     => new PlayerWaiting( this, timeUntilIdleAnimation ),
			PlayerState.Interacting => new PlayerInteracting( this ),
			PlayerState.Moving      => new PlayerMoving( this, turnSpeed, snapDistance ),
			PlayerState.Locked      => new PlayerLocked( this ),
			_                       => throw new ArgumentOutOfRangeException( nameof(state), state, null )
		};

		_state?.OnEnter();
	}

	public void LockForSeconds( float seconds )
	{
		ChangeState( PlayerState.Locked );
		StartCoroutine( UnlockIn( seconds ) );
	}

	private IEnumerator UnlockIn( float seconds )
	{
		yield return new WaitForSeconds( seconds );
		ChangeState( PlayerState.Waiting );
	}
}
