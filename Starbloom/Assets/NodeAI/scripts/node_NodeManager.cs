//************************************COPYRIGHT 2017 CURTIS MARTINDALE***********************************
//***********************************************Version 1.0*********************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class node_NodeManager : MonoBehaviour {

	public bool nodesAreChildren;
	public List <Transform> nodes;

	void Awake () {
		if(nodesAreChildren){
			foreach(Transform child in transform){
				nodes.Add(child.transform);
			}
		}
	}
}
