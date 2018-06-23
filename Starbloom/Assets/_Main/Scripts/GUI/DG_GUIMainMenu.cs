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

    [Header("Online Toggle")]
    public UnityEngine.UI.Toggle OnlineToggle;


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

        if (QuickFind.GameSettings.BypassMainMenu)
            QuickFind.NetworkMaster.StartGame();

        OnlineToggle.isOn = QuickFind.GameSettings.PlayOnline;
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
        foreach (DG_TextStatic TS in StaticTextArray)
            TS.ManualLoad();

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





    public void GameStart(int CharacterID, int CharacterGender)
    {
        if (CharacterID == -1) { Debug.Log("No Char was selected, assigning new."); CharacterID = QuickFind.CharacterManager.GetAvailablePlayerID(); }
        if (CharacterGender == -1) { Debug.Log("No Gender was selected, assigning new."); CharacterGender = Random.Range(0, 2); }
        AwaitingResponse = true;

        int[] OutData = new int[3];
        OutData[0] = QuickFind.NetworkSync.UserID;
        OutData[1] = CharacterID;
        OutData[2] = CharacterGender;

        QuickFind.NetworkSync.GenerateNewChar(OutData);
    }
    public void ReturnCharacterGenerated(int[] IncomingData)
    {
        DG_NetworkSync.Users U = QuickFind.NetworkSync.GetUserByID(IncomingData[0]);
        U.PlayerCharacterID = IncomingData[1];
        int CharacterGender = IncomingData[2];

        QuickFind.CharacterManager.SpawnCharController(CharacterGender, U);

        if (!AwaitingResponse) return;
        AwaitingResponse = false;

        U.CharacterLink.ActivatePlayer();

        if (QuickFind.Farm.PlayerCharacters[QuickFind.NetworkSync.PlayerCharacterID].Name == string.Empty)
            QuickFind.Farm.PlayerCharacters[QuickFind.NetworkSync.PlayerCharacterID].Name = "Default Name " + QuickFind.NetworkSync.PlayerCharacterID.ToString();
        if (QuickFind.Farm.FarmName == string.Empty)
            QuickFind.Farm.FarmName = "Default Farm Name";

        MainMenuCam.enabled = false;
        QuickFind.PlayerCam.MainCam.enabled = true;
        QuickFind.EnableCanvas(UICanvas, false);
        QuickFind.GUI_MainOverview.OpenUI(true);

        if (!PhotonNetwork.isMasterClient)
        {
            QuickFind.WeatherHandler.RequestMasterWeather();
            QuickFind.TimeHandler.RequestMasterTimes();
        }

        QuickFind.GUI_Inventory.UpdateInventoryVisuals();
        QuickFind.GUI_Inventory.SetHotbarSlot(QuickFind.GUI_Inventory.HotbarSlots[0]);

        QuickFind.NetworkSync.SetSelfInScene(QuickFind.SceneList.GetSceneIndexByString(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name));

        QuickFind.NetworkObjectManager.GenerateObjectData();
        QuickFind.GUI_MainOverview.SetMoneyValue(0, QuickFind.Farm.SharedMoney, true);
        QuickFind.GUI_MainOverview.SetGuiDayValue(QuickFind.Farm.Month, QuickFind.Farm.Day);

        QuickFind.FadeScreen.FadeIn(DG_GUI_FadeScreen.FadeInSpeeds.NormalFade);
    }







    //Network Call
    public void Connected()
    {
        if (!AwaitingResponse)
            return;

        AwaitingResponse = false;


        if (QuickFind.CharacterManager.DebugCharacters != null)
        {
            for (int i = 0; i < QuickFind.CharacterManager.DebugCharacters.Length; i++)
                Destroy(QuickFind.CharacterManager.DebugCharacters[i]);
        }

        if (QuickFind.GameSettings.BypassMainMenu)
            GameStart(-1,-1);
        else
            OpenCharacterCreationScreen();
    }
}
