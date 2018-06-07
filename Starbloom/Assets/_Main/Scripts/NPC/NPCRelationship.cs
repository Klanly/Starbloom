using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

// TODO: Route per player ID
[RequireComponent(typeof(EventRecord))]
public class NPCRelationship : SerializedMonoBehaviour
{
	[SerializeField]
	public List<string> Tags = new List<string>();

	public bool IsPlayerUnknown { get { return !Tags.Contains("PlayerKnown"); } }
	public bool HasMetPlayer { get { return Tags.Contains("PlayerMet"); } }
	public bool HasLove { get{ return Tags.Contains("HasLove"); } }
	public bool IsMarried { get{ return Tags.Contains("IsMarried"); } }
	
	public int CurrentFriendLevel { get { return EvaluateLevel(FriendPoints, Settings.FriendshipLevels); } }
	public int CurrentLoveLevel { get { return EvaluateLevel(LovePoints, Settings.LoveLevels); } }
	public int CurrentMarriageLevel { get { return EvaluateLevel(MarriagePoints, Settings.MarriageLevels); } }

	public int FriendPoints = 0;
	public int LovePoints = 0;
	public int MarriagePoints = 0;


	protected int EvaluateLevel( int _points, int[] _stages )
	{
		// Level always lags behind i by 1
		for( int i = 0; i < _stages.Length; ++i )
			if (_stages[i] > _points)
				return i - 1;

		return _stages.Length - 1;
	}

	public NPCRelationshipSettings Settings;
}