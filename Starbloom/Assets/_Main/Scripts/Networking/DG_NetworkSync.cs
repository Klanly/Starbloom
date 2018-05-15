using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_NetworkSync : Photon.MonoBehaviour
{
    public int UserID = 0;

    PhotonView PV;
    List<int> UserIDs;

    private void Awake()
    {
        if (transform.parent != null)
            Destroy(this.gameObject);
        else
        {
            PV = transform.GetComponent<PhotonView>();
            transform.SetParent(QuickFind.NetworkMaster.transform);
            QuickFind.IDMaster = this;
            PV.RPC("SetNewID", PhotonTargets.MasterClient);

            if (!PhotonNetwork.isMasterClient)
            {
                QuickFind.WeatherHandler.RequestMasterWeather();
                QuickFind.TimeHandler.RequestMasterTimes();
            }
        }
    }

    [PunRPC]
    void SetNewID()
    {
        if (UserIDs == null)
            UserIDs = new List<int>();

        int NewID;
        if (UserIDs.Count != 0)
            NewID = UserIDs[UserIDs.Count - 1] + 1;
        else
            NewID = 1;
        UserIDs.Add(NewID);

        PV.RPC("SendOutIDList", PhotonTargets.All, UserIDs.ToArray());
    }
    [PunRPC]
    void SendOutIDList(int[] UserIdSent)
    {
        UserIDs = new List<int>();

        for (int i = 0; i < UserIdSent.Length; i++)
            UserIDs.Add(UserIdSent[i]);

        if (UserID == 0)
        {
            UserID = UserIDs[UserIDs.Count - 1];
            Debug.Log("UserID == " + UserID.ToString());
        }
    }











    //Network Messages
    //////////////////////////////////////////////////////////////////////////////////////////////////



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
