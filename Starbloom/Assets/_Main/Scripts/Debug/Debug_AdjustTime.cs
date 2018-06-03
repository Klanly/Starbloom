using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Debug_AdjustTime : MonoBehaviour {


    public SimulateDayEnd NewDay;
    public SetSpecificTime TimeSpecific;
    public SetTime TimePresets;



    [System.Serializable]
    public class SimulateDayEnd
    {
        [ButtonGroup] public void SetNewDay() { QuickFind.TimeHandler.SetNewDay(false); }
        [ButtonGroup] public void SetNewRainyDay() { QuickFind.TimeHandler.SetNewDay(true); }
        }

    [System.Serializable]
    public class SetSpecificTime
    {
        public int Year;
        public int Month;
        public int Day;
        public int Hour;
        public int Minute;
        [ButtonGroup] public void ChangeToSpecificTime() { QuickFind.NetworkSync.AdjustTimeByValues(Year, Month, Day, Hour, Minute); }
    }

    [System.Serializable]
    public class SetTime
    {
        public TimeHandler.TimePresetEnums PresetTime;
        [ButtonGroup] public void ChangeToPreset() { QuickFind.NetworkSync.AdjustTimeByPreset((int)PresetTime); }
    }
}
