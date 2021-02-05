using System;
using UnityEditor;
using UnityEngine;
using System.Reflection;

[CustomEditor( typeof(AudioData) )]
public class AudioDataEditor : Editor
{
	private static GUIContent addListElement = new GUIContent( "+", "duplicate" );
	private static GUIContent removeListElement = new GUIContent("-", "delete");

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		var clips = serializedObject.FindProperty( "clips" );

		GUILayout.Label("Clips", EditorStyles.boldLabel);
		for( int i = 0; i < clips.arraySize ; i++ )
		{
			var clip = clips.GetArrayElementAtIndex( i );

			using( new EditorGUILayout.HorizontalScope() )
			{
				EditorGUILayout.PropertyField( clip, GUIContent.none );

				if( GUILayout.Button( "Play", GUILayout.Width(40) ) ) { PlayClip( clip.objectReferenceValue as AudioClip ); }
			}
		}

		using( new EditorGUILayout.HorizontalScope() )
		{
			GUILayout.FlexibleSpace();
			GUILayout.Label("Size:", GUILayout.Width(40));
			EditorGUILayout.PropertyField(clips.FindPropertyRelative("Array.size"), GUIContent.none , GUILayout.Width(30));

			if( GUILayout.Button( addListElement, EditorStyles.miniButtonMid, GUILayout.Width( 20 ) ) )
			{
				clips.InsertArrayElementAtIndex(clips.arraySize);
			}

			if( GUILayout.Button( removeListElement, EditorStyles.miniButtonRight, GUILayout.Width( 20 ) ) )
			{
				int oldSize = clips.arraySize;
				clips.DeleteArrayElementAtIndex(oldSize -1);
				if (clips.arraySize == oldSize) {
					clips.DeleteArrayElementAtIndex(oldSize -1);
				}
			}
		}

		EditorGUILayout.PropertyField(serializedObject.FindProperty("baseVolumeDB"), new GUIContent("Base Volume", "Volume gain in decibels."));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("spacialBlend"), new GUIContent("Spacial Blend", "0 sounds the same everywhere, 1 for fully 3D."));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("pitchShift"),new GUIContent("Pitch Shift", "Measured in semitones"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("randomVolume"), new GUIContent("Randomize Volume", "Randomize volume between +/- this value, in decibels."));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("randomPitch") , new GUIContent("Randomize Pitch", "Randomize pitch between +/- this value, in semitones."));

		serializedObject.ApplyModifiedProperties();
	}

	public static void PlayClip( AudioClip clip, int startSample = 0, bool loop = false )
	{
		Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;

		Type audioUtilClass = unityEditorAssembly.GetType( "UnityEditor.AudioUtil" );

		MethodInfo method = audioUtilClass.GetMethod( "PlayPreviewClip",
													  BindingFlags.Static | BindingFlags.Public,
													  null,
													  new[] { typeof(AudioClip), typeof(int), typeof(bool) },
													  null );

		method.Invoke( null, new object[] { clip, startSample, loop } );
	}
}
