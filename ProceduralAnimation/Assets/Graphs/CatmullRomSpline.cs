using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Interpolation between points with a Catmull-Rom spline
public class CatmullRomSpline : MonoBehaviour
{
	//Has to be at least 4 points
	private Vector2[] controlPointsList = new Vector2[4];
	[SerializeField] private SOGraph graph;

	[Space]
	[SerializeField] [Range(0, 1)] private float time;
	[SerializeField] [Range(0, 0.5f)] private float value;

	//Display without having to press play
	void OnDrawGizmos()
	{
		controlPointsList[0] = graph.firstBender;
		controlPointsList[1] = Vector2.zero;
		controlPointsList[2] = new Vector2(1, graph.EndValue);
		controlPointsList[3] = graph.secondBender;

		Vector2 pos = GetCatmullRomPosition(time, controlPointsList[0], controlPointsList[1], controlPointsList[2], controlPointsList[3]);
		value = pos.y;
		Gizmos.DrawSphere(pos, 0.05f);

		foreach (Vector2 point in controlPointsList) 
		{
			Gizmos.DrawSphere(point, 0.04f);
		}

		Gizmos.color = Color.white;

		//Draw the Catmull-Rom spline between the points
		for (int i = 0; i < controlPointsList.Length; i++)
		{
			//Cant draw between the endpoints
			//Neither do we need to draw from the second to the last endpoint
			//...if we are not making a looping line
			if (i == 0 || i == controlPointsList.Length - 2 || i == controlPointsList.Length - 1)
			{
				continue;
			}

			DisplayCatmullRomSpline(i);
		}
	}

	//Display a spline between 2 points derived with the Catmull-Rom spline algorithm
	void DisplayCatmullRomSpline(int pos)
	{
		//The 4 points we need to form a spline between p1 and p2
		Vector2 p0 = controlPointsList[ClampListPos(pos - 1)];
		Vector2 p1 = controlPointsList[pos];
		Vector2 p2 = controlPointsList[ClampListPos(pos + 1)];
		Vector2 p3 = controlPointsList[ClampListPos(pos + 2)];

		//The start position of the line
		Vector2 lastPos = p1;

		//The spline's resolution
		//Make sure it's is adding up to 1, so 0.3 will give a gap, but 0.2 will work
		float resolution = 0.025f;

		//How many times should we loop?
		int loops = Mathf.FloorToInt(1f / resolution);

		for (int i = 1; i <= loops; i++)
		{
			//Which t position are we at?
			float t = i * resolution;

			//Find the coordinate between the end points with a Catmull-Rom spline
			Vector2 newPos = GetCatmullRomPosition(t, p0, p1, p2, p3);

			//Draw this line segment
			if (newPos.y < 0)
				Gizmos.color = Color.red;
			else if (newPos.x < 0 || newPos.x > 1)
				Gizmos.color = Color.green;
			else
				Gizmos.color = Color.white;

			Gizmos.DrawLine(lastPos, newPos);

			//Save this pos so we can draw the next line segment
			lastPos = newPos;
		}
	}

	//Clamp the list positions to allow looping
	int ClampListPos(int pos)
	{
		if (pos < 0)
		{
			pos = controlPointsList.Length - 1;
		}

		if (pos > controlPointsList.Length)
		{
			pos = 1;
		}
		else if (pos > controlPointsList.Length - 1)
		{
			pos = 0;
		}

		return pos;
	}

	//Returns a position between 4 Vector2 with Catmull-Rom spline algorithm
	//http://www.iquilezles.org/www/articles/minispline/minispline.htm
	Vector2 GetCatmullRomPosition(float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
	{
		//The coefficients of the cubic polynomial (except the 0.5f * which I added later for performance)
		Vector2 a = 2f * p1;
		Vector2 b = p2 - p0;
		Vector2 c = 2f * p0 - 5f * p1 + 4f * p2 - p3;
		Vector2 d = -p0 + 3f * p1 - 3f * p2 + p3;

		//The cubic polynomial: a + b * t + c * t^2 + d * t^3
		Vector2 pos = 0.5f * (a + (b * t) + (c * t * t) + (d * t * t * t));

		return pos;
	}
}
