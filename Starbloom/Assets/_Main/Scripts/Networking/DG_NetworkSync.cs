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
        public int SceneID;
        public int PhotonViewID;
    }

    public int UserID = 0;
    [HideInInspector] public int CurrentScene;
    [HideInInspector] public int PhotonViewID;


    public List<Users> UserList;





    PhotonView PV;




    private void Update()
    {
        if (Input.GetKey(KeyCode.Alpha1))
            SetCharacterDifferentScene();
        if (Input.GetKey(KeyCode.Alpha2))
            SetCharacterMainScene();
    }

    void SetCharacterDifferentScene()
    { QuickFind.NetworkSync.SetSelfInScene(1); }
    void SetCharacterMainScene()
    { QuickFind.NetworkSync.SetSelfInScene(0); }



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
            {
                QuickFind.WeatherHandler.RequestMasterWeather();
                QuickFind.TimeHandler.RequestMasterTimes();
            }
        }
    }





    //Network Messages
    //////////////////////////////////////////////////////////////////////////////////////////////////



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
            NewUser.SceneID = TransferedIn[index]; index++;
            NewUser.PhotonViewID = TransferedIn[index]; index++;

            UserList.Add(NewUser);
        }

        if (UserID == 0)
        {
            UserID = UserList[UserList.Count - 1].ID;
            Debug.Log("UserID == " + UserID.ToString());
        }
    }





    public Users GetUserByID(int ID)
    {
        for (int i = 0; i < UserList.Count; i++)
        {
            if (UserList[i].ID == ID)
                return UserList[i];
        }
        return null;
    }
    public Users GetUserByPhotonViewID(int ID)
    {
        for (int i = 0; i < UserList.Count; i++)
        {
            if (UserList[i].PhotonViewID == ID)
                return UserList[i];
        }
        return null;
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
