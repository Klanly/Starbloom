using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class WeatherHandler : MonoBehaviour
{

    public enum Seasons
    {
        Spring,
        Summer,
        Fall,
        Winter,
        All
    }
    public enum WeatherTyps
    {
        Clear,
        Fog,
        Raining,
        Thunderstorm,
        Snowing,
        HeavySnow,
        All,
        LightClouds,
        Cloudy
    }

    [System.Serializable]
    public class WeatherSetting
    {
        [Header("Data")]
        public WeatherTyps Weather;
        public CloudsGenerator.CloudTypes CloudType;
        public EnviroWeatherPreset PresetValue;
        [Button(ButtonSizes.Small)] public void ChangeWeather() { QuickFind.NetworkSync.AdjustWeather((int)QuickFind.WeatherHandler.CurrentSeason, (int)Weather); }
    }


    [System.Serializable]
    public class SeasonalWeatherRolls
    {
        public WeatherRoll[] Rolls;
    }
    [System.Serializable]
    public class WeatherRoll
    {
        public WeatherTyps Type;
        public float Percent;
    }




    [ReadOnly] public Seasons CurrentSeason = Seasons.Spring;
    [ReadOnly] public WeatherTyps CurrentWeather = WeatherTyps.Clear;


    [Header("Chance Rolls")]
    public SeasonalWeatherRolls SpringChanceRolls;
    public SeasonalWeatherRolls SummerChanceRolls;
    public SeasonalWeatherRolls FallChanceRolls;
    public SeasonalWeatherRolls WinterChanceRolls;


    [Header("Presets")]
    [ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "Weather", NumberOfItemsPerPage = 8, Expanded = false)]
    public WeatherSetting[] SpringWeather;
    [ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "Weather", NumberOfItemsPerPage = 8, Expanded = false)]
    public WeatherSetting[] SummerWeather;
    [ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "Weather", NumberOfItemsPerPage = 8, Expanded = false)]
    public WeatherSetting[] FallWeather;
    [ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "Weather", NumberOfItemsPerPage = 8, Expanded = false)]
    public WeatherSetting[] WinterWeather;




    private void Awake()
    {
        QuickFind.WeatherHandler = this;
    }








    public void SetNewDayWeather(bool ForceRain)
    {
        int Weather = QuickFind.Farm.Weather.TomorrowWeather;
        int Season = QuickFind.Farm.Month - 1;

        AdjustSeason(Season, Weather);
        QuickFind.NetworkSync.AdjustWeather(Season, Weather);

        if (!ForceRain)
        {
            QuickFind.Farm.Weather.TodayWeather = Weather;
            QuickFind.Farm.Weather.TomorrowWeather = QuickFind.Farm.Weather.TwoDayAwayWeather;
            QuickFind.Farm.Weather.TwoDayAwayWeather = RollNewWeatherType();
        }
        else
        {
            QuickFind.Farm.Weather.TodayWeather = 3;
            QuickFind.Farm.Weather.TomorrowWeather = 3;
            QuickFind.Farm.Weather.TwoDayAwayWeather = 3;
        }

        QuickFind.NetworkSync.AdjustFutureWeather();
        QuickFind.TimeHandler.NewDayCalculationsComplete();
    }

    public int RollNewWeatherType()
    {
        int Month = QuickFind.Farm.Month;
        if (QuickFind.Farm.Day > 28)
        {
            Month++;
            if (Month > 4) //New Year;
                Month = 1;
        }
        switch (Month)
        {
            case 1:return RollSeason(SpringChanceRolls);
            case 2: return RollSeason(SummerChanceRolls);
            case 3: return RollSeason(FallChanceRolls);
            case 4: return RollSeason(WinterChanceRolls);
        }
        return 0;
    }
    public int RollSeason(SeasonalWeatherRolls SeasonChanceRolls)
    {
        float Roll = Random.Range(0f, 1f);
        for(int i = 0; i < SeasonChanceRolls.Rolls.Length; i++)
        {
            if (Roll < SeasonChanceRolls.Rolls[i].Percent)
                return (int)SeasonChanceRolls.Rolls[i].Type;
        }
        return 0;
    }



    public void RequestMasterWeather()
    {
        QuickFind.NetworkSync.RequestMasterWeather();
    }
    public void SyncWeatherToMaster()
    {
        List<int> WeatherValues = new List<int>();
        WeatherValues.Add((int)CurrentSeason);
        WeatherValues.Add((int)CurrentWeather);

        QuickFind.NetworkSync.SyncWeatherToMaster(WeatherValues.ToArray());
    }
    public void GetMasterWeather(int[] Weather)
    {
        AdjustSeason(Weather[0], Weather[1]);
    }


    public void AdjustSeason(int Season, int Weather)
    {
        Seasons SetSeason = (Seasons)Season;
        WeatherTyps SetWeather = (WeatherTyps)Weather;

        CurrentSeason = SetSeason;
        CurrentWeather = SetWeather;

        WeatherSetting[] WeatherArray = null;
        switch (SetSeason)
        {
            case Seasons.Spring: WeatherArray = SpringWeather; break;
            case Seasons.Summer: WeatherArray = SummerWeather; break;
            case Seasons.Fall: WeatherArray = FallWeather; break;
            case Seasons.Winter: WeatherArray = WinterWeather; break;
        }
        WeatherSetting WS = null;
        for (int i = 0; i < WeatherArray.Length; i++)
        {
            if (WeatherArray[i].Weather == SetWeather)
                WS = WeatherArray[i];
        }

        if (WS == null) WS = WeatherArray[0];

        SetWeatherValues(WS);
    }
    void SetWeatherValues(WeatherSetting Weather)
    {
        QuickFind.WeatherController.SetWeatherOverwrite(Weather.PresetValue);
        QuickFind.CloudGeneration.GenerateCloudsByType(Weather.CloudType);

        if (Weather.PresetValue.wetnessLevel > 0) QuickFind.RainDropHandler.IsRaining = true;
        else QuickFind.RainDropHandler.IsRaining = false;
    }
}