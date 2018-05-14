using UnityEngine;
using System.Collections;

public class characterButton : MonoBehaviour {

	public GameObject frog;
	public GameObject GUI01;
	public GameObject GUI02;
	public GUISkin customSkin;

	
	
	private Rect FpsRect ;
	private string frpString;


	

	void Start () 
	{
	
			}
	
 void OnGUI() 
	{
		GUI.skin = customSkin;

		GUI.Box (new Rect (0, 0, 880, 156),"");
		
		if (GUI.Button(new Rect(30, 20, 70, 30),"Idle")){
		 frog.GetComponent<Animation>().wrapMode= WrapMode.Loop;
		  	frog.GetComponent<Animation>().CrossFade("VB_Idle");
	  }
		if (GUI.Button(new Rect(105, 20, 70, 30),"Greeting")){
		 frog.GetComponent<Animation>().wrapMode= WrapMode.Loop;
			frog.GetComponent<Animation>().CrossFade("VB_Greeting");
	  }

		if (GUI.Button(new Rect(180, 20, 70, 30),"Talk")){
			frog.GetComponent<Animation>().wrapMode= WrapMode.Loop;
			frog.GetComponent<Animation>().CrossFade("VB_Talk");
		}

		if (GUI.Button(new Rect(255, 20, 70, 30),"Walk")){
			frog.GetComponent<Animation>().wrapMode= WrapMode.Loop;
			frog.GetComponent<Animation>().CrossFade("VB_Walk");
		}
		if (GUI.Button(new Rect(330, 20, 70, 30),"L_Walk")){
			frog.GetComponent<Animation>().wrapMode= WrapMode.Loop;
			frog.GetComponent<Animation>().CrossFade("VB_L_Walk");
		}


		if (GUI.Button(new Rect(405, 20, 70, 30),"R_Walk")){
		 frog.GetComponent<Animation>().wrapMode= WrapMode.Loop;
			frog.GetComponent<Animation>().CrossFade("VB_R_Walk");
	  }

		if (GUI.Button(new Rect(480, 20, 70, 30),"B_Walk")){
			frog.GetComponent<Animation>().wrapMode= WrapMode.Loop;
			frog.GetComponent<Animation>().CrossFade("VB_B_Walk");
		}
		if (GUI.Button(new Rect(555, 20, 70, 30),"Run")){
			frog.GetComponent<Animation>().wrapMode= WrapMode.Loop;
			frog.GetComponent<Animation>().CrossFade("VB_Run");
		}

		if (GUI.Button(new Rect(630, 20, 70, 30),"L_Run")){
			frog.GetComponent<Animation>().wrapMode= WrapMode.Loop;
			frog.GetComponent<Animation>().CrossFade("VB_L_Run");
		}

		if (GUI.Button(new Rect(705, 20, 70, 30),"R_Run")){
			frog.GetComponent<Animation>().wrapMode= WrapMode.Loop;
			frog.GetComponent<Animation>().CrossFade("VB_R_Run");
		}

		
		if (GUI.Button(new Rect(780, 20, 70, 30),"B_Run")){
			frog.GetComponent<Animation>().wrapMode= WrapMode.Loop;
			frog.GetComponent<Animation>().CrossFade("VB_B_Run");

		}

		if (GUI.Button(new Rect(30, 60, 70, 30),"Jump")){
			frog.GetComponent<Animation>().wrapMode= WrapMode.Loop;
			frog.GetComponent<Animation>().CrossFade("VB_Jump");
		}

		if (GUI.Button(new Rect(105, 60, 70, 30),"DrawDagger")){
			frog.GetComponent<Animation>().wrapMode= WrapMode.Loop;
			frog.GetComponent<Animation>().CrossFade("VB_DrawDagger");
		}

		if (GUI.Button(new Rect(180, 60, 70, 30),"AttackStandy")){
			frog.GetComponent<Animation>().wrapMode= WrapMode.Once;
			frog.GetComponent<Animation>().CrossFade("VB_Attackstandy");
		}

		if (GUI.Button(new Rect(255, 60, 70, 30),"Attack")){
			frog.GetComponent<Animation>().wrapMode= WrapMode.Once;
			frog.GetComponent<Animation>().CrossFade("VB_Attack00");
		}
		if (GUI.Button(new Rect(330, 60, 70, 30),"Attack01")){
			frog.GetComponent<Animation>().wrapMode= WrapMode.Loop;
			frog.GetComponent<Animation>().CrossFade("VB_Attack01");
		}

		if (GUI.Button(new Rect(405, 60, 70, 30),"Attack02")){
			frog.GetComponent<Animation>().wrapMode= WrapMode.Loop;
			frog.GetComponent<Animation>().CrossFade("VB_Attack02");
		}

		if (GUI.Button(new Rect(480, 60, 70, 30),"Combo")){
			frog.GetComponent<Animation>().wrapMode= WrapMode.Loop;
			frog.GetComponent<Animation>().CrossFade("VB_Combo");
		}

		if (GUI.Button(new Rect(555, 60, 70, 30),"Kick")){
			frog.GetComponent<Animation>().wrapMode= WrapMode.Loop;
			frog.GetComponent<Animation>().CrossFade("VB_Kick");
		}

		if (GUI.Button(new Rect(630, 60, 70, 30),"Skill")){
			frog.GetComponent<Animation>().wrapMode= WrapMode.Loop;
			frog.GetComponent<Animation>().CrossFade("VB_Skill");
		}

		if (GUI.Button(new Rect(705, 60, 70, 30),"M_Avoid")){
			frog.GetComponent<Animation>().wrapMode= WrapMode.Loop;
			frog.GetComponent<Animation>().CrossFade("VB_M_Avoid");
		}

		if (GUI.Button(new Rect(780, 60, 70, 30),"L_Avoid")){
			frog.GetComponent<Animation>().wrapMode= WrapMode.Loop;
			frog.GetComponent<Animation>().CrossFade("VB_L_Avoid");
		}

		if (GUI.Button(new Rect(30, 100, 70, 30),"R_Avoid")){
			frog.GetComponent<Animation>().wrapMode= WrapMode.Loop;
			frog.GetComponent<Animation>().CrossFade("VB_R_Avoid");
		}

		if (GUI.Button(new Rect(105, 100, 70, 30),"Buff")){
			frog.GetComponent<Animation>().wrapMode= WrapMode.Loop;
			frog.GetComponent<Animation>().CrossFade("VB_Buff");
		}

		if (GUI.Button(new Rect(180, 100, 70, 30),"Run01")){
			frog.GetComponent<Animation>().wrapMode= WrapMode.Loop;
			frog.GetComponent<Animation>().CrossFade("VB_Run01");
		}

		if (GUI.Button(new Rect(255, 100, 70, 30),"RunAttack")){
			frog.GetComponent<Animation>().wrapMode= WrapMode.Loop;
			frog.GetComponent<Animation>().CrossFade("VB_RunAttack");
		}

		if (GUI.Button(new Rect(330, 100, 70, 30),"Sliding")){
			frog.GetComponent<Animation>().wrapMode= WrapMode.Loop;
			frog.GetComponent<Animation>().CrossFade("VB_Sliding");
		}

		if (GUI.Button(new Rect(405, 100, 70, 30),"Rappel")){
			frog.GetComponent<Animation>().wrapMode= WrapMode.Loop;
			frog.GetComponent<Animation>().CrossFade("VB_Rappel");
		}

		if (GUI.Button(new Rect(480, 100, 70, 30),"L_Run01")){
			frog.GetComponent<Animation>().wrapMode= WrapMode.Loop;
			frog.GetComponent<Animation>().CrossFade("VB_L_Run01");
		}

		if (GUI.Button(new Rect(555, 100, 70, 30),"R_Run01")){
			frog.GetComponent<Animation>().wrapMode= WrapMode.Loop;
			frog.GetComponent<Animation>().CrossFade("VB_R_Run01");
		}

		if (GUI.Button(new Rect(630, 100, 70, 30),"B_Run01")){
			frog.GetComponent<Animation>().wrapMode= WrapMode.Loop;
			frog.GetComponent<Animation>().CrossFade("VB_B_Run01");
		}

		if (GUI.Button(new Rect(705, 100, 70, 30),"Jump01")){
			frog.GetComponent<Animation>().wrapMode= WrapMode.Loop;
			frog.GetComponent<Animation>().CrossFade("VB_Jump01");
		}

		if (GUI.Button(new Rect(780, 100, 70, 30),"PickUp")){
			frog.GetComponent<Animation>().wrapMode= WrapMode.Loop;
			frog.GetComponent<Animation>().CrossFade("VB_Pickup");
		}


		//---------------------------------------------------------

		if (GUI.Button(new Rect(790, 160, 30, 30),"1")){

			GUI01.SetActive(true);
			GUI02.SetActive(false);

		}

		if (GUI.Button(new Rect(825, 160, 30, 30),"2")){

			GUI01.SetActive(false);
			GUI02.SetActive(true);
			
		}


	    
				if (GUI.Button (new Rect (20, 580, 140, 40), "Ver 2.6")) {
						frog.GetComponent<Animation>().wrapMode = WrapMode.Loop;
			frog.GetComponent<Animation>().CrossFade ("VB_Idle");
				}

	
		
 }
	
	// Update is called once per frame
	void Update () 
	{
		
	
	if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();

	}





	
}
