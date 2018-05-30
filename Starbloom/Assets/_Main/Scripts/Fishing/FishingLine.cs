using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer)), ExecuteInEditMode]
public class FishingLine : MonoBehaviour
{
	public LineRenderer m_LineRenderer;
	public Transform m_Start;
	public Transform m_End;


	public bool UseSegmentsByDistance { get{ return m_UseSegmentsByDistance; } }
	public bool m_UseSegmentsByDistance = true;

	public int m_Segments = 20;
	public float m_SegmentsPerUnit = 2;

	[Range(0f, 1f)]
	public float m_Tension = 0f;
	public bool m_UsePerlinTension = false;
	public float m_PerlinTensionSpeed = 0.5f;

	public bool m_ProjectFromHighest = true;

	public bool m_CheckCollision = true;
	public Vector3 m_CollisionOffset = Vector3.up;
	public float m_CollisionCheckHeight = 5f;
	public LayerMask m_CollisionLayers;

	public bool m_SmoothLine = true;
	public float m_SmoothFactor = 1f;

	public Vector3[] m_SegmentLocs;

	public Ease m_LineEase = Ease.Linear;

	void Update()
	{
		Vector3 localStart = transform.InverseTransformPoint(m_Start.position);// - transform.position;
		Vector3 localEnd = transform.InverseTransformPoint(m_End.position);// - transform.position;

		int segs = !UseSegmentsByDistance ? m_Segments : Mathf.CeilToInt((localEnd - localStart).magnitude * m_SegmentsPerUnit);

		m_SegmentLocs = new Vector3[segs + 1];

		// Project from highest, simple conditional swap
		if ( m_ProjectFromHighest && localStart.y < localEnd.y )
		{
			Vector3 oldStart = localStart;
			localStart = localEnd;
			localEnd = oldStart;
		}

		m_SegmentLocs[0] = localStart;
		m_SegmentLocs[segs] = localEnd;

		if (m_UsePerlinTension)
			m_Tension = Mathf.PerlinNoise(Time.time * m_PerlinTensionSpeed, 0.5f);

		float slice = 1f / segs;
		float frac = slice; // Skip the start node

		for( int i = 1; i < segs; ++i )
		{
			float easeFrac = DOVirtual.EasedValue( 0f, 1f, frac, m_LineEase );
			m_SegmentLocs[i] = Vector3.Lerp( localStart, localEnd, easeFrac );
			m_SegmentLocs[i].y = Mathf.Lerp( DOVirtual.EasedValue(localStart.y, localEnd.y, easeFrac, m_LineEase), m_SegmentLocs[i].y, m_Tension );

			// Prevent clipping against geo
			if( m_CheckCollision )
			{
				RaycastHit hitInfo;
				Vector3 gpos = transform.TransformPoint(m_SegmentLocs[i]);
				Debug.DrawLine(gpos + Vector3.up * m_CollisionCheckHeight, gpos, Color.red);
				if (Physics.Linecast(gpos + Vector3.up * m_CollisionCheckHeight, gpos, out hitInfo, m_CollisionLayers))
				{
					Debug.DrawLine(gpos + Vector3.up * m_CollisionCheckHeight, hitInfo.point, Color.green);
					m_SegmentLocs[i] = transform.InverseTransformPoint(hitInfo.point + m_CollisionOffset);
				}
			}

			frac += slice;

			//Debug.DrawLine(m_SegmentLocs[i], m_SegmentLocs[i] + Vector3.up, Color.green);
		}

		if( m_SmoothLine )
			m_SegmentLocs = Utility.MakeSmoothCurve(m_SegmentLocs, m_SmoothFactor);

		m_LineRenderer.positionCount = m_SegmentLocs.Length;
		m_LineRenderer.SetPositions(m_SegmentLocs);
	}

	Vector3 DOVirtualEase(Vector3 from, Vector3 to, float frac, Ease ease)
	{
		return new Vector3(
			DOVirtual.EasedValue(from.x, to.x, frac, ease),
			DOVirtual.EasedValue(from.y, to.y, frac, ease),
			DOVirtual.EasedValue(from.z, to.z, frac, ease));
	}
}
