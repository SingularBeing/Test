using UnityEngine;

[System.Serializable] 
public class BezierObj : System.Object
{
	public Vector3 p0;
	public Vector3 p1;
	public Vector3 p2;
	public Vector3 p3;
	public float ti = 0f;
	private Vector3 b0 = Vector3.zero;
	private Vector3 b1 = Vector3.zero;
	private Vector3 b2 = Vector3.zero;
	private Vector3 b3 = Vector3.zero;
	private float Ax;
	private float Ay;
	private float Az;
	private float Bx;
	private float By;
	private float Bz;
	private float Cx;
	private float Cy;
	private float Cz;
	// Init function v0 = 1st point, v1 = handle of the 1st point , v2 = handle of the 2nd point, v3 = 2nd point
	// handle1 = v0 + v1
	// handle2 = v3 + v2
	public BezierObj (Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3)
	{ 
		this.p0 = v0; 
		this.p1 = v1; 
		this.p2 = v2; 
		this.p3 = v3; 
	}
	// 0.0 >= t <= 1.0
	public Vector3 GetPointAtTime (float t)
	{ 
		this.CheckConstant (); 
		float t2 = t * t; 
		float t3 = t * t * t; 
		float x = this.Ax * t3 + this.Bx * t2 + this.Cx * t + p0.x; 
		float y = this.Ay * t3 + this.By * t2 + this.Cy * t + p0.y; 
		float z = this.Az * t3 + this.Bz * t2 + this.Cz * t + p0.z; 
		return new Vector3 (x, y, z); 
	}

	private void SetConstant ()
	{ 
		this.Cx = 3f * ((this.p0.x + this.p1.x) - this.p0.x); 
		this.Bx = 3f * ((this.p3.x + this.p2.x) - (this.p0.x + this.p1.x)) - this.Cx; 
		this.Ax = this.p3.x - this.p0.x - this.Cx - this.Bx; 
		this.Cy = 3f * ((this.p0.y + this.p1.y) - this.p0.y); 
		this.By = 3f * ((this.p3.y + this.p2.y) - (this.p0.y + this.p1.y)) - this.Cy; 
		this.Ay = this.p3.y - this.p0.y - this.Cy - this.By; 
		this.Cz = 3f * ((this.p0.z + this.p1.z) - this.p0.z); 
		this.Bz = 3f * ((this.p3.z + this.p2.z) - (this.p0.z + this.p1.z)) - this.Cz; 
		this.Az = this.p3.z - this.p0.z - this.Cz - this.Bz; 
	}
	// Check if p0, p1, p2 or p3 have changed
	public void CheckConstant ()
	{ 
		if (this.p0 != this.b0 || this.p1 != this.b1 || this.p2 != this.b2 || this.p3 != this.b3) { 
			this.SetConstant (); 
			this.b0 = this.p0; 
			this.b1 = this.p1; 
			this.b2 = this.p2; 
			this.b3 = this.p3; 
		} 
	}

	float length = 0;
	public Vector3[] points;

	//where _num is the desired output of points and _precision is how good we want matching to be
	public void CalculatePoints (int _num, int _precision = 100)
	{
		if (_num > _precision)
			Debug.LogError ("_num must be less than _precision");

		//calculate the length using _precision to give a rough estimate, save lengths in array
		length = 0;
		//store the lengths between PointsAtTime in an array
		float[] arcLengths = new float[_precision];

		Vector3 oldPoint = GetPointAtTime (0);

		for (int p = 1; p < arcLengths.Length; p++) {
			Vector3 newPoint = GetPointAtTime ((float)p / _precision); //get next point
			arcLengths [p] = Vector3.Distance (oldPoint, newPoint); //find distance to old point
			length += arcLengths [p]; //add it to the bezier's length
			oldPoint = newPoint; //new is old for next loop
		}

		//create our points array
		points = new Vector3[_num];
		//target length for spacing
		float segmentLength = length / _num;

		//arc index is where we got up to in the array to avoid the Shlemiel error http://www.joelonsoftware.com/articles/fog0000000319.html
		int arcIndex = 0;

		float walkLength = 0; //how far along the path we've walked
		oldPoint = GetPointAtTime (0);

		//iterate through points and set them
		for (int i = 0; i < points.Length; i++) {
			float iSegLength = i * segmentLength; //what the total length of the walkLength must equal to be valid
			//run through the arcLengths until past it
			while (walkLength < iSegLength) {
				walkLength += arcLengths [arcIndex]; //add the next arcLength to the walk
				arcIndex++; //go to next arcLength
			}
			//walkLength has exceeded target, so lets find where between 0 and 1 it is
			points [i] = GetPointAtTime ((float)arcIndex / arcLengths.Length);

		}

		//CheckConstant ();
	}
}
