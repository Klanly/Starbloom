using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_GameStartHandler : MonoBehaviour {


    public class PlayerCreation
    {
        public int PlayerID = -2;
        public int DesiredPlayerID = 0;
        public int DesiredCharacterGender = -2;
    }


    public Camera MainMenuCam = null;

    bool AwaitingResponse = true;
    PlayerCreation Player1;
    PlayerCreation Player2;
    [System.NonSerialized] public bool GameHasStarted = false;


    private void Awake() { QuickFind.GameStartHandler = this; Player1 = new PlayerCreation(); Player2 = new PlayerCreation(); }

    private void Start()
    {
        if (!QuickFind.GameSettings.BypassMainMenu)
        {
            MainMenuCam.enabled = true;
            MainMenuCam.gameObject.SetActive(true);
            QuickFind.PlayerCam.EnableCamera(QuickFind.PlayerCam.CameraRiggs[0], false, true);
        }

        if (QuickFind.GameSettings.BypassMainMenu)
            QuickFind.NetworkMaster.StartGame();
    }








    public void TriggerGameStart(bool isPlayer1, int DesiredPlayerID, int DesiredGender, int PlayerID)
    {
        PlayerCreation Player;
        if (isPlayer1) Player = Player1;
        else Player = Player2;

        Player.PlayerID = PlayerID;
        Player.DesiredPlayerID = DesiredPlayerID;
        Player.DesiredPlayerID = DesiredGender;

        if (isPlayer1)
            QuickFind.FadeScreen.FadeOut(DG_GUI_FadeScreen.FadeInSpeeds.QuickFade, this.gameObject, "FadeOutCompleteP1");
        else
            QuickFind.FadeScreen.FadeOut(DG_GUI_FadeScreen.FadeInSpeeds.QuickFade, this.gameObject, "FadeOutCompleteP2");
    }

    public void FadeOutCompleteP1()
    {
        GameStart(Player1);
    }
    public void FadeOutCompleteP2()
    {
        GameStart(Player2);
    }

    public void GameStart(PlayerCreation Player)
    {
        if (Player.DesiredPlayerID == 0)
        {
            if (PhotonNetwork.isMasterClient) Player.DesiredPlayerID = 0;
            else Player.DesiredPlayerID = QuickFind.CharacterManager.GetAvailablePlayerID();
        }
        if (Player.DesiredCharacterGender == -2)
        {
            if(QuickFind.GameSettings.GeneratedGender != DG_PlayerCharacters.GenderValue.Both) Player.DesiredCharacterGender = (int)QuickFind.GameSettings.GeneratedGender;
            else Player.DesiredCharacterGender = Random.Range(0, 2);
        }
        AwaitingResponse = true;

        int[] OutData = new int[3];
        OutData[0] = QuickFind.NetworkSync.NetID;
        OutData[1] = Player.DesiredPlayerID;
        OutData[2] = Player.DesiredCharacterGender;

        QuickFind.NetworkSync.GenerateNewChar(OutData);
    }
    public void ReturnCharacterGenerated(int[] IncomingData)
    {
        DG_NetworkSync.Users FoundUser = null;
        for (int i = 0; i < QuickFind.NetworkSync.UserList.Count; i++)
        {
            DG_NetworkSync.Users UserI = QuickFind.NetworkSync.UserList[i];
            if (UserI.PlayerCharacterID == IncomingData[1] || UserI.PlayerCharacterID == -1) { FoundUser = UserI; break; }
        }
        if (FoundUser == null) { FoundUser = new DG_NetworkSync.Users(); QuickFind.NetworkSync.UserList.Add(FoundUser); }
        FoundUser.NetID = IncomingData[0];
        FoundUser.PlayerCharacterID = IncomingData[1];
        FoundUser.SceneID = -1;

        int CharacterGender = IncomingData[2];
        QuickFind.Farm.PlayerCharacters[FoundUser.PlayerCharacterID].CharacterGender = (DG_PlayerCharacters.GenderValue)CharacterGender;
        QuickFind.CharacterManager.SpawnCharController(CharacterGender, FoundUser);




        if (!AwaitingResponse) return;
        if (FoundUser.NetID != QuickFind.NetworkSync.NetID) return;


        if (QuickFind.NetworkSync.Player1PlayerCharacter == -1)
        {
            QuickFind.NetworkSync.Player1PlayerCharacter = FoundUser.PlayerCharacterID;
            QuickFind.PlayerTrans = FoundUser.CharacterLink.PlayerTrans;
            FoundUser.CharacterLink.PlayerCam = QuickFind.PlayerCam.CameraRiggs[0];
            QuickFind.PlayerCam.CameraRiggs[0].PlayerID = FoundUser.PlayerCharacterID;
        }
        else
        {
            QuickFind.NetworkSync.Player2PlayerCharacter = FoundUser.PlayerCharacterID;
            FoundUser.CharacterLink.PlayerCam = QuickFind.PlayerCam.CameraRiggs[1];
            QuickFind.PlayerCam.CameraRiggs[1].PlayerID = FoundUser.PlayerCharacterID;
        }

        AwaitingResponse = false;
        GameHasStarted = true;

        if (QuickFind.Farm.FarmName == string.Empty) QuickFind.Farm.FarmName = "Default Farm Name";


        FoundUser.CharacterLink.ActivatePlayer();
        QuickFind.CharacterManager.GameStartSpawnClothing();

        if (QuickFind.GameSettings.BypassMainMenu) CharacterFinishedBeingCreated(FoundUser.PlayerCharacterID);
        else QuickFind.MainMenuUI.OpenCharacterCreationScreen();
    }
    public void CharacterFinishedBeingCreated(int PlayerID)
    {
        MainMenuCam.enabled = false;
        QuickFind.PlayerCam.EnableCamera(QuickFind.NetworkSync.GetCharacterLinkByPlayerID(PlayerID).PlayerCam, true);
        QuickFind.EnableCanvas(QuickFind.MainMenuUI.UICanvas, false);
        QuickFind.GUI_MainOverview.OpenUI(true);

        if (!PhotonNetwork.isMasterClient)
        {
            QuickFind.WeatherHandler.RequestMasterWeather();
            QuickFind.TimeHandler.RequestMasterTimes();
        }

        QuickFind.GUI_Inventory.UpdateInventoryVisuals();
        QuickFind.GUI_Inventory.SetHotbarSlot(QuickFind.GUI_Inventory.HotbarSlots[0]);

        QuickFind.NetworkObjectManager.GenerateObjectData();
        QuickFind.GUI_MainOverview.SetMoneyValue(0, QuickFind.Farm.SharedMoney, true);
        QuickFind.GUI_MainOverview.SetGuiDayValue(QuickFind.Farm.Month, QuickFind.Farm.Day);
        //

        //Set Starting Scene here.
        QuickFind.SceneTransitionHandler.TriggerSceneChange(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, 0, PlayerID);
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
            GameStart(Player1);
        else
            QuickFind.MainMenuUI.OpenPlayerSelectScreen();
    }

}
