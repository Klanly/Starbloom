using UnityEngine;
using System.Collections;

public class FollowCamera : MonoBehaviour {

	#region Variables (private)

	[SerializeField]
	private float distanceAway;
	[SerializeField]
	private float distanceUp;
	[SerializeField]
	private float smooth;
	[SerializeField]
	private Transform follow;
	private Vector3 targetPosition;

	#endregion

	void Start() 
	
	{
		follow = GameObject.FindWithTag ("Cam").transform;
	}
	
	void LateUpdate() 
	{

		targetPosition = follow.position + follow.up * distanceUp - follow.forward * distanceAway;
		Debug.DrawRay (follow.position, Vector3.up * distanceUp, Color.red);
		Debug.DrawRay (follow.position, -1f * follow.forward * distanceAway, Color.blue);
		Debug.DrawRay (follow.position, targetPosition, Color.magenta);

		transform.position = Vector3.Lerp (transform.position, targetPosition, Time.deltaTime * smooth);
		transform.LookAt (follow);
	}
}
