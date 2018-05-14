using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locomotion : MonoBehaviour
{
	public Rigidbody Mover;
	public float MaxSpeed = 1f;
	public float TurnSpeed = 180f;

	public Vector3 InputDir { get; protected set; }
	public float Heading { get { return Vector3.SignedAngle(transform.forward, InputDir, Vector3.up); } }
	public float CurSpeed { get { return InputDir.magnitude * MaxSpeed; } }

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

		if (InputDir != Vector3.zero)
		{
			Quaternion desiredLook = Quaternion.LookRotation(InputDir);
			if (!desiredLook.AlmostEquals(transform.rotation, 0.1f))
				transform.rotation = Quaternion.RotateTowards(transform.rotation, desiredLook, TurnSpeed * Time.deltaTime);
		}
	}
}