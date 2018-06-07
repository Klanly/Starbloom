using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( fileName = "DefaultRelationshipSettings", menuName = "NPC/Relationship Settings" )]
public class NPCRelationshipSettings : ScriptableObject
{
	public int[] FriendshipLevels;
	public int[] LoveLevels;
	public int[] MarriageLevels;
}