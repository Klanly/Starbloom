using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


public class DG_FishingAtlasObject : MonoBehaviour
{
    [System.Serializable]
    public class CatchWaitTime
    {
        public float MinTime;
        public float MaxTime;
    }

    [System.Serializable]
    public class FishWeightRange
    {
        public float MinWeight;
        public float MaxWeight;
    }



    [HideInInspector] public int DatabaseID;
    [HideInInspector] public bool LockItem;


    public string Name;
    public int ItemObjectRefDatabaseID;

    public bool HasCustomEXPReward;
    [ShowIf("HasCustomEXPReward")]
    public int ExpGainPerCatch;

    public DG_ItemObject.ItemQualityLevels RodRequired;
    public TimeHandler.TimeOfDayPeriod CatchableTimes;
    public Fishing_MasterHandler.WaterTypes[] WaterTypes;
    public WeatherHandler.Seasons[] CatchableSeasons;
    public WeatherHandler.WeatherTyps[] CatchableWeather;

    public CatchWaitTime WaitTimeRange;
    public FishWeightRange FishWeight;

}
