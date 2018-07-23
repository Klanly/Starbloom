using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_ShippingBinGUI : MonoBehaviour {

    [System.Serializable]
    public class PlayerShippingGUI
    {
        [Header("Grid")]
        public RectTransform DisplayGrid = null;
        public DG_UICustomGridScroll GridScroll;
        [Header("Canvases")]
        public GameObject DropPanel = null;
        public CanvasGroup UICanvas = null;
        public UnityEngine.UI.GraphicRaycaster Raycaster;
        [System.NonSerialized] public bool BinUIisOpen = false;
    }

    public PlayerShippingGUI[] ShippingGUIS;


    private void Awake()
    {
        QuickFind.ShippingBinGUI = this;
    }

    private void Start()
    {
        QuickFind.EnableCanvas(ShippingGUIS[0].UICanvas, false, ShippingGUIS[0].Raycaster);
        QuickFind.EnableCanvas(ShippingGUIS[1].UICanvas, false, ShippingGUIS[1].Raycaster);
        transform.localPosition = Vector3.zero;
        ShippingGUIS[0].DropPanel.SetActive(false);
        ShippingGUIS[1].DropPanel.SetActive(false);
        this.enabled = false;
    }


    private void Update()
    {
        for (int i = 0; i < ShippingGUIS.Length; i++)
        {
            if (QuickFind.InputController.Players[i].CharLink == null) continue;

            if (QuickFind.GUI_Inventory.PlayersInventory[i].isFloatingInventoryItem) return;

            int PlayerID = QuickFind.NetworkSync.Player1PlayerCharacter;
            if (i == 1) PlayerID = QuickFind.NetworkSync.Player2PlayerCharacter;

            if (QuickFind.InputController.Players[i].InteractButton == DG_GameButtons.ButtonState.Up && QuickFind.GUI_Inventory.PlayersInventory[i].CurrentHoverItem != null)
            {
                DG_InventoryItem CurrentHoverItem = QuickFind.GUI_Inventory.PlayersInventory[i].CurrentHoverItem;
                SetItem(CurrentHoverItem);
            }
        }
    }






    public void OpenBinUI(DG_ContextObject Bin, int PlayerID)
    {
        int ArrayNum = 0;
        if (PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) ArrayNum = 1;

        QuickFind.ShippingBin.ActiveBinObject = Bin;
        ShippingGUIS[ArrayNum].GridScroll.ResetGrid();
        LoadBin((ArrayNum == 0));

        ShippingGUIS[ArrayNum].BinUIisOpen = true;
        QuickFind.EnableCanvas(ShippingGUIS[ArrayNum].UICanvas, true, ShippingGUIS[ArrayNum].Raycaster);
        QuickFind.GUI_Inventory.OpenShippingUI(PlayerID);
        this.enabled = true;
    }

    public void P1CloseBinUI()
    {
        CloseBinUI(0);
    }
    public void P2CloseBinUI()
    {
        CloseBinUI(1);
    }

    void CloseBinUI(int Index)
    {
        PlayerShippingGUI PSG = ShippingGUIS[Index];

        int PlayerID = QuickFind.NetworkSync.Player1PlayerCharacter;
        if(Index == 1) PlayerID = QuickFind.NetworkSync.Player2PlayerCharacter;

        this.enabled = false;
        PSG.BinUIisOpen = false;
        QuickFind.EnableCanvas(PSG.UICanvas, false, PSG.Raycaster);
        QuickFind.GUI_Inventory.CloseShippingUI(PlayerID);
        PSG.DropPanel.SetActive(false);
    }


    public void OpenDropPanel(bool isOpen, bool isPlayer1)
    {
        int ArrayNum = 0;
        if (!isPlayer1) ArrayNum = 1;

        ShippingGUIS[ArrayNum].DropPanel.SetActive(isOpen);
    }




    public void FloatingItemDropped(bool isPlayer1)
    {
        int ArrayNum = 0;
        if (!isPlayer1) ArrayNum = 1;

        SetItem(QuickFind.GUI_Inventory.PlayersInventory[ArrayNum].PickedUpItemSlot);
    }
    void SetItem(DG_InventoryItem II)
    {
        int PlayerID = QuickFind.NetworkSync.Player1PlayerCharacter;
        if (!II.isPlayer1) PlayerID = QuickFind.NetworkSync.Player2PlayerCharacter;

        int ArrayNum = 0;
        if (!II.isPlayer1) ArrayNum = 1;

        DG_PlayerCharacters.RucksackSlot RS = QuickFind.InventoryManager.GetRuckSackSlotInventoryItem(II, PlayerID);
        DG_ItemObject Object = QuickFind.ItemDatabase.GetItemFromID(RS.ContainedItem);
        if (!Object.isItem) { return; }

        QuickFind.ShippingBin.SetStackInShippingBin(RS);
        QuickFind.InventoryManager.SetItemValueInRucksack(RS, PlayerID, II.SlotID, 0, 0, 0, 0, 0, 0, false, PlayerID);
        QuickFind.GUI_Inventory.ClearFloatingObject(PlayerID);
        ShippingGUIS[ArrayNum].DropPanel.SetActive(false);
        II.ContainsItem = false;
        QuickFind.TooltipHandler.HideToolTip(PlayerID);
        LoadBin(II.isPlayer1);
    }


    public void BinItemPressed(DG_ShippingBinItem BinItem)
    {
        int PlayerID = QuickFind.NetworkSync.Player1PlayerCharacter;
        if (!BinItem.isPlayer1) PlayerID = QuickFind.NetworkSync.Player2PlayerCharacter;

        int ArrayNum = 0;
        if (PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) ArrayNum = 1;

        QuickFind.InventoryManager.ShiftStackToFromShippingBin(BinItem, PlayerID);
        QuickFind.GUI_Inventory.PlayersInventory[ArrayNum].CurrentHoverItem = null;
        LoadBin(BinItem.isPlayer1);
    }






    public void LoadBin(bool isPlayer1)
    {
        int ArrayNum = 0;
        if (!isPlayer1) ArrayNum = 1;

        PlayerShippingGUI PSG = ShippingGUIS[ArrayNum];

        int index = 0;
        int GridCount = PSG.DisplayGrid.childCount;

        List<DG_ShippingBin.ShippingBinItem> Bin = QuickFind.ShippingBin.DailyShippingItems;
        for (int i = 0; i < Bin.Count; i++)
        {
            DG_ShippingBin.ShippingBinItem SBI = Bin[i];
            DG_ShippingBinItem SGI;
            if (index < GridCount)
            {
                SGI = PSG.DisplayGrid.GetChild(index).GetComponent<DG_ShippingBinItem>();
                SGI.gameObject.SetActive(true);
            }
            else
            {
                Transform New = Instantiate(PSG.DisplayGrid.GetChild(0));
                New.SetParent(PSG.DisplayGrid);
                SGI = New.GetComponent<DG_ShippingBinItem>();
                SGI.gameObject.SetActive(true);
                GridCount++;
            }

            SGI.ReferenceItem = SBI;
            SGI.UpdateVisuals(isPlayer1);
            index++;
        }

        if (index < (GridCount) || index == 0)
        {
            for (int i = index; i < GridCount; i++)
                PSG.DisplayGrid.GetChild(i).gameObject.SetActive(false);
        }
    }
}
