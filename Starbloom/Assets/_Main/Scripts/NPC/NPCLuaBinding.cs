using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;

public class NPCLuaBinding : MonoBehaviour
{
	public static NPCChat CurrentChatAgent;

	void Awake()
	{
		Lua.RegisterFunction("HasTalkedToday", this, typeof(NPCLuaBinding).GetMethod("HasTalkedToday"));
		Lua.RegisterFunction("HasTag", this, typeof(NPCLuaBinding).GetMethod("HasTag"));
		Lua.RegisterFunction("SetTag", this, typeof(NPCLuaBinding).GetMethod("SetTag"));
		Lua.RegisterFunction("RemoveTag", this, typeof(NPCLuaBinding).GetMethod("RemoveTag"));
		Lua.RegisterFunction("AddFriendPoints", this, typeof(NPCLuaBinding).GetMethod("AddFriendPoints"));
		Lua.RegisterFunction("AddLovePoints", this, typeof(NPCLuaBinding).GetMethod("AddLovePoints"));
		Lua.RegisterFunction("AddMarriagePoints", this, typeof(NPCLuaBinding).GetMethod("AddMarriagePoints"));
	}

	void OnDestroy()
	{
		Lua.UnregisterFunction("HasTalkedToday");
		Lua.UnregisterFunction("HasTag");
		Lua.UnregisterFunction("SetTag");
		Lua.UnregisterFunction("RemoveTag");
		Lua.UnregisterFunction("AddFriendPoints");
		Lua.UnregisterFunction("AddLovePoints");
		Lua.UnregisterFunction("AddMarriagePoints");
	}

	protected void ValidateChatAgent()
	{
		Debug.AssertFormat(null != CurrentChatAgent, "Chat agent must be non-null");
	}

	#region Callbacks
	public bool HasTalkedToday()
	{
		ValidateChatAgent();
		return CurrentChatAgent.HasTalkedToday();
	}

	public string PreviousConversation()
	{
		ValidateChatAgent();
		return CurrentChatAgent.PreviousConversation();
	}

	public bool HasTag(string _tag)
	{
		ValidateChatAgent();
		return CurrentChatAgent.HasTag(_tag);
	}

	public void SetTag(string _tag)
	{
		ValidateChatAgent();
		CurrentChatAgent.SetTag(_tag);
	}

	public void RemoveTag(string _tag)
	{
		ValidateChatAgent();
		CurrentChatAgent.RemoveTag(_tag);
	}

	public void AddFriendPoints(float _points)
	{
		ValidateChatAgent();
		CurrentChatAgent.AddFriendPoints(_points);
	}

	public void AddLovePoints(float _points)
	{
		ValidateChatAgent();
		CurrentChatAgent.AddLovePoints(_points);
	}

	public void AddMarriagePoints(float _points)
	{
		ValidateChatAgent();
		CurrentChatAgent.AddMarriagePoints(_points);
	}
	#endregion
}