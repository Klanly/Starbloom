using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        public DG.Tweening.Ease EaseMode;
    }

    [Header("Links")]
    public Canvas FadeCanvas = null;
    public UnityEngine.UI.Image FadeImage = null;

    [Header("Times")]
    public SpeedTimes[] FadeInTimes;
    public SpeedTimes[] FadeOutTimes;

    [Header("Callbacks")]
    public UnityEngine.Events.UnityEvent FadeOutComplete;
    public UnityEngine.Events.UnityEvent FadeInComplete;



    float Timer;
    SpeedTimes CurrentSpeedTime;



    private void Awake()
    {
        QuickFind.FadeScreen = this;
    }

    private void Start()
    {
        this.enabled = false;
    }


    private void Update()
    {
        
    }




    public void FadeOut(FadeInSpeeds Speed)
    {
        //CurrentSpeedTime = 
    }
    public void FadeIn(FadeInSpeeds Speed)
    {

    }

    //SpeedTimes GetNewSpeedTime(SpeedTimes[] Times, FadeInSpeeds Speed)
    //{
    //    for(int i = 0; i < Times.Length; i++)
    //    {
    //
    //    }
    //}
}
