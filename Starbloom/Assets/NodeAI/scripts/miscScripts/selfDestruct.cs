//************************************COPYRIGHT 2017 CURTIS MARTINDALE***********************************
//***********************************************Version 1.2*********************************************
using UnityEngine;
using System.Collections;

public class selfDestruct : MonoBehaviour {
	[Tooltip("how long this gameObject should exist in scene before being destroyed.")]
	public float destroyTime = 5.0f; 
	[Tooltip("If true, this object will only be disabled, not destroyed.")]
	public bool disableOnly;
	void Start () {
		StartCoroutine("destroyMe", destroyTime);
	}

	IEnumerator destroyMe(float destroyTime){
		yield return new WaitForSeconds(destroyTime);
		if(!disableOnly)
			Destroy (gameObject);
		else
			gameObject.SetActive(false);
	}
}
