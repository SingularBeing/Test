using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputScript : MonoBehaviour
{

	public static List<MousePosition> _mousePositions = new List<MousePosition> ();
	public static List<MousePosition> _magneticPositions = new List<MousePosition> ();

	public float lastEdit;
	public bool startTime;

	void OnDisable ()
	{
		_mousePositions.Clear ();
	}

	public List<MousePosition> GetMagnetics ()
	{
		return _magneticPositions;
	}

	void Update ()
	{
		if (Input.GetMouseButtonDown (0)) {
			BezierScene._canUseValues = false;
			_mousePositions.Clear ();
		}

		if (Input.GetMouseButton (0)) {
			startTime = true;
			//mouse button down
			Vector3 currentMousePosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			if (_mousePositions.Count == 0) {
				_mousePositions.Add (new MousePosition (currentMousePosition, lastEdit, 0, 0));
			}

			if (!ContainsPosition (currentMousePosition)) {
				//check for the distance between the points
				if (Vector3.Distance (_mousePositions [_mousePositions.Count - 1]._position, currentMousePosition) > 0.5f) {
					float y1 = _mousePositions [_mousePositions.Count - 1]._position.y, y2 = currentMousePosition.y;
					float x1 = _mousePositions [_mousePositions.Count - 1]._position.x, x2 = currentMousePosition.x;

					float slope = (y1 - y2) / (x1 - x2);

					_mousePositions.Add (new MousePosition (currentMousePosition, lastEdit, Vector3.Angle (_mousePositions [_mousePositions.Count - 1]._position, currentMousePosition), slope));
				}
				/*if (Vector3.Distance (_mousePositions [_mousePositions.Count - 1], currentMousePosition) > 1) {
					_mousePositions.Add (currentMousePosition);
				}*/
			}
		}

		if (startTime) {
			//start the timer
			lastEdit += 0.01f;
		} else {
			lastEdit = 0;
		}

		if (Input.GetMouseButtonUp (0)) {
			startTime = false;
			EditZValues ();
			Debug.Log ("Count1:" + _mousePositions.Count);
			BezierScene.CalculateMagneticPath ();
			BezierScene._canUseValues = true;
			Debug.Log ("Count2:" + _mousePositions.Count);
		}
	}

	public static void ResetMagneticPositions (List<MousePosition> pos)
	{
		_magneticPositions.Clear ();
		if (_magneticPositions.Count > 0)
			Debug.Log ("HOW THE HELL");

		_magneticPositions = new List<MousePosition> (pos);

		Debug.Log ("Mouses:" + _mousePositions.Count + ", Magnetics:" + _magneticPositions.Count);
	}

	bool ContainsPosition (Vector3 _pos)
	{
		foreach (MousePosition pos in _mousePositions) {
			if (pos._position == _pos)
				return true;
		}
		return false;
	}

	void EditZValues ()
	{
		_magneticPositions.Clear ();

		for (int i = 0; i < _mousePositions.Count; i++) {
			_mousePositions [i]._position = new Vector3 (_mousePositions [i]._position.x, _mousePositions [i]._position.y, 0);
		}

		foreach (MousePosition p in _mousePositions) {
			_magneticPositions.Add (p);
		}

		Debug.Log ("Mouses:" + _mousePositions.Count + ", Magnetics:" + _magneticPositions.Count);
	}

}

[System.Serializable]
public class MousePosition
{
	public Vector3 _position;
	public float _speed;
	public float _angle;
	public float _slope;

	public MousePosition (Vector3 _v, float _f, float _a, float _s)
	{
		_position = _v;
		_speed = _f;
		_angle = _a;
		_slope = _s;
	}
}