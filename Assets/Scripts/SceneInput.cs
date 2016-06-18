using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneInput : MonoBehaviour
{
	public RedBall _redBall;
	public StillPositionBAll _still;

	public List<Vector3> m_MousePositions = new List<Vector3> ();
	public List<BezierPoint> m_Points = new List<BezierPoint> ();
	public BezierObj _bezier;

	private Vector3 mousePosition;
	public Vector3 _startPoint, _endPoint;
	public Vector3 _startTangent, _endTangent;

	public Vector3 _outStartTangent, _outEndTangent;



	void Update ()
	{
		

		//check for the left mouse button
		if (Input.GetMouseButtonDown (0)) {
			m_MousePositions.Clear ();
		}
		if (Input.GetMouseButton (0)) {
			//we are in the touch phase
			Vector3 transformedPosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			mousePosition = new Vector3 (transformedPosition.x, transformedPosition.y, 0);
			if (!m_MousePositions.Contains (mousePosition)) {
				m_MousePositions.Add (mousePosition);
			}
		} 
		if (Input.GetMouseButtonUp (0)) {
			//Debug.Log (m_MousePositions.Count);
			/*for (int i = 0; i < m_MousePositions.Count; i++) {
				m_MousePositions [i] = transform.InverseTransformPoint (m_MousePositions [i]);
			}*/
			CalculateAllPoints ();
		}
	}

	void OnDrawGizmos ()
	{
		Gizmos.color = Color.red;
		if (m_MousePositions.Count > 0) {
			foreach (Vector3 _position in m_MousePositions) {
				Gizmos.DrawSphere (_position, 0.05f);
			}

			Gizmos.color = Color.green;
			if (m_Points.Count > 0) {
				foreach (BezierPoint point in m_Points) {
					Gizmos.color = point.distortion > 0 ? Color.black : Color.green;
					Gizmos.DrawSphere (point.position, 0.065f);
				}
			}

			//draw the points
			Gizmos.color = Color.yellow;
			Gizmos.DrawSphere (_startPoint, 0.06f);
			Gizmos.DrawSphere (_endPoint, 0.06f);
			/*Gizmos.color = Color.yellow;
			Gizmos.DrawSphere (_startTangent, 0.06f);
			Gizmos.DrawSphere (_endTangent, 0.06f);*/

			Gizmos.color = Color.cyan;
			Gizmos.DrawSphere (actualPositionOnSpline, 0.07f);
		}
	}

	Vector3 actualPositionOnSpline;

	/*void DeleteSomePoints ()
	{
		for (int i = 0; i < m_MousePositions.Count; i++) {
			if (i + 1 == m_MousePositions.Count - 1 || i == 0 || i == m_MousePositions.Count - 1)
				continue;

			//Debug.Log ("DIS:" + Vector3.Distance (m_MousePositions [i], m_MousePositions [i + 1]));
			if (Vector3.Distance (m_MousePositions [i], m_MousePositions [i + 1]) < 0.2f) {
				m_MousePositions.RemoveAt (i);
			}
		}
	}*/

	void CalculateAllPoints ()
	{
		//grab end points
		_startPoint = m_MousePositions [0];
		_endPoint = m_MousePositions [m_MousePositions.Count - 1];
		//DeleteSomePoints ();
		//grab handle points
		//check to see how many points there are
		int _pointAmount = m_MousePositions.Count;
		//find an average
		int _startTangent = (int)(_pointAmount * 0.3f);
		int _endTangent = _pointAmount - _startTangent;
		this._startTangent = m_MousePositions [_startTangent];
		this._endTangent = m_MousePositions [_endTangent];

		//check to see if the two points are positive or negative
		//grab the slope
		float x1 = _startPoint.x, x2 = _endPoint.x;
		float y1 = _startPoint.y, y2 = _endPoint.y;
		float slope = (y1 - y2) / (x1 - x2);

		Debug.Log (slope);
		//if the slope is negative, it is going downward. This means we need to move to the right

		float timeDif = 1f / _startTangent;
		_still._t = timeDif;

		CalculateSpeedValues ();

		//Debug.Log (string.Format ("StartPos: {0}, EndPos: {1}, StartTang: {2}, EndTang: {3}", _startPoint, _endPoint, _startTangent, _endTangent));
		_bezier = new BezierObj (this._startPoint, this._startTangent, this._endTangent, this._endPoint);
		_redBall._bezier = _still._bezier = _bezier;

		GenerateEqualDistancePoints ();
	}

	void CalculateSpeedValues ()
	{
		//check between each point, if over a max threshold, set the speed equivalent
		for (int i = 0; i < m_MousePositions.Count; i++) {
			if (i + 1 == m_MousePositions.Count - 1 || i == 0 || i == m_MousePositions.Count - 1)
				continue;

			float _mD = Vector3.Distance (m_MousePositions [i], m_MousePositions [i + 1]);
			if (_mD < 0.2f) {
				Debug.Log ("UNDER 0.2");
				//set the distance accordingly
				//find the nearest node
				float dis = 10000f;
				int currentIndex = -1;
				for (int x = 0; x < m_Points.Count; x++) {
					float _d = Vector3.Distance (m_MousePositions [i + 1], m_Points [x].position);
					if (_d < dis) {
						currentIndex = x;
						dis = _d;
					}
				}

				if (currentIndex != -1) {
					Debug.Log ("Pointme: " + currentIndex);
					m_Points [currentIndex].distortion = 1;
					Debug.Log ("Speed:" + m_Points [currentIndex].distortion);
				}
			}
		}
	}

	void GenerateEqualDistancePoints ()
	{
		m_Points.Clear ();
		int startGreenPos = (int)(m_MousePositions.Count / 6.5f);
		for (int i = 0; i < 6; i++) {
			m_Points.Add (new BezierPoint (0, m_MousePositions [startGreenPos * (i + 1)]));
		}
	}

	void CheckMouseMovement ()
	{
		//check each point for the mouse movement. If a point is farther away, it takes a faster time to get there
		//which indicates a magnetic effect
		for (int i = 0; i < m_MousePositions.Count; i++) {
			//check the current position and the one in front of it
			if (i == m_MousePositions.Count - 1)
				break;
			
		}
	}

	/*public Vector3 GetVelocity (float t)
	{
		//Debug.Log (_bezier.GetFirstDerivative (m_MousePositions [0], m_MousePositions [1], m_MousePositions [2], t));
		return transform.TransformPoint (_bezier.GetFirstDerivative (m_MousePositions [0], m_MousePositions [1], m_MousePositions [2], t)) - transform.position;
	}

	public Vector3 GetPoint (float t)
	{
		return transform.TransformPoint (_bezier.GetPoint (m_MousePositions [0], m_MousePositions [1], m_MousePositions [2], t));
	}*/

}

public class BezierPoint
{
	public float distortion;
	public Vector3 position;

	public BezierPoint (float _s, Vector3 _p)
	{
		this.distortion = _s;
		this.position = _p;
	}
}