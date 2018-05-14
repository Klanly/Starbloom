using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_MainMenuItem : MonoBehaviour {


    public DG_GUIMainMenu MainMenu;
    public DG_GUIMainMenu.MainMenuButtons ButtonType;


    public void ButtonHit()
    {
        MainMenu.ButtonHit(this);
    }
}
