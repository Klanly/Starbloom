using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DG_GUI_FadeScreen : MonoBehaviour {

    public enum FadeInSpeeds
    {
        Instant,
        QuickFade,
        NormalFade,
        SlowFade,
        SuperSlowFade
    }
    [System.Serializable]
    public class SpeedTimes
    {
        [Header("--------------------------------")]
        public FadeInSpeeds Speed;
        public float Time;
    }

    [Header("Links")]
    public UnityEngine.UI.Image FadeImage = null;

    [Header("Times")]
    public SpeedTimes[] FadeInTimes;
    public SpeedTimes[] FadeOutTimes;


    float Timer;
    bool isFadeIn = false;
    SpeedTimes CurrentSpeedTime;
    GameObject ReturnO;
    string ReturnM;


    private void Awake()
    {
        QuickFind.FadeScreen = this;
        Color C = FadeImage.color;
        C.a = 1;
        FadeImage.color = C;
    }
    private void Start()
    {
        this.enabled = false;
        if (!QuickFind.GameSettings.BypassMainMenu)
            FadeIn(FadeInSpeeds.SlowFade);
    }





    private void Update()
    {
        Timer = Timer - Time.deltaTime;
        SetCanvas(Timer);

        if (Timer < 0)
        {
            this.enabled = false;
            if (ReturnO != null)
                ReturnO.SendMessage(ReturnM);
        }
    }



    void SetCanvas(float TimeValue)
    {
        float value = 0;
        if (isFadeIn) value = TimeValue / CurrentSpeedTime.Time;
            else value = (CurrentSpeedTime.Time - TimeValue) / CurrentSpeedTime.Time;
            
        if (value < 0) value = 0;
            else if (value > 1) value = 1;

        Color C = FadeImage.color;
        C.a = value;
        FadeImage.color = C;
    }



    public void FadeOut(FadeInSpeeds Speed, GameObject ReturnObj = null, string ReturnMessage = "")
    {
        ReturnO = ReturnObj;
        ReturnM = ReturnMessage;
        CurrentSpeedTime = GetNewSpeedTime(FadeOutTimes, Speed);
        SetFade(false);
    }
    public void FadeIn(FadeInSpeeds Speed, GameObject ReturnObj = null, string ReturnMessage = "")
    {
        ReturnO = ReturnObj;
        ReturnM = ReturnMessage;
        CurrentSpeedTime = GetNewSpeedTime(FadeInTimes, Speed);
        SetFade(true);
    }


    void SetFade(bool isFadein)
    {
        Timer = CurrentSpeedTime.Time;
        isFadeIn = isFadein;
        SetCanvas(Timer);
        this.enabled = true;
    }

    SpeedTimes GetNewSpeedTime(SpeedTimes[] Times, FadeInSpeeds Speed)
    {
        for(int i = 0; i < Times.Length; i++)
        {
            if (Times[i].Speed == Speed)
                return Times[i];
        }

        Debug.Log("There wasn't a speed enum set in the inspector with the requested value 'Fade Handler', perhaps it was deleted on accident.");
        return null;
    }
}
