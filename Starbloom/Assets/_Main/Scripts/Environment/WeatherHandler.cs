using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class WeatherHandler : MonoBehaviour {

    public enum Seasons
    {
        Spring,
        Summer,
        Fall,
        Winter
    }
    public enum WeatherTyps
    {
        Clear,
        Overcast,
        Raining,
        Snowing
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
    }


    [ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "Weather", NumberOfItemsPerPage = 8, Expanded = false)]
    public WeatherSetting[] SpringWeather;
    [ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "Weather", NumberOfItemsPerPage = 8, Expanded = false)]
    public WeatherSetting[] SummerWeather;
    [ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "Weather", NumberOfItemsPerPage = 8, Expanded = false)]
    public WeatherSetting[] FallWeather;
    [ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "Weather", NumberOfItemsPerPage = 8, Expanded = false)]
    public WeatherSetting[] WinterWeather;



    [Header("Debug")]
    public Seasons DebugSeason;
    public WeatherTyps DebugWeather;

    [ButtonGroup]
    public void ChangeSeason()
    {
        QuickFind.IDMaster.AdjustWeather((int)DebugSeason, (int)DebugWeather);
    }





    private void Awake()
    {
        QuickFind.WeatherHandler = this;
    }





    public void AdjustSeason(int Season, int Weather)
    {
        Seasons SetSeason = (Seasons)Season;
        WeatherTyps SetWeather = (WeatherTyps)Weather;


    }
}
