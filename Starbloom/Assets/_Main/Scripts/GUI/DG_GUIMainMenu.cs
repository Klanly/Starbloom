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


    public Camera MainMenuCam = null;

    [Header("Canvases")]
    public CanvasGroup UICanvas = null;

    [Header("Static Text")]
    public DG_TextStatic[] StaticTextArray;


    bool AwaitingResponse = true;



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

        transform.localPosition = Vector3.zero;

        foreach (DG_TextStatic TS in StaticTextArray)
            TS.ManualLoad();

        if (QuickFind.GameSettings.BypassMainMenu)
            QuickFind.NetworkMaster.StartGame();
    }




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
        QuickFind.EnableCanvas(UICanvas, false);
        QuickFind.NetworkMaster.CreateNewRoom = true;
        QuickFind.NetworkMaster.StartGame();
    }
    public void OpenCharacterCreationScreen()
    {
        QuickFind.CharacterCreation.OpenCharacterCreation(true);
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





    public void TriggerGameStart()
    {
        QuickFind.FadeScreen.FadeOut(DG_GUI_FadeScreen.FadeInSpeeds.QuickFade, this.gameObject, "GameStart");
    }

    public void GameStart()
    {
        GameObject newPlayerObject = PhotonNetwork.Instantiate("MainPlayer", Vector3.zero, Quaternion.identity, 0);
        DG_CharacterLink CL = newPlayerObject.GetComponent<DG_CharacterLink>();
        CL.ActivatePlayer();


        MainMenuCam.enabled = false;
        QuickFind.PlayerCam.MainCam.enabled = true;
        QuickFind.EnableCanvas(UICanvas, false);
        QuickFind.GUI_MainOverview.OpenUI(true);

        if (!PhotonNetwork.isMasterClient)
        {
            QuickFind.WeatherHandler.RequestMasterWeather();
            QuickFind.TimeHandler.RequestMasterTimes();
        }

        if (QuickFind.GameSettings.BypassMainMenu && !PhotonNetwork.isMasterClient)
            QuickFind.NetworkSync.PlayerCharacterID = QuickFind.CharacterManager.GetAvailablePlayerID();

        QuickFind.GUI_Inventory.UpdateInventoryVisuals();
        QuickFind.GUI_Inventory.SetHotbarSlot(QuickFind.GUI_Inventory.HotbarSlots[0]);


        QuickFind.NetworkObjectManager.GenerateObjectData();

        QuickFind.FadeScreen.FadeIn(DG_GUI_FadeScreen.FadeInSpeeds.NormalFade);
    }




    //Network Call
    public void Connected()
    {
        if (!AwaitingResponse)
            return;

        AwaitingResponse = false;


        if (QuickFind.CharacterManager.DebugCharacter != null)
        {
            for (int i = 0; i < transform.childCount; i++)
                Destroy(QuickFind.CharacterManager.DebugCharacter);
        }

        if (QuickFind.GameSettings.BypassMainMenu)
            GameStart();
        else
            OpenCharacterCreationScreen();
    }
}
