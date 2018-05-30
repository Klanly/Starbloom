using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class DG_FishingLevelStats : MonoBehaviour {

    [System.Serializable]
    public class FishingLevel
    {
        public string DevNotes;
        public float FishingBarSize;
        public int ExpMin;
    }


    [InfoBox("Fishing Strength Used to calculate Total Size of Fish Bar.")]
    [ListDrawerSettings(ListElementLabelName = "DevNotes")]
    public FishingLevel[] FishingLevelStats;



    private void Awake()
    {
        QuickFind.FishingStatsHandler = this;
    }




    public int GetMyFishingLevelInt()
    {
        return GetIndexFromCurrentExp(QuickFind.Farm.PlayerCharacters[QuickFind.NetworkSync.PlayerCharacterID].NonCombatSkillEXP.Fishing);
    }
    public FishingLevel GetMyFishingLevel()
    {
        return GetPlayersFishingLevel(QuickFind.NetworkSync.PlayerCharacterID);
    }
    public FishingLevel GetPlayersFishingLevel(int PlayerNum)
    {
        return FishingLevelStats[GetIndexFromCurrentExp(QuickFind.Farm.PlayerCharacters[PlayerNum].NonCombatSkillEXP.Fishing)];
    }
    public int GetIndexFromCurrentExp(int ExpAmount)
    {
        int AcceptableLevel = 0;
        for (int i = 0; i < FishingLevelStats.Length; i++)
        {
            if (ExpAmount >= FishingLevelStats[i].ExpMin)
                AcceptableLevel = i;
            else
                return AcceptableLevel;
        }
        return AcceptableLevel;
    }
}
