using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof(SimpleCurve))]
public class SimpleCurveEditor : Editor
{

	Vector3 _p0, _p1, _p2, _p3;

	public override void OnInspectorGUI ()
	{
		SimpleCurve curve = (SimpleCurve)target;

		EditorGUI.BeginChangeCheck ();
		{
			_p0 = EditorGUILayout.Vector3Field ("Point 0", _p0);
			_p1 = EditorGUILayout.Vector3Field ("Point 1", _p1);
			_p2 = EditorGUILayout.Vector3Field ("Point 2", _p2);
			_p3 = EditorGUILayout.Vector3Field ("Point 3", _p3);
		}
		if (EditorGUI.EndChangeCheck ()) {
			curve.m_BezierObj = new BezierObj (_p0, _p1, _p2, _p3);
			curve.m_BezierObj.CalculatePoints (6, 6);
		}

		GUILayout.Label ("Points Amount:" + curve.m_BezierObj.points.Length);
	}

	private void OnSceneGUI ()
	{
		SimpleCurve curve = (SimpleCurve)target;

		if (curve.m_BezierObj == null) {
			curve.CalculateNew ();
		}

		if (curve.m_BezierObj != null) {
			Handles.DrawBezier (curve.m_BezierObj.p0, curve.m_BezierObj.p3, curve.m_BezierObj.p1, curve.m_BezierObj.p2, Color.yellow, null, 5);
		}
	}

}