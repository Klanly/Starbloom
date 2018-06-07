using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;

[RequireComponent(typeof(NPCRelationship)), RequireComponent(typeof(NPCController))]
public class NPCChat : MonoBehaviour
{
	protected NPCController Controller;
	protected NPCRelationship Relation;

	private void Awake()
	{
		Controller = GetComponent<NPCController>();
		Relation = GetComponent<NPCRelationship>();
	}

	[Button("Debug Conversation")]
	private void DebugGenConversation()
	{
		Debug.Log(GetConversationName());
	}

	public string GetConversationName()
	{
		string npcName = Controller.NPCName.ToUpper();
		if (Relation.IsPlayerUnknown)
			return string.Format("{0}_UNKNOWN", npcName);

		if (!Relation.HasMetPlayer)
			return string.Format("{0}_INTRO", npcName);

		//if (GameLogic.HasActiveEvent)
		//	return string.Format("{0}_{1}{2}", npcName, GameLogic.CurrentEventName, GameLogic.CurrentEventIteration);

		string curSeason = QuickFind.WeatherHandler.CurrentSeason.ToString().ToUpper();
		List<string> possibleConvs = new List<string>(3);

		possibleConvs.Add(string.Format("{0}_FRIEND{1}_{2}", npcName, Relation.CurrentFriendLevel, curSeason));

		if (Relation.HasLove)
			possibleConvs.Add(string.Format("{0}_LOVE{1}_{2}", npcName, Relation.CurrentLoveLevel, curSeason));
		
		if( Relation.IsMarried )
			possibleConvs.Add(string.Format("{0}_MARRIAGE{1}_{2}", npcName, Relation.CurrentMarriageLevel, curSeason));

		return possibleConvs.ToArray().RandomItem();
	}

	[Button("Debug Talk")]
	public void Talk()
	{
	}
}