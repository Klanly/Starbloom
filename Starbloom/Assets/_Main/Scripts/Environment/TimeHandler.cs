using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

public class TimeHandler : MonoBehaviour
{
	public static event Action<int> OnNewDay;
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
		public int currentSecond = 0;
		public int currentMinute = 45;
		public int currentHour = 5;
	}





	[ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "PresetTime", NumberOfItemsPerPage = 8, Expanded = false)]
	public TimePreset[] TimePresets;

    //TimeTracking
    int KnownHour;
    int KnownTenMinute;




	private void Awake()
	{ QuickFind.TimeHandler = this; }


    private void Update()
    {
        Tenkoku.Core.TenkokuModule TimeModule = QuickFind.WeatherModule;

        bool TimeChanged = false;

        int Hour = TimeModule.currentHour;
        if(Hour != KnownHour) { KnownHour = Hour; TimeChanged = true; if (null != OnNewHour) OnNewHour(KnownHour); }

        int TenMinute = (int)(TimeModule.currentMinute / 10);
        int TenMult = TenMinute * 10;
        if (KnownTenMinute != TenMult) { KnownTenMinute = TenMult; TimeChanged = true; if (null != OnNewTenMinute) OnNewHour(KnownTenMinute); }

        if (TimeChanged) QuickFind.GUI_MainOverview.SetGuiTimeValue(KnownHour, KnownTenMinute);
    }






    public int GetCurrentHour() { return QuickFind.WeatherModule.currentHour; }
	public void RequestMasterTimes() { QuickFind.NetworkSync.RequestMasterTime(); }


	public void SyncTimeToMaster()
	{
		List<float> TimeValues = new List<float>();
		Tenkoku.Core.TenkokuModule TimeModule = QuickFind.WeatherModule;

		TimeValues.Add(TimeModule.currentSecond);
		TimeValues.Add(TimeModule.currentMinute);
		TimeValues.Add(TimeModule.currentHour);
		TimeValues.Add(TimeModule.currentDay);
		TimeValues.Add(TimeModule.currentMonth);
		TimeValues.Add(TimeModule.currentYear);
		TimeValues.Add(TimeModule.setLatitude);
		TimeValues.Add(TimeModule.setLongitude);
		TimeValues.Add(TimeModule.timeCompression);

		QuickFind.NetworkSync.SyncTimeToMaster(TimeValues.ToArray());
	}
	public void GetMasterTimes(float[] Times)
	{
		Tenkoku.Core.TenkokuModule TimeModule = QuickFind.WeatherModule;
		TimeModule.currentSecond = (int)Times[0];
		TimeModule.currentMinute = (int)Times[1];
		TimeModule.currentHour = (int)Times[2];
		TimeModule.currentDay = (int)Times[3];
		TimeModule.currentMonth = (int)Times[4];
		TimeModule.currentYear = (int)Times[5];
		TimeModule.setLatitude = Times[6];
		TimeModule.setLongitude = Times[7];
		TimeModule.timeCompression = Times[8];
	}

    public void AdjustTimeByValues(int Year, int Month, int Day, int Hour, int Minute)
    {
        QuickFind.Farm.Year = Year;
        QuickFind.Farm.Month = Month;
        QuickFind.Farm.Day = Day;

        Tenkoku.Core.TenkokuModule TimeModule = QuickFind.WeatherModule;

        TimeModule.currentHour = Hour;
        TimeModule.currentMinute = Minute;
        TimeModule.currentSecond = 0;
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
        Tenkoku.Core.TenkokuModule TimeModule = QuickFind.WeatherModule;

        TimeModule.currentSecond = TP.currentSecond;
        TimeModule.currentMinute = TP.currentMinute;
        TimeModule.currentHour = TP.currentHour;
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

		QuickFind.NetworkSync.AdjustTimeByValues(year, Month, Day, 6, 0);
		QuickFind.WeatherHandler.SetNewDayWeather(ForceRain);
	}

	public void NewDayCalculationsComplete()
	{
        QuickFind.NetworkGrowthHandler.CheckDayChangedHasBeenWatered();
        QuickFind.GUI_MainOverview.SetMoneyValue(0, QuickFind.Farm.SharedMoney, true);
        QuickFind.GUI_MainOverview.SetGuiDayValue(QuickFind.Farm.Month, QuickFind.Farm.Day);
        if (null != OnNewDay) OnNewDay(QuickFind.Farm.Day);
	}




}
