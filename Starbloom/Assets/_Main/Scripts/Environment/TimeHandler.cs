using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;



public class TimeHandler : MonoBehaviour
{
    public enum TimePresetEnums
    {
        Morning,
        Noon,
        Evening,
        Night
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

	protected int m_Minute = 0;
	protected int m_Hour = 0;
	protected int m_Day = 0;
	protected int m_Month = 0;
	protected int m_Year = 0;



    [Header("Debug")]
    public TimePresetEnums DebugTime;

    [ButtonGroup]
    public void ChangeTime()
    {
        QuickFind.NetworkSync.AdjustTimeByPreset((int)DebugTime);
    }
	
    private void Awake()
    {
        QuickFind.TimeHandler = this;
    }

	public void RequestMasterTimes()
    {
        QuickFind.NetworkSync.RequestMasterTime();
    }
    public void SyncTimeToMaster()
    {
        List<float> TimeValues = new List<float>();
        Tenkoku.Core.TenkokuModule TimeModule = QuickFind.WeatherModule;

		m_Minute = TimeModule.currentMinute;
		m_Hour = TimeModule.currentHour;
		m_Day = TimeModule.currentDay;
		m_Month = TimeModule.currentMonth;
		m_Year = TimeModule.currentYear;

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
}
