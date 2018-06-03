using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class Bed : MonoBehaviour
{
	[SerializeField, ReadOnly]
	protected HashSet<int> mSleepingPlayers = new HashSet<int>();

	private void Start()
	{
		TimeHandler.OnNewDay += HandleNewDay;
	}

	private void OnDestroy()
	{
		TimeHandler.OnNewDay -= HandleNewDay;
	}

	protected void HandleNewDay( int _day )
	{
		mSleepingPlayers.Clear();
	}
	
	[Button("Sleep", ButtonSizes.Small)]
	public void Use()
	{
		Use( QuickFind.NetworkSync.PlayerCharacterID );
	}

	public void Use( int _pid )
	{
		mSleepingPlayers.Add( _pid );

		TryEndDay();
	}

	public void TryEndDay()
	{
		List<DG_NetworkSync.Users> users = QuickFind.NetworkSync.UserList;
		if (users.TrueForAll(x => mSleepingPlayers.Contains(x.PlayerCharacterID)))
			QuickFind.TimeHandler.SetNewDay(false); 
	}
}