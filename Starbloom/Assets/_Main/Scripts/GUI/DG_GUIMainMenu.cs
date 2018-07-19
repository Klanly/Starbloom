using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class DG_GUIMainMenu : Photon.MonoBehaviour
{
    public enum MainMenuButtons
    {
        New,
        Load,
        Mulitplayer,
        Quit
    }


    public bool isPlayer1;

    [Header("Canvases")]
    public CanvasGroup UICanvas = null;

    [Header("Static Text")]
    public DG_TextStatic[] StaticTextArray;

    [Header("Online Toggle")]
    public UnityEngine.UI.Toggle OnlineToggle;






    private void Awake()
    {
        QuickFind.MainMenuUI = this;
    }
    private void Start()
    {
        if (!QuickFind.GameSettings.BypassMainMenu) QuickFind.EnableCanvas(UICanvas, true);
        else QuickFind.EnableCanvas(UICanvas, false);
        transform.localPosition = Vector3.zero;
        OnlineToggle.isOn = QuickFind.GameSettings.PlayOnline;

        foreach (DG_TextStatic TS in StaticTextArray)
            TS.ManualLoad();
    }





    public void OpenPlayerSelectScreen()
    { Debug.Log("Select Player to Play as HERE! :)"); }

    public void OpenCharacterCreationScreen()
    { QuickFind.CharacterCreation.OpenCharacterCreation(true); }


    public void ButtonHit(DG_MainMenuItem ItemRoot)
    {
        switch(ItemRoot.ButtonType)
        {
            case MainMenuButtons.New: NewGameMenu(); break;
            case MainMenuButtons.Load: break;
            case MainMenuButtons.Mulitplayer: JoinGameMultiplayer(); break;
            case MainMenuButtons.Quit: Quit(); break;
        }
    }
    public void ToggleOnline(UnityEngine.UI.Toggle Tog)
    {
        QuickFind.GameSettings.PlayOnline = Tog.isOn;
    }


    void NewGameMenu()
    {
        foreach (DG_TextStatic TS in StaticTextArray)
            TS.ManualLoad();

        QuickFind.EnableCanvas(UICanvas, false);
        QuickFind.NetworkMaster.CreateNewRoom = true;
        QuickFind.NetworkMaster.StartGame();
    }

    void LoadGameMenu()
    {

    }

    void JoinGameMultiplayer()
    {
        QuickFind.EnableCanvas(UICanvas, false);
        QuickFind.NetworkMaster.CreateNewRoom = false;
        QuickFind.NetworkMaster.StartGame();
    }

    void Quit()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
