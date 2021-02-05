
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

[CustomEditor(typeof(Interactable), true)]
public class InteractableEditor : Editor
{
	public void OnSceneGUI()
	{
		var inter = (Interactable) target;

		var standPoint = inter.StandPoint;

		var style = new GUIStyle();

		style.fontStyle        = FontStyle.Bold;
		style.fontSize         = 12;
		style.alignment        = TextAnchor.MiddleCenter;
		style.normal.textColor = Color.black;

		Handles.Label(standPoint.position + Vector3.up * 0.5f, "StandPoint", style);
		using( var check = new EditorGUI.ChangeCheckScope() )
		{
			Vector3    standPos   = standPoint.position;
			Quaternion standRot   = standPoint.rotation;
			Vector3    standScale = standPoint.localScale;

			Handles.TransformHandle( ref standPos, ref standRot, ref standScale);

			if( check.changed )
			{
				Undo.RecordObject( standPoint, "Moved StandPoint" );
				EditorUtility.SetDirty( standPoint );

				if( NavMesh.SamplePosition( standPos, out var hit, 5.0f, NavMesh.AllAreas ) )
				{
					standPos = hit.position;
				}

				standPoint.position   = standPos;
				standPoint.rotation   = standRot;
				//standPoint.localScale = standScale;
			}
		}

		using(new Handles.DrawingScope( Color.red ))
		{
			Handles.DrawAAPolyLine( Texture2D.whiteTexture, 2.0f, inter.transform.position, standPoint.position );
		}
	}
}
