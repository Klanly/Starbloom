//************************************COPYRIGHT 2017 CURTIS MARTINDALE***********************************
//***********************************************Version 1.0*********************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class node_Gizmo : MonoBehaviour {

	public enum GizmoShape{Sphere, Cube};
	public GizmoShape gizmoShape;

	public Color gizmoColor = Color.red;
	public float gizmoSize = 1.0f;

	void OnDrawGizmos() {
		Gizmos.color = gizmoColor;
		switch(gizmoShape){
		case GizmoShape.Sphere : Gizmos.DrawSphere(transform.position, gizmoSize); break;
		case GizmoShape.Cube : Gizmos.DrawCube(transform.position, new Vector3(1.0f * gizmoSize, 1.0f * gizmoSize, 1.0f * gizmoSize)); break;
		}
	}
}
