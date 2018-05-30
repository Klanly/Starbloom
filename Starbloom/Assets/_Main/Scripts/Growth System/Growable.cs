using Devdog.InventoryPro;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(EventRecord))]
public class Growable : SerializedMonoBehaviour
{
	[Serializable]
	public class GrowthStage
	{
		[SerializeField]
		public GameObject Prefab;
	}

	public int DaysGrown { get { return mTimeProvider.Day - DayPlanted; } }

	[ReadOnly]
	public int DayPlanted = 0;

	public ItemAmountRow YieldItem = new ItemAmountRow();
	public ItemRarity[] Rarities = new ItemRarity[1];

    [DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.ExpandedFoldout, KeyLabel = "Day", ValueLabel = "StageSettings")]
	public Dictionary<int, GrowthStage> Stages = new Dictionary<int, GrowthStage>();

	ITimeEventHandler mTimeProvider;

	[Inject]
	public void Construct(ITimeEventHandler _timeHandler)
	{
		mTimeProvider = _timeHandler;
		DayPlanted = mTimeProvider.Day;
		mTimeProvider.OnDay += OnDayChanged;
	}

	protected void OnDayChanged( int _day )
	{
		Debug.LogFormat("Day changed - {0}", _day);
	}
}
