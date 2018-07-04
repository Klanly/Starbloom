//************************************COPYRIGHT 2017 CURTIS MARTINDALE***********************************
//***********************************************Version 1.0*********************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class node_MoveWayPoint : MonoBehaviour {

	[Tooltip("The AI agent you want to follow this wayPoint.")]
	public GameObject myAI; 

	private Vector3 newPosition;
	private float tempMoveSpeed;
	private node_AIMovement myAIM;

	void Start () {
		newPosition = transform.position;
		myAIM = myAI.GetComponent<node_AIMovement>();
	}

	void Update () {

		if (Input.GetMouseButtonDown(0))
		{
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out hit))
			{
				if(hit.collider.tag == "floor"){
					newPosition = new Vector3(hit.point.x, hit.point.y + 1, hit.point.z);
					transform.position = newPosition;
					myAIM.StartCoroutine(myAIM.wayPointHit(0, true));
				}
			}
		}
	}
}
