using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_SleepHandler : MonoBehaviour {


    public List<int> SleepingPlayers = new List<int>();


    private void Awake()
    {
        QuickFind.SleepHandler = this;
    }

    public void NewDay()
    {
        SleepingPlayers.Clear();
    }

    //Net Out
    public void BedInteract(int PlayerID)
    {
        QuickFind.NetworkSync.SetUserInBed(PlayerID);
    }

    //Network Receive
    public void AddSleepingPlayer(int PlayerAdded)
    {
        SleepingPlayers.Add(PlayerAdded);
        if (PhotonNetwork.isMasterClient)
        {
            if (SleepingPlayers.Count == QuickFind.NetworkSync.UserList.Count)
                QuickFind.TimeHandler.SetDayEnd();
        }
    }
}
