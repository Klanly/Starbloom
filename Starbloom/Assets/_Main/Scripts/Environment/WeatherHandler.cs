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
        Overcast,
        Raining,
        Thunderstorm,
        Snowing,
        All
    }

    [System.Serializable]
    public class WeatherSetting
    {
        [Header("Data")]
        public WeatherTyps Weather;


        [Header("Clouds")]
        public float weather_cloudAltoStratusAmt = 0.0f;
        public float weather_cloudCirrusAmt = 0.0f;
        public float weather_cloudCumulusAmt = 0.2f;
        public float weather_cloudScale = 0.5f;
        public float weather_cloudSpeed = 0.0f;
        public float weather_OvercastDarkeningAmt = 1.0f;
        public float weather_OvercastAmt = 0.0f;
        [Header("Rain")]
        public float weather_RainAmt = 0.0f;
        public float weather_lightning = 0.0f;
        [Header("Snow")]
        public float weather_SnowAmt = 0.0f;
        [Header("Wind")]
        public float weather_WindAmt = 0.0f;
        public float weather_WindDir = 0.0f;
        [Header("Fog")]
        public float weather_FogAmt = 0.0f;
        public float weather_FogHeight = 0.0f;
        [Header("Temperature")]
        public float weather_temperature = 75.0f;
        [Header("Humidity")]
        public float weather_humidity = 0.25f;
        [Header("Rainbow")]
        public float weather_rainbow = 0.0f;


        public WaterSettings Water;


        [System.Serializable]
        public class WaterSettings
        {
            [Header("Waves")]
            public float beaufortScale;
            public float flowSpeed;
            public float waveScale;
            public float heightProjection;
            [Header("Bright")]
            public float overallBright;
            public float overallTransparency;
            [Header("Reflect")]
            public float reflectTerm;
            public float reflectSharpen;
            [Header("Specular")]
            public float roughness;
            public float roughness2;
            public Color specularColor;
        }
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




    [System.NonSerialized] public Seasons CurrentSeason = Seasons.Spring;
    [System.NonSerialized] public WeatherTyps CurrentWeather = WeatherTyps.Clear;


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
        Debug.Log("Assign Weather preset for Enviro.");

        if (Weather.weather_RainAmt > 0) QuickFind.RainDropHandler.IsRaining = true;
        else QuickFind.RainDropHandler.IsRaining = false;


        Debug.Log("Adjust Settings for Aquas.");  
    }
}