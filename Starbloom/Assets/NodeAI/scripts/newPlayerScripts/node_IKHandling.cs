using UnityEngine;
using System.Collections;

public class node_IKHandling : MonoBehaviour {


	[Tooltip("Transform to place left hand.")]
	public Transform LeftHandTarget;
	[Tooltip("Transform to place right hand.")]
	public Transform RightHandTarget;

	[HideInInspector]
	public bool useIK;
	Animator anim;

	void Start () {
		useIK = true;
		anim =GetComponent<Animator>();
	}

	void OnAnimatorIK(){

		if(!useIK)
			return;
		
			anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
			anim.SetIKPosition(AvatarIKGoal.LeftHand, LeftHandTarget.position);
			anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
			anim.SetIKRotation(AvatarIKGoal.LeftHand, LeftHandTarget.rotation);

			anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
			anim.SetIKPosition(AvatarIKGoal.RightHand, RightHandTarget.position);
			anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
			anim.SetIKRotation(AvatarIKGoal.RightHand, RightHandTarget.rotation);
	}
}
