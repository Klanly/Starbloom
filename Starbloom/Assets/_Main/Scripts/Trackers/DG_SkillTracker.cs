using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class DG_SkillTracker : MonoBehaviour {

    public enum SkillTags
    {
        Farming,
        Mining,
        Foraging,
        Fishing,
    }

    [System.Serializable]
    public class EquippedExpReward
    {
        public SkillTags SkillType;
        public int[] QualityLevelRewards;
    }





    private void Awake() { QuickFind.SkillTracker = this; }

    [ListDrawerSettings(ShowIndexLabels = true)] public int[] EXPLevels;
    [ListDrawerSettings(ListElementLabelName = "SkillType")] public EquippedExpReward[] Rewards;

    [Header("Debug")]
    public int DebugValue;
    [Button(ButtonSizes.Small)] public void DebugSetPlayer1FishingLevel() { DebugIncreasePlayer1SkillLevel(SkillTags.Fishing, DebugValue); }
    [Button(ButtonSizes.Small)] public void DebugSetPlayer1FarmingLevel() { DebugIncreasePlayer1SkillLevel(SkillTags.Farming, DebugValue); }
    [Button(ButtonSizes.Small)] public void DebugSetPlayer1ForagingLevel() { DebugIncreasePlayer1SkillLevel(SkillTags.Foraging, DebugValue); }
    [Button(ButtonSizes.Small)] public void DebugSetPlayer1MiningLevel() { DebugIncreasePlayer1SkillLevel(SkillTags.Mining, DebugValue); }



    void DebugIncreasePlayer1SkillLevel(SkillTags Skill, int Value)
    {
        int CurrentSkillEXP = GetSkillExp(Skill, QuickFind.NetworkSync.Player1PlayerCharacter);
        int CurrentSkillLevel = GetSkillLevel(Skill, QuickFind.NetworkSync.Player1PlayerCharacter, CurrentSkillEXP);
        SetSkillExp(Skill, Value, QuickFind.NetworkSync.Player1PlayerCharacter);
        Debug.Log(Skill.ToString() + " Set EXP " + Value.ToString());
    }




    public void IncreaseSkillLevel(SkillTags SkillType, DG_ItemObject.ItemQualityLevels ReceivedQuality, int PlayerID)
    {
        int CurrentEXP = GetSkillExp(SkillType, PlayerID);
        int AdditiveEXP = GetRewardByType(SkillType).QualityLevelRewards[(int)ReceivedQuality];
        SetSkillExp(SkillType, CurrentEXP + AdditiveEXP, PlayerID);
    }

    public void IncreaseFishingLevel(DG_FishingRoller.FishRollValues ActiveFishReference, DG_ItemObject.ItemQualityLevels ReceivedQuality, int PlayerID)
    {
        //Fishing EXP is adjusted in Fishing Atlas on a per fish basis.  As some fish may be drastically more difficult to catch.
        int CurrentEXP = GetSkillExp(SkillTags.Fishing, PlayerID);

        int AdditiveEXP = 0;
        if(ActiveFishReference.AtlasObject.HasCustomEXPReward)
            AdditiveEXP = ActiveFishReference.AtlasObject.ExpGainPerCatch;
        else
            AdditiveEXP = GetRewardByType(SkillTags.Fishing).QualityLevelRewards[(int)ReceivedQuality];

        SetSkillExp(SkillTags.Fishing, CurrentEXP + AdditiveEXP, PlayerID);
    }










    public int GetMySkillLevel(SkillTags Skill, int PlayerID)
    {
        int EXP = QuickFind.SkillTracker.GetSkillExp(DG_SkillTracker.SkillTags.Fishing, PlayerID);
        return GetSkillLevel(Skill, PlayerID, EXP);
    }
    public int GetSkillLevel(SkillTags Skill, int PlayerID, int CurrentExp)
    {
        for (int i = 0; i < EXPLevels.Length; i++)
        {
            if (CurrentExp >= EXPLevels[i]) continue;
            else return i;
        }
        return EXPLevels.Length - 1;
    }

    public int GetSkillExp(SkillTags Skill, int PlayerID)
    {
        switch (Skill)
        {
            case SkillTags.Farming: return QuickFind.Farm.PlayerCharacters[PlayerID].NonCombatSkillEXP.Farming;
            case SkillTags.Mining: return QuickFind.Farm.PlayerCharacters[PlayerID].NonCombatSkillEXP.Mining;
            case SkillTags.Foraging: return QuickFind.Farm.PlayerCharacters[PlayerID].NonCombatSkillEXP.Foraging;
            case SkillTags.Fishing: return QuickFind.Farm.PlayerCharacters[PlayerID].NonCombatSkillEXP.Fishing;
        }
        return 0;
    }

    EquippedExpReward GetRewardByType(SkillTags Type)
    {
        for (int i = 0; i < Rewards.Length; i++)
        {
            if (Rewards[i].SkillType == Type)
                return Rewards[i];
        }
        return null;
    }








    //Networking
    public void SetSkillExp(SkillTags Skill, int Amount, int PlayerID)
    {
        int[] SendInts = new int[3];
        SendInts[0] = (int)Skill;
        SendInts[1] = Amount;
        SendInts[2] = PlayerID;

        QuickFind.NetworkSync.UpdatePlayerStat(SendInts);
    }


    public void IncomingSetSkillExp(int[] ReceivedInts)
    {
        SkillTags Skill = (SkillTags)ReceivedInts[0];
        int StatValue = ReceivedInts[1];
        DG_PlayerCharacters.PlayerCharacter PC = QuickFind.Farm.PlayerCharacters[ReceivedInts[2]];

        switch (Skill)
        {
            case SkillTags.Farming: PC.NonCombatSkillEXP.Farming = StatValue; break;
            case SkillTags.Mining: PC.NonCombatSkillEXP.Mining = StatValue; break;
            case SkillTags.Foraging: PC.NonCombatSkillEXP.Foraging = StatValue; break;
            case SkillTags.Fishing: PC.NonCombatSkillEXP.Fishing = StatValue; break;
        }
    }
}
