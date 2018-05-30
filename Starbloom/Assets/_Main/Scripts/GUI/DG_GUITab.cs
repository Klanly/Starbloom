using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DG_GUITab : MonoBehaviour {


    public DG_OverviewTabsGUI.TabType Type;
    public Image Icon = null;
    public RectTransform TabShiftRect = null;
    public RectTransform ScaleRect = null;
    public Image HoverOverImage = null;


    [Header("Icon Scale Effect")]
    public float ScaleSize;
    public float ScaleTime;



    float Timer;
    bool ScaleUp = true;
    bool EndLoop = false;





    private void Awake()
    {
        this.enabled = false;
        HoverOverImage.enabled = false;
    }



    public void ItemHoverIn()
    {
        ScaleUp = true;
        Timer = ScaleTime;
        HoverOverImage.enabled = true;
        EndLoop = false;
        this.enabled = true;
    }
    public void ItemHoverOut()
    {
        ScaleUp = false;
        Timer = ScaleTime;
        HoverOverImage.enabled = false;
        EndLoop = true;
        this.enabled = true;
    }
    public void ItemTabHit()
    {
        QuickFind.GUI_OverviewTabs.TabHit(this);
    }



    private void Update()
    {
        Timer = Timer - Time.deltaTime;

        if (Timer < 0)
        {
            Timer = ScaleTime;
            if (!ScaleUp && EndLoop)
            {
                Timer = ScaleTime;
                this.enabled = false;
            }
            ScaleUp = !ScaleUp;
        }

        float TimerPercentage = Timer / ScaleTime;
        if (ScaleUp)
            TimerPercentage = 1 - TimerPercentage;

        float ScaleDiff = ScaleSize - 1;
        float Scale = 1 + (ScaleDiff * TimerPercentage);

        ScaleRect.localScale = new Vector3(Scale, Scale);
    }
}
