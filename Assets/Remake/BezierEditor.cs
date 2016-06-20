using UnityEngine;
using System.Collections;
using UnityEditor;

/// <summary>
/// Used to draw the bezier curves in the scene view
/// </summary>
[CustomEditor (typeof(BezierScene))]
public class BezierEditor : Editor
{
	/// <summary>
	/// Raises the scene GU event.
	/// </summary>
	void OnSceneGUI ()
	{
		//draw the magnetic bezier curve from the ball data
		Handles.DrawBezier (RedBall.p1, RedBall.p4, RedBall.p2, RedBall.p3, Color.magenta, null, 4);
	}

}