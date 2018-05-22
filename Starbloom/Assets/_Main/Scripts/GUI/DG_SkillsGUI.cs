using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_SkillsGUI : MonoBehaviour {

    [Header("Canvases")]
    public CanvasGroup UICanvas = null;


    private void Awake()
    {
        QuickFind.GUI_Skills = this;
    }



    private void Start()
    {
        QuickFind.EnableCanvas(UICanvas, false);
        transform.localPosition = Vector3.zero;
    }


    public void OpenUI()
    {
        QuickFind.GUI_OverviewTabs.CloseAllTabs();
        QuickFind.EnableCanvas(UICanvas, true);
    }
    public void CloseUI()
    {
        QuickFind.EnableCanvas(UICanvas, false);
    }

}
