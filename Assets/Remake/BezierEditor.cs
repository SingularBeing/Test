using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof(BezierScene))]
public class BezierEditor : Editor
{

	void OnSceneGUI ()
	{
		BezierScene scene = (BezierScene)target;
		//draw the bezier lines
		if (BezierScene._canUseValues) {
			if (InputScript._mousePositions.Count > 4) {
				Vector3 p0 = InputScript._mousePositions [0]._position;
				for (int i = 0; i < InputScript._mousePositions.Count; i += 3) {
					Vector3 p1 = InputScript._mousePositions [i]._position;
					Vector3 p2 = InputScript._mousePositions [i + 1]._position;
					Vector3 p3 = InputScript._mousePositions [i + 2]._position;

					Handles.DrawBezier (p0, p3, p1, p2, Color.yellow, null, 2f);

					p0 = p3;
				}
			}
		}
		//draw the magnetic lines
		//start off the same as the regular
		if (BezierScene._canUseValues) {
			if (InputScript._magneticPositions.Count > 4) {
				Vector3 p0 = InputScript._magneticPositions [0]._position;
				for (int i = 0; i < InputScript._magneticPositions.Count; i += 3) {
					Vector3 p1 = InputScript._magneticPositions [i]._position;
					Vector3 p2 = InputScript._magneticPositions [i + 1]._position;
					Vector3 p3 = InputScript._magneticPositions [i + 2]._position;

					Handles.DrawBezier (p0, p3, p1, p2, Color.magenta, null, 2f);

					p0 = p3;
				}
			}
		}
	}

}