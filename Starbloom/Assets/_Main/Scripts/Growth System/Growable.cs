using Sirenix.OdinInspector;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using WorldTime;

[RequireComponent(typeof(EventRecord))]
public class Growable : SerializedMonoBehaviour
{
	[Serializable]
	public class GrowthStage
	{
		[SerializeField]
		public GameObject Prefab;
	}

	public int DaysGrown { get { return QuickFind.Farm.Day - DayPlanted; } }
	public int CurrentGrowthDay { get { return DaysGrown + GrowthOffset; } }
	public int CurrentStageDay { get { return Stages.Lower_Bound(DaysGrown).Key; } }
	public GrowthStage CurrentStage { get { return Stages.Lower_Bound(DaysGrown).Value; } }

	[ReadOnly]
	public int DayPlanted = 0;
	[ReadOnly, Tooltip("Used by set the current growth stage")]
	public int GrowthOffset = 0;

	[DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.ExpandedFoldout, KeyLabel = "Day", ValueLabel = "Stage Settings")]
	public SortedDictionary<int, GrowthStage> Stages = new SortedDictionary<int, GrowthStage>();

	private void Awake()
	{
		TimeHandler.OnNewDay += OnDayChanged;
	}

	private void OnDestroy()
	{
		TimeHandler.OnNewDay -= OnDayChanged;
	}
	
	protected void OnDayChanged( int _day )
	{
		Debug.LogFormat("Day changed - {0}", _day);
	}

	public void SetGrowthDay( int _day )
	{
		int curDays = CurrentGrowthDay;
		int delta = _day - curDays;
		GrowthOffset += delta;
	}
}
