using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_BuildDebugToolbar : MonoBehaviour {

    public UnityEngine.UI.Text Text;
    public float updateInterval = 0.5F;

    private float accum = 0; // FPS accumulated over the interval
    private int frames = 0; // Frames drawn over the interval
    private float timeleft; // Left time for current interval


    void Start()
    {
        timeleft = updateInterval;
    }

    void Update()
    {
        timeleft -= Time.deltaTime;
        accum += Time.timeScale / Time.deltaTime;
        ++frames;

        // Interval ended - update GUI text and start new interval
        if (timeleft <= 0.0)
        {
            // display two fractional digits (f2 format)
            float fps = accum / frames;
            string format = System.String.Format("{0:F2} FPS", fps);
            Text.text = format;

            if (fps < 30)
                Text.color = Color.yellow;
            else
                if (fps < 10)
                Text.color = Color.red;
            else
                Text.color = Color.green;
            //	DebugConsole.Log(format,level);
            timeleft = updateInterval;
            accum = 0.0F;
            frames = 0;
        }
    }
}
