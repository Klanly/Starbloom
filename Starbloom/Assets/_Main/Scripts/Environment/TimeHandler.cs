using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

public class TimeHandler : MonoBehaviour
{
    public static event Action<int> OnNewHour;
    public static event Action<int> OnNewTenMinute;

    public enum TimePresetEnums
    {
        Morning,
        Noon,
        Evening,
        Night
    }
    [System.Serializable]
	public class TimeOfDayPeriod
	{
		public float StartHour;
		public float EndHour;
	}
	[System.Serializable]
	public class TimePreset
	{
		[Header("Data")]
		public TimePresetEnums PresetTime;
		public int currentMinute = 45;
		public int currentHour = 5;
        [Button] public void DebugSetTime() { QuickFind.TimeHandler.AdjustTimeByPreset((int)PresetTime); }
	}




    public int DayStartHour = 6;
    public int DayStartMinute = 0;
	[ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "PresetTime", NumberOfItemsPerPage = 8, Expanded = false)]
	public TimePreset[] TimePresets;

    //TimeTracking
    int KnownHour;
    int KnownTenMinute;




	private void Awake()
	{ QuickFind.TimeHandler = this; }


    private void Update()
    {
        if (QuickFind.WeatherController == null) return;

        bool TimeChanged = false;

        int Hour = QuickFind.WeatherController.GameTime.Hours;
        if (Hour != KnownHour) { KnownHour = Hour; TimeChanged = true; if (null != OnNewHour) OnNewHour(KnownHour); }

        int TenMinute = (int)(QuickFind.WeatherController.GameTime.Minutes / 10);
        int TenMult = TenMinute * 10;
        if (KnownTenMinute != TenMult) { KnownTenMinute = TenMult; TimeChanged = true; if (null != OnNewTenMinute) OnNewHour(KnownTenMinute); QuickFind.EnvironmentColorSync.AllowSnowUpdate = true; }

        if (QuickFind.GUI_MainOverview == null) return;
        if (TimeChanged) QuickFind.GUI_MainOverview.SetGuiTimeValue(KnownHour, KnownTenMinute);
    }






    public int GetCurrentHour() { return QuickFind.WeatherController.GameTime.Hours; }
	public void RequestMasterTimes() { QuickFind.NetworkSync.RequestMasterTime(); }


	public void SyncTimeToMaster()
	{
		List<float> TimeValues = new List<float>();

		TimeValues.Add(QuickFind.WeatherController.GameTime.Hours);
		TimeValues.Add(QuickFind.WeatherController.GameTime.Minutes);

        QuickFind.NetworkSync.SyncTimeToMaster(TimeValues.ToArray());
	}
	public void GetMasterTimes(float[] Times)
	{
		int Hour = (int)Times[0];
		int Minute = (int)Times[1];

        AdjustTimeByValues(Hour, Minute);
    }





    public void AdjustTimeByPreset(int TimePreset)
    {
        TimePresetEnums Preset = (TimePresetEnums)TimePreset;
        TimePreset TP = null;
        for (int i = 0; i < TimePresets.Length; i++)
        {
            if (TimePresets[i].PresetTime == Preset)
                TP = TimePresets[i];
        }

        SetTime(TP);
    }

    void SetTime(TimePreset TP)
    {
        AdjustTimeByValues(TP.currentHour, TP.currentMinute);
    }


    public void AdjustTimeByValues(int Hour, int Minute, int Year = 0, int Month = 0, int Day = 0)
    {
        if(Year != 0) QuickFind.Farm.Year = Year;
        if(Month != 0) QuickFind.Farm.Month = Month;
        if(Day != 0) QuickFind.Farm.Day = Day;

        QuickFind.WeatherController.SetInternalTime(QuickFind.Farm.Year, QuickFind.Farm.Day, Hour, Minute, 0);
    }













    public void SetDayEnd()
    {
        QuickFind.ShippingBin.DayEndTallyMoney();
        Debug.Log("Todo. Make Money Tally by Catagory GUI");
        SetNewDay(false);
    }


	public void SetNewDay(bool ForceRain)
	{
        int year = QuickFind.Farm.Year;
		int Month = QuickFind.Farm.Month;
		int Day = QuickFind.Farm.Day;


		Day++;
		if (Day > 30) //NEW Month
		{
			Day = 1;
			Month++;
			if (Month > 4) //New Year;
			{
				Month = 1;
				year++;
			}
		}

        QuickFind.NetworkSync.AdjustTimeByValues(DayStartHour, DayStartMinute, year, Month, Day);
		QuickFind.WeatherHandler.SetNewDayWeather(ForceRain);
	}

	public void NewDayCalculationsComplete()
	{
        QuickFind.NetworkGrowthHandler.CheckDayChangedHasBeenWatered();
        QuickFind.GUI_MainOverview.SetMoneyValue(0, QuickFind.Farm.SharedMoney, true);
        QuickFind.GUI_MainOverview.SetGuiDayValue(QuickFind.Farm.Month, QuickFind.Farm.Day);
        QuickFind.SleepHandler.NewDay();

    }




}
