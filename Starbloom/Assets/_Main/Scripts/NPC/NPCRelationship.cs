using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using PixelCrushers.DialogueSystem;
using System;

// TODO: Route per player ID
[RequireComponent(typeof(EventRecord))]
public class NPCRelationship : SerializedMonoBehaviour
{
	[SerializeField, DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.CollapsedFoldout, KeyLabel = "Player", ValueLabel = "Record")]
	public Dictionary<int, List<string>> Tags = new Dictionary<int, List<string>>();

	public bool IsPlayerUnknown(int _pid) { return !Tags[_pid].Contains("PlayerKnown"); }
	public bool HasMetPlayer(int _pid) { return Tags[_pid].Contains("PlayerMet"); }
	public bool HasLove(int _pid) { return Tags[_pid].Contains("HasLove"); }
	public bool IsMarried(int _pid) { return Tags[_pid].Contains("IsMarried"); }

	public float CurrentFriendLevel(int _pid) { return EvaluateLevel(PlayerData[_pid].FriendPoints, Settings.FriendshipLevels); }
	public float CurrentLoveLevel(int _pid) { return EvaluateLevel(PlayerData[_pid].LovePoints, Settings.LoveLevels); }
	public float CurrentMarriageLevel(int _pid) { return EvaluateLevel(PlayerData[_pid].MarriagePoints, Settings.MarriageLevels); }

	[Serializable]
	public class RelationData
	{
		[SerializeField] public string LastConversation;
		[SerializeField] public int LastDayTalked = -1;
		[SerializeField] public float FriendPoints;
		[SerializeField] public float LovePoints;
		[SerializeField] public float MarriagePoints;
	}

	public Dictionary<int, RelationData> PlayerData = new Dictionary<int, RelationData>();

	protected int EvaluateLevel(float _points, int[] _stages)
	{
		// Level always lags behind i by 1
		for (int i = 0; i < _stages.Length; ++i)
			if (_stages[i] > _points)
				return i - 1;

		return _stages.Length - 1;
	}

	public NPCRelationshipSettings Settings;
}