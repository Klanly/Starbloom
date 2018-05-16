using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class DG_GUIMainMenu : MonoBehaviour {


    public enum MainMenuButtons
    {
        New,
        Load,
        Mulitplayer,
        Quit
    }


    public Camera MainMenuCam = null;


    [Header("Canvases")]
    public CanvasGroup UICanvas = null;
    public CanvasGroup CharUICanvas = null;


    [Header("Static Text")]
    public DG_TextStatic[] StaticTextArray;





    private void Awake()
    {
        QuickFind.MainMenuUI = this;
    }

    private void Start()
    {
        if (!QuickFind.GameSettings.BypassMainMenu)
        {
            MainMenuCam.enabled = true;
            MainMenuCam.gameObject.SetActive(true);
            QuickFind.PlayerCam.MainCam.enabled = false;
            QuickFind.EnableCanvas(UICanvas, true);
        }
            
        QuickFind.EnableCanvas(CharUICanvas, false);

        transform.localPosition = Vector3.zero;

        foreach (DG_TextStatic TS in StaticTextArray)
            TS.ManualLoad();

        if (QuickFind.GameSettings.BypassMainMenu)
            FadeComplete();
    }




    public void ButtonHit(DG_MainMenuItem ItemRoot)
    {
        switch(ItemRoot.ButtonType)
        {
            case MainMenuButtons.New: NewGameMenu(); break;
            case MainMenuButtons.Load: break;
            case MainMenuButtons.Mulitplayer: break;
            case MainMenuButtons.Quit: Quit(); break;
        }
    }


    void NewGameMenu()
    {
        TriggerFade();
        //QuickFind.EnableCanvas(UICanvas, false);
    }

    void LoadGameMenu()
    {

    }

    void MultiplayerMenu()
    {

    }

    void Quit()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }





    void TriggerFade()
    {
        QuickFind.FadeScreen.FadeOut(DG_GUI_FadeScreen.FadeInSpeeds.QuickFade, this.gameObject, "FadeComplete");
    }

    public void FadeComplete()
    {
        MainMenuCam.enabled = false;
        QuickFind.PlayerCam.MainCam.enabled = true;
        QuickFind.EnableCanvas(UICanvas, false);
        QuickFind.NetworkMaster.StartGame();
    }
}
