//************************************COPYRIGHT 2016 CURTIS MARTINDALE***********************************
//***********************************************Version 1.1*********************************************
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class levelControl : MonoBehaviour {

	public GameObject HUDoption;
	public GameObject HUDtext;
	public bool hudOpen;
	public node_PlayerMovement playerMove;

	void Start()
	{
		HUDoption.SetActive(true);
		HUDtext.SetActive(false);
		hudOpen = false;

	}

    void Update()
    {

        //if (Input.GetButtonDown("Fire4"))
        //{
        //    if (hudOpen)
        //    {
        //        HUDoption.SetActive(true);
        //        HUDtext.SetActive(false);
        //    }
        //    else
        //    {
        //        HUDoption.SetActive(false);
        //        HUDtext.SetActive(true);
        //    }
        //    hudOpen = !hudOpen;
        //}

        //Press P to reload the level. Useful for testing
        if (Input.GetKeyDown(KeyCode.P))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            playerMove.useGamePad = !playerMove.useGamePad;
        }
        //Press ESC to Exit Application
        //if (Input.GetKey("escape"))
        //{
        //    Application.Quit();
        //}
    }
}
