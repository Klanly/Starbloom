//************************************COPYRIGHT 2017 CURTIS MARTINDALE***********************************
//***********************************************Version 1.0*********************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class node_DemoSpawner : MonoBehaviour {
	[Tooltip("Drag your level pieces prefabs here")]
	public GameObject [] pieces;
	[Tooltip("Drag the transforms where you want to spawn the level pieces here")]
	public Transform[] spawnLocations;
	[Tooltip("if true, pieces will have a random y rotation by 90 degrees")]
	public bool randomYRotation;
	[Tooltip("Drag the agent prefab you want to spawn to this slot")]
	public GameObject agent;
	[Tooltip("Transform were the agent will be spawned")]
	public Transform agentSpawnLocation;
	[Tooltip("Drag the navmesh surface object here")]
	public NavMeshSurface myNavMeshSurface;

	void Start () {

		foreach(Transform t in spawnLocations){
			GameObject pieceInstance;
			pieceInstance = Instantiate(pieces[Random.Range(0, pieces.Length)], t.transform.position, t.transform.rotation) as GameObject;

			if(randomYRotation){
				int tempy = Random.Range(0,3);
				if(tempy == 0){
					pieceInstance.transform.eulerAngles = new Vector3(transform.eulerAngles.x, 0.0f, transform.eulerAngles.z);
				}
				if(tempy == 1){
					pieceInstance.transform.eulerAngles = new Vector3(transform.eulerAngles.x, 90.0f, transform.eulerAngles.z);
				}
				if(tempy == 2){
					pieceInstance.transform.eulerAngles = new Vector3(transform.eulerAngles.x, 180.0f, transform.eulerAngles.z);
				}
				if(tempy == 3){
					pieceInstance.transform.eulerAngles = new Vector3(transform.eulerAngles.x, 270.0f, transform.eulerAngles.z);
				}
			}
		}
		myNavMeshSurface.BuildNavMesh();
		GameObject agentInstance;
		agentInstance = Instantiate(agent, agentSpawnLocation.position, agentSpawnLocation.rotation) as GameObject;
		GameObject.FindWithTag("MainCamera").GetComponent<followCam>().target = agentInstance.transform;

	}
}
