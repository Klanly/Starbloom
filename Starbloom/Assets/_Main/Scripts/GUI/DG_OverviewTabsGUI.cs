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

    [System.Serializable]
    public class PlayerOverviewTabs
    {
        public Transform TabGrid = null;
        [Header("Canvases")]
        public CanvasGroup UICanvas = null;
        public UnityEngine.UI.GraphicRaycaster Raycaster;

        public Transform[] TabUIs;

        [System.NonSerialized] public bool UIisOpen = false;
    }

    public PlayerOverviewTabs[] OverviewTabs;

    private void Awake()
    {
        QuickFind.GUI_OverviewTabs = this;
    }



    private void Start()
    {
        QuickFind.EnableCanvas(OverviewTabs[0].UICanvas, false, OverviewTabs[0].Raycaster);
        QuickFind.EnableCanvas(OverviewTabs[1].UICanvas, false, OverviewTabs[1].Raycaster);
        transform.localPosition = Vector3.zero;
    }



    public void OpenUI(int Index)
    {
        if (QuickFind.GUI_Inventory.PlayersInventory[Index].OuterInventoryIsOpen) return;

        QuickFind.ContextDetectionHandler.Contexts[Index].COEncountered = null;
        QuickFind.ContextDetectionHandler.Contexts[Index].LastEncounteredContext = null;

        int PlayerID = QuickFind.NetworkSync.Player1PlayerCharacter;
        if (Index == 1) PlayerID = QuickFind.NetworkSync.Player2PlayerCharacter;

        DG_CharacterLink CharLink = QuickFind.InputController.Players[Index].CharLink;

        OverviewTabs[Index].UIisOpen = !OverviewTabs[Index].UIisOpen;
        QuickFind.EnableCanvas(OverviewTabs[Index].UICanvas, OverviewTabs[Index].UIisOpen, OverviewTabs[Index].Raycaster);
        if (OverviewTabs[Index].UIisOpen)
        {
            QuickFind.PlayerCam.EnableCamera(CharLink.PlayerCam, false, true);
            QuickFind.GUI_Inventory.OpenUI(PlayerID);
            SetTab(TabType.Inventory, (Index == 0));
            QuickFind.InputController.GetPlayerByPlayerID(PlayerID).CurrentInputState = DG_PlayerInput.CurrentInputState.InMenu;
        }
        else
        {
            QuickFind.PlayerCam.EnableCamera(CharLink.PlayerCam, true);
            CloseAllTabs(Index, PlayerID);
            QuickFind.InputController.GetPlayerByPlayerID(PlayerID).CurrentInputState = DG_PlayerInput.CurrentInputState.Default;
        }
    }

    public void CloseAllTabs(int Index, int PlayerID)
    {
        PlayerOverviewTabs POT = OverviewTabs[Index];

        for (int i = 0; i < POT.TabUIs.Length; i++)
            POT.TabUIs[i].SendMessage("CloseUI", PlayerID);
    }


    public void TabHit(DG_GUITab Tab)
    {
        int PlayerID = QuickFind.NetworkSync.Player1PlayerCharacter;
        if (!Tab.isPlayer1) PlayerID = QuickFind.NetworkSync.Player2PlayerCharacter;

        switch (Tab.Type)
        {
            case TabType.Inventory: QuickFind.GUI_Inventory.OpenUI(PlayerID); break;
            case TabType.Skills: QuickFind.GUI_Skills.OpenUI(PlayerID); break;
            case TabType.Relationship: break;
            case TabType.Map: break;
            case TabType.Crafting: QuickFind.GUI_Crafting.OpenUI(PlayerID); break;
        }
        SetTab(Tab.Type, Tab.isPlayer1);
    }

    void SetTab(TabType TabType, bool isPlayer1)
    {
        int ArrayNum = 0;
        if (!isPlayer1) ArrayNum = 1;
        PlayerOverviewTabs POT = OverviewTabs[ArrayNum];

        for (int i = 0; i < POT.TabGrid.childCount; i++)
        {
            DG_GUITab TabScript = POT.TabGrid.GetChild(i).GetComponent<DG_GUITab>();
            if (TabScript.Type == TabType)
                TabScript.TabShiftRect.localPosition = new Vector2(0, -10);
            else
                TabScript.TabShiftRect.localPosition = Vector2.zero;
        }
    }
}
