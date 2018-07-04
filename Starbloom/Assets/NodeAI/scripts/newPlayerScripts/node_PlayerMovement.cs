using UnityEngine;


    public class node_PlayerMovement : MonoBehaviour
    {
		[Tooltip("The speed that the player will move at.")]
	    public float speed = 6f;          
		[Tooltip("If true, gamepad is enabled for rotation, if false, mouse is enabled for rotation.")]
		public bool useGamePad;

        Animator anim;                      // Reference to the animator component.
        Rigidbody playerRigidbody;          // Reference to the player's rigidbody.
        int floorMask;                      // A layer mask so that a ray can be cast just at gameobjects on the floor layer.
        float camRayLength = 100f;          // The length of the ray from the camera into the scene.

		private float turnDir;
		private float forwardDir;
		private float rh;
		private float rv;
		private  Vector3 movement;                   // The vector to store the direction of the player's movement.
		private Vector3 rotateDirection;
		private Vector3 animMove;
		private float h;
		private float v;

        void Awake ()
        {
            // Create a layer mask for the floor layer.
            floorMask = LayerMask.GetMask ("Floor");

            // Set up references.
            anim = GetComponent <Animator> ();
            playerRigidbody = GetComponent <Rigidbody> ();

			//Hide cursor.
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Confined;

        }

		void FixedUpdate ()
        {
            // Store the input axes.
            h = Input.GetAxisRaw("Horizontal");
			v = Input.GetAxisRaw("Vertical");

            // Move the player around the scene.
            Move (h, v);

            // Turn the player to face the mouse cursor.
            Turning ();

			animMove = v * Vector3.forward + h * Vector3.right;
			if(animMove.magnitude > 1){
				animMove.Normalize();
			}
			Vector3 localMove = transform.InverseTransformDirection(animMove);
			turnDir = localMove.x;
			forwardDir = localMove.z;
			anim.SetFloat("Forward",forwardDir, 0.1f, Time.deltaTime);
			anim.SetFloat("Turn", turnDir, 0.1f, Time.deltaTime);
        }

        void Move (float h, float v)
        {
            // Set the movement vector based on the axis input.
            movement.Set (h, 0f, v);
            
            // Normalise the movement vector and make it proportional to the speed per second.
            movement = movement * speed * Time.deltaTime;

            // Move the player to it's current position plus the movement.
            playerRigidbody.MovePosition (transform.position + movement);
        }


        void Turning ()
        {
			if(!useGamePad){
	            // Create a ray from the mouse cursor on screen in the direction of the camera.
	            Ray camRay = Camera.main.ScreenPointToRay (Input.mousePosition);

	            // Create a RaycastHit variable to store information about what was hit by the ray.
	            RaycastHit floorHit;

	            // Perform the raycast and if it hits something on the floor layer...
	            if(Physics.Raycast (camRay, out floorHit, camRayLength, floorMask))
	            {
	                // Create a vector from the player to the point on the floor the raycast from the mouse hit.
	                Vector3 playerToMouse = floorHit.point - transform.position;

	                // Ensure the vector is entirely along the floor plane.
	                playerToMouse.y = 0f;

	                // Create a quaternion (rotation) based on looking down the vector from the player to the mouse.
	                Quaternion newRotatation = Quaternion.LookRotation (playerToMouse);

	                // Set the player's rotation to this new rotation.
	                playerRigidbody.MoveRotation (newRotatation);
	            }
			}

			if(useGamePad)
			{
				rh = Input.GetAxisRaw ("RightStickHorizontal");
				rv = Input.GetAxisRaw ("RightStickVertical");
				if(rh != 0 || rv != 0)
				{
					rotateDirection = (Vector3.forward * rv) + (Vector3.right * rh);
					rotateDirection.y = 0;
					Quaternion newRotatation = Quaternion.LookRotation (rotateDirection);
					playerRigidbody.MoveRotation(newRotatation);
				}
			}
        }
}