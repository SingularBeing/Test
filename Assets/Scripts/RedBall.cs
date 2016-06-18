using UnityEngine;
using System.Collections;

public class RedBall : MonoBehaviour
{

	public BezierObj _bezier;

	float _t;

	void Update ()
	{
		if (_bezier != null) {
			Vector3 vec = _bezier.GetPointAtTime (_t);
			transform.position = vec;

			_t += 0.001f;
			if (_t > 1f)
				_t = 0f;
		}
	}

}