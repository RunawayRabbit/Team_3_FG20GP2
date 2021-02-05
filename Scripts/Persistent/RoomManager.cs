using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviour
{
	// How long to spend fading in/out on scene transitions?
	public static readonly float animationTime = 1.5f;
	private static RoomManager Instance { get; set; }

	public static Action<Room, Room> OnTransitionStart;
	public static Action<Room, Room> OnTransitionComplete;

	private static List<(InteractableSceneTransition, Room)> _activeTransitions = new List<(InteractableSceneTransition, Room)>();

	private CinemachineBrain _camBrain;
	public enum Room
	{
		MainHub,
		Library,
		Observatory,
	}

	private static readonly string[] Names =
	{
		"MainHub", "new_Library", "ObservatoryScene", "Greenhouse Scene V2", "PersistentScene",
	};

	private static int[] _buildIds;

	public int[] BuildIds
	{
		get
		{
			if( _buildIds == null )
			{
				_buildIds = new int[Names.Length];

				for( int i = 0; i < Names.Length; i++ ) { _buildIds[i] = GetBuildIndex( Names[i] ); }
			}

			return _buildIds;
		}
	}

	public static Scene CurrentScene { get; private set; }

	public static Room CurrentRoom { get; private set; }

	void Awake()
	{
		Instance = this;

		if(SceneManager.sceneCount < 2)
			LoadRoom( Room.MainHub );

		SceneManager.activeSceneChanged += OnSceneChanged;

		_camBrain = Camera.main.GetComponent<CinemachineBrain>();

		GlobalState.ResetEverything();
	}

	private void OnDestroy() { SceneManager.activeSceneChanged -= OnSceneChanged; }

	private void OnSceneChanged( Scene previousScene, Scene newScene )
	{
		OnSceneChanged();
	}

	private void OnSceneChanged()
	{
		var persistent = SceneManager.GetSceneByName( "PersistentScene" );
		if( persistent.isLoaded )
		{
			SceneManager.SetActiveScene( persistent );
		}
	}
	private static void LoadPersistentRoom( Scene currentScene )
	{
		int buildId = SceneUtility.GetBuildIndexByScenePath( "PersistentScene" );
		SceneManager.LoadScene( buildId, LoadSceneMode.Additive );
	}

	private static void LoadRoom( Room roomToLoad )
	{
		var buildId = Instance.BuildIds[(int) roomToLoad];

		if( !SceneManager.GetSceneByBuildIndex( buildId ).isLoaded )
		{
			SceneManager.LoadScene( buildId, LoadSceneMode.Additive );
		}

		UpdateRoomState( roomToLoad );
	}

	private static void UpdateRoomState( Room currentRoom )
	{
		var buildId = Instance.BuildIds[(int) currentRoom];
		CurrentScene = SceneManager.GetSceneByBuildIndex( buildId );
		CurrentRoom = currentRoom;
	}

	public static void TransitionToRoom( Room roomToLoad )
	{
		if( Instance == null )
		{
			LoadPersistentRoom( SceneManager.GetActiveScene() );
			// Hacky but I don't know how else to get around the one frame load.
			return;
		}

		if( roomToLoad == CurrentRoom ) return;

		var buildId = Instance.BuildIds[(int) roomToLoad];


		if( SceneManager.GetSceneByBuildIndex( buildId ).isLoaded ) return;

		Instance.StartCoroutine( TransitionToRoomAsync( roomToLoad, buildId ) );
	}

	private static int GetBuildIndex( string sceneName )
	{
		return SceneUtility.GetBuildIndexByScenePath( $"Assets/Scenes/{sceneName}.unity" );
	}

	private static IEnumerator TransitionToRoomAsync( Room roomToLoad, int buildId )
	{
		var startTime = Time.time;
		// Start loading the next scene
		PlayerManager.ChangePlayerState( PlayerMovement.PlayerState.Interacting );

		// Wait for the transition animation to complete
		while( Time.time - startTime < animationTime ) yield return null;

		// Jump-cut cinemachine
		var oldBlend = Instance._camBrain.m_DefaultBlend;

		Instance._camBrain.m_DefaultBlend =
			new CinemachineBlendDefinition( CinemachineBlendDefinition.Style.Cut, 0.0f );


		OnTransitionStart?.Invoke( CurrentRoom, roomToLoad );
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync( buildId, LoadSceneMode.Additive );

		while( !asyncLoad.isDone ) { yield return null; }


		// Unload the previous scene
		int unloadId = Instance.BuildIds[(int) CurrentRoom];

		AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync( unloadId );

		while( !asyncUnload.isDone ) { yield return null; }

		// Call registered people to inform them
		OnTransitionComplete?.Invoke( CurrentRoom, roomToLoad );

		//Reset cinemachine
		Instance._camBrain.m_DefaultBlend = oldBlend;

		UpdateRoomState( roomToLoad );
	}

	public static Transform GetExitPosition( Room from, Room to )
	{
		string fromSceneName = Names[(int) from];

		foreach( (InteractableSceneTransition transitioner, Room room) in _activeTransitions )
		{
			if( transitioner.GetSceneName() == fromSceneName
				&& room == to)
			{
				return transitioner.GetExitPoint();
			}
		}

		return null;
	}


	public static void RegisterTransition( InteractableSceneTransition transitioner, Room room )
	{
		_activeTransitions.Add( (transitioner, room) );
	}

	public static void UnregisterTransition( InteractableSceneTransition transitioner, Room room )
	{
		_activeTransitions.Remove( (transitioner, room) );
	}
}
