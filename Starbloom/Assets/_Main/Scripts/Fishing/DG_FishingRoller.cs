using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_FishingRoller : MonoBehaviour {

    public class FishRollValues
    {
        public DG_FishingAtlasObject AtlasObject;
        public float WaitTime;
        public float Weight;
        public DG_ItemObject.ItemQualityLevels QualityLevel;
    }



    private void Awake()
    {
        QuickFind.FishingRoller = this;
    }


    public FishRollValues GetNewFishRoll(Fishing_MasterHandler.WaterTypes CurrentWater)
    {
        FishRollValues ReturnFish = new FishRollValues();
        DG_PlayerCharacters.RucksackSlot CurrentRucksackSlot = QuickFind.ItemActivateableHandler.CurrentRucksackSlot;
        DG_ItemObject.ItemQualityLevels FishingRodQuality = (DG_ItemObject.ItemQualityLevels)CurrentRucksackSlot.CurrentStackActive;
        WeatherHandler.Seasons CurrentSeason = QuickFind.WeatherHandler.CurrentSeason;
        WeatherHandler.WeatherTyps CurrentWeather = QuickFind.WeatherHandler.CurrentWeather;
        int HourOfDay = QuickFind.TimeHandler.GetCurrentHour();

        List<DG_FishingAtlasObject> AvailableFish = QuickFind.FishingCompendium.FetchFishWithinCurrentCriteria(FishingRodQuality, CurrentWater, CurrentSeason, CurrentWeather, HourOfDay);

        ReturnFish.AtlasObject = RollForFish(AvailableFish);
        ReturnFish.WaitTime = GetFishWaitTime(ReturnFish.AtlasObject);
        ReturnFish.Weight = GetFishWeight(ReturnFish.AtlasObject);
        ReturnFish.QualityLevel = GetQualityLevel(ReturnFish.AtlasObject);

        return ReturnFish;
    }


    public DG_FishingAtlasObject RollForFish(List<DG_FishingAtlasObject> AvailableFish)
    {
        int RandomPick = Random.Range(0, AvailableFish.Count);
        return AvailableFish[RandomPick];
    }
    public float GetFishWaitTime(DG_FishingAtlasObject AtlasObject)
    {
        float RandomTimeFrame = Random.Range(AtlasObject.WaitTimeRange.MinTime, AtlasObject.WaitTimeRange.MaxTime);
        return RandomTimeFrame;
    }
    public float GetFishWeight(DG_FishingAtlasObject AtlasObject)
    {
        float RandomFishWeight = Random.Range(AtlasObject.FishWeight.MinWeight, AtlasObject.FishWeight.MaxWeight);
        return RandomFishWeight;
    }
    public DG_ItemObject.ItemQualityLevels GetQualityLevel(DG_FishingAtlasObject AtlasObject)
    {
        Debug.Log("TODO - Determine Fish Level Based On Rod Level, and Fishing Level");
        return DG_ItemObject.ItemQualityLevels.Low;
    }
}
