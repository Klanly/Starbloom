using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Locomotion))]
public class LocomotionAnim : MonoBehaviour
{
	public Animator Anim;
	public Locomotion Loco;

	void Update()
	{
		if (null == Anim || null == Loco)
			return;

		//Anim.SetFloat("Direction", Loco.Heading);
		Anim.SetFloat("Speed", Loco.CurSpeed);
	}
}