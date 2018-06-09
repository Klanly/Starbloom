using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DG_CraftButton : MonoBehaviour {

    public Image Icon = null;
    public RectTransform ScaleRect = null;
    public Image HoverOverImage = null;
    public Image ActiveHotbarItem = null;

    [Header("Icon Scale Effect")]
    public float ScaleSize;
    public float ScaleTime;

    float Timer;
    bool ScaleUp = true;
    bool EndLoop = false;



    private void Awake()
    {
        HoverOverImage.enabled = false;
        ActiveHotbarItem.enabled = false;
        this.enabled = false;
    }


    public void ItemHoverIn()
    {
        ScaleUp = true;
        Timer = ScaleTime;
        HoverOverImage.enabled = true;
        EndLoop = false;
        this.enabled = true;

        QuickFind.GUI_Crafting.CurrentHoverItem = this;
        //QuickFind.TooltipHandler.ShowToolTip(QuickFind.ItemDatabase.GetItemFromID(RSS.ContainedItem).ToolTipType);
    }
    public void ItemHoverOut()
    {
        ScaleUp = false;
        Timer = ScaleTime;
        HoverOverImage.enabled = false;
        EndLoop = true;
        this.enabled = true;

        QuickFind.TooltipHandler.HideToolTip();
    }

    public void ItemPressed()
    {
        if (Input.GetMouseButtonDown(1)) return;

        QuickFind.GUI_Crafting.CraftButtonPressed();
    }



    private void Update()
    {
        Timer = Timer - Time.deltaTime;

        if (Timer < 0)
        {
            Timer = 0;
            if (EndLoop) this.enabled = false;
            else { ScaleUp = !ScaleUp; Timer = ScaleTime; }
        }

        float TimerPercentage = Timer / ScaleTime;
        if (ScaleUp)
            TimerPercentage = 1 - TimerPercentage;

        float ScaleDiff = ScaleSize - 1;
        float Scale = 1 + (ScaleDiff * TimerPercentage);

        ScaleRect.localScale = new Vector3(Scale, Scale);
    }

}
