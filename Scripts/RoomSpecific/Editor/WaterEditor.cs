using UnityEditor;
using UnityEngine;

[CustomEditor( typeof(Water) )]
public class WaterEditor : Editor
{
	private Water water;
	private float oldY;
	private Transform _transform;

	private void OnEnable()
	{
		if( !this.target ) return;
		water = (Water) target;

		if(water.BasePosition == Vector3.zero) water.SetPositionAsBase();

		_transform = water.transform;
		oldY       = _transform.position.y;
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		SerializedProperty speed  = serializedObject.FindProperty( "speed" );
		SerializedProperty levels = serializedObject.FindProperty( "levels" );

		EditorGUILayout.HelpBox( "Use Page Up and Page Down in play mode to test.", MessageType.Info, true );

		using( var check = new EditorGUI.ChangeCheckScope() )
		{
			EditorGUILayout.PropertyField( speed, true );

			//@NOTE: SHENNANIGANS!
			levels.arraySize  = EditorGUILayout.IntField( "Water Level Count", levels.arraySize - 1 ) + 1;
			levels.isExpanded = EditorGUILayout.Foldout( levels.isExpanded, "Water Levels" );

			if( levels.isExpanded )
			{
				for( int i = 1; i < levels.arraySize; ++i )
				{
					SerializedProperty transformProp = levels.GetArrayElementAtIndex( i );
					EditorGUILayout.PropertyField( transformProp, new GUIContent( "Depth " + i ) );
				}
			}

			if( check.changed ) serializedObject.ApplyModifiedProperties();
		}
	}

	private void OnSceneGUI()
	{
		if( !Application.isPlaying )
		{
			if( _transform.hasChanged )
			{
				_transform.hasChanged = false;

				float delta = _transform.position.y - oldY;

				for( var i = 1; i < water.levels.Length; i++ ) { water.levels[i] += delta; }

				oldY = _transform.position.y;
			}
		}

		for( var i = 0; i < water.levels.Length; i++ ) { DrawWaypoints( i ); }

		;
	}

	private void DrawWaypoints( int index )
	{
		var style = new GUIStyle();

		style.fontStyle        = FontStyle.Bold;
		style.fontSize         = 12;
		style.alignment        = TextAnchor.MiddleCenter;
		style.normal.textColor = Color.white;

		var startColor = new Color( 0.1f, 1.0f, 0.2f, 0.7f );
		var endColor   = new Color( 0.1f, 0.2f, 1.0f, 0.7f );

		float currentHeight = water.levels[index];

		using( new Handles.DrawingScope( Color.Lerp( startColor, endColor, (float) index / water.levels.Length ) ) )
		{
			Vector3 waterLevelPos = water.BasePosition + (Vector3.down * currentHeight);

			Handles.Label( waterLevelPos, $"Water Level {index}", style );

			if( index == 0
				&& !Application.isPlaying )
				return;

			for( float i = 0.0f; i < 12.0f; i += 1.5f ) { Handles.DrawWireDisc( waterLevelPos, Vector3.up, i ); }

			float size = HandleUtility.GetHandleSize( waterLevelPos ) * 0.5f;

			using( var check = new EditorGUI.ChangeCheckScope() )
			{
				float moved = _transform.position.y
							  - Handles.Slider( waterLevelPos, Vector3.up, size, Handles.ArrowHandleCap, 0.01f ).y;

				if( check.changed )
				{
					Undo.RecordObject( water, "Moved Water Level" );
					EditorUtility.SetDirty( water );

					water.levels[index] = moved;
				}
			}
		}
	}
}
