using System.Collections;
using Cinemachine;
using UnityEngine;

public class ObservatoryPuzzle : MonoBehaviour
{
	[SerializeField] private ZodiacPuzzle zodiacPuzzle;
	[SerializeField] private float transitionTime = 1.2f;
	[SerializeField] private float cooldownPeriod = 2.0f;

	[SerializeField] private CinemachineVirtualCamera transitionCam;
	[SerializeField] private CinemachineVirtualCamera moonCam;
	[SerializeField] private CinemachineVirtualCamera starCam;

	private CinemachineBrain brain;
	private CinemachineBlendDefinition oldBlend;

	private bool _isCoolingDown = false;

	public void StartPuzzle()
	{
		if( _isCoolingDown ) return;

		// Get camera, even if we have no persistent scene
		if( CameraDirector.IsLoaded ) { brain = CameraDirector.Camera.GetComponent<CinemachineBrain>(); }
		else { brain                          = Camera.main.GetComponent<CinemachineBrain>(); }

		// cache the old cinemachine blend mode, replace it with ours
		oldBlend = brain.m_DefaultBlend;

		brain.m_DefaultBlend =
			new CinemachineBlendDefinition( CinemachineBlendDefinition.Style.EaseInOut, transitionTime );

		// ease into the TransitionCam
		transitionCam.Priority = 100;

		// lock the player
		PlayerMovement player;

		if( PlayerManager.IsLoaded )
			player = PlayerManager.CurrentPlayer.GetComponent<PlayerMovement>();
		else
			player = GameObject.FindWithTag( "Player" ).GetComponent<PlayerMovement>();

		player.ChangeState( PlayerMovement.PlayerState.Locked );

		// activate ourselves, deactivate children so they don't show.
		// @NOTE: We need to be active so that we can run the coroutine
		gameObject.SetActive( true );

		for( int i = 0; i < transform.childCount; i++ ) { transform.GetChild( i ).gameObject.SetActive( false ); }

		StartCoroutine( TransitionIn() );
	}

	private IEnumerator TransitionIn()
	{
		//fade to black
		TransitionFader.FadeToBlack( transitionTime * 0.95f );

		// Wait while the camera moves to transition and the screen fades to black
		yield return new WaitForSeconds( transitionTime );

		// Set the zodiac puzzle to the correct mode for startup
		ZodiacPuzzle.Mode mode = GlobalState.ObservatoryPowered ? ZodiacPuzzle.Mode.Stars : ZodiacPuzzle.Mode.Moon;
		zodiacPuzzle.ChangeMode( mode, true );

		// jump-cut the camera to the correct starting mode
		brain.m_DefaultBlend = new CinemachineBlendDefinition( CinemachineBlendDefinition.Style.Cut, 0 );
		;
		transitionCam.Priority = 0;
		starCam.Priority       = GlobalState.ObservatoryPowered ? 100 : 0;
		moonCam.Priority       = GlobalState.ObservatoryPowered ? 0 : 100;

		// Pause for effect
		yield return new WaitForSeconds( 0.2f );

		// Activate all children in preperation for the screen to fade back in
		for( int i = 0; i < transform.childCount; i++ ) { transform.GetChild( i ).gameObject.SetActive( true ); }

		// Fade back in
		TransitionFader.WipeFromBlackFromCenter( transitionTime );

		// Reset the cinemachine blend mode back to it's original state before we started.
		brain.m_DefaultBlend = oldBlend;
	}


	public void ExitPuzzle()
	{
		StartCoroutine( TransitionOut() );
	}

	private IEnumerator TransitionOut()
	{
		//wipe to black
		TransitionFader.WipeToBlackFromCenter( transitionTime );

		// Wait while the camera moves to transition and the screen wipes to black
		yield return new WaitForSeconds( transitionTime );

		// Turn the puzzle off
		zodiacPuzzle.ChangeMode( ZodiacPuzzle.Mode.Off, true );

		// unlock the player
		PlayerMovement player;
		if( PlayerManager.IsLoaded )
			player = PlayerManager.CurrentPlayer.GetComponent<PlayerMovement>();
		else
			player = GameObject.FindWithTag( "Player" ).GetComponent<PlayerMovement>();
		player.ChangeState( PlayerMovement.PlayerState.Waiting );

		//wipe to black
		TransitionFader.FadeFromBlack( transitionTime );

		// Deactivate all children. Remember that THIS object must remain active or the coroutine dies!
		for( int i = 0; i < transform.childCount; i++ ) { transform.GetChild( i ).gameObject.SetActive( false ); }

		//cooldown for re-entering..
		_isCoolingDown = true;

		yield return new WaitForSeconds( cooldownPeriod );

		_isCoolingDown = false;
	}
}
