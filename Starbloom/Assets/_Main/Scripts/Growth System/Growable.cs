using Devdog.InventoryPro;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EventRecord))]
public class Growable : SerializedMonoBehaviour
{
	[Serializable]
	public class GrowthStage
	{
		[SerializeField]
		public GameObject Prefab;
	}

	public ItemAmountRow YieldItem = new ItemAmountRow();
	public ItemRarity[] Rarities = new ItemRarity[1];

    [DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.ExpandedFoldout, KeyLabel = "Day", ValueLabel = "StageSettings")]
	public Dictionary<int, GrowthStage> Stages = new Dictionary<int, GrowthStage>();

	protected void Start()
	{
	}

	protected void Update()
	{
	}
}
