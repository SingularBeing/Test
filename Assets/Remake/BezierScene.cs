using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This script handle ALL of the bezier methods
/// </summary>
public class BezierScene : MonoBehaviour
{

	//Turntable Formula:
	//radius_turnTableFrame_(time) = (speed*time) (cos(angle-(rotation*time)), sin(angle-(rotation*time)));

	public static Transform me;
	public List<MousePosition> positions;
	public List<MousePosition> Gpositions;
	public InputScript input;

	void Start ()
	{
		me = transform;
	}

	void Update ()
	{
		positions = InputScript._mousePositions;
		Gpositions = InputScript._magneticPositions;
	}

	public InputScript m_Input;

	public static bool _canUseValues;

	public static Vector3 GetPoint (float _t)
	{
		return me.TransformPoint (GetPoint (me.InverseTransformPoint (InputScript._mousePositions [0]._position), me.InverseTransformPoint (InputScript._mousePositions [1]._position), me.InverseTransformPoint (InputScript._mousePositions [2]._position), me.InverseTransformPoint (InputScript._mousePositions [3]._position), _t));
	}

	/*public static Vector3 GetPoint (Vector3 _0, Vector3 _1, Vector3 _2, float _t)
	{
		_t = Mathf.Clamp01 (_t);
		float lessOne = 1f - _t;
		return lessOne * lessOne * _0 + 2f * lessOne * _t * _1 + _t * _t * _2;
	}

	/// <summary>
	/// Produces lines tangent to the curve
	/// </summary>
	/// <returns>The first derivative.</returns>
	/// <param name="_0">0.</param>
	/// <param name="_1">1.</param>
	/// <param name="_2">2.</param>
	/// <param name="_t">T.</param>
	public static Vector3 GetFirstDerivative (Vector3 _0, Vector3 _1, Vector3 _2, float _t)
	{
		return 2f * (1f - _t) * (_1 - _0) + 2f * _t * (_2 - _1);
	}*/

	public static Vector3 GetPoint (Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
	{
		t = Mathf.Clamp01 (t);
		float oneMinusT = 1f - t;
		return
			oneMinusT * oneMinusT * oneMinusT * p0 +
		3f * oneMinusT * oneMinusT * t * p1 +
		3f * oneMinusT * t * t * p2 +
		t * t * t * p3;
	}

	/*public static Vector3 GetFirstDerivative (Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
	{
		t = Mathf.Clamp01 (t);
		float oneMinusT = 1f - t;
		return
			3f * oneMinusT * oneMinusT * (p1 - p0) +
		6f * oneMinusT * t * (p2 - p1) +
		3f * t * t * (p3 - p2);
	}


	public Vector3 GetVelocity (float _t)
	{
		return transform.TransformPoint (BezierScene.GetFirstDerivative (transform.InverseTransformPoint (InputScript._mousePositions [0]), transform.InverseTransformPoint (InputScript._mousePositions [1]), transform.InverseTransformPoint (InputScript._mousePositions [2]), transform.InverseTransformPoint (InputScript._mousePositions [3]), _t)) - transform.position;
	}

	public Vector3 GetDirection (float _t)
	{
		return GetVelocity (_t).normalized;
	}*/

	private Vector3 CalculatePosition (float t, Vector3 p0, Vector3 h0, Vector3 h1, Vector3 p1)
	{ 
		float tt = t * t; 
		float temp = 1 - t; 
		float c = 3 * temp * tt; 
		temp *= temp; 
		float b = 3 * temp * t; 
		temp *= temp; 
		float a = temp; 
		return a * p0 + b * h0 + c * h1 + tt * t * p1; 
		// (1-t)^3 p0 + 3(1-t)^2 t p1 + 3(1-t) t^2 p2 + t^3 p3 
	}


	public int GetSegmentAtTime (ref float t)
	{ // get segment and retrieve the delta value to t 
		int nbSegment = InputScript._mousePositions.Count - 1; 
		float tRect = t * nbSegment; 
		int seg = (int)tRect; // cast to int to get segment 
		tRect -= seg; // 0-1 for that segment 
		t = tRect; 
		return seg; 
	}

	public static void CalculateMagneticPath ()
	{
		//grab all of the fast speeds
		List<MousePosition> _tempMP = new List<MousePosition> ();
		List<int> indexes = new List<int> ();
		for (int i = 0; i < InputScript._magneticPositions.Count; i++) {
			MousePosition pos = InputScript._magneticPositions [i];
			if (pos._speed < 0.05f) {
				_tempMP.Add (pos);
				indexes.Add (i);
			}
		}

		Debug.Log ("TempAmount:" + _tempMP.Count);

		//now that we have them, we need to expand the path at this node
		//and then make it last for a few more

		List<MousePosition> newPositions = new List<MousePosition> ();
		//calculate the effected nodes
		//lets say, per 1 pre-effected node, there are 5 others that become effected (smooth transition)
		/*for (int i = 0; i < InputScript._magneticPositions.Count; i++) {
			
		}*/

		//try the first curve example


		InputScript.ResetMagneticPositions (newPositions);
		//InputScript._magneticPositions.Clear ();
		//InputScript._magneticPositions = newPositions;

	}

	public static int PositionOnLine (Vector3 posi)
	{
		for (int i = 0; i < InputScript._mousePositions.Count; i++) {
			MousePosition p = InputScript._mousePositions [i];
			if (p._position == posi) {
				Debug.Log ("Index:" + i);
				return i;
			}
		}

		return -1;
	}

	void OnDrawGizmos ()
	{
		Gizmos.color = Color.green;
		//draw the bezier points
		for (int i = 0; i < InputScript._mousePositions.Count; i++) {
			MousePosition pos = InputScript._mousePositions [i];
			if (i == 0) {
				Gizmos.color = pos._speed < 0.05f ? Color.blue : pos._speed < 0.09f && pos._speed > 0.05f ? Color.yellow : Color.red;
				Gizmos.DrawSphere (pos._position, 0.05f);
			} else {
				Gizmos.color = InputScript._mousePositions [i - 1]._angle - pos._angle > 20 ? Color.white : pos._speed < 0.05f ? Color.blue : pos._speed < 0.09f && pos._speed > 0.05f ? Color.yellow : Color.red;
				Gizmos.DrawSphere (pos._position, 0.05f);
			}
		}
		//draw the bezier normals
		if (InputScript._mousePositions.Count > 0 && BezierScene._canUseValues) {
			
		}
	}

}