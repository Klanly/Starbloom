using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_OverviewTabsGUI : MonoBehaviour {

    public enum TabType
    {
        Inventory,
        Skills,
        Relationship,
        Map,
        Crafting
    }


    public Transform TabGrid = null;
    [Header("Canvases")]
    public CanvasGroup UICanvas = null;

    public Transform[] TabUIs;


    [HideInInspector] public bool UIisOpen = false;


    private void Awake()
    {
        QuickFind.GUI_OverviewTabs = this;
    }



    private void Start()
    {
        QuickFind.EnableCanvas(UICanvas, false);
        transform.localPosition = Vector3.zero;
    }



    public void OpenUI()
    {
        QuickFind.ContextDetectionHandler.COEncountered = null;
        QuickFind.ContextDetectionHandler.LastEncounteredContext = null;

        UIisOpen = !UIisOpen;
        QuickFind.EnableCanvas(UICanvas, UIisOpen);
        if (UIisOpen)
        {
            QuickFind.GUI_Inventory.OpenUI();
            SetTab(TabType.Inventory);
            QuickFind.InputController.InputState = DG_PlayerInput.CurrentInputState.InMenu;
        }
        else
        {
            CloseAllTabs();
            QuickFind.InputController.InputState = DG_PlayerInput.CurrentInputState.Default;
        }
    }

    public void CloseAllTabs()
    {
        for (int i = 0; i < TabUIs.Length; i++)
            TabUIs[i].SendMessage("CloseUI");
    }


    public void TabHit(DG_GUITab Tab)
    {
        switch(Tab.Type)
        {
            case TabType.Inventory: QuickFind.GUI_Inventory.OpenUI(); break;
            case TabType.Skills: QuickFind.GUI_Skills.OpenUI(); break;
            case TabType.Relationship: break;
            case TabType.Map: break;
            case TabType.Crafting: QuickFind.GUI_Crafting.OpenUI(); break;
        }
        SetTab(Tab.Type);
    }

    void SetTab(TabType TabType)
    {
        for(int i = 0; i < TabGrid.childCount; i++)
        {
            DG_GUITab TabScript = TabGrid.GetChild(i).GetComponent<DG_GUITab>();
            if (TabScript.Type == TabType)
                TabScript.TabShiftRect.localPosition = new Vector2(0, -10);
            else
                TabScript.TabShiftRect.localPosition = Vector2.zero;
        }
    }
}
