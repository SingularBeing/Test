using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneInput : MonoBehaviour
{

	public List<Vector3> m_MousePositions = new List<Vector3> ();

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

			//draw the points
			Gizmos.color = Color.green;
			Gizmos.DrawSphere (_startPoint, 0.06f);
			Gizmos.DrawSphere (_endPoint, 0.06f);
			Gizmos.color = Color.yellow;
			Gizmos.DrawSphere (_startTangent, 0.06f);
			Gizmos.DrawSphere (_endTangent, 0.06f);
		}
	}

	void CalculateAllPoints ()
	{
		//grab end points
		_startPoint = m_MousePositions [0];
		_endPoint = m_MousePositions [m_MousePositions.Count - 1];
		//grab handle points
		//check to see how many points there are
		int _pointAmount = m_MousePositions.Count;
		//find an average
		int _startTangent = (int)(_pointAmount * 0.3f);
		int _endTangent = _pointAmount - _startTangent;
		this._startTangent = m_MousePositions [_startTangent];
		this._endTangent = m_MousePositions [_endTangent];
		this._outStartTangent = this._startTangent * 2;
		this._outStartTangent.Normalize ();
		this._outEndTangent = this._endTangent * 2;
		this._outEndTangent.Normalize ();
		//Debug.Log (string.Format ("StartPos: {0}, EndPos: {1}, StartTang: {2}, EndTang: {3}", _startPoint, _endPoint, _startTangent, _endTangent));

	}

}