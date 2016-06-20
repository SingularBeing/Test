using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// Used for all input calculations
/// </summary>
public class InputScript : MonoBehaviour
{
	/// <summary>
	/// All of the mouse positions when creating a path
	/// </summary>
	public static List<MousePosition> _mousePositions = new List<MousePosition> ();
	/// <summary>
	/// All of the magnetic positions (not currently used)
	/// </summary>
	public static List<MousePosition> _magneticPositions = new List<MousePosition> ();
	/// <summary>
	/// The last edit time (used for fast mouse movement)
	/// </summary>
	public float lastEdit;
	/// <summary>
	/// We can add to the last edit
	/// </summary>
	public bool startTime;
	/// <summary>
	/// The current red ball
	/// </summary>
	public RedBall ball;
	/// <summary>
	/// The line renderers for the main line (used) and the magnetic line (not currently used)
	/// </summary>
	public LineRenderer _line, _magneticLine;
	/// <summary>
	/// The text in the game view that shows how many nodes there are
	/// </summary>
	public Text _nodeAmount;

	/// <summary>
	/// Raises the disable event.
	/// </summary>
	void OnDisable ()
	{
		//clear the current positions so there are no lingering static paths
		_mousePositions.Clear ();
		_magneticPositions.Clear ();
	}

	/// <summary>
	/// Gets the magnetics list.
	/// </summary>
	/// <returns>The magnetics list.</returns>
	public List<MousePosition> GetMagnetics ()
	{
		//return the magnetic positions list
		return _magneticPositions;
	}

	/// <summary>
	/// Update this instance.
	/// </summary>
	void Update ()
	{
		//update the node text
		_nodeAmount.text = "Nodes :" + _mousePositions.Count;
		//check for the mouse down click event
		if (Input.GetMouseButtonDown (0)) {
			//now we cannot use the bezier values until done
			BezierScene._canUseValues = false;
			//clear the last mouse positions
			_mousePositions.Clear ();
			//the red ball now can not move until done
			RedBall._canMove = false;
			//reset the line renderers
			GameObject[] lineRenderer = GameObject.FindGameObjectsWithTag ("LineRenderer");
			foreach (GameObject g in lineRenderer) {
				Destroy (g);
			}
			BezierScene.allLineRenderers.Clear ();
		}
		//check for the mouse down event
		if (Input.GetMouseButton (0)) {
			//we can start on the edit time
			startTime = true;
			//set the current mouse position
			Vector3 currentMousePosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			//if there are no mouse positions currently, add one
			if (_mousePositions.Count == 0) {
				_mousePositions.Add (new MousePosition (currentMousePosition, lastEdit, 0, 0));
			}
			//if the current list of mouse positions does not contain the current mouse position,
			//calculate values and add this mouse position to the list
			if (!ContainsPosition (currentMousePosition)) {
				//check for the distance between the points
				if (Vector3.Distance (_mousePositions [_mousePositions.Count - 1]._position, currentMousePosition) > 0.5f) {
					//used for the slope
					float y1 = _mousePositions [_mousePositions.Count - 1]._position.y, y2 = currentMousePosition.y;
					float x1 = _mousePositions [_mousePositions.Count - 1]._position.x, x2 = currentMousePosition.x;
					//slope: (y2-y1)/(x2-x1)
					float slope = (y2 - y1) / (x2 - x1);
					//add the mouse position to the main list
					_mousePositions.Add (new MousePosition (currentMousePosition, lastEdit, Vector3.Angle (_mousePositions [_mousePositions.Count - 1]._position, currentMousePosition), slope));
				}
			}
		}
		//if we are checking for the new point position, start adding time
		if (startTime) {
			//add time to the point creation
			lastEdit += 0.01f;
		} 
		//if we are not checking anymore, set the time to 0
		else {
			//set the time to 0
			lastEdit = 0;
		}
		//check for the mouse up click event
		if (Input.GetMouseButtonUp (0)) {
			//disable the start time
			startTime = false;
			//modify the Z values to make them all 0 (so that the paths are not invisible)
			EditZValues ();
			//calculate the magnetic path (currently semi-working)
			BezierScene.CalculateMagneticPath ();
			//set the magnetic positions from a new list, used for a weird bug that would edit the value
			//of the previous list from the new list.
			AllPositions ();
			//we can now use the bezier values
			BezierScene._canUseValues = true;
			//the ball can now move
			RedBall._canMove = true;
		}
	}

	/// <summary>
	/// Set the new list of positions, also create the line renderers
	/// </summary>
	public void AllPositions ()
	{
		if (_line != null) {
			//set the current line renderer's vertex count
			_line.SetVertexCount (_mousePositions.Count);
			//
			for (int i = 0; i < _mousePositions.Count; i++) {
				//get the current index of MousePosition from the _mousePosition list
				MousePosition p = _mousePositions [i];
				//add a new vertex to the line renderer
				_line.SetPosition (i, p._position);
			}
			//set the current line renderer's vertex count
			_magneticLine.SetVertexCount (_magneticPositions.Count);
			//
			for (int i = 0; i < _magneticPositions.Count; i++) {
				//get the current index of MousePosition from the _magneticPositions list
				MousePosition p = _magneticPositions [i];
				//add a new vertex to the line renderer
				_magneticLine.SetPosition (i, p._position);
			}
		}
	}

	/// <summary>
	/// Resets the magnetic positions.
	/// </summary>
	public static void ResetMagneticPositions ()
	{
		//reset the magnetic position list
		_magneticPositions.Clear ();
		//if the list is greater than 0 after we reset it, we have an issue... :P
		if (_magneticPositions.Count > 0)
			Debug.LogError ("Magnetic Positions is SOMEHOW greater than 0 AFTER we reset it...");
	}

	/// <summary>
	/// Does the current list of positions contain the given position?
	/// </summary>
	/// <returns><c>true</c>, if position was contained, <c>false</c> otherwise.</returns>
	/// <param name="_pos">Position.</param>
	bool ContainsPosition (Vector3 _pos)
	{
		//check each position for this position
		foreach (MousePosition pos in _mousePositions) {
			//if true, return true
			if (pos._position == _pos)
				return true;
		}
		//if the list does not contain the current position, return false
		return false;
	}

	/// <summary>
	/// Edit the Z values of the mouse positions to be all 0
	/// </summary>
	void EditZValues ()
	{
		//clear the magnetic positions
		ResetMagneticPositions ();
		//go through each position and set the z value to 0
		for (int i = 0; i < _mousePositions.Count; i++) {
			_mousePositions [i]._position = new Vector3 (_mousePositions [i]._position.x, _mousePositions [i]._position.y, 0);
		}
	}

}

[System.Serializable]
/// <summary>
/// The holder for the mouse information:
/// Mouse position, mouse speed, current angle, and current slope
/// </summary>
public class MousePosition
{
	/// <summary>
	/// The position.
	/// </summary>
	public Vector3 _position;
	/// <summary>
	/// The speed.
	/// </summary>
	public float _speed;
	/// <summary>
	/// The angle.
	/// </summary>
	public float _angle;
	/// <summary>
	/// The slope.
	/// </summary>
	public float _slope;

	/// <summary>
	/// Initializes a new instance of the <see cref="MousePosition"/> class.
	/// </summary>
	/// <param name="_v">Position.</param>
	/// <param name="_f">Speed.</param>
	/// <param name="_a">Angle.</param>
	/// <param name="_s">Slope.</param>
	public MousePosition (Vector3 _pos, float _spd, float _ang, float _slo)
	{
		_position = _pos;
		_speed = _spd;
		_angle = _ang;
		_slope = _slo;
	}
}