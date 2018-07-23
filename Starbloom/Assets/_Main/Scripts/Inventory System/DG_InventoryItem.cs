using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DG_InventoryItem : MonoBehaviour {

    public Image Icon = null;
    public RectTransform ScaleRect = null;
    public Image HoverOverImage = null;
    public Image Disabled = null;
    public Image ActiveHotbarItem = null;
    public Image QualityLevelOverlay = null;
    public TMPro.TextMeshProUGUI AmountText = null;
    public TMPro.TextMeshProUGUI QualityAmountText = null;

    [Header("Icon Scale Effect")]
    public float ScaleSize;
    public float ScaleTime;

    [Header("Drag Item")]
    public bool isDragDisplay;
    [Header("Trash")]
    public bool isTrash = false;

    [System.NonSerialized] public bool isMirror;
    [System.NonSerialized] public int SlotID;
    [System.NonSerialized] public bool ContainsItem = false;
    [System.NonSerialized] public bool IsStorageSlot = false;
    [System.NonSerialized] public bool isPlayer1;

    float Timer;
    bool ScaleUp = true;
    bool EndLoop = false;




    private void Awake()
    {
        HoverOverImage.enabled = false;
        ActiveHotbarItem.enabled = false;
        QualityLevelOverlay.enabled = false;
        QualityAmountText.text = string.Empty;
        AmountText.text = string.Empty;
    }
    private void Start()
    {
        if(!isDragDisplay) this.enabled = false;
        if (!isTrash) Icon.sprite = QuickFind.GUI_Inventory.DefaultNullSprite;
    }



    public void ItemHoverIn()
    {
        ScaleUp = true;
        Timer = ScaleTime;
        HoverOverImage.enabled = true;
        EndLoop = false;
        this.enabled = true;

        int ArrayNum = 0;
        if (!isPlayer1) ArrayNum = 1;

        int PlayerID = QuickFind.NetworkSync.Player1PlayerCharacter;
        if (!isPlayer1) PlayerID = QuickFind.NetworkSync.Player2PlayerCharacter;

        QuickFind.GUI_Inventory.PlayersInventory[ArrayNum].CurrentHoverItem = this;

        if (ContainsItem)
        {
            if(!isMirror) QuickFind.TooltipHandler.TooltipGuis[ArrayNum].HoveredInventoryItem = this;

            DG_PlayerCharacters.RucksackSlot RSS = QuickFind.InventoryManager.GetRuckSackSlotInventoryItem(this, PlayerID);
            QuickFind.TooltipHandler.TooltipGuis[ArrayNum].ActiveRucksackSlot = RSS;
            QuickFind.TooltipHandler.ShowToolTip(QuickFind.ItemDatabase.GetItemFromID(RSS.ContainedItem).ToolTipType, PlayerID);
        }
    }
    public void ItemHoverOut()
    {
        ScaleUp = false;
        Timer = ScaleTime;
        HoverOverImage.enabled = false;
        EndLoop = true;
        this.enabled = true;

        int PlayerID = QuickFind.NetworkSync.Player1PlayerCharacter;
        if (!isPlayer1) PlayerID = QuickFind.NetworkSync.Player2PlayerCharacter;

        QuickFind.TooltipHandler.HideToolTip(PlayerID);
    }

    public void ItemPressed()
    {
        if (Input.GetMouseButtonDown(1)) return;

        int ArrayNum = 0;
        if (!isPlayer1) ArrayNum = 1;

        if (QuickFind.ShippingBinGUI.ShippingGUIS[ArrayNum].BinUIisOpen) QuickFind.ShippingBinGUI.ShippingGUIS[ArrayNum].DropPanel.SetActive(false);
        QuickFind.GUI_Inventory.InventoryItemPressed(this);
    }

    public void OnDrag()
    {
        if (isMirror) return;

        int ArrayNum = 0;
        if (!isPlayer1) ArrayNum = 1;

        if (!QuickFind.GUI_Inventory.PlayersInventory[ArrayNum].isFloatingInventoryItem) ItemPressed();
    }
    public void DragReleased()
    {
        if (isMirror) return;

        int ArrayNum = 0;
        if (!isPlayer1) ArrayNum = 1;

        if (QuickFind.ShippingBinGUI.ShippingGUIS[ArrayNum].BinUIisOpen) QuickFind.ShippingBinGUI.ShippingGUIS[ArrayNum].DropPanel.SetActive(false);
        ItemPressed();
    }



    private void Update()
    {
        Timer = Timer - Time.deltaTime;

        if (Timer < 0)
        {
            Timer = 0;
            if (!isDragDisplay && EndLoop) this.enabled = false;
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
