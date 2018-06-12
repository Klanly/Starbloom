using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;

[RequireComponent(typeof(NPCRelationship)), RequireComponent(typeof(NPCController)), RequireComponent(typeof(ConversationTrigger))]
public class NPCChat : MonoBehaviour
{
	protected NPCController Controller;
	protected NPCRelationship Relation;
	protected ConversationTrigger ConversationTrg;

	protected int SpeakerPID = -1;
	protected bool IsBusy = false;

	private void Awake()
	{
		Controller = GetComponent<NPCController>();
		Relation = GetComponent<NPCRelationship>();
		ConversationTrg = GetComponent<ConversationTrigger>();
	}

	private void OnDestroy()
	{
	}

	[Button("Debug Conversation")]
	private void DebugGenConversation()
	{
		Debug.Log(GetConversationName());
	}

	protected void OnConversationStart(Transform actor)
	{
		IsBusy = true;

		Lua.RegisterFunction("HasTalkedToday", this, typeof(NPCChat).GetMethod("HasTalkedToday"));
		Lua.RegisterFunction("HasTag", this, typeof(NPCChat).GetMethod("HasTag"));
		Lua.RegisterFunction("SetTag", this, typeof(NPCChat).GetMethod("SetTag"));
		Lua.RegisterFunction("RemoveTag", this, typeof(NPCChat).GetMethod("RemoveTag"));
		Lua.RegisterFunction("AddFriendPoints", this, typeof(NPCChat).GetMethod("AddFriendPoints"));
		Lua.RegisterFunction("AddLovePoints", this, typeof(NPCChat).GetMethod("AddLovePoints"));
		Lua.RegisterFunction("AddMarriagePoints", this, typeof(NPCChat).GetMethod("AddMarriagePoints"));
	}

	protected void OnConversationEnd(Transform actor)
	{
		Lua.UnregisterFunction("HasTag");
		Lua.UnregisterFunction("SetTag");
		Lua.UnregisterFunction("RemoveTag");
		Lua.UnregisterFunction("AddFriendPoints");
		Lua.UnregisterFunction("AddLovePoints");
		Lua.UnregisterFunction("AddMarriagePoints");

		Relation.PlayerData[SpeakerPID].LastDayTalked = QuickFind.Farm.TotalDays;

		SpeakerPID = -1;
		IsBusy = false;
	}

	protected void InitializeVariables(int _pid)
	{
		// Validate player relationship states
		if (!Relation.Tags.ContainsKey(_pid))
			Relation.Tags[_pid] = new List<string>();

		if (!Relation.PlayerData.ContainsKey(_pid))
			Relation.PlayerData.Add(_pid, new NPCRelationship.RelationData());
	}

	#region Lua Callbacks
	public bool HasTalkedToday()
	{
		return Relation.PlayerData[SpeakerPID].LastDayTalked == QuickFind.Farm.TotalDays;
	}

	public string PreviousConversation()
	{
		return Relation.PlayerData[SpeakerPID].LastConversation;
	}

	public bool HasTag(string _tag)
	{
		return Relation.Tags[SpeakerPID].Contains(_tag);
	}

	public void SetTag(string _tag)
	{
		if (!Relation.Tags[SpeakerPID].Contains(_tag))
			Relation.Tags[SpeakerPID].Add(_tag);
	}

	public void RemoveTag(string _tag)
	{
		if (Relation.Tags[SpeakerPID].Contains(_tag))
			Relation.Tags[SpeakerPID].Remove(_tag);
	}

	public void AddFriendPoints(float _points)
	{
		Relation.PlayerData[SpeakerPID].FriendPoints += _points;
	}

	public void AddLovePoints(float _points)
	{
		Relation.PlayerData[SpeakerPID].LovePoints += _points;
	}

	public void AddMarriagePoints(float _points)
	{
		Relation.PlayerData[SpeakerPID].MarriagePoints += _points;
	}
	#endregion

	public string GetConversationName()
	{
		return GetConversationName(QuickFind.NetworkSync.PlayerCharacterID);
	}

	public string GetConversationName(int _pid)
	{
		string npcName = Controller.NPCName.ToUpper();
		Debug.AssertFormat(!string.IsNullOrEmpty(npcName), "NPC name not set on transform [{0}]", transform.name);

		if (Relation.IsPlayerUnknown(_pid))
			return string.Format("{0}_UNKNOWN", npcName);

		if (!Relation.HasMetPlayer(_pid))
			return string.Format("{0}_INTRO", npcName);

		//if (GameLogic.HasActiveEvent)
		//	return string.Format("{0}_{1}{2}", npcName, GameLogic.CurrentEventName, GameLogic.CurrentEventIteration);

		string curSeason = QuickFind.WeatherHandler.CurrentSeason.ToString().ToUpper();
		List<string> possibleConvs = new List<string>(3);

		possibleConvs.Add(string.Format("{0}_FRIEND{1}_{2}", npcName, Relation.CurrentFriendLevel(_pid), curSeason));

		if (Relation.HasLove(_pid))
			possibleConvs.Add(string.Format("{0}_LOVE{1}_{2}", npcName, Relation.CurrentLoveLevel(_pid), curSeason));

		if (Relation.IsMarried(_pid))
			possibleConvs.Add(string.Format("{0}_MARRIAGE{1}_{2}", npcName, Relation.CurrentMarriageLevel(_pid), curSeason));

		string c = possibleConvs.ToArray().RandomItem();

		return c;
	}

	public void OnInteract()
	{
		Talk();
	}

	[Button("Debug Talk")]
	public void Talk()
	{
		Talk(QuickFind.NetworkSync.PlayerCharacterID);
	}

	public void Talk(int _pid)
	{
		if (IsBusy)
			return;

		InitializeVariables(_pid);

		SpeakerPID = _pid;

		string c;
		if (HasTalkedToday())
			c = PreviousConversation(); 
		else
			c = GetConversationName();

		ConversationTrg.actor = QuickFind.PlayerTrans;
		ConversationTrg.conversation = c;
		Relation.PlayerData[SpeakerPID].LastConversation = c;

		ConversationTrg.TryStartConversation(transform);
	}
}