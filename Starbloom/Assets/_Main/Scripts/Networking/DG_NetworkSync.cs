using System.Collections;
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
        public int PhotonViewID;
        public Transform PhotonClone;
    }

    public int UserID = 0;
    [HideInInspector] public int PlayerCharacterID;
    [HideInInspector] public int CurrentScene;
    [HideInInspector] public int PhotonViewID;


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
        PV.RPC("SetNewID", PhotonTargets.MasterClient);
        if (!PhotonNetwork.isMasterClient)
            QuickFind.NetworkSync.RequestPlayerDataSync();
        else
            QuickFind.MainMenuUI.Connected();
    }




    //Network Messages
    //////////////////////////////////////////////////////////////////////////////////////////////////


    public Users GetUserByID(int ID)
    {
        for (int i = 0; i < UserList.Count; i++)
            { if (UserList[i].ID == ID) return UserList[i]; }
        return null;
    }
    public Users GetUserByPhotonViewID(int ID)
    {
        for (int i = 0; i < UserList.Count; i++)
        { if (UserList[i].PhotonViewID == ID) return UserList[i]; }
        return null;
    }
    public Transform GetCharacterTransformByPhotonViewID(int ViewID)
    {
        for (int i = 0; i < QuickFind.CharacterManager.transform.childCount; i++)
        {
            Transform T = QuickFind.CharacterManager.transform.GetChild(i);
            PhotonView PV = T.GetComponent<PhotonView>();
            if (PV.viewID == ViewID) return T;
        }
        return null;
    }



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
        TransferInts.Add(UserList.Count);
        for(int i = 0; i < UserList.Count; i++)
        {
            TransferInts.Add(UserList[i].ID);
            TransferInts.Add(UserList[i].PlayerCharacterID);
            TransferInts.Add(UserList[i].SceneID);
            TransferInts.Add(UserList[i].PhotonViewID);
        }

        PV.RPC("SendOutIDList", PhotonTargets.All, TransferInts.ToArray());
    }
    [PunRPC]
    void SendOutIDList(int[] TransferedIn)
    {
        UserList.Clear();
        int UserCount = TransferedIn[0];
        int index = 1;
        for(int i = 0; i < UserCount; i++)
        {
            Users NewUser = new Users();
            NewUser.ID = TransferedIn[index]; index++;
            NewUser.PlayerCharacterID = TransferedIn[index]; index++;
            NewUser.SceneID = TransferedIn[index]; index++;
            NewUser.PhotonViewID = TransferedIn[index]; index++;

            NewUser.PhotonClone = GetCharacterTransformByPhotonViewID(NewUser.PhotonViewID);

            UserList.Add(NewUser);
        }
        if (UserID == 0)
            UserID = UserList[UserList.Count - 1].ID;

        Debug.Log("UserID == " + UserID.ToString());
        Debug.Log("Connected Online == " + QuickFind.GameSettings.PlayOnline.ToString());
    }



    public void SetPhotonViewID(int PhotonID)
    {
        PhotonViewID = PhotonID;
        int[] IntGroup = new int[3];
        IntGroup[0] = UserID;
        IntGroup[1] = PhotonID;
        IntGroup[2] = PlayerCharacterID;
        PV.RPC("SendUserPhoton", PhotonTargets.All, IntGroup);
    }
    [PunRPC]
    void SendUserPhoton(int[] IntGroup)
    {
        Users U = GetUserByID(IntGroup[0]);
        U.PhotonViewID = IntGroup[1];
        U.PlayerCharacterID = IntGroup[2];
        U.PhotonClone = GetCharacterTransformByPhotonViewID(IntGroup[1]);
    }


    public void SetSelfInScene(int NewScene)
    {
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

        if(info[0] == QuickFind.NetworkSync.PlayerCharacterID)
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

        if (info[0] == QuickFind.NetworkSync.PlayerCharacterID) QuickFind.StorageUI.UpdateStorageVisuals();
    }
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
    public void RequestMasterWeather()
    { PV.RPC("MasterSendForthWeather", PhotonTargets.All); }

    [PunRPC] void MasterSendForthWeather()
    { if (PhotonNetwork.isMasterClient) QuickFind.WeatherHandler.SyncWeatherToMaster(); }
    //////////////////////////////////////////////////////
    public void SyncWeatherToMaster(int[] WeatherValues)
    { PV.RPC("SendOutWeatherByMaster", PhotonTargets.All, WeatherValues); }

    [PunRPC] void SendOutWeatherByMaster(int[] WeatherValues)
    { QuickFind.WeatherHandler.GetMasterWeather(WeatherValues); }


    //Time
    //////////////////////////////////////////////////////
    public void AdjustTimeByPreset(int Time)
    { PV.RPC("SendOutTimeByPreset", PhotonTargets.All, Time); }

    [PunRPC] void SendOutTimeByPreset(int Time)
    {  QuickFind.TimeHandler.AdjustTimeByPreset(Time); }

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
        PV.RPC("GatherWorldObjects", PhotonTargets.MasterClient, PV.ownerId);
    }
    [PunRPC]
    void GatherWorldObjects(int ReturnPhotonOwner)
    {
        PhotonPlayer PP = PhotonPlayer.Find(ReturnPhotonOwner);
        PV.RPC("SendOutWorldObjectInts", PP, QuickFind.SaveHandler.GatherWorldInts(false).ToArray());
        PV.RPC("SendOutWorldObjectFloats", PP, QuickFind.SaveHandler.GatherWorldFloats(false).ToArray());
    }
    [PunRPC]
    void SendOutWorldObjectInts(int[] IntValues)
    {
        QuickFind.SaveHandler.GetWorldInts(IntValues, false);
    }
    [PunRPC]
    void SendOutWorldObjectFloats(float[] FloatValues)
    {
        QuickFind.SaveHandler.GetWorldFloats(FloatValues, false);
        QuickFind.NetworkObjectManager.GenerateSceneObjects(CurrentScene);
    }





    public void CreateNewNetworkSceneObject(int ItemID, int GrowthLevel, Vector3 Position, float Direction)
    {

    }
    [PunRPC]
    void CreateSceneObject()
    {

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
        Destroy(QuickFind.NetworkObjectManager.FindObject(Received[0], Received[1]));   
    }


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
}
