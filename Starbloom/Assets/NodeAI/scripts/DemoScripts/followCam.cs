using UnityEngine;
using System.Collections;

public class followCam : MonoBehaviour {
	
	// The target we are following
	public Transform target;
	// The distance in the x-z plane to the target
	public float distance = 10.0f;
	// the height we want the camera to be above the target
	public float height = 5.0f;
	// How much we 
	public float heightDamping = 2.0f;
	public float rotationDamping = 3.0f;
	
	public float speed = 1.0f;
	private float tempSpeed;
	private bool followTarget;

	public Vector3 movement;
	// Use this for initialization
	void Start () {
		followTarget = true;
		tempSpeed = speed;
		if(!followTarget){
			movement = new Vector3 (transform.position.x,transform.position.y, transform.position.z);
		}
	}
	
	// Update is called once per frame
	void LateUpdate () {
		
		float wantedRotationAngle;
		float wantedHeight;
		float currentRotationAngle;
		float currentHeight;
		Quaternion currentRotation;
		float zoom = Input.GetAxisRaw("Mouse ScrollWheel");
		
		if(height >= 10.0f && height <= 45.0f ){
			height += transform.position.y * -zoom * Time.deltaTime * 50;
		}
		if(height < 10.0f){
			height = 10.0f;
		}
		if(height > 45.0f){
			height = 45.0f;
		}
		/*if(Input.GetKeyDown(KeyCode.Space)){
			followTarget = !followTarget;
		}*/
		
		if(!followTarget){
			if(Input.GetKeyDown(KeyCode.LeftShift)){
				speed = tempSpeed * 2.5f;
			}
			if(Input.GetKeyUp(KeyCode.LeftShift)){
				speed = tempSpeed;
			}
			Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
			transform.position += new Vector3(move.x * speed * Time.deltaTime, 0, move.z * speed * Time.deltaTime);
			//transform.position.z += move.z * speed * Time.deltaTime;
			wantedRotationAngle = 0;
			wantedHeight = height;
			currentRotationAngle = transform.eulerAngles.y;
			currentHeight = transform.position.y;
			// Damp the rotation around the y-axis
			currentRotationAngle = Mathf.LerpAngle (currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);
			// Damp the height
			currentHeight = Mathf.Lerp (currentHeight, wantedHeight, heightDamping * Time.deltaTime);
			// Convert the angle into a rotation
			currentRotation = Quaternion.Euler (0, currentRotationAngle, 0);
			transform.position -= new Vector3(0, currentRotation.y * Vector3.forward.y * distance, 0);
			// Set the height of the camera
			movement += new Vector3(move.x * speed * Time.deltaTime, 0, move.z * speed * Time.deltaTime);
			transform.position = new Vector3(movement.x, currentHeight, movement.z);
			
		}
		
		if(followTarget){
			// Early out if we don't have a target
			if (!target)
				return;
			// Calculate the current rotation angles
			wantedRotationAngle = target.eulerAngles.y;
			wantedHeight = target.position.y + height;
			currentRotationAngle = transform.eulerAngles.y;
			currentHeight = transform.position.y;
			// Damp the rotation around the y-axis
			currentRotationAngle = Mathf.LerpAngle (currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);
			// Damp the height
			currentHeight = Mathf.Lerp (currentHeight, wantedHeight, heightDamping * Time.deltaTime);
			// Convert the angle into a rotation
			currentRotation = Quaternion.Euler (0, currentRotationAngle, 0);
			// Set the position of the camera on the x-z plane to:
			// distance meters behind the target
			movement = new Vector3(target.position.x, currentHeight,target.position.z - distance);
			transform.position = target.position;
			transform.position -= currentRotation * Vector3.forward * distance;
			// Set the height of the camera
			transform.position = new Vector3(target.position.x, currentHeight,target.position.z - distance);
			// Always look at the target
			transform.LookAt (target);
		}
	}
}
