using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Security.Cryptography.X509Certificates;

/// <summary>
/// This script handle ALL of the bezier methods
/// </summary>
public class BezierScene : MonoBehaviour
{

	//Turntable Formula:
	//radius_turnTableFrame_(time) = (speed*time) (cos(angle-(rotation*time)), sin(angle-(rotation*time)));

	/// <summary>
	/// This
	/// </summary>
	public static Transform me;
	/// <summary>
	/// The positions.
	/// </summary>
	public List<MousePosition> positions;
	/// <summary>
	/// The magnetic positions
	/// </summary>
	public List<MousePosition> Gpositions;
	/// <summary>
	/// The current input script
	/// </summary>
	public InputScript input;
	/// <summary>
	/// The current middle point
	/// </summary>
	public static Vector3 middlePoint;
	/// <summary>
	/// The current line splines (split the main curve into multiple)
	/// </summary>
	public static List<MousePosition[]> LineSplines = new List<MousePosition[]> ();
	/// <summary>
	/// The default line prefab.
	/// </summary>
	public GameObject defaultLinePrefab;
	/// <summary>
	/// All line renderers.
	/// </summary>
	public static List<LineAndInfo> allLineRenderers = new List<LineAndInfo> ();
	/// <summary>
	/// The red ball.
	/// </summary>
	public GameObject redBall;
	/// <summary>
	/// The red ball as a static member.
	/// </summary>
	public static GameObject redBallStatic;
	/// <summary>
	/// The line renderer prefab as a static member.
	/// </summary>
	private static GameObject useLine;

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start ()
	{
		//clear all previous line renderers
		allLineRenderers.Clear ();
		//set the static member of the line renderer prefab
		useLine = defaultLinePrefab;
		//set the static member of the ball
		redBallStatic = redBall;
		//set the current instance on this
		me = transform;
	}

	/// <summary>
	/// Update this instance.
	/// </summary>
	void Update ()
	{
		//set the current positions (visual purposes)
		positions = InputScript._mousePositions;
		//set the current magnetic positions (visual purposes)
		Gpositions = InputScript._magneticPositions;
	}

	/// <summary>
	/// Can we use this script's info?
	/// </summary>
	public static bool _canUseValues;

	/// <summary>
	/// Get a point from the bezier curve
	/// </summary>
	/// <returns>The point.</returns>
	/// <param name="t">Time.</param>
	public static Vector3 GetPoint (float t)
	{
		//current index
		int i;
		//if the time is greater than 1, reset
		if (t >= 1f) {
			t = 1f;
			//rewind the index
			i = InputScript._mousePositions.Count - 3;
		} else {
			//clamp the time to 0 or 1 * the amount of curves currently in place
			t = Mathf.Clamp01 (t) * CurveCount;
			//set the current index
			i = (int)t;
			//set the current time minus the current index
			t -= i;
			//set the current index 3 times greater
			i *= 3;
		}
		//create a new BezierModel, used for a storing class
		BezierModel model = new BezierModel (InputScript._mousePositions [i]._position, InputScript._mousePositions [i + 1]._position, InputScript._mousePositions [i + 2]._position, InputScript._mousePositions [i + 3]._position);
		//return the point on the bezier curve
		return me.TransformPoint (BezierScene.GetPoint (model.p0, model.p1, model.p2, model.p3, t));
	}

	/// <summary>
	/// Gets the a point on the magnetic bezier curve. Can sometimes break.
	/// </summary>
	/// <returns>The magnetic bezier point.</returns>
	/// <param name="t">Time.</param>
	public static Vector3 GetMagPoint (float t)
	{
		//current index
		int i = 0;

		//if the time is greater than 1, reset
		if (t >= 1f) {
			t = 1f;
			//rewind the index
			i = RedBall.magPos.Count - 3;
		} else {
			//clamp the time to 0 or 1 * the amount of curves currently in place
			t = Mathf.Clamp01 (t) * CurveCount;
			//set the current index
			i = (int)t;
			//set the current time minus the current index
			t -= i;
			//set the current index 3 times greater
			i *= 3;
		}
		//create a new BezierModel, used for a storing class
		BezierModel model = new BezierModel (RedBall.magPos [i], RedBall.magPos [i + 1], RedBall.magPos [i + 2], RedBall.magPos [i + 3]);
		//return the point on the bezier curve
		return me.TransformPoint (BezierScene.GetPoint (model.p0, model.p1, model.p2, model.p3, t));
	}

	/// <summary>
	/// Get a point on the specified bezier curve at the specified time
	/// </summary>
	/// <returns>The point.</returns>
	/// <param name="p0">Point 0: Starting point.</param>
	/// <param name="p1">Point 1: Handle 1.</param>
	/// <param name="p2">Point 2: Handle 2.</param>
	/// <param name="p3">Point 3: Ending point.</param>
	/// <param name="t">Time.</param>
	public static Vector3 GetPoint (Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
	{
		//clamp the point between 0 and 1
		t = Mathf.Clamp01 (t);
		//minus 1 from time
		float oneMinusT = 1f - t;
		//the rest SOMEHOW works. seriously
		return
			oneMinusT * oneMinusT * oneMinusT * p0 +
		3f * oneMinusT * oneMinusT * t * p1 +
		3f * oneMinusT * t * t * p2 +
		t * t * t * p3;
	}

	public static void CalculateMagneticPath ()
	{
		//grab all of the fast speeds
		List<MousePosition> _tempMP = new List<MousePosition> ();
		List<int> indexes = new List<int> ();
		for (int i = 0; i < InputScript._magneticPositions.Count; i++) {
			//grab the current index of the magnetic positions
			MousePosition pos = InputScript._magneticPositions [i];
			//if the speed is less than [set amount], add the point to the list
			if (pos._speed < 0.05f) {
				//add to the list
				_tempMP.Add (pos);
				//add the index to the list
				indexes.Add (i);
			}
		}

		//Set the ball's bezier locations of the magnetic bezier curve
		//starting point
		RedBall.p1 = InputScript._mousePositions [0]._position;
		//handle 1
		RedBall.p2 = me.transform.InverseTransformPoint (BezierScene.GetPoint (0.25f)) + (me.transform.InverseTransformPoint (BezierScene.GetPoint (0.5f)) / 2) * 7f;
		//handle 2
		RedBall.p3 = me.transform.InverseTransformPoint (BezierScene.GetPoint (0.75f)) + (me.transform.InverseTransformPoint (BezierScene.GetPoint (0.5f)) / 2) * 7f;
		//end point
		RedBall.p4 = InputScript._mousePositions [InputScript._mousePositions.Count - 1]._position;

		//now that we have them, we need to expand the path at this node
		//and then make it last for a few more
		//split the line into multiple splines

		//make a new list of mouse position arrays
		List<MousePosition[]> split = new List<MousePosition[]> ();
		//grab the amount of splits we can do
		int splitIndex = (int)(InputScript._mousePositions.Count / 4);
		//go through each mouse position and add the spline to the list of splines
		for (int i = 0; i < InputScript._mousePositions.Count; i += splitIndex) {
			//make a new list of positions
			List<MousePosition> po = new List<MousePosition> ();
			//create a new line from the prefab
			GameObject newLine = (GameObject)Instantiate (useLine);
			//set the name to one more noticable
			newLine.name = "LineRend (" + (allLineRenderers.Count - 1) + ")";
			//get the line renderer component from the object
			LineRenderer rend = newLine.GetComponent<LineRenderer> ();
			//add the renderer to the list
			allLineRenderers.Add (new LineAndInfo (rend, 0));
			//while under the split index, set the vertexs
			if (i != (i + splitIndex)) {
				//set the vertex count
				allLineRenderers [allLineRenderers.Count - 1]._rend.SetVertexCount (splitIndex);
				//for each index, set the vertex
				for (int h = 0; h < splitIndex; h++) {
					//add the vertex to the line's list
					po.Add (new MousePosition (InputScript._mousePositions [i + h]._position, InputScript._mousePositions [i + h]._slope, InputScript._mousePositions [i + h]._angle, InputScript._mousePositions [i + h]._speed));
					//set the vertex on the renderer
					allLineRenderers [allLineRenderers.Count - 1]._rend.SetPosition (h, po [po.Count - 1]._position);
				}
			}

			//set the positions
			allLineRenderers [allLineRenderers.Count - 1].positions = po.ToArray ();
			//grab the angle
			allLineRenderers [allLineRenderers.Count - 1]._angle = Vector3.Angle (new Vector3 (po [0]._position.x, po [0]._position.y, 0), new Vector3 (po [po.Count - 1]._position.x, po [po.Count - 1]._position.y, 0));

			//check the current slope
			float y1 = po [0]._position.y, y2 = po [po.Count - 1]._position.y;
			float x1 = po [0]._position.x, x2 = po [po.Count - 1]._position.x;
			float currentSlope = (y2 - y1) / (x2 - x1);
			//set the line's slope to the current slope
			allLineRenderers [allLineRenderers.Count - 1]._slope = currentSlope;

			//make a new instance of the line material
			Material tempMat = new Material (allLineRenderers [allLineRenderers.Count - 1]._rend.material);

			//if we have a positive slope:
			if (currentSlope > 0) {
				//check if the angle is less than 45
				if (allLineRenderers [allLineRenderers.Count - 1]._angle < 45) {
					tempMat.color = Color.blue;
				} 
				//check if the angle is greater than 45
				else if (allLineRenderers [allLineRenderers.Count - 1]._angle > 45) {
					tempMat.color = Color.green;
				}
			}
			//if we have a negative slope:
			else if (currentSlope < 0) {
				//check if the angle is less than 45
				if (allLineRenderers [allLineRenderers.Count - 1]._angle < 45) {
					tempMat.color = Color.red;
				} 
				//check if the angle is greater than 45
				else if (allLineRenderers [allLineRenderers.Count - 1]._angle > 45) {
					tempMat.color = Color.cyan;
				}
			}
			//set the current material to the appropriate one
			allLineRenderers [allLineRenderers.Count - 1]._rend.material = tempMat;
			//add this spline to the list
			split.Add (po.ToArray ());
		}
		//create the magnetic nodes on the ball (The magnetic spline)
		RedBall.CreateMagneticNodes ();
		//create the magnetic line renderer
		GameObject magLine = (GameObject)Instantiate (useLine);
		//get the line renderer
		LineRenderer magRend = magLine.GetComponent<LineRenderer> ();
		//set the vertex count to the magnetic point count
		magRend.SetVertexCount (RedBall.magPos.Count);
		//for each point, add the vertex to the line renderer
		for (int i = 0; i < RedBall.magPos.Count; i++) {
			magRend.SetPosition (i, RedBall.magPos [i]);
		}

		//------------------------------//
		// The following is for testing //
		//----------------------------- //

		//check for simple shapes

		//get the slope
		/*float _0y = InputScript._mousePositions [0]._position.y;
		float _1y = InputScript._mousePositions [InputScript._mousePositions.Count - 1]._position.y;
		float _0x = InputScript._mousePositions [0]._position.x;
		float _1x = InputScript._mousePositions [InputScript._mousePositions.Count - 1]._position.x;
		float simpleRightCurveSlope = (_1y - _0y) / (_1x - _1x);
		//if the slope if positive
		if (simpleRightCurveSlope > 0) {
			//is linear to the right (up)
			//can setup force adding
			RedBall red = redBallStatic.GetComponent<RedBall> ();
			float redTime = RedBall._t;
			//check if there is a nearby force
			MousePosition closestForce = null;
			float closestDistance = 1000f;
			//set the closest distance to the closest position
			foreach (MousePosition p in _tempMP) {
				float distanceNow = Vector3.Distance (BezierScene.GetPoint (redTime), p._position);
				if (distanceNow < closestDistance) {
					closestDistance = distanceNow;
					closestForce = p;
				}
			}
			//once we have this, we can apply force


			//not finished
		}*/
	}

	/// <summary>
	/// Grab the index of a position on the line
	/// </summary>
	/// <returns>The index of the position on the line.</returns>
	/// <param name="posi">Position.</param>
	public static int PositionOnLine (Vector3 posi)
	{
		//for each mouse position, check if the position given is any of them
		for (int i = 0; i < InputScript._mousePositions.Count; i++) {
			//grab the mouse position of the current index
			MousePosition p = InputScript._mousePositions [i];
			//if the positions are the same, return the index
			if (p._position == posi) {
				return i;
			}
		}

		//if none are the same, return -1
		return -1;
	}

	/// <summary>
	/// Raises the draw gizmos event.
	/// </summary>
	void OnDrawGizmos ()
	{
		//set the color to green
		Gizmos.color = Color.green;
		//draw the bezier points
		for (int i = 0; i < InputScript._mousePositions.Count; i++) {
			//grab the current index of MousePosition
			MousePosition pos = InputScript._mousePositions [i];
			//if we are at the index of 0, don't check for the angle
			//if (i == 0) {
			//set the color based from the mouse speed
			Gizmos.color = pos._speed < 0.05f ? Color.blue : pos._speed < 0.09f && pos._speed > 0.05f ? Color.yellow : Color.red;
			//draw the sphere in-game
			Gizmos.DrawSphere (pos._position, 0.05f);
			//} 
		}
		//if the mouse positions are greater than 0 and we can use these values
		if (InputScript._mousePositions.Count > 0 && BezierScene._canUseValues) {
			//set the color to black
			Gizmos.color = Color.black;
			//draw a middle point on the bezier curve
			Gizmos.DrawSphere (middlePoint, 0.08f);	
		}
		//set the color to blue
		Gizmos.color = Color.blue;
		//go through each position in the ball's magnetic list
		foreach (Vector3 pos in RedBall.magPos) {
			//draw the magnetic position point on the magnetic bezier
			Gizmos.DrawSphere (pos, 0.08f);
		}
	}

	/// <summary>
	/// Gets the curve count.
	/// </summary>
	/// <value>The curve count.</value>
	public static int CurveCount {
		get {
			return (InputScript._mousePositions.Count - 1) / 3;
		}
	}

}

/// <summary>
/// Bezier model. Used for data storing.
/// </summary>
public class BezierModel
{
	public Vector3 p0;
	public Vector3 p1;
	public Vector3 p2;
	public Vector3 p3;

	/// <summary>
	/// Initializes a new instance of the <see cref="BezierModel"/> class.
	/// </summary>
	/// <param name="v0">Point 0: Starting point.</param>
	/// <param name="v1">Point 1: Handle 1.</param>
	/// <param name="v2">Point 2: Handle 2.</param>
	/// <param name="v3">Point 3: End point.</param>
	public BezierModel (Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3)
	{ 
		this.p0 = v0; 
		this.p1 = v1; 
		this.p2 = v2; 
		this.p3 = v3; 
	}

}

/// <summary>
/// A more in-depth line renderer data storage
/// </summary>
public class LineAndInfo
{
	//the current line renderer
	public LineRenderer _rend;
	//the current line's slope
	public float _slope;
	//the current line's angle
	public float _angle;
	//the current line's positions
	public MousePosition[] positions;

	/// <summary>
	/// Initializes a new instance of the <see cref="LineAndInfo"/> class.
	/// </summary>
	/// <param name="_r">Line renderer.</param>
	/// <param name="_s">Slope.</param>
	public LineAndInfo (LineRenderer _r, float _s)
	{
		_rend = _r;
		_slope = _s;
	}
}