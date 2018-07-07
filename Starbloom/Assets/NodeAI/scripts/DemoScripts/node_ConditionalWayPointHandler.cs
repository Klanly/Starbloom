//************************************COPYRIGHT 2017 CURTIS MARTINDALE***********************************
//***********************************************Version 1.0*********************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class node_ConditionalWayPointHandler : MonoBehaviour {

	[Tooltip("this is public to assign an initial target wayPoint.")]
	public Transform myTargetWayPoint; 
	[Tooltip("set this to true if you want to patrol the list of wayPoints from the aiMovement script.")]
	public bool usePatrol; 

	[HideInInspector]
	public int wayPointIndex;
	[HideInInspector]
	public Transform[] myWayPoints;
	[HideInInspector]
	public bool atWayPoint = false;
	[HideInInspector]
	public bool atConditionalWayPoint;

	private DG_AIEntity myaiMovement;
	private float wayPointDistance;

	void Awake () {
		myaiMovement = this.GetComponent<DG_AIEntity>();
		if(myTargetWayPoint == null || usePatrol){
			myTargetWayPoint = myWayPoints[0];
		}
	}

	void Update () {
		wayPointDistance = Vector3.Distance(transform.position, myTargetWayPoint.position);
		if(wayPointDistance <= myaiMovement.MovementSettings.stopDistanceAdjust + myaiMovement.agent.stoppingDistance && myaiMovement.agent.speed != 0){
			wayPointHit();
		}
		if(atConditionalWayPoint){
			if (myTargetWayPoint.GetComponent<node_WayPointConditional>().moveToNextWayPoint == true){
				myTargetWayPoint.GetComponent<node_WayPointConditional>().atThisWayPoint = false;
				myTargetWayPoint.GetComponent<node_WayPointConditional>().moveToNextWayPoint = false;
				myTargetWayPoint = myTargetWayPoint.GetComponent<node_WayPointConditional>().nextWayPointConditional;
				//myaiMovement.TargetPosition = myTargetWayPoint;
				myaiMovement.agent.speed = myaiMovement.MovementSettings.walkSpeed;
				myaiMovement.agent.SetDestination(myTargetWayPoint.position);
				atConditionalWayPoint = false;
			}
		}
	}

	void wayPointHit(){
		myaiMovement.agent.speed = 0;
		myTargetWayPoint.GetComponent<node_WayPointConditional>().atThisWayPoint = true;
		myTargetWayPoint.GetComponent<node_WayPointConditional>().thingAtThisWayPoint = this.gameObject;
		myTargetWayPoint.GetComponent<node_WayPointConditional>().thingsWayPoints = myWayPoints;
		if(myTargetWayPoint.GetComponent<node_WayPointConditional>().conditionalWayPoint ){
			atConditionalWayPoint = true;
		}
		if(usePatrol){
			wayPointIndex += 1;
			if(wayPointIndex >= myWayPoints.Length){
				wayPointIndex = 0;
			}

			if(!myTargetWayPoint.GetComponent<node_WayPointConditional>().conditionalWayPoint){
				myTargetWayPoint.GetComponent<node_WayPointConditional>().atThisWayPoint = false;
				myTargetWayPoint = myWayPoints[wayPointIndex];
				//myaiMovement.TargetPosition = myTargetWayPoint;
				myaiMovement.agent.speed = myaiMovement.MovementSettings.walkSpeed;
			}
		}
	}
}
