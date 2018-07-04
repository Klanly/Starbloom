//************************************COPYRIGHT 2016 CURTIS MARTINDALE***********************************
//***********************************************Version 1.1*********************************************
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class demoLevelSelect : MonoBehaviour {

	void OnTriggerEnter(Collider other) {
		if(other.tag == "Player"){
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}
	}
}
