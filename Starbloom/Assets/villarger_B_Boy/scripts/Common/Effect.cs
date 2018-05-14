using UnityEngine;
using System.Collections;

public class Effect : MonoBehaviour 
{

	public GameObject falcon;//effect
	public GameObject effect1;
	public GameObject effect2; 



	void Start()
	{
		falcon.SetActive(false);
		effect1.SetActive(false);
		effect2.SetActive(false);
	

	}

	void Falcon()
	{
		falcon.SetActive(true);
		
		
	}

	void Effect1()
	{
		effect1.SetActive(true);
		
		
	}

	void Effect2()
	{
		effect2.SetActive(true);
		
		
	}

	void Destroy()
	{
		falcon.SetActive(false);
		effect1.SetActive(false);
		effect2.SetActive(false);
	
	
	}
 
}
