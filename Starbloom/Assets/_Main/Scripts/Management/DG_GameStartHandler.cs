using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_GameStartHandler : MonoBehaviour {



    public Camera MainMenuCam = null;
    [System.NonSerialized] public bool GameIsConnected = false;
    int AwaitingSpawnOrder = -1;


    private void Awake() { QuickFind.GameStartHandler = this; }

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

        this.enabled = false;
    }


    //Network Call
    public void Connected()
    {
        QuickFind.NetworkSync.NetID = PhotonNetwork.player.ID;

        if (QuickFind.CharacterManager.DebugCharacters != null)
        {
            for (int i = 0; i < QuickFind.CharacterManager.DebugCharacters.Length; i++)
                Destroy(QuickFind.CharacterManager.DebugCharacters[i]);
        }

        if (QuickFind.GameSettings.BypassMainMenu) CreateNewPlayer(GetAvailablePlayerID());
        else QuickFind.PlayerSelectionScreen.OpenPlayerSelectionScreen();
    }





    public void CreateNewPlayer(int DesiredPlayerID)
    {
        int[] OutData = new int[3];
        OutData[0] = PhotonNetwork.player.ID;
        OutData[1] = DesiredPlayerID;

        if (!QuickFind.Farm.PlayerCharacters[DesiredPlayerID].CharacterCreated)
        {
            int Gender = (int)QuickFind.GameSettings.GeneratedGender; if (Gender == 2) Gender = Random.Range(0, 2);
            OutData[2] = Gender;
        }

        QuickFind.NetworkSync.GenerateNewChar(OutData);
    }


    public void ReturnCharacterGenerated(int[] IncomingData)
    {
        DG_NetworkSync.Users U = new DG_NetworkSync.Users();
        QuickFind.NetworkSync.UserList.Add(U);
        U.NetID = IncomingData[0];
        U.PlayerCharacterID = IncomingData[1];
        U.SceneID = -1;

        bool NewChar = false;
        if(!QuickFind.Farm.PlayerCharacters[U.PlayerCharacterID].CharacterCreated)
        {
            QuickFind.Farm.PlayerCharacters[U.PlayerCharacterID].CharacterCreated = true;
            QuickFind.Farm.PlayerCharacters[U.PlayerCharacterID].CharacterGender = (DG_PlayerCharacters.GenderValue)IncomingData[2];
            QuickFind.ClothingHairManager.SetGameStartDefaultValues(U, (DG_PlayerCharacters.GenderValue)IncomingData[2]);
            NewChar = true;
        }

        if (U.NetID != PhotonNetwork.player.ID) { return; }

        GameIsConnected = true;
        if (QuickFind.NetworkSync.Player1PlayerCharacter == -1) QuickFind.NetworkSync.Player1PlayerCharacter = U.PlayerCharacterID;
        else QuickFind.NetworkSync.Player2PlayerCharacter = U.PlayerCharacterID;

        AwaitingSpawnOrder = U.PlayerCharacterID;

        if (QuickFind.GameSettings.BypassMainMenu || !NewChar) CharacterFinishedBeingCreated();
        else QuickFind.CharacterCreation.OpenCharacterCreation(true, U.PlayerCharacterID);
    }


    public void CharacterFinishedBeingCreated() { QuickFind.NetworkSync.NewPlayerFinishedEditingChar(); }

    public void SpawnUsers()
    {
        DG_NetworkSync.Users LoadAfterSpawn = null;
        for (int i = 0; i < QuickFind.NetworkSync.UserList.Count; i++)
        {
            DG_NetworkSync.Users U = QuickFind.NetworkSync.UserList[i];
            if(U.CharacterLink == null)
            {
                DG_PlayerCharacters.PlayerCharacter PC = QuickFind.Farm.PlayerCharacters[U.PlayerCharacterID];
                QuickFind.CharacterManager.SpawnCharController(PC.CharacterGender, U);
                QuickFind.ClothingHairManager.PlayerJoined(U);

                if (U.PlayerCharacterID == AwaitingSpawnOrder) LoadAfterSpawn = U;
            }
        }
        if (LoadAfterSpawn != null) LoadEverythingAfterPlayerIsReadyToGo(LoadAfterSpawn.PlayerCharacterID);
    }

    void LoadEverythingAfterPlayerIsReadyToGo(int PlayerID)
    {
        AwaitingSpawnOrder = -1;
        int ArrayNum = 0;
        if (PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) ArrayNum = 1;

        DG_NetworkSync.Users U = QuickFind.NetworkSync.GetUserByPlayerID(PlayerID);
        U.CharacterLink.ActivatePlayer();

        if (QuickFind.Farm.FarmName == string.Empty) QuickFind.Farm.FarmName = "Default Farm Name";

        U.CharacterLink.PlayerCam = QuickFind.PlayerCam.CameraRiggs[ArrayNum];
        QuickFind.PlayerCam.CameraRiggs[ArrayNum].PlayerID = PlayerID;


        MainMenuCam.enabled = false;
        QuickFind.PlayerCam.EnableCamera(U.CharacterLink.PlayerCam, true);
        QuickFind.GUI_MainOverview.OpenMainOverlay();
        QuickFind.GUI_MainOverview.OpenUI(true, PlayerID);
        QuickFind.GUI_Inventory.UpdateInventoryVisuals(PlayerID);
        QuickFind.GUI_Inventory.SetHotbarSlot(QuickFind.GUI_Inventory.PlayersInventory[ArrayNum].HotbarSlots[0]);

        if (ArrayNum == 0)
        {
            if (!PhotonNetwork.isMasterClient)
            {
                QuickFind.WeatherHandler.RequestMasterWeather();
                QuickFind.TimeHandler.RequestMasterTimes();
            }
            QuickFind.NetworkObjectManager.GenerateObjectData();
            QuickFind.GUI_MainOverview.SetMoneyValue(0, QuickFind.Farm.SharedMoney, true);
            QuickFind.GUI_MainOverview.SetGuiDayValue(QuickFind.Farm.Month, QuickFind.Farm.Day);
        }

        //Set Starting Scene here.
        QuickFind.SceneTransitionHandler.TriggerSceneChange(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, 0, PlayerID);
    }









    public int GetAvailablePlayerID()
    {
        for (int i = 0; i < int.MaxValue; i++)
        {
            bool Allow = true;
            for (int iN = 0; iN < QuickFind.NetworkSync.UserList.Count; iN++)
            {
                if (QuickFind.NetworkSync.UserList[iN].PlayerCharacterID == i)
                {
                    Allow = false;
                    break;
                }
            }

            if (Allow)
                return i;
        }
        return 0;
    }
}
