using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class Bed : MonoBehaviour
{
	//[SerializeField, ReadOnly]
	public static HashSet<int> mSleepingPlayers = new HashSet<int>();

	private void Start()
	{
		TimeHandler.OnNewDay += HandleNewDay;
	}

	private void OnDestroy()
	{
		TimeHandler.OnNewDay -= HandleNewDay;
	}

    protected void HandleNewDay(int _day)
    {
        mSleepingPlayers.Clear();
    }
	



    //Network Send
    [Button("Sleep", ButtonSizes.Small)]
    void SetNetworkPlayerInBed()
    {
        QuickFind.NetworkSync.SetUserInBed(QuickFind.NetworkSync.PlayerCharacterID);
    }
    //Network Receive
    public static void AddSleepingPlayer(int PlayerAdded)
    {
        mSleepingPlayers.Add(PlayerAdded);
        if(PhotonNetwork.isMasterClient)
            TryEndDay();
    }



    public static void TryEndDay()
    {
        List<DG_NetworkSync.Users> users = QuickFind.NetworkSync.UserList;
        if (users.TrueForAll(x => mSleepingPlayers.Contains(x.PlayerCharacterID)))
            QuickFind.TimeHandler.SetNewDay(false);
    }

}