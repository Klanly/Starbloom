using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class DG_FishingCompendium : MonoBehaviour
{

    [HideInInspector] public DG_FishingAtlasObject[] ItemCatagoryList;
    [HideInInspector] public int ListCount;






    private void Awake()
    {
        QuickFind.FishingCompendium = this;
    }





    public List<DG_FishingAtlasObject> FetchFishWithinCurrentCriteria(   DG_ItemObject.ItemQualityLevels EquippedRod, 
                                                            Fishing_MasterHandler.WaterTypes CurrentWater,
                                                            WeatherHandler.Seasons CurrentSeason,
                                                            WeatherHandler.WeatherTyps CurrentWeather,
                                                            int HourOfDay)
    {

        List<DG_FishingAtlasObject> ReturnList = new List<DG_FishingAtlasObject>();

        for(int i = 0; i < ItemCatagoryList.Length; i++)
        {
            DG_FishingAtlasObject FD = ItemCatagoryList[i];
            if (FD.RodRequired != EquippedRod)
                continue;
            if (!ContainsWaterType(FD.WaterTypes, CurrentWater))
                continue;
            if (!ContainsSeason(FD.CatchableSeasons, CurrentSeason))
                continue;
            if (!ContainsWeather(FD.CatchableWeather, CurrentWeather))
                continue;
            if (!WithinTimeOfDay(FD.CatchableTimes, HourOfDay))
                continue;
            ReturnList.Add(FD);
        }

        return ReturnList;
    }


    bool ContainsWaterType(Fishing_MasterHandler.WaterTypes[] Array, Fishing_MasterHandler.WaterTypes Current)
    { for(int i = 0; i < Array.Length; i++) { if (Array[i] == Current || Array[i] == Fishing_MasterHandler.WaterTypes.All) return true; } return false; }

    bool ContainsSeason(WeatherHandler.Seasons[] Array, WeatherHandler.Seasons Current)
    { for (int i = 0; i < Array.Length; i++) { if (Array[i] == Current || Array[i] == WeatherHandler.Seasons.All) return true; } return false; }

    bool ContainsWeather(WeatherHandler.WeatherTyps[] Array, WeatherHandler.WeatherTyps Current)
    { for (int i = 0; i < Array.Length; i++) { if (Array[i] == Current || Array[i] == WeatherHandler.WeatherTyps.All) return true; } return false; }

    bool WithinTimeOfDay(TimeHandler.TimeOfDayPeriod TimePeriod, int CurrentHour)
    { if (CurrentHour >= TimePeriod.StartHour && CurrentHour < TimePeriod.EndHour) return true; else return false;}
}
