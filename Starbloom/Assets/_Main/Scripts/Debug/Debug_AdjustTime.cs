using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Debug_AdjustTime : MonoBehaviour {


    public SimulateDayEnd NewDay;
    public SetSpecificTime TimeSpecific;
    public SetTime TimePresets;
    [Header("Weather")]
    public WeatherHandler.Seasons DebugSeason;
    public WeatherHandler.WeatherTyps DebugWeather;
    [ButtonGroup]
    public void ChangeSeason()
    {
        QuickFind.NetworkSync.AdjustTimeByValues(QuickFind.TimeHandler.GetCurrentHour(), 0, QuickFind.Farm.Year, ((int)DebugSeason + 1), QuickFind.Farm.Day);
        QuickFind.NetworkSync.AdjustWeather((int)DebugSeason, (int)DebugWeather);
    }



    [System.Serializable]
    public class SimulateDayEnd
    {
        [ButtonGroup] public void SetNewDay() { QuickFind.TimeHandler.SetNewDay(false); }
        [ButtonGroup] public void SetNewRainyDay() { QuickFind.TimeHandler.SetNewDay(true); }
        [ButtonGroup] public void SetNewMonth() { QuickFind.Farm.Day = 30; QuickFind.TimeHandler.SetNewDay(true); }
    }

    [System.Serializable]
    public class SetSpecificTime
    {
        public int Year;
        public int Month;
        public int Day;
        public int Hour;
        public int Minute;
        [ButtonGroup] public void ChangeToSpecificTime() { QuickFind.NetworkSync.AdjustTimeByValues(Hour, Minute, Year, Month, Day); }
    }

    [System.Serializable]
    public class SetTime
    {
        public TimeHandler.TimePresetEnums PresetTime;
        [ButtonGroup] public void ChangeToPreset() { QuickFind.NetworkSync.AdjustTimeByPreset((int)PresetTime); }
    }
}
