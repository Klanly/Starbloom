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
            PV = transform.GetComponent<PhotonView>();
            transform.SetParent(QuickFind.NetworkMaster.transform);
            QuickFind.NetworkSync = this;
            PV.RPC("SetNewID", PhotonTargets.MasterClient);
            if (!PhotonNetwork.isMasterClient)
                QuickFind.NetworkSync.RequestPlayerDataSync();
            else
                QuickFind.MainMenuUI.Connected();               
        }
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
        int[] IntGroup = new int[2];
        IntGroup[0] = UserID;
        IntGroup[1] = PhotonID;
        PV.RPC("SendUserPhoton", PhotonTargets.All, IntGroup);
    }
    [PunRPC]
    void SendUserPhoton(int[] IntGroup)
    {
        Users U = GetUserByID(IntGroup[0]);
        U.PhotonViewID = IntGroup[1];
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
        List<int> OutgoingInts = new List<int>();
        List<float> OutgoingFloats = new List<float>();

        Transform NOM = QuickFind.NetworkObjectManager.transform;
        for (int i = 0; i < NOM.childCount; i++)
        {
            Transform Child = NOM.GetChild(i);
            NetworkScene NS = Child.GetComponent<NetworkScene>();
            OutgoingInts.Add(NS.SceneID);
            OutgoingInts.Add(Child.childCount);
            OutgoingFloats.Add(Child.childCount);
            for (int iN = 0; iN < Child.childCount; iN++)
            {
                NetworkObject NO = Child.GetChild(iN).GetComponent<NetworkObject>();
                OutgoingInts.Add(NO.ItemRefID);
                OutgoingInts.Add(NO.ItemGrowthLevel);

                OutgoingFloats.Add(NO.Position.x);
                OutgoingFloats.Add(NO.Position.y);
                OutgoingFloats.Add(NO.Position.z);
                OutgoingFloats.Add(NO.YFacing);
            }
        }

        PhotonPlayer PP = PhotonPlayer.Find(ReturnPhotonOwner);
        PV.RPC("SendOutWorldObjectInts", PP, OutgoingInts.ToArray());
        PV.RPC("SendOutWorldObjectFloats", PP, OutgoingFloats.ToArray());
    }
    [PunRPC]
    void SendOutWorldObjectInts(int[] Incoming)
    {
        Transform NOM = QuickFind.NetworkObjectManager.transform;
        int index = 0;    
        for (int i = 0; i < NOM.childCount; i++)
        {
            Transform Child = NOM.GetChild(i);
            NetworkScene NS = Child.GetComponent<NetworkScene>();
            NS.SceneID = Incoming[index]; index++;
            int count = Incoming[index]; index++;
            for (int iN = 0; iN < count; iN++)
            {
                GameObject GO = new GameObject();
                GO.transform.SetParent(Child);
                NetworkObject NO = GO.AddComponent<NetworkObject>();
                NO.ItemRefID = Incoming[index]; index++;
                NO.ItemGrowthLevel = Incoming[index]; index++;
            }
        }
    }
    [PunRPC]
    void SendOutWorldObjectFloats(float[] Incoming)
    {
        Transform NOM = QuickFind.NetworkObjectManager.transform;
        int index = 0;
        for (int i = 0; i < NOM.childCount; i++)
        {
            Transform Child = NOM.GetChild(i);
            NetworkScene NS = Child.GetComponent<NetworkScene>();
            int count = (int)Incoming[index]; index++;
            for (int iN = 0; iN < count; iN++)
            {
                NetworkObject NO = Child.GetChild(iN).GetComponent<NetworkObject>();

                float x = Incoming[index]; index++;
                float y = Incoming[index]; index++;
                float z = Incoming[index]; index++;
                NO.Position = new Vector3(x, y, z);
                NO.YFacing = Incoming[index]; index++;

                NO.transform.position = NO.Position;
                NO.transform.eulerAngles = new Vector3(0, NO.YFacing, 0);
            }
        }
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
}
