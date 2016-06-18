using UnityEngine;
using System.Collections;

public class StillPositionBAll : MonoBehaviour
{

	public BezierObj _bezier;

	public float _t;

	void Update ()
	{
		if (_bezier != null) {
			Vector3 vec = _bezier.GetPointAtTime (_t);
			transform.position = vec;
		}
	}

}