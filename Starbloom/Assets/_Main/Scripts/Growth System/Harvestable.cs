using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Growable))]
public class Harvestable : SerializedMonoBehaviour
{
	[Serializable]
	public class HarvestSettings
	{
		[SerializeField]
		public DG_ItemObject.ItemQualityLevels[] Rarities = new DG_ItemObject.ItemQualityLevels[1];

		[SerializeField]
		public int ResetDay = -1;

		[SerializeField]
		public bool KillCrop = true;
	}

	[DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.ExpandedFoldout, KeyLabel = "Stage", ValueLabel = "Harvest Settings")]
	public Dictionary<int, HarvestSettings> HarvestStages = new Dictionary<int, HarvestSettings>();

	protected Growable mGrowable;

	protected virtual void Awake()
	{
		mGrowable = GetComponent<Growable>();
	}

	public void Harvest(int _pid = 0)
	{
		int stageDay = mGrowable.CurrentStageDay;

		// Only allow harvesting on defined days
		if (!HarvestStages.ContainsKey(stageDay))
			return;

		Harvest(_pid, HarvestStages[stageDay]);
	}

	public void Harvest(int _pid, HarvestSettings _settings )
	{

		if (_settings.KillCrop)
			GameObject.Destroy(mGrowable.gameObject);

		//else if (_settings.ResetDay > 0)
		//	mGrowable.SetGrowthDay(_settings.ResetDay);
	}
}