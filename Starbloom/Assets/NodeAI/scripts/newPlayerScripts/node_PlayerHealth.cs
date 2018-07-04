using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

    public class node_PlayerHealth : MonoBehaviour
    {
	[Tooltip("The amount of health the player starts the game with.")]
	public int startingHealth = 100;                            
	[Tooltip("The current health the player has.")]
	public int currentHealth;                                   
	[Tooltip("Reference to the UI's health bar.")]
	public Slider healthSlider;                                 
	[Tooltip("Reference to an image to flash on the screen on being hurt.")]
	public Image damageImage;                                   
	[Tooltip("The audio clip to play when the player dies.")]
	public AudioClip deathClip;                                 
	[Tooltip("The speed the damageImage will fade at.")]
	public float flashSpeed = 5f;                               
	[Tooltip("The colour the damageImage is set to, to flash.")]
	public Color flashColour = new Color(1f, 0f, 0f, 0.1f);     
	[Tooltip("Text that pops up when player dies. Not required.")]
	public GameObject gameOverText;

        Animator anim;                                              // Reference to the Animator component.
        AudioSource playerAudio;                                    // Reference to the AudioSource component.
        node_PlayerMovement playerMovement;                         // Reference to the player's movement.
        node_PlayerShooting playerShooting;                         // Reference to the PlayerShooting script.
        bool isDead;                                                // Whether the player is dead.
        bool damaged;                                               // True when the player gets damaged.

        void Awake ()
        {
            // Setting up the references.
            anim = GetComponent <Animator> ();
            playerAudio = GetComponent <AudioSource> ();
            playerMovement = GetComponent <node_PlayerMovement> ();
            playerShooting = GetComponentInChildren <node_PlayerShooting> ();

            // Set the initial health of the player.
            currentHealth = startingHealth;
        }

        void Update ()
        {
            // If the player has just been damaged...
            if(damaged)
            {
                // ... set the colour of the damageImage to the flash colour.
                damageImage.color = flashColour;
            }
            // Otherwise...
            else
            {
                // ... transition the colour back to clear.
                damageImage.color = Color.Lerp (damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
            }

            // Reset the damaged flag.
            damaged = false;
        }


        public void TakeDamage (int amount)
        {
            // Set the damaged flag so the screen will flash.
            damaged = true;

            // Reduce the current health by the damage amount.
            currentHealth -= amount;

            // Set the health bar's value to the current health.
            healthSlider.value = currentHealth;

            // Play the hurt sound effect.
            playerAudio.Play ();

            // If the player has lost all it's health and the death flag hasn't been set yet...
            if(currentHealth <= 0 && !isDead)
            {
                // ... it should die.
                Death ();
            }
        }


        void Death ()
        {
			if(isDead)
				return;
            // Turn off any remaining shooting effects.
            playerShooting.DisableEffects ();

            // Tell the animator that the player is dead.
            anim.SetTrigger ("Die");

            // Set the audiosource to play the death clip and play it (this will stop the hurt sound from playing).
            playerAudio.clip = deathClip;
            playerAudio.Play ();

            // Turn off the movement and shooting scripts.
            playerMovement.enabled = false;
            playerShooting.enabled = false;
			isDead = true;
			
			if(gameOverText != null)
				gameOverText.SetActive(true);
		
			StartCoroutine("startOver");

        }

		IEnumerator startOver()
		{
			yield return new WaitForSeconds(5.0f);
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);

		}
}