using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class DG_NetworkSync : Photon.MonoBehaviour
{
    [System.Serializable]
    public class Users
    {
        [Header("Network")]
        [ReadOnly] public int ID; //Note User ID is Assigned From Photon Network ID.
        [Header("Character")]
        [ReadOnly] public int PlayerCharacterID;
        [ReadOnly] public int SceneID;
        [ReadOnly] public DG_CharacterLink CharacterLink;

    }

    [ReadOnly] public int UserID =  -1;
    [ReadOnly] public int PlayerCharacterID = -1;
    [ReadOnly] public int CurrentScene = -1;
    [System.NonSerialized] public DG_CharacterLink CharacterLink;
    [System.NonSerialized] public PhotonView PV;

    bool AwaitingSync = false;
    bool FirstLoaded = false;

    [Header("UserList")]
    public List<Users> UserList;
    int[] UsersInScene;








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
        PhotonPlayer PP = PhotonNetwork.player;
        PV.RPC("SetNewID", PhotonTargets.MasterClient, PP.ID);
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
    public Users GetUserByCharacterLink(DG_CharacterLink Sync)
    {
        for (int i = 0; i < UserList.Count; i++)
        { if (UserList[i].CharacterLink == Sync) return UserList[i]; }
        return null;
    }
    public int GetPlayerIDByUserID(int UserID)
    {
        for (int i = 0; i < UserList.Count; i++)
        { if (UserList[i].ID == UserID) return UserList[i].PlayerCharacterID; }
        return -1;
    }
    public DG_CharacterLink GetCharacterLinkByUserID(int ID)
    {
        Users U = GetUserByID(ID);
        if (U != null)
            return U.CharacterLink;
        else
            return null;
    }
    public void RefreshUsersIDsInScene(bool ExcludeSelf)
    {
        if (UsersInScene == null) UsersInScene = new int[4];
        for (int i = 0; i < UsersInScene.Length; i++)
        {
            if (i >= UserList.Count) { UsersInScene[i] = -1; continue;  }

            Users U = UserList[i];

            if (U.SceneID == CurrentScene)
            {
                if (ExcludeSelf && U.ID != UserID)
                    UsersInScene[i] = -1;
                else
                    UsersInScene[i] = i;
            }
            else
                UsersInScene[i] = -1;
        }
    }

    #endregion



    #region Users
    //////////////////////////////////////////////////////

    [PunRPC]
    void SetNewID(int PhotonID)
    {
        Users NewUser = new Users();

        NewUser.ID = PhotonID;
        UserList.Add(NewUser);

        List<int> TransferInts = new List<int>();
        TransferInts.Add(NewUser.ID);
        TransferInts.Add(UserList.Count);
        for (int i = 0; i < UserList.Count; i++)
        {
            TransferInts.Add(UserList[i].ID);
            TransferInts.Add(UserList[i].PlayerCharacterID);
            TransferInts.Add(UserList[i].SceneID);
            TransferInts.Add((int)QuickFind.Farm.PlayerCharacters[UserList[i].PlayerCharacterID].CharacterGender);
        }

        UserList.Remove(NewUser);

        PV.RPC("SendOutIDList", PhotonTargets.All, TransferInts.ToArray());
    }
    [PunRPC]
    void SendOutIDList(int[] TransferedIn)
    {
        //UserList.Clear();

        if (UserID == -1) { UserID = TransferedIn[0];}

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
                QuickFind.GameStartHandler.Connected();
        }
    }



    public void NetSetUserInScene(int[] OutData)
    { PV.RPC("SendUserInScene", PhotonTargets.Others, OutData);}
    [PunRPC] void SendUserInScene(int[] InData)
    { QuickFind.SceneTransitionHandler.RemoteSetUserInScene(InData); }

    public void UserRequestingOwnership(int[] OutData)
    { PV.RPC("OwnershipRequestReceived", PhotonTargets.All, OutData); }
    [PunRPC]
    void OwnershipRequestReceived(int[] InData)
    { QuickFind.NetworkObjectManager.GetSceneByID(InData[0]).ReceivedMasterRequest(InData[1]); }




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
        DG_CharacterLink CL = GetCharacterLinkByUserID(InData[0]);
        if(CL != null) CL.MoveSync.UpdatePlayerPos(InData);
    }

    public void UpdatePlayerAnimationState(int[] OutgoingData)
    {
        PV.RPC("ReceivePlayerAnimationState", PhotonTargets.Others, OutgoingData);
    }
    [PunRPC]
    void ReceivePlayerAnimationState(int[] InData)
    {
        GetCharacterLinkByUserID(InData[0]).MoveSync.AnimSync.UpdatePlayerAnimationState(InData);
    }


    public void GenerateNewChar(int[] OutgoingData)
    {
        PV.RPC("NewCharGenerationCalled", PhotonTargets.All, OutgoingData);
    }
    [PunRPC]
    void NewCharGenerationCalled(int[] InData)
    {
        QuickFind.GameStartHandler.ReturnCharacterGenerated(InData);
    }



    public void TriggerAnimationSubState(int[] OutgoingData)
    {PV.RPC("ReceiveAnimationSubState", PhotonTargets.Others, OutgoingData);}
    [PunRPC]
    void ReceiveAnimationSubState(int[] InData)
    {GetCharacterLinkByUserID(InData[0]).MoveSync.AnimSync.ReceiveNetAnimation(InData);}


    #endregion




    #region Inventory
    //////////////////////////////////////////////////////
    public void RequestPlayerDataSync()
    {
        PhotonPlayer PP = PhotonNetwork.player;
        PV.RPC("GatherPlayerData", PhotonTargets.MasterClient, PP.ID);
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
            QuickFind.GameStartHandler.Connected();
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

    public void SetUserEquipment(int[] OutData) { PV.RPC("ReceiveUserEquipment", PhotonTargets.Others, OutData); }
    [PunRPC] void ReceiveUserEquipment(int[] InData) { QuickFind.ClothingHairManager.NetReceivedClothingAdd(InData); }



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

    public void AdjustTimeByValues(int Hour, int Minute, int Year, int Month, int Day)
    {
        int[] OutInts = new int[5]; OutInts[0] = Hour;  OutInts[1] = Minute; OutInts[2] = Year; OutInts[3] = Month; OutInts[4] = Day;
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
    {
        PV.RPC("GatherWorldObjects", PhotonTargets.MasterClient, UserID);
    }
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
    { GetUserByPlayerID(Data[0]).CharacterLink.MagnetAttract.ClaimObject(Data); }

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




    #region AI Units
    public void SendEnemyHit(int[] OutData)
    { PV.RPC("ReceiveEnemyHit", PhotonTargets.All, OutData); }
    [PunRPC]
    void ReceiveEnemyHit(int[] Data)
    { QuickFind.CombatHandler.ReceiveHitData(Data); }

    public void SendAIDestination(int[] OutData)
    {
        RefreshUsersIDsInScene(false);
        for(int i = 0; i < UsersInScene.Length; i++)
        {
            if(UsersInScene[i] != -1)
            {
                PhotonPlayer PP = PhotonPlayer.Find(UserList[UsersInScene[i]].ID);
                PV.RPC("ReceiveAIDestination", PP, OutData);
            }
        }
    }
    [PunRPC]
    void ReceiveAIDestination(int[] Data)
    {
        NetworkObject NO = QuickFind.NetworkObjectManager.GetItemByID(Data[0], Data[1]);
        NO.transform.GetChild(0).GetComponent<DG_AIEntity>().ReceiveAgentDestination(Data);
    }


    public void RequestAIPositionsFromOwner(int[] OutData)
    { PV.RPC("ReceiveRequestAILocations", PhotonTargets.Others, OutData); }
    [PunRPC]
    void ReceiveRequestAILocations(int[] InData)
    { QuickFind.SceneTransitionHandler.AIPositionsRequested(InData); }



    public void ReturnAIPositionsToReqester(int ReturnID, int[] OutData)
    {
        PhotonPlayer PP = PhotonPlayer.Find(ReturnID); //This is correct
        PV.RPC("ReceiveReturnAIPositions", PP, OutData);
    }
    [PunRPC]
    void ReceiveReturnAIPositions(int[] InData)
    { QuickFind.SceneTransitionHandler.AIPositionsReceived(InData); }


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
