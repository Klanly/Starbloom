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

    public bool isPlayer1;

    public Transform TabGrid = null;
    [Header("Canvases")]
    public CanvasGroup UICanvas = null;

    public Transform[] TabUIs;

    [System.NonSerialized] public bool UIisOpen = false;


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
        if (QuickFind.GUI_Inventory.OuterInventoryIsOpen) return;

        QuickFind.ContextDetectionHandler.COEncountered = null;
        QuickFind.ContextDetectionHandler.LastEncounteredContext = null;

        int PlayerID = QuickFind.NetworkSync.Player1PlayerCharacter;
        if (!isPlayer1) PlayerID = QuickFind.NetworkSync.Player2PlayerCharacter;

        DG_CharacterLink CharLink = QuickFind.NetworkSync.GetCharacterLinkByPlayerID(PlayerID);

        UIisOpen = !UIisOpen;
        QuickFind.EnableCanvas(UICanvas, UIisOpen);
        if (UIisOpen)
        {
            QuickFind.PlayerCam.EnableCamera(CharLink.PlayerCam, false, true);
            QuickFind.GUI_Inventory.OpenUI();
            SetTab(TabType.Inventory);
            QuickFind.InputController.GetPlayerByPlayerID(PlayerID).CurrentInputState = DG_PlayerInput.CurrentInputState.InMenu;
        }
        else
        {
            QuickFind.PlayerCam.EnableCamera(CharLink.PlayerCam, true);
            CloseAllTabs();
            QuickFind.InputController.GetPlayerByPlayerID(PlayerID).CurrentInputState = DG_PlayerInput.CurrentInputState.Default;
        }
    }

    public void CloseAllTabs()
    {
        for (int i = 0; i < TabUIs.Length; i++)
            TabUIs[i].SendMessage("CloseUI");
    }


    public void TabHit(DG_GUITab Tab)
    {
        int PlayerID = QuickFind.NetworkSync.Player1PlayerCharacter;
        if (!isPlayer1) PlayerID = QuickFind.NetworkSync.Player2PlayerCharacter;

        switch (Tab.Type)
        {
            case TabType.Inventory: QuickFind.GUI_Inventory.OpenUI(); break;
            case TabType.Skills: QuickFind.GUI_Skills.OpenUI(); break;
            case TabType.Relationship: break;
            case TabType.Map: break;
            case TabType.Crafting: QuickFind.GUI_Crafting.OpenUI(PlayerID); break;
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
