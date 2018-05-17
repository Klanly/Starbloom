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


    //Users
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









    //Inventory
    //////////////////////////////////////////////////////
    public void RequestPlayerDataSync()
    {
        PV.RPC("GatherPlayerData", PhotonTargets.MasterClient, PV.ownerId);
    }
    [PunRPC]
    void GatherPlayerData(int PhotonOwnerID)
    {
        List<string> StringData = new List<string>();
        List<int> IntData = new List<int>();
        DG_PlayerCharacters PlayerData = QuickFind.Farm;

        StringData.Add(PlayerData.FarmName);

        IntData.Add(PlayerData.PlayerCharacters.Count);
        StringData.Add(PlayerData.PlayerCharacters.Count.ToString());

        for (int i = 0; i < PlayerData.PlayerCharacters.Count; i++)
        {
            //STRINGS
            DG_PlayerCharacters.PlayerCharacter PC = PlayerData.PlayerCharacters[i];
            StringData.Add(PC.Name);

            //Equipment

            DG_PlayerCharacters.CharacterEquipment CE = PC.Equipment;
            IntData.Add(CE.HatId);
            IntData.Add(CE.Ring1);
            IntData.Add(CE.Ring2);
            IntData.Add(CE.Boots);

            //Cosmetics

            //Rucksack
            IntData.Add(CE.RuckSackUnlockedSize);
            DG_PlayerCharacters.RucksackSlot[] RS = CE.RucksackSlots;
            for(int iN = 0; iN < CE.RuckSackUnlockedSize; iN++)
            {
                DG_PlayerCharacters.RucksackSlot RSlot = RS[iN];
                IntData.Add(RSlot.ContainedItem);
                IntData.Add(RSlot.StackValue);
            }
        }

        PhotonPlayer PP = PhotonPlayer.Find(PhotonOwnerID);
        PV.RPC("GetPlayerStringValues", PP, StringData.ToArray());
        PV.RPC("GetPlayerIntValues", PP, IntData.ToArray());
    }

    [PunRPC]
    void GetPlayerStringValues(string[] StringValues)
    {
        DG_PlayerCharacters PlayerData = QuickFind.Farm;
        PlayerData.FarmName = StringValues[0];
        int count = int.Parse(StringValues[1]);
        int Index = 2;

        for (int i = 0; i < count; i++)
        {
            DG_PlayerCharacters.PlayerCharacter PC = PlayerData.PlayerCharacters[i];
            PC.Name = StringValues[Index];  Index++;
        }
    }
    [PunRPC]
    void GetPlayerIntValues(int[] IntValues)
    {
        DG_PlayerCharacters PlayerData = QuickFind.Farm;
        int Count = IntValues[0];
        int Index = 1;

        for (int i = 0; i < Count; i++)
        {
            DG_PlayerCharacters.PlayerCharacter PC = PlayerData.PlayerCharacters[i];
            DG_PlayerCharacters.CharacterEquipment CE = PC.Equipment;
            //Equipment
            CE.HatId = IntValues[Index]; Index++;
            CE.Ring1 = IntValues[Index]; Index++;
            CE.Ring2 = IntValues[Index]; Index++;
            CE.Boots = IntValues[Index]; Index++;
            //Cosmetics
            //Rucksack
            CE.RuckSackUnlockedSize = IntValues[Index]; Index++;
            DG_PlayerCharacters.RucksackSlot[] RS = CE.RucksackSlots;
            for (int iN = 0; iN < CE.RuckSackUnlockedSize; iN++)
            {
                DG_PlayerCharacters.RucksackSlot RSlot = RS[iN];
                RSlot.ContainedItem = IntValues[Index]; Index++;
                RSlot.StackValue = IntValues[Index]; Index++;
            }
        }

        if (!PhotonNetwork.isMasterClient)
            QuickFind.MainMenuUI.Connected();
    }





    public void SetRucksackValue(int slot, int ItemID, int StackValue)
    {                                                            
        List<int> IntData = new List<int>();
        IntData.Add(PlayerCharacterID);
        IntData.Add(slot);
        IntData.Add(ItemID);
        IntData.Add(StackValue);

        PV.RPC("NewRucksackValue", PhotonTargets.All, IntData.ToArray());
    }
    [PunRPC]
    void NewRucksackValue(int[] info)
    {
        DG_PlayerCharacters.CharacterEquipment CE = QuickFind.Farm.PlayerCharacters[info[0]].Equipment;
        DG_PlayerCharacters.RucksackSlot RS = CE.RucksackSlots[info[1]];
        RS.ContainedItem = info[2];
        RS.StackValue = info[3];
    }













    //Weather
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
}
