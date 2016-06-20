using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Handles all of the Red Ball movement
/// </summary>
public class RedBall : MonoBehaviour
{
	/// <summary>
	/// The default bezier curve (Not the magnetic path)
	/// </summary>
	public BezierScene _bezier;
	/// <summary>
	/// Used to speed up the ball (Can lead to 'interesting' results)
	/// </summary>
	public float _speedEnhancer = 0f;

	/// <summary>
	/// The current time frame (0.0 - 1.0)
	/// </summary>
	public static float _t;
	/// <summary>
	/// Can the ball move?
	/// </summary>
	public static bool _canMove;
	/// <summary>
	/// Are we following the magnetic path?
	/// </summary>
	public static bool followMagneticField;
	/// <summary>
	/// The magnetic path positions.
	/// </summary>
	public static List<Vector3> magPos = new List<Vector3> ();
	/// <summary>
	/// The points:
	/// p1 = Point 1,
	/// p2 = Handle 1,
	/// p3 = Handle 2,
	/// p4 = Point 2
	/// </summary>
	public static Vector3 p1, p2, p3, p4;
	/// <summary>
	/// The locations of all fast mouse movement areas
	/// </summary>
	public List<MousePosition> _tempMP = new List<MousePosition> ();
	/// <summary>
	/// The current 'scene' for the main Bezier curve
	/// </summary>
	public BezierScene scene;
	/// <summary>
	/// This
	/// </summary>
	public static RedBall me;
	/// <summary>
	/// The distance from this ball to the end point of the main Bezier curve.
	/// Used when on the magnetic path for knowing when it has reached the end
	/// (time scale gets weird)
	/// </summary>
	public float _distance;
	/// <summary>
	/// A visual boolean for following the magnetic bezier curve
	/// </summary>
	public bool followsMagnetPath;
	/// <summary>
	/// A backup of the followsMagnetPath
	/// </summary>
	private bool bFMP;

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start ()
	{
		//set the current instance of this
		me = this;
		//just a precaution, grab all of the fast moving nodes
		GrabFastPoints ();
	}

	/// <summary>
	/// Raises the disable event.
	/// </summary>
	void OnDisable ()
	{
		//reset the magnetic positions list
		magPos.Clear ();
		//reset the movement
		_canMove = false;
		//reset the following of the magnetic curve
		followMagneticField = false;
	}

	/// <summary>
	/// Update this instance.
	/// </summary>
	void Update ()
	{
		if (bFMP != followsMagnetPath) {
			bFMP = followsMagnetPath;
			followMagneticField = bFMP;
		}
		//if the fast moving nodes are greater than 0:
		if (magPos.Count > 0) {
			//check the distance from the ball to the last position on the original bezier curve
			_distance = Vector3.Distance (transform.position, InputScript._mousePositions [InputScript._mousePositions.Count - 1]._position);
			//if the distance is really close, reset the ball
			if (_distance < 0.1f) {
				//reset the time
				_t = 0;
				//reset the fast points
				GrabFastPoints ();
				//reset the magnetic field following
				followMagneticField = false;
			}
		}
		//if we can move and we can use the bezier values, perform position changing
		if (_canMove && BezierScene._canUseValues) {
			//if we are near a fast node, change paths
			/*if (NearFastMovement ()) {
				//we are now following the magnetic path
				followMagneticField = true;
			}*/
			//add to the current time plus the speed enhancer
			_t += 0.001f + _speedEnhancer;
			//if the time is greater than 1f, reset
			if (_t > 1f) {
				//reset the time
				_t = 0f;
				//reset the fast nodes
				GrabFastPoints ();
				//reset the magnetic path following
				followMagneticField = false;
			}
			//move the ball accordingly
			Vector3 vec = _bezier.transform.InverseTransformPoint (followMagneticField ? BezierScene.GetMagPoint (_t) : BezierScene.GetPoint (_t));
			transform.position = vec;
		}
	}

	/// <summary>
	/// Is the ball near a fast movement? If so, we start moving on the magnetic path.
	/// </summary>
	/// <returns><c>true</c>, if fast movement was neared, <c>false</c> otherwise.</returns>
	bool NearFastMovement ()
	{
		//Set all of the fast mouse moving nodes
		GrabFastPoints ();
		//for each node in the fast moving nodes, check the distance of the
		//ball to that node. If we are close, start moving on the magnetic 
		//bezier curve
		foreach (MousePosition op in _tempMP) {
			//if the distance is less than [set amount], return true
			if (Vector3.Distance (transform.position, op._position) < 0.1f) {
				return true;
			}
		}
		//we are not near any fast nodes
		return false;
	}

	/// <summary>
	/// Set all of the fast mouse moving nodes
	/// </summary>
	void GrabFastPoints ()
	{
		//make a new list of fast points
		_tempMP = new List<MousePosition> ();
		//go through all of the original mouse position nodes to find the
		//faster nodes
		for (int i = 0; i < InputScript._mousePositions.Count; i++) {
			//get the node
			MousePosition pos = InputScript._mousePositions [i];
			//if the node's internal speed is less then [set amount], then
			//add it to the list
			if (pos._speed < 0.05f) {
				//add the node to the list
				_tempMP.Add (pos);
			}
		}
	}

	/// <summary>
	/// Create the path line up for the magnetic nodes
	/// </summary>
	public static void CreateMagneticNodes ()
	{
		//clears the original list
		magPos.Clear ();
		//go through the time scale 1f and set the nodes equally
		for (float i = 0; i <= 1; i += 0.0833f) {
			//add the position on the bezier curve
			magPos.Add (BezierScene.GetPoint (p1, p2, p3, p4, i));
		}
	}

}