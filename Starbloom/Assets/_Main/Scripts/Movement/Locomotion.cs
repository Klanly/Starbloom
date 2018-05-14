using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locomotion : MonoBehaviour
{
	public Rigidbody Mover;
	public float MaxSpeed = 1f;
	Vector3 InputDir;

	public void SetInput( Vector3 _heading )
	{
		InputDir = _heading;
	}

	private void LateUpdate()
	{
		Vector3 motion = InputDir * MaxSpeed;
		float prevYVel = Mover.velocity.y;
		motion.y = prevYVel;
		Mover.velocity = motion;
	}
}