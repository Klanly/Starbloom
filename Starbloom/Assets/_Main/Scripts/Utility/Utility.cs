using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
	public static void SetEmissiveColor( Material _mat, Color _color, MaterialGlobalIlluminationFlags _giFlag = MaterialGlobalIlluminationFlags.RealtimeEmissive )
	{
		_mat.EnableKeyword("_EMISSION");
		_mat.globalIlluminationFlags = _giFlag;
		_mat.SetColor("_EmissionColor", _color);
	}

	// Array to curve is original vector3 array, smoothness is the number of interpolations
	public static Vector3[] MakeSmoothCurve( Vector3[] arrayToCurve, float smoothness)
	{
		List<Vector3> points;
		List<Vector3> curvedPoints;

		int pointsLen = 0;
		int curvedLen = 0;

		if (smoothness < 1.0f) smoothness = 1.0f;

		pointsLen = arrayToCurve.Length;
		curvedLen = (pointsLen * Mathf.RoundToInt(smoothness)) - 1;
		curvedPoints = new List<Vector3>(curvedLen);

		float t = 0f;

		for(int pointInTimeOnCurve = 0; pointInTimeOnCurve < curvedLen+1; ++pointInTimeOnCurve)
		{
			t = Mathf.InverseLerp(0, curvedLen, pointInTimeOnCurve);
			points = new List<Vector3>(arrayToCurve);

			for (int j = pointsLen - 1; j > 0; --j)
			{
				for(int i = 0; i < j; ++i )
				{
					points[i] = (1 - t) * points[i] + t * points[i + 1];
				}
			}

			curvedPoints.Add(points[0]);
		}

		return curvedPoints.ToArray();
	}
}
