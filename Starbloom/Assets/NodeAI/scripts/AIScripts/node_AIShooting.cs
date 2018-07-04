//************************************COPYRIGHT 2017 CURTIS MARTINDALE***********************************
//***********************************************Version 1.0*********************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class node_AIShooting : MonoBehaviour {

	public enum combatType{Ranged, Melee, RangedAndMelee};
	public combatType CombatType;

	[Tooltip("Main Interaction layers. Select all enemy, player and level geometry layers. Typically only 'default', 'localPlayer' and 'enemy' need to be selected.")]
	public LayerMask interactLayers;
	[Tooltip("Just the layer for the player, playa.")]
	public LayerMask playerLayer;
	[Tooltip("damage dealt to player when hit.")]
	public float baseDamage = 1.0f; 
	[Tooltip("An audio clip to play when a shot happens.")]
	public AudioClip shotClip; 
	[Tooltip("Reference to the laser shot line renderer.")]
	public LineRenderer laserShotLine; 
	[Tooltip("how fast enemy fires weapon.")]
	public float fireRate = 0.75f; 
	[Tooltip("use this to raise or lower where enemy's laser hits player.")]
	public float playerHeightOffSet = 0.4f; 
	[Tooltip("If this agent is set to use melee, the distance at which the agent will attempt a melee attack.")]
	public float meleeDistance = 2.5f;
	[Tooltip("When a melee attack is triggered, a sphere with a radius this size is triggered in front of the agent. If the target is within this radius, the attack is successful")]
	public float meleeAttackRadius = 1.5f;
	[Tooltip("Agents gun/ranged weapon renderer.")]
	public Renderer gunModel;
	[Tooltip("Agents melee weapon renderer.")]
	public Renderer meleeModel;

	private Transform player; // Reference to the player's transform.
	private node_AIMovement aiM; //reference to animator
	private bool attacking; // if true, enemy is attacking
	private Animator myAnim;

	void Awake ()
	{
		if(GameObject.FindGameObjectWithTag("Player")){
			player = GameObject.FindGameObjectWithTag("Player").transform;
		}
		laserShotLine.enabled = false;
		aiM = this.GetComponent<node_AIMovement>();
		if(this.GetComponent<Animator>() != null){
			myAnim = this.GetComponent<Animator>();
		}

		if(CombatType == combatType.Ranged || CombatType == combatType.RangedAndMelee)
		{
			if(meleeModel)
				meleeModel.enabled = false;
			if(gunModel)
				gunModel.enabled = true;
		}
		if(CombatType == combatType.Melee)
		{
			if(meleeModel)
				meleeModel.enabled = true;
			if(gunModel)
				gunModel.enabled = false;		}
	}

	void Update ()
	{
		if(CombatType == combatType.Ranged)
		{
			useRangedAttack();
		}
		if(CombatType == combatType.Melee)
		{
			useMeleeAttack();
		}

		if(CombatType == combatType.RangedAndMelee)
		{
			if(aiM.playerDistance >= meleeDistance)
			{
				if(gunModel.enabled == false)
				{
					myAnim.SetBool("meleeAttack", false);

					meleeModel.enabled = false;
					gunModel.enabled = true;
				}

				useRangedAttack();

			}
			if(aiM.playerDistance < meleeDistance)
			{
				if(meleeModel.enabled == false)
				{
					myAnim.SetBool("Fire", false);
					attacking = false;
					CancelInvoke();

					gunModel.enabled = false;
					meleeModel.enabled = true;
				}
				useMeleeAttack();
			}
		}
	}

	void useRangedAttack()
	{
		if (!attacking && aiM.attackOk == true){
			InvokeRepeating("Shoot", fireRate, fireRate);
			attacking = true;
			if(myAnim != null){
				myAnim.SetBool("Fire", true);
			}
		}

		if (attacking && aiM.attackOk == false){
			CancelInvoke();
			attacking = false;
			if(myAnim != null){
				myAnim.SetBool("Fire", false);
			}
		}

		if(laserShotLine.enabled == true){
			laserShotLine.SetPosition(0, laserShotLine.transform.position);
			laserShotLine.SetPosition(1, new Vector3(player.position.x, player.position.y + playerHeightOffSet, player.position.z));
		}
	}

	void useMeleeAttack()

	{
		if (aiM.attackOk == true){
			if(myAnim != null){
				myAnim.SetBool("meleeAttack", true);
			}
		}

		if (aiM.attackOk == false){
			if(myAnim != null){
				myAnim.SetBool("meleeAttack", false);
			}
		}
	}

	void Shoot ()
	{
		RaycastHit hit;
		if(Physics.Linecast(transform.position, new Vector3 (player.position.x, player.position.y + 0.65f , player.position.z), out hit, interactLayers)) {
			if (hit.collider.gameObject.layer == player.gameObject.layer){
				ShotEffects();
				hit.collider.SendMessageUpwards("TakeDamage", baseDamage, SendMessageOptions.DontRequireReceiver);
			}
		}
	}
	void meleeAttack ()
	{
		Collider [] hitColliders = Physics.OverlapSphere(transform.position, meleeAttackRadius, playerLayer);
		int i = 0;
		while (i < hitColliders.Length)
		{
			if(hitColliders[i].gameObject.layer == player.gameObject.layer)
			{
				hitColliders[i].SendMessageUpwards("TakeDamage", baseDamage, SendMessageOptions.DontRequireReceiver);

			}
			i++;
		}
	}

	void ShotEffects ()
	{
		laserShotLine.enabled = true;
		StartCoroutine("laserKill", 0.2F);
		AudioSource.PlayClipAtPoint(shotClip, transform.position);
	}

	IEnumerator laserKill(float chillTime) {
		yield return new WaitForSeconds(chillTime);
		laserShotLine.enabled = false;
	}
}
