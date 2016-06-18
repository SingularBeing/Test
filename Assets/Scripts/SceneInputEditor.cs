using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof(SceneInput))]
public class SceneInputEditor : Editor
{

	void OnSceneGUI ()
	{
		SceneInput input = (SceneInput)target;

		if (input.m_MousePositions.Count > 0) {
			Handles.DrawBezier (input._startPoint, input._endPoint, input._startTangent, input._endTangent, Color.yellow, null, 5);
			Handles.DrawBezier (input._startPoint, input._endPoint, input._outStartTangent, input._outEndTangent, Color.magenta, null, 5);
		}
	}

}