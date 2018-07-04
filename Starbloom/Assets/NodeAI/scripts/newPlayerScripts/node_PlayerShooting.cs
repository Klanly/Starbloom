using UnityEngine;
using System.Collections;


    public class node_PlayerShooting : MonoBehaviour
    {
		[Tooltip("The damage inflicted by each bullet.")]
	    public int damagePerShot = 1;    
		[Tooltip("The time between each shot.")]
	    public float timeBetweenBullets = 0.15f;        
		[Tooltip("The distance the gun can fire.")]
	    public float range = 100f;                     
		[Tooltip("Light Used for gun fx.")]
		public Light faceLight;								
        float effectsDisplayTime = 0.2f;                // The proportion of the timeBetweenBullets that the effects will display for.

		[Tooltip("Spot where raycast for shooting originates. Pulled further back then fx spot to prevent shooting through walls.")]
		public Transform fireSpot;
		[Tooltip("Spot where all your shot fx take place. Muzzle flash, bullet tracers, etc.")]
		public Transform fxSpot;
		[Tooltip("muzzleflash model. put at tip of gun.")]
		public GameObject muzzleFlash;
		[Tooltip("Objects in the scene that bullets can hit. Typically Default and Enemy.")]
		public LayerMask shootableMask;
		[Tooltip("Select the layer that the enemies are on. Typical on the Enemy layer.")]
		public LayerMask enemyMask;
		[Tooltip("Drag your Ranged/gun model here.")]
		public Renderer gunModel;
		[Tooltip("Drag your melee Model Here.")]
		public Renderer meleeModel;
		[Tooltip("How much melee damage you do when you attack an unaware enemy. Great for stealth take downs.")]
		public float meleeSneakAttackDamage = 10.0f;
		[Tooltip("How much damage your melee attack does on a hostile enemy.")]
		public float meleeNormalDamage = 1.0f;
		[Tooltip("If this is true, enemies within the gun sound radius will be alerted to the players position when you fire your weapon.")]
		public bool useGunSoundRadius;
		[Tooltip("How far your gunshots are noticed by enemies.")]
		public float gunSoundRadius = 15.0f;
		[Tooltip("how often your gunShots are noticed by enemies. This is done in intervals for performance reasons. For higher quality, lower interval, for better performance raise interval")]
		public float gunSoundUpdateInterval = 1.0f;
		[Tooltip("sound to play when your gun is fired.")]
		public AudioClip gunSound;
		[Tooltip("Sound played when melee attack is triggered")]
		public AudioClip meleeSound;
		[Tooltip("This is used to spawn a hit reaction to your bullets. It pools this effect for performance reasons")]
		public node_objectPooler sparkPooler;

		private bool usingMelee;
		private Animator myAnim;
		private node_IKHandling myIK;
		private node_PlayerMovement myMovement;
		private bool gunSoundUpdateOK;

		float timer;                                    // A timer to determine when to fire.
		Ray shootRay = new Ray();                       // A ray from the gun end forwards.
		RaycastHit shootHit;                            // A raycast hit to get information about what was hit.
		LineRenderer gunLine;                           // Reference to the line renderer.
		AudioSource gunAudio;                           // Reference to the audio source.
		Light gunLight;                                 // Reference to the light component.

        void Awake ()
        {
			myAnim = this.GetComponent<Animator>();
			myIK= this.GetComponent<node_IKHandling>();
			myMovement= this.GetComponent<node_PlayerMovement>();
			gunSoundUpdateOK = true;
			if(muzzleFlash)
				muzzleFlash.SetActive(false);
            // Set up the references.
			gunLine = fxSpot.GetComponent <LineRenderer> ();
			gunAudio = fxSpot.GetComponent<AudioSource> ();
			gunLight = fxSpot.GetComponent<Light> ();
        }

        void Update ()
        {
        return;

            // Add the time since Update was last called to the timer.
            timer += Time.deltaTime;


            // If the Fire1 button is being press and it's time to fire...
			if(!usingMelee && Input.GetButton ("Fire1") && timer >= timeBetweenBullets && Time.timeScale != 0 || !usingMelee &&  Input.GetAxis("RightTrigger") > 0 && timer >= timeBetweenBullets && Time.timeScale != 0)
            {
                // ... shoot the gun.
                Shoot ();
            }
			if(usingMelee && Input.GetButton("Fire1") || usingMelee && Input.GetAxis("RightTrigger") > 0)
			{
				myAnim.SetBool("meleeAttack", true);
			}
			if(usingMelee && Input.GetButtonUp("Fire1") || myMovement.useGamePad && usingMelee && Input.GetAxis("RightTrigger") <= 0)
			{

				myAnim.SetBool("meleeAttack", false);

			}
			if(Input.GetButtonDown ("Fire2"))
			{
				usingMelee = !usingMelee;
				myAnim.SetBool("useMelee", usingMelee);
				if(usingMelee)
				{
					myIK.useIK = false;
					gunModel.enabled = false;
					meleeModel.enabled = true;
				}
				else
				{
					myIK.useIK = true;
					gunModel.enabled = true;
					meleeModel.enabled = false;
				}
			}

            // If the timer has exceeded the proportion of timeBetweenBullets that the effects should be displayed for...
            if(timer >= timeBetweenBullets * effectsDisplayTime)
            {
                // ... disable the effects.
                DisableEffects ();
            }
        }


        public void DisableEffects ()
        {
            // Disable the line renderer and the light.
            gunLine.enabled = false;
			faceLight.enabled = false;
            gunLight.enabled = false;
			if(muzzleFlash)
				muzzleFlash.SetActive(false);

        }
		IEnumerator resetGunSound()
		{
			yield return new WaitForSeconds(gunSoundUpdateInterval);
			gunSoundUpdateOK = true;
		}

		void meleeAttack ()
		{
			gunAudio.clip = meleeSound;
			gunAudio.Play ();
			Collider [] hitColliders = Physics.OverlapSphere(transform.position, 2.0f, enemyMask);
			int i = 0;
			while (i < hitColliders.Length)
			{
				if(hitColliders[i].tag == "Enemy")
				{
					if(hitColliders[i].GetComponent<node_AIMovement>() != null){
						node_AIMovement tempAim = hitColliders[i].GetComponent<node_AIMovement>();
						if(tempAim.chase == false && tempAim.checkingForPlayer == false)
						{
							hitColliders[i].GetComponent<node_AIHealth>().Damage(meleeSneakAttackDamage);
						}
						else{
							hitColliders[i].GetComponent<node_AIHealth>().Damage(meleeNormalDamage);
						}
					}
				}
				i++;
			}
		}
        void Shoot ()
        {
            // Reset the timer.
            timer = 0f;

            // Play the gun shot audioclip.
			gunAudio.clip = gunSound;
            gunAudio.Play ();
			if(useGunSoundRadius && gunSoundUpdateOK)
			{
				Collider[] hitColliders = Physics.OverlapSphere(transform.position, gunSoundRadius);
				int i = 0;
				while (i < hitColliders.Length)
				{
					hitColliders[i].SendMessage("wakeUp", SendMessageOptions.DontRequireReceiver);
					i++;
				}
				gunSoundUpdateOK = false;
				StartCoroutine("resetGunSound");
			}
            // Enable the lights.
            gunLight.enabled = true;
			faceLight.enabled = true;
			if(muzzleFlash)
			{
				muzzleFlash.SetActive(true);
				muzzleFlash.transform.localEulerAngles = new Vector3(muzzleFlash.transform.localEulerAngles.x, muzzleFlash.transform.localEulerAngles.y, Random.Range(0.0f,360.0f));
			}

            // Enable the line renderer and set it's first position to be the end of the gun.
            gunLine.enabled = true;
			gunLine.SetPosition (0, fxSpot.transform.position);

            // Set the shootRay so that it starts at the end of the gun and points forward from the barrel.
			shootRay.origin = fireSpot.transform.position;
			shootRay.direction = fireSpot.transform.forward;

            // Perform the raycast against gameobjects on the shootable layer and if it hits something...
            if(Physics.Raycast (shootRay, out shootHit, range, shootableMask))
            {
				node_AIHealth health = shootHit.collider.GetComponent <node_AIHealth> ();
				if(health != null)
                {
					health.Damage(damagePerShot);
                }
				if(sparkPooler != null)
				{
					GameObject obj = sparkPooler.GetPooledObject();
					if(obj != null)
					{
						obj.SetActive(true);
						obj.transform.position = shootHit.point;
				
					}
				}
                // Set the second position of the line renderer to the point the raycast hit.
                gunLine.SetPosition (1, shootHit.point);
            }
            // If the raycast didn't hit anything on the shootable layer...
            else
            {
                // ... set the second position of the line renderer to the fullest extent of the gun's range.
                gunLine.SetPosition (1, shootRay.origin + shootRay.direction * range);
            }
        }
}