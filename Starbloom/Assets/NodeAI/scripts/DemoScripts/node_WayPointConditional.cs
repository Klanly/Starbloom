//************************************COPYRIGHT 2017 CURTIS MARTINDALE***********************************
//***********************************************Version 1.0*********************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class node_WayPointConditional : MonoBehaviour {
	
	[Tooltip("Set true if this node is used in a conditional setting. Check out the conditionalWayPointDemo scene for an example")]
	public bool conditionalWayPoint;
	[Tooltip("use this to id custom wayPoints for unique behaviours. Typically only used on conditinal wayPoints")]
	public string wayPointName;

	[HideInInspector]
	public bool atThisWayPoint;
	[HideInInspector]
	public Transform[] thingsWayPoints;
	[HideInInspector]
	public GameObject thingAtThisWayPoint;
	[HideInInspector]
	public Transform nextWayPointConditional;
	[HideInInspector]
	public bool moveToNextWayPoint;

	void Update () {

		if(atThisWayPoint == true){
			if(Input.GetKeyDown(KeyCode.Alpha1)){
				foreach(Transform wPoint in thingsWayPoints){
					if(wPoint.GetComponent<node_WayPointConditional>().wayPointName == "red"  && wPoint != this.transform){
						nextWayPointConditional = wPoint;
						moveToNextWayPoint = true;
					}
				}
			}
			if(Input.GetKeyDown(KeyCode.Alpha2)){
				foreach(Transform wPoint in thingsWayPoints){
					if(wPoint.GetComponent<node_WayPointConditional>().wayPointName == "blue"  && wPoint != this.transform){
						nextWayPointConditional = wPoint;
						moveToNextWayPoint = true;
					}
				}
			}
			if(Input.GetKeyDown(KeyCode.Alpha3)){
				foreach(Transform wPoint in thingsWayPoints){
					if(wPoint.GetComponent<node_WayPointConditional>().wayPointName == "orange"  && wPoint != this.transform){
						nextWayPointConditional = wPoint;
						moveToNextWayPoint = true;
					}
				}
			}

			if(Input.GetKeyDown(KeyCode.Alpha4)){
				foreach(Transform wPoint in thingsWayPoints){
					if(wPoint.GetComponent<node_WayPointConditional>().wayPointName == "green" && wPoint != this.transform){
						nextWayPointConditional = wPoint;
						moveToNextWayPoint = true;
					}
				}
			}
		}
	}
}
