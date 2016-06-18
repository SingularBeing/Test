using UnityEngine;
using System.Collections;
using UnityEditor;

public class SimpleCurve : MonoBehaviour
{

	public BezierObj m_BezierObj;
	float m_T = 0f;

	public void CalculateNew ()
	{
		m_BezierObj = new BezierObj (new Vector3 (0, 0), new Vector3 (0, 2), new Vector3 (4, 2), new Vector3 (4, 0));
		m_BezierObj.CalculatePoints (6, 6);
	}

	void Update ()
	{
		
	}

	void OnDrawGizmos ()
	{
		Gizmos.color = Color.green;
		if (m_BezierObj != null) {
			foreach (Vector3 _position in m_BezierObj.points) {
				//	Vector3 _fixedPosition = transform.TransformPoint (_position);
				Gizmos.DrawSphere (_position, 0.05f);
			}

		}
	}

}