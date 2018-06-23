﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class DG_NetworkSync : Photon.MonoBehaviour
{
    [System.Serializable]
    public class Users
    {
        public int ID;
        public int PlayerCharacterID;
        public int SceneID;
        public DG_CharacterLink CharacterLink;
        public DG_MovementSync MoveSync;
    }

    public int UserID = 0;
    [HideInInspector] public int PlayerCharacterID;
    [HideInInspector] public int CurrentScene;
    [HideInInspector] public int PhotonViewID;
    bool AwaitingSync = false;
    bool FirstLoaded = false;

    public List<Users> UserList;




    PhotonView PV;




    private void Awake()
    {
        UserList = new List<Users>();

        if (transform.parent != null)
            Destroy(this.gameObject);
        else
        {
            if (!PhotonNetwork.offlineMode)
                QueueNetConnected();
        }
    }
    private void Start()
    {
        if (PhotonNetwork.offlineMode)
            QueueNetConnected();
    }
    void QueueNetConnected()
    {
        PV = transform.GetComponent<PhotonView>();
        transform.SetParent(QuickFind.NetworkMaster.transform);
        QuickFind.NetworkSync = this;

        AwaitingSync = true;
        PV.RPC("SetNewID", PhotonTargets.MasterClient);
    }




    //Network Messages
    //////////////////////////////////////////////////////////////////////////////////////////////////




    #region Get Helpers

    public Users GetUserByID(int ID)
    {
        for (int i = 0; i < UserList.Count; i++)
            { if (UserList[i].ID == ID) return UserList[i]; }
        return null;
    }
    public Users GetUserByPlayerID(int ID)
    {
        for (int i = 0; i < UserList.Count; i++)
        { if (UserList[i].PlayerCharacterID == ID) return UserList[i]; }
        return null;
    }
    public Users GetUserByMovementSync(DG_MovementSync Sync)
    {
        for (int i = 0; i < UserList.Count; i++)
        { if (UserList[i].MoveSync == Sync) return UserList[i]; }
        return null;
    }

    public DG_MovementSync GetCharacterMoveSyncByUserID(int ID)
    {
        Users U = GetUserByID(ID);
        return U.MoveSync;
    }

    #endregion



    #region Users
    //////////////////////////////////////////////////////

    [PunRPC]
    void SetNewID()
    {
        Users NewUser = new Users();

        int NewID;
        if (UserList.Count != 0)
            NewID = UserList[UserList.Count - 1].ID + 1;
        else
            NewID = 1;

        NewUser.ID = NewID;
        UserList.Add(NewUser);

        List<int> TransferInts = new List<int>();
        TransferInts.Add(NewUser.ID);
        TransferInts.Add(UserList.Count);
        for (int i = 0; i < UserList.Count; i++)
        {
            TransferInts.Add(UserList[i].ID);
            TransferInts.Add(UserList[i].PlayerCharacterID);
            TransferInts.Add(UserList[i].SceneID);
            TransferInts.Add((int)QuickFind.Farm.PlayerCharacters[UserList[i].PlayerCharacterID].Visuals.CharacterGender);
        }

        UserList.Remove(NewUser);

        PV.RPC("SendOutIDList", PhotonTargets.All, TransferInts.ToArray());
    }
    [PunRPC]
    void SendOutIDList(int[] TransferedIn)
    {
        //UserList.Clear();

        if (UserID == 0) { UserID = TransferedIn[0];}

        int UserCount = TransferedIn[1];
        if (UserCount == 1) UserList.Clear();
        int index = 2;
        for(int i = 0; i < UserCount; i++)
        {
            if (GetUserByID(TransferedIn[index]) != null) { index = index + 4;  continue; }

            Users NewUser = new Users();
            NewUser.ID = TransferedIn[index]; index++;
            NewUser.PlayerCharacterID = TransferedIn[index]; index++;
            NewUser.SceneID = TransferedIn[index]; index++;

            if (NewUser.ID == UserID || FirstLoaded) { UserList.Add(NewUser); index++; continue; }
            QuickFind.CharacterManager.SpawnCharController(TransferedIn[index], NewUser); index++;
            UserList.Add(NewUser);
        }

        if (AwaitingSync)
        {
            AwaitingSync = false;
            FirstLoaded = true;

            Debug.Log("UserID == " + UserID.ToString());
            Debug.Log("Connected Online == " + QuickFind.GameSettings.PlayOnline.ToString());

            if (!PhotonNetwork.isMasterClient)
                QuickFind.NetworkSync.RequestPlayerDataSync();
            else
                QuickFind.MainMenuUI.Connected();
        }
    }





    public void SetSelfInScene(int NewScene)
    {
        QuickFind.NetworkSync.CurrentScene = NewScene;
        CurrentScene = NewScene;
        int[] IntGroup = new int[2];
        IntGroup[0] = UserID;
        IntGroup[1] = NewScene;
        PV.RPC("SendUserInScene", PhotonTargets.All, IntGroup);
    }
    [PunRPC] void SendUserInScene(int[] IntGroup)
    {
        Users U = GetUserByID(IntGroup[0]);
        U.SceneID = IntGroup[1];
    }


    public void SetUserInBed(int OutData)
    {
        PV.RPC("UserSetInBed", PhotonTargets.All, OutData);
    }
    [PunRPC]
    void UserSetInBed(int InData)
    {
        Bed.AddSleepingPlayer(InData);
    }



    public void UpdatePlayerMovement(int[] OutgoingData)
    {
        PV.RPC("ReceivePlayerMovement", PhotonTargets.Others, OutgoingData);
    }
    [PunRPC]
    void ReceivePlayerMovement(int[] InData)
    {
        GetCharacterMoveSyncByUserID(InData[0]).UpdatePlayerPos(InData);
    }

    public void UpdatePlayerAnimationState(int[] OutgoingData)
    {
        PV.RPC("ReceivePlayerAnimationState", PhotonTargets.Others, OutgoingData);
    }
    [PunRPC]
    void ReceivePlayerAnimationState(int[] InData)
    {
        GetCharacterMoveSyncByUserID(InData[0]).AnimSync.UpdatePlayerAnimationState(InData);
    }


    public void GenerateNewChar(int[] OutgoingData)
    {
        PV.RPC("NewCharGenerationCalled", PhotonTargets.All, OutgoingData);
    }
    [PunRPC]
    void NewCharGenerationCalled(int[] InData)
    {
        QuickFind.MainMenuUI.ReturnCharacterGenerated(InData);
    }



    #endregion




    #region Inventory
    //////////////////////////////////////////////////////
    public void RequestPlayerDataSync()
    {
        PV.RPC("GatherPlayerData", PhotonTargets.MasterClient, PV.ownerId);
    }
    [PunRPC]
    void GatherPlayerData(int PhotonOwnerID)
    {
        PhotonPlayer PP = PhotonPlayer.Find(PhotonOwnerID);
        PV.RPC("GetPlayerStringValues", PP, QuickFind.SaveHandler.GatherPlayerDataStrings(false).ToArray());
        PV.RPC("GetPlayerIntValues", PP, QuickFind.SaveHandler.GatherPlayerDataInts(false).ToArray());
    }

    [PunRPC]
    void GetPlayerStringValues(string[] StringValues)
    {
        QuickFind.SaveHandler.GetStringValues(StringValues, false);
    }
    [PunRPC]
    void GetPlayerIntValues(int[] IntValues)
    {
        QuickFind.SaveHandler.GetIntValues(IntValues, false);

        if (!PhotonNetwork.isMasterClient)
            QuickFind.MainMenuUI.Connected();
    }





    public void SetRucksackValue(int PlayerID, int Slot, int ContainedItem, int CurrentStackActive, int LowValue, int NormalValue, int HighValue, int MaximumValue)
    {                                                            
        List<int> IntData = new List<int>();
        IntData.Add(PlayerID);
        IntData.Add(Slot);
        IntData.Add(ContainedItem);
        IntData.Add(CurrentStackActive);
        IntData.Add(LowValue);
        IntData.Add(NormalValue);
        IntData.Add(HighValue);
        IntData.Add(MaximumValue);

        PV.RPC("NewRucksackValue", PhotonTargets.All, IntData.ToArray());
    }
    [PunRPC]
    void NewRucksackValue(int[] info)
    {
        int index = 0;
        DG_PlayerCharacters.CharacterEquipment CE = QuickFind.Farm.PlayerCharacters[info[index]].Equipment; index++;
        DG_PlayerCharacters.RucksackSlot RS = CE.RucksackSlots[info[index]]; index++;
        RS.ContainedItem = info[index]; index++;
        RS.CurrentStackActive = info[index]; index++;
        RS.LowValue = info[index]; index++;
        RS.NormalValue = info[index]; index++;
        RS.HighValue = info[index]; index++;
        RS.MaximumValue = info[index]; index++;

        QuickFind.GUI_Inventory.UpdateInventoryVisuals();
    }

    public void SetStorageValue(int Scene, int NetObjectIndex, int Slot, int ContainedItem, int CurrentStackActive, int LowValue, int NormalValue, int HighValue, int MaximumValue)
    {
        List<int> IntData = new List<int>();
        IntData.Add(Scene);
        IntData.Add(NetObjectIndex);
        IntData.Add(Slot);
        IntData.Add(ContainedItem);
        IntData.Add(CurrentStackActive);
        IntData.Add(LowValue);
        IntData.Add(NormalValue);
        IntData.Add(HighValue);
        IntData.Add(MaximumValue);

        PV.RPC("NewStorageValue", PhotonTargets.All, IntData.ToArray());
    }
    [PunRPC]
    void NewStorageValue(int[] info)
    {
        int index = 0;
        NetworkScene NS = QuickFind.NetworkObjectManager.GetSceneByID(info[index]); index++;
        NetworkObject NO = NS.transform.GetChild(info[index]).GetComponent<NetworkObject>(); index++;
        DG_PlayerCharacters.RucksackSlot RS = NO.StorageSlots[info[index]]; index++;
        RS.ContainedItem = info[index]; index++;
        RS.CurrentStackActive = info[index]; index++;
        RS.LowValue = info[index]; index++;
        RS.NormalValue = info[index]; index++;
        RS.HighValue = info[index]; index++;
        RS.MaximumValue = info[index]; index++;

        QuickFind.StorageUI.UpdateStorageVisuals();
    }

    public void SetNewFarmMoneyValue(int NewValue) { PV.RPC("NewMoneyValueSet", PhotonTargets.All, NewValue); }
    [PunRPC] void NewMoneyValueSet(int NewValue) { QuickFind.MoneyHandler.ReceiveFarmMoneyValue(NewValue); }

    public void SetItemInShippingBin(int[] OutData){PV.RPC("ItemSetInShippingBin", PhotonTargets.All, OutData);}
    [PunRPC] void ItemSetInShippingBin(int[] InData) { QuickFind.ShippingBin.ItemSetInShippingBin(InData); }

    public void ClearBinItem(int OutData) { PV.RPC("ClearBinReceived", PhotonTargets.All, OutData); }
    [PunRPC] void ClearBinReceived(int InData) { QuickFind.ShippingBin.ClearBinItem(InData); }



    #endregion




    #region Weather
    //////////////////////////////////////////////////////
    public void AdjustWeather(int Season, int Weather)
    {
        List<int> WeatherNums = new List<int>();
        WeatherNums.Add(Season);
        WeatherNums.Add(Weather);
        PV.RPC("SendOutWeatherChange", PhotonTargets.All, WeatherNums.ToArray());
    }
    [PunRPC]
    void SendOutWeatherChange(int[] WeatherNums)
    {
        int Season = WeatherNums[0];
        int Weather = WeatherNums[1];
        QuickFind.WeatherHandler.AdjustSeason(Season, Weather);
    }
    //////////////////////////////////////////////////////
    public void RequestMasterWeather(){ PV.RPC("MasterSendForthWeather", PhotonTargets.All); }
    [PunRPC] void MasterSendForthWeather(){ if (PhotonNetwork.isMasterClient) QuickFind.WeatherHandler.SyncWeatherToMaster(); }
    //////////////////////////////////////////////////////
    public void SyncWeatherToMaster(int[] WeatherValues){ PV.RPC("SendOutWeatherByMaster", PhotonTargets.Others, WeatherValues); }
    [PunRPC] void SendOutWeatherByMaster(int[] WeatherValues){ QuickFind.WeatherHandler.GetMasterWeather(WeatherValues); }

    public void AdjustFutureWeather()
    {
        List<int> WeatherNums = new List<int>();
        WeatherNums.Add(QuickFind.Farm.Weather.TodayWeather);
        WeatherNums.Add(QuickFind.Farm.Weather.TomorrowWeather);
        WeatherNums.Add(QuickFind.Farm.Weather.TwoDayAwayWeather);
        PV.RPC("SendOutFutureWeather", PhotonTargets.Others, WeatherNums.ToArray());
    }
    [PunRPC]
    void SendOutFutureWeather(int[] WeatherNums)
    {
        QuickFind.Farm.Weather.TodayWeather = WeatherNums[0];
        QuickFind.Farm.Weather.TomorrowWeather = WeatherNums[1];
        QuickFind.Farm.Weather.TwoDayAwayWeather = WeatherNums[2];
        QuickFind.TimeHandler.NewDayCalculationsComplete();
    }


    //Time
    //////////////////////////////////////////////////////
    public void AdjustTimeByPreset(int Time){ PV.RPC("SendOutTimeByPreset", PhotonTargets.All, Time); }
    [PunRPC] void SendOutTimeByPreset(int Time){  QuickFind.TimeHandler.AdjustTimeByPreset(Time); }

    public void AdjustTimeByValues(int Year, int Month, int Day, int Hour, int Minute)
    {
        int[] OutInts = new int[5]; OutInts[0] = Year;  OutInts[1] = Month; OutInts[2] = Day; OutInts[3] = Hour; OutInts[4] = Minute;
        PV.RPC("SendOutTimeByValues", PhotonTargets.All, OutInts);
    }

    [PunRPC]
    void SendOutTimeByValues(int[] inInts)
    { QuickFind.TimeHandler.AdjustTimeByValues(inInts[0], inInts[1], inInts[2], inInts[3], inInts[4]);}

    //////////////////////////////////////////////////////
    public void RequestMasterTime()
    { PV.RPC("MasterSendForthTimes", PhotonTargets.All); }

    [PunRPC] void MasterSendForthTimes()
    { if(PhotonNetwork.isMasterClient) QuickFind.TimeHandler.SyncTimeToMaster(); }

    //////////////////////////////////////////////////////
    public void SyncTimeToMaster(float[] TimeValues)
    { PV.RPC("SendOutTimeByMaster", PhotonTargets.All, TimeValues); }

    [PunRPC] void SendOutTimeByMaster(float[] TimeValues)
    { QuickFind.TimeHandler.GetMasterTimes(TimeValues); }

    #endregion




    #region World Objects
    public void RequestWorldObjects()
    { PV.RPC("GatherWorldObjects", PhotonTargets.MasterClient, PV.ownerId); }
    [PunRPC]
    void GatherWorldObjects(int ReturnPhotonOwner)
    {
        PhotonPlayer PP = PhotonPlayer.Find(ReturnPhotonOwner);
        PV.RPC("SendOutWorldObjectInts", PP, QuickFind.SaveHandler.GatherWorldInts(false).ToArray());
    }
    [PunRPC]
    void SendOutWorldObjectInts(int[] IntValues)
    {
        QuickFind.SaveHandler.GetWorldInts(IntValues, false);
        QuickFind.NetworkObjectManager.GenerateSceneObjects();
    }

    public void RemoveNetworkSceneObject(int Scene, int ItemIndex)
    {
        int[] Sent = new int[2];
        Sent[0] = Scene;
        Sent[1] = ItemIndex;

        PV.RPC("RemoveSceneObject", PhotonTargets.All, Sent);
    }
    [PunRPC]
    void RemoveSceneObject(int[] Received)
    {
        NetworkObject NO = QuickFind.NetworkObjectManager.GetItemByID(Received[0], Received[1]);
        if(NO.SurrogateObjectIndex != 0)
        { NetworkObject NO2 = QuickFind.NetworkObjectManager.GetItemByID(Received[0], NO.SurrogateObjectIndex); NO2.SurrogateObjectIndex = 0; }
        NetworkScene NS = QuickFind.NetworkObjectManager.GetSceneByID(Received[0]);
        NS.NetworkObjectList.Remove(NO);

        DG_ItemObject IO = QuickFind.ItemDatabase.GetItemFromID(NO.ItemRefID);
        if (IO.UsePoolIDForSpawn)
            QuickFind.PrefabPool.SafeReturnNetworkPoolObject(NO);
        Destroy(NO.gameObject);
    }


    public void AddNetworkSceneObject(int[] Data)
    {   PV.RPC("AddSceneObject", PhotonTargets.All, Data); }
    [PunRPC]
    void AddSceneObject(int[] Data)
    { QuickFind.NetworkObjectManager.CreateSceneObject(Data); }


    public void WaterNetworkObject(int[] OutData)
    { PV.RPC("SendWatered", PhotonTargets.All, OutData); }
    [PunRPC]
    void SendWatered(int[] Data)
    { QuickFind.WateringSystem.WaterOne(Data); }

    public void SendHitBreakable(int[] OutData)
    { PV.RPC("SendBreakableHit", PhotonTargets.All, OutData);}
    [PunRPC]
    void SendBreakableHit(int[] Data)
    { QuickFind.BreakableObjectsHandler.ReceiveHitData(Data); }


    public void ClaimMagneticObject(int[] OutData)
    { PV.RPC("CharacterClaimedMagneticObject", PhotonTargets.All, OutData); }
    [PunRPC]
    public void CharacterClaimedMagneticObject(int[] Data)
    { GetUserByPlayerID(Data[0]).MoveSync.GetComponent<DG_MagnetAttraction>().ClaimObject(Data); }

    public void SetTilledSurrogate(int[] OutData)
    { PV.RPC("ReceiveSurrogateTilled", PhotonTargets.All, OutData); }
    [PunRPC]
    public void ReceiveSurrogateTilled(int[] Data)
    { QuickFind.ObjectPlacementManager.ReceiveTilledSurrogate(Data); }


    public void HarvestObject(int[] OutData)
    { PV.RPC("ReceivedHarvestedObject", PhotonTargets.All, OutData); }
    [PunRPC]
    public void ReceivedHarvestedObject(int[] Data)
    { QuickFind.NetworkGrowthHandler.ReceivedPlantHarvested(Data); }


    public void SignalTreeFall(int[] OutData)
    { PV.RPC("ReceiveTreeFall", PhotonTargets.All, OutData); }
    [PunRPC]
    public void ReceiveTreeFall(int[] Data)
    {
        NetworkObject NO = QuickFind.NetworkObjectManager.GetItemByID(Data[0], Data[1]);
        NO.transform.GetChild(0).GetComponent<DG_TreeFall>().ReceiveTreefallEvent(Data);
    }

    public void PlayDestroyEffect(int[] OutData)
    { PV.RPC("ReceiveDestroyEffect", PhotonTargets.All, OutData); }
    [PunRPC]
    public void ReceiveDestroyEffect(int[] Data)
    {
        NetworkObject NO = QuickFind.NetworkObjectManager.GetItemByID(Data[0], Data[1]);
        NO.transform.GetChild(0).GetComponent<DG_FXContextObjectReference>().TriggerBreak();
    }

    public void PlayClusterPickEffect(int[] OutData)
    { PV.RPC("ReceiveClusterPickEffect", PhotonTargets.All, OutData); }
    [PunRPC]
    public void ReceiveClusterPickEffect(int[] Data)
    {
        NetworkObject NO = QuickFind.NetworkObjectManager.GetItemByID(Data[0], Data[1]);
        NO.transform.GetChild(0).GetComponent<DG_FXContextObjectReference>().TriggerPick();
    }

    public void WallAdjustMessage(int[] OutData)
    { PV.RPC("ReceiveWallAdjust", PhotonTargets.All, OutData); }
    [PunRPC]
    public void ReceiveWallAdjust(int[] Data)
    {
        NetworkObject NO = QuickFind.NetworkObjectManager.GetItemByID(Data[0], Data[1]);
        NO.transform.GetChild(0).GetComponent<DG_DynamicWall>().IncomingWallMessage(Data[2]);
    }



    #endregion




    #region Enemies
    public void SendEnemyHit(int[] OutData)
    { PV.RPC("ReceiveEnemyHit", PhotonTargets.All, OutData); }
    [PunRPC]
    void ReceiveEnemyHit(int[] Data)
    { QuickFind.CombatHandler.ReceiveHitData(Data); }
    #endregion




    #region Events
    /////////////////////////////////////////////////////
    public void GameWasLoaded()
    {
        PV.RPC("UpdateLoadedGame", PhotonTargets.Others);
    }
    [PunRPC]
    void UpdateLoadedGame()
    {
        RequestPlayerDataSync();
        RequestWorldObjects();
    }

    #endregion




    #region Player Stats
    /////////////////////////////////////////////////////
    public void UpdatePlayerStat(int[] ReceivedInts)
    {
        PV.RPC("LoadPlayerStat", PhotonTargets.All, ReceivedInts);
    }
    [PunRPC]
    void LoadPlayerStat(int[] ReceivedInts)
    {
        QuickFind.SkillTracker.IncomingSetSkillExp(ReceivedInts);
    }
    #endregion



}
