using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class DG_PlayerCharacters : MonoBehaviour {

    [System.Serializable]
    public class PlayerCharacter
    {
        public string Name;

        [Header("Stats")]
        public int CurrentEnergy;
        public int MaxEnergy;
        public int CurrentHealth;
        public int MaxHealth;

        [Header("Total Experience for Each Skill")]
        public NonCombatSkills NonCombatSkillEXP;

        [Header("Equipment")]
        public CharacterEquipment Equipment;

        [Header("Acheivements")]
        public CharacterAchievements Acheivements;
    }

    [System.Serializable]
    public class CharacterEquipment
    {
        [Header("Equipment Slots -------------------------------------------------------------------------------")]
        public int HatId;
        public int Ring1;
        public int Ring2;
        public int Boots;

        [Header("Rucksack")]
        public int RuckSackUnlockedSize = 12;
        [ListDrawerSettings(NumberOfItemsPerPage = 12, Expanded = false)]
        public RucksackSlot[] RucksackSlots;
    }


    [System.Serializable]
    public class SavedWeather
    {
        public int TodayWeather;
        public int TomorrowWeather;
        public int TwoDayAwayWeather;
    }


    [System.Serializable]
    public class NonCombatSkills
    {
        public int Farming;
        public int Mining;
        public int Foraging;
        public int Fishing;
        [Button(ButtonSizes.Small)]public void DebugIncreaseFishingLevel(){int FishingLevel = QuickFind.FishingStatsHandler.GetMyFishingLevelInt(); Fishing = QuickFind.FishingStatsHandler.FishingLevelStats[FishingLevel + 1].ExpMin;}
    }

    [System.Serializable]
    public class RucksackSlot
    {
        [Header("Slot")]
        public int ContainedItem;
        public int CurrentStackActive;
        public int LowValue;
        public int NormalValue;
        public int HighValue;
        public int MaximumValue;


        public int GetStackValue()
        { return LowValue + NormalValue + HighValue + MaximumValue; }

        public void AddStackQualityValue(DG_ItemObject.ItemQualityLevels QualityLevel, int AdjustValue)
        {
            if (QualityLevel == DG_ItemObject.ItemQualityLevels.Low) LowValue += AdjustValue;
            if (QualityLevel == DG_ItemObject.ItemQualityLevels.Normal) NormalValue += AdjustValue;
            if (QualityLevel == DG_ItemObject.ItemQualityLevels.High) HighValue += AdjustValue;
            if (QualityLevel == DG_ItemObject.ItemQualityLevels.Max) MaximumValue += AdjustValue;
        }
        public bool CheckifStackHasQualityLevel(DG_ItemObject.ItemQualityLevels QualityLevel)
        {
            if (QualityLevel == DG_ItemObject.ItemQualityLevels.Low && LowValue > 0) return true;
            if (QualityLevel == DG_ItemObject.ItemQualityLevels.Normal && NormalValue > 0) return true;
            if (QualityLevel == DG_ItemObject.ItemQualityLevels.High && HighValue > 0) return true;
            if (QualityLevel == DG_ItemObject.ItemQualityLevels.Max && MaximumValue > 0) return true;
            return false;
        }
        public int GetNumberOfQuality(DG_ItemObject.ItemQualityLevels QualityLevel)
        {
            if (QualityLevel == DG_ItemObject.ItemQualityLevels.Low) return LowValue;
            if (QualityLevel == DG_ItemObject.ItemQualityLevels.Normal) return NormalValue;
            if (QualityLevel == DG_ItemObject.ItemQualityLevels.High) return HighValue;
            if (QualityLevel == DG_ItemObject.ItemQualityLevels.Max) return MaximumValue;
            return 0;
        }
        public void ClearRucksack()
        {
            LowValue = 0; NormalValue = 0; HighValue = 0; MaximumValue = 0;
        }
    }

    [System.Serializable]
    public class CharacterAchievements
    {
        [ListDrawerSettings(NumberOfItemsPerPage = 5)]
        public int[] LargestFishCaught;

#if UNITY_EDITOR
        [Button(ButtonSizes.Small)] public void SyncArraySizeToFishCompendiumSize() { LargestFishCaught = new int[QuickFindInEditor.GetEditorFishingCompendium().ItemCatagoryList.Length]; }
#endif

    }












    public string FarmName;
    public int SharedMoney;
    public int Year;
    public int Month;
    public int Day;



    [ListDrawerSettings(NumberOfItemsPerPage = 1, Expanded = false)]
    public List<PlayerCharacter> PlayerCharacters;

    public SavedWeather Weather;



    private void Awake()
    {
        QuickFind.Farm = this;
    }








    public void SetSkillInt(int SkillName, int StatValue, int PlayerNum)
    {
        switch (SkillName)
        {
            case 1: PlayerCharacters[PlayerNum].NonCombatSkillEXP.Farming = StatValue; break;
            case 2: PlayerCharacters[PlayerNum].NonCombatSkillEXP.Mining = StatValue; break;
            case 3: PlayerCharacters[PlayerNum].NonCombatSkillEXP.Foraging = StatValue; break;
            case 4: PlayerCharacters[PlayerNum].NonCombatSkillEXP.Fishing = StatValue; break;
        }
    }
}
