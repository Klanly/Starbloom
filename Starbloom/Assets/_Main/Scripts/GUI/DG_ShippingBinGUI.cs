using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_ShippingBinGUI : MonoBehaviour {

    [Header("Grid")]
    public RectTransform DisplayGrid = null;
    public DG_UICustomGridScroll GridScroll;
    [Header("Canvases")]
    public GameObject DropPanel = null;
    public CanvasGroup UICanvas = null;
    [HideInInspector] public bool BinUIisOpen = false;


    private void Awake()
    {
        QuickFind.ShippingBinGUI = this;
    }

    private void Start()
    {
        QuickFind.EnableCanvas(UICanvas, false);
        transform.localPosition = Vector3.zero;
        DropPanel.SetActive(false);
        this.enabled = false;
    }


    private void Update()
    {
        if (QuickFind.GUI_Inventory.isFloatingInventoryItem) return;

        if (QuickFind.InputController.MainPlayer.ButtonSet.Interact.Up && QuickFind.GUI_Inventory.CurrentHoverItem != null)
        {
            DG_InventoryItem CurrentHoverItem = QuickFind.GUI_Inventory.CurrentHoverItem;
            SetItem(CurrentHoverItem);
        }
    }






    public void OpenBinUI(DG_ContextObject Bin)
    {
        QuickFind.ShippingBin.ActiveBinObject = Bin;
        GridScroll.ResetGrid();
        LoadBin();

        BinUIisOpen = true;
        QuickFind.EnableCanvas(UICanvas, true);
        QuickFind.GUI_Inventory.OpenShippingUI();
        this.enabled = true;
    }
    public void CloseBinUI()
    {
        this.enabled = false;
        BinUIisOpen = false;
        QuickFind.EnableCanvas(UICanvas, false);
        QuickFind.GUI_Inventory.CloseShippingUI();
        DropPanel.SetActive(false);
    }


    public void OpenDropPanel(bool isOpen)
    {
        DropPanel.SetActive(isOpen);
    }




    public void FloatingItemDropped()
    {
        SetItem(QuickFind.GUI_Inventory.PickedUpItemSlot);
    }
    void SetItem(DG_InventoryItem II)
    {
        DG_PlayerCharacters.RucksackSlot RS = QuickFind.InventoryManager.GetRuckSackSlotInventoryItem(II);
        DG_ItemObject Object = QuickFind.ItemDatabase.GetItemFromID(RS.ContainedItem);
        if (!Object.isItem) { return; }

        QuickFind.ShippingBin.SetStackInShippingBin(RS);
        QuickFind.InventoryManager.SetItemValueInRucksack(RS, QuickFind.NetworkSync.PlayerCharacterID, II.SlotID, 0, 0, 0, 0, 0, 0, false);
        QuickFind.GUI_Inventory.ClearFloatingObject();
        DropPanel.SetActive(false);
        II.ContainsItem = false;
        QuickFind.TooltipHandler.HideToolTip();
        LoadBin();
    }


    public void BinItemPressed(DG_ShippingBinItem BinItem)
    {
        QuickFind.InventoryManager.ShiftStackToFromShippingBin(BinItem);
        QuickFind.GUI_Inventory.CurrentHoverItem = null;
        LoadBin();
    }






    public void LoadBin()
    {
        int index = 0;
        int GridCount = DisplayGrid.childCount;

        List<DG_ShippingBin.ShippingBinItem> Bin = QuickFind.ShippingBin.DailyShippingItems;
        for (int i = 0; i < Bin.Count; i++)
        {
            DG_ShippingBin.ShippingBinItem SBI = Bin[i];
            DG_ShippingBinItem SGI;
            if (index < GridCount)
            {
                SGI = DisplayGrid.GetChild(index).GetComponent<DG_ShippingBinItem>();
                SGI.gameObject.SetActive(true);
            }
            else
            {
                Transform New = Instantiate(DisplayGrid.GetChild(0));
                New.SetParent(DisplayGrid);
                SGI = New.GetComponent<DG_ShippingBinItem>();
                SGI.gameObject.SetActive(true);
                GridCount++;
            }

            SGI.ReferenceItem = SBI;
            SGI.UpdateVisuals();
            index++;
        }

        if (index < (GridCount - 1) || index == 0)
        {
            for (int i = index; i < GridCount; i++)
                DisplayGrid.GetChild(i).gameObject.SetActive(false);
        }
    }
}
