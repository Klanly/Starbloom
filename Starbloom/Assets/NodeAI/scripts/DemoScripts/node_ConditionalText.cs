using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class node_ConditionalText : MonoBehaviour {

	public GameObject demoText;
	private node_AIMovement myaiMovement;

	void Start () {
		myaiMovement = this.GetComponent<node_AIMovement>();
	}

	void Update () {

		if(myaiMovement.agent.speed == 0 && demoText.activeSelf == false){
			demoText.SetActive(true);
		}

		if(myaiMovement.agent.speed != 0 && demoText.activeSelf == true){
			demoText.SetActive(false);
		}
	}
}
