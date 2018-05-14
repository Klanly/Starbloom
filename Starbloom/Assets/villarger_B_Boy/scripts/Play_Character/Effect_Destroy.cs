using UnityEngine;
using System.Collections;

public class Effect_Destroy : MonoBehaviour 
{
	public float destroyTime;



	void Start()
	{
		Destroy (gameObject, destroyTime);
	}

}
