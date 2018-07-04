//************************************COPYRIGHT 2017 CURTIS MARTINDALE***********************************
//***********************************************Version 1.1*********************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class node_AIHealth : MonoBehaviour {

	[Tooltip("How many hiys agent can take before death.")]
	public float HP = 8.0f; 
	[Tooltip("Object to spawn on death. If ragDollEnemy is true, this object is reset to null. Not Required.")]
	public GameObject spawnOnDeath;
	[Tooltip("If true, this agent is using a ragdoll which will be activated upon death.")]
	public bool ragDollEnemy;
	[Tooltip("If true, the agents ragdoll will be destroyed. If false, the ragdoll will stay in the scene indefinately.")]
	public bool destroyRagDoll;
	[Tooltip("If destroyRagDoll istrue, how long the ragdoll will remain in the scene before being destroyed.")]
	public float ragDollDestroyTime = 10.0f;

	private bool dead;
	private node_AIMovement aim;

	void Awake () {
		aim = this.GetComponent<node_AIMovement>();
		if(ragDollEnemy)
		{
			spawnOnDeath = null;
		}
	}

	public void Damage(float damage){
		HP -= damage;
		if(HP <= 0){
			if(spawnOnDeath){
				Instantiate(spawnOnDeath, transform.position, transform.rotation);
				Destroy (gameObject);
			}
			if(ragDollEnemy && !dead)
			{
				this.gameObject.layer = 29;
				Destroy(this.GetComponent<CapsuleCollider>());
				Destroy(this.GetComponent<Animator>());
				Destroy(this.GetComponent<node_AIAnimation>());
				aim.activateMark(false);
				Destroy(this.GetComponent<node_AIMovement>());
				Destroy(this.GetComponent<node_AIShooting>().laserShotLine);
				Destroy(this.GetComponent<node_AIShooting>());
				Destroy(this.GetComponent<NavMeshAgent>());
				Rigidbody[] bodies = GetComponentsInChildren<Rigidbody>();
				foreach (Rigidbody rb in bodies)
				{
					rb.gameObject.layer = 29;
					rb.isKinematic = false;
				}
				if(!destroyRagDoll)
					Destroy(this.GetComponent<node_AIHealth>());
				else
					StartCoroutine("getRidOfBody");
			}
			dead = true;
		}
		else{
			if(!aim.chase){
				if(aim.playerVisible(aim.detectionSettings.myEyes.position) == true){
					aim.pursuePlayer(true);
				}
				else{
					aim.pursuePlayer(false);
				}
			}
		}
	}

	IEnumerator getRidOfBody()
	{
		yield return new WaitForSeconds(ragDollDestroyTime);
		Destroy(this.gameObject);
	}
}
