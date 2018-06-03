using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_UI_WobbleAndFade : MonoBehaviour {

    enum State
    {
        WobblingUp,
        WobblingDown,
        Shrinking
    }


    [Header("Wobble")]
    public bool isWobble;
    public float WobbleTime;
    public float WobbleMaxScale;
    public int WobbleCount;
    int CurrentCount = 0;

    [Header("Shrink")]
    public bool isShrink;
    public float ShrinkTime;

    [Header("ScaleAtAwake")]
    bool ScaleAtAwake = true;

    Transform _T;
    Vector3 KnownScale;
    State CurrentState;
    float Timer;


    private void Awake()
    {
        _T = transform;
        KnownScale = _T.localScale;
        if(ScaleAtAwake)
            _T.localScale = Vector3.zero;
        this.enabled = false;
    }

    private void OnEnable()
    {
        _T.localScale = KnownScale;
        if (isWobble) { Timer = WobbleTime; CurrentCount = 0; CurrentState = State.WobblingUp; }
        else if (isShrink) { Timer = ShrinkTime; CurrentState = State.Shrinking; }
    }

    public void Enable(){ KnownScale = _T.localScale; this.enabled = true; }

    public void Disable() { if(_T != null) _T.localScale = KnownScale; this.enabled = false; }

    private void DisableThis() { _T.localScale = Vector3.zero; this.enabled = false; }


    private void Update()
    {
        Timer = Timer - Time.deltaTime;

        if (Timer < 0) //Shift Direction, or Shift to Scale, or Simply Just Stop.
        {
            if (CurrentState == State.WobblingUp) { CurrentState = State.WobblingDown; Timer = WobbleTime; }
            else if (CurrentState == State.WobblingDown){ CurrentCount++; if (CurrentCount >= WobbleCount) {if (isShrink){Timer = ShrinkTime;CurrentState = State.Shrinking;}else DisableThis();} else { CurrentState = State.WobblingUp; Timer = WobbleTime; } }
            else if(CurrentState == State.Shrinking) { DisableThis(); } }

        float TimerPercentage = 0;
        if (CurrentState == State.WobblingUp)   { TimerPercentage = Timer / WobbleTime; TimerPercentage = 1 - TimerPercentage; }
        if (CurrentState == State.WobblingDown) { TimerPercentage = Timer / WobbleTime; }
        if (CurrentState == State.Shrinking)    { TimerPercentage = Timer / ShrinkTime; }

        float ScaleDiff = 0;
        if (CurrentState == State.WobblingUp || CurrentState == State.WobblingDown) ScaleDiff = WobbleMaxScale - 1;
        if (CurrentState == State.Shrinking) ScaleDiff = 1;

        float Scale = 0;
        if (CurrentState == State.WobblingUp || CurrentState == State.WobblingDown) Scale = 1 + (ScaleDiff * TimerPercentage);
        if (CurrentState == State.Shrinking) Scale = (ScaleDiff * TimerPercentage);

        Vector3 NewScale = KnownScale * Scale;
        _T.localScale = NewScale;
    }
}
