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

	[ReadOnly] public GameObject ActiveStage = null;
	[ReadOnly] public int ActiveStageDay = -1;
	[ReadOnly] public int DayPlanted = 0;

	[ReadOnly, Tooltip("Used by set the current growth stage")]
	public int GrowthOffset = 0;

	[DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.ExpandedFoldout, KeyLabel = "Day", ValueLabel = "Stage Settings")]
	public SortedDictionary<int, GrowthStage> Stages = new SortedDictionary<int, GrowthStage>();


    [Button(ButtonSizes.Small)]void DebugDayButton(){QuickFind.TimeHandler.SetNewDay(false);}
    [Button(ButtonSizes.Small)] void DebugRainyDayButton() { QuickFind.TimeHandler.SetNewDay(false); }


    private void Awake()
	{
		TimeHandler.OnNewDay += OnDayChanged;
	}

	private void Start()
	{
		DayPlanted = QuickFind.Farm.Day;
		RefreshStage();
	}

	private void OnEnable()
	{
		DayPlanted = QuickFind.Farm.Day;
		RefreshStage();
	}


    

	private void OnDestroy()
	{
		TimeHandler.OnNewDay -= OnDayChanged;
	}
	
	protected void OnDayChanged( int _day )
	{
        if(QuickFind.GameSettings.ShowGrowthDebug) Debug.LogFormat("Day changed - {0}", _day);

		RefreshStage();
	}

	public void SetGrowthDay( int _day )
	{
		int curDays = CurrentGrowthDay;
		int delta = _day - curDays;
		GrowthOffset += delta;
		RefreshStage();
	}

	public void RefreshStage()
	{
		int curStageDay = CurrentStageDay + GrowthOffset;
		bool recalcDay = ActiveStageDay != curStageDay;
		ActiveStageDay = curStageDay;

		if( recalcDay )
		{
			if( null != ActiveStage )
				GameObject.Destroy(ActiveStage);

			ActiveStageDay = curStageDay;
			GrowthStage curStage = CurrentStage;
			ActiveStage = GameObject.Instantiate(curStage.Prefab, transform);
            Transform T = ActiveStage.transform;
            T.localPosition = Vector3.zero;
            T.localEulerAngles = Vector3.zero;
            T.localScale = new Vector3(1, 1, 1);
		}
	}
}
