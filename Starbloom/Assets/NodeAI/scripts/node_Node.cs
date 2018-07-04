//************************************COPYRIGHT 2017 CURTIS MARTINDALE***********************************
//***********************************************Version 1.0*********************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class node_Node : MonoBehaviour {

	public Color myColor = Color.red;
	public float myRadius = 0.5f;
	public string nodeGroup;

	void OnDrawGizmos() {
		Gizmos.color = myColor;
		Gizmos.DrawSphere(transform.position, myRadius);
	}
}
