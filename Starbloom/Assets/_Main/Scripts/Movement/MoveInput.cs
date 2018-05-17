using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Locomotion))]
public class MoveInput : MonoBehaviour
{
	public bool InputDetected = false;
	protected Locomotion MoveController;

	void Update()
	{
		if (null == MoveController)
			MoveController = GetComponent<Locomotion>();

		DG_PlayerInput.Player MP = QuickFind.InputController.MainPlayer;
		if (MP.VerticalAxis != 0 || MP.HorizontalAxis != 0)
			InputDetected = true;
			
		Vector3 inputVec = new Vector3(MP.HorizontalAxis, 0f, MP.VerticalAxis);
		if (inputVec.magnitude > 1f)
			inputVec.Normalize();

		inputVec = Quaternion.Euler(0f, QuickFind.PlayerCam.MainCam.transform.rotation.eulerAngles.y, 0f) * inputVec; 

		MoveController.SetInput( inputVec );
	}
}