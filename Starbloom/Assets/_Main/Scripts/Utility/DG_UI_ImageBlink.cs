using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_UI_ImageBlink : MonoBehaviour {

    public float BlinkTimeOn = .5f;
    public float BlinkTimeOff = .2f;

    bool On;
    float Timer;


    private void Start()
    {
        Timer = BlinkTimeOn;
        On = true;
    }

    void Update ()
    {
        Timer = Timer - Time.deltaTime;
        if(Timer < 0)
        {
            if(On)
            {
                On = false;
                Timer = BlinkTimeOff;
                transform.GetComponent<UnityEngine.UI.Image>().enabled = false;
            }
            else
            {
                On = true;
                Timer = BlinkTimeOn;
                transform.GetComponent<UnityEngine.UI.Image>().enabled = true;
            }
        }
    }
}
