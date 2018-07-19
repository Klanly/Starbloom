using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class DG_InventoryGUI : MonoBehaviour
{

    public bool isPlayer1;

    [Header("Canvases")]
    public CanvasGroup UICanvas = null;
    [Header("Grid Parents")]
    public Transform HotbarGrid;
    public Transform NonHotbarGrid;
    public Transform HotbarMirrorGrid;

    [Header("Null Item")]
    public Sprite DefaultNullSprite = null;

    [Header("Floating Inventory Item")]
    public RectTransform FloatingRect;
    public DG_InventoryItem FloatingItem;

    [Header("Shifing UI Position")]
    public RectTransform InventoryFrame;
    public Vector3 StoragePosition;
    public Vector3 ShopPosition;
    public Vector3 ShippingBinPosition;
    Vector3 StartingPosition;

    [System.NonSerialized] public bool InventoryIsOpen;
    [System.NonSerialized] public bool OuterInventoryIsOpen;


    [System.NonSerialized] public DG_InventoryItem[] GuiItemSlots;
    [System.NonSerialized] public DG_InventoryItem[] HotbarSlots;

    //InventorySwap Stuff
    [System.NonSerialized] public DG_InventoryItem CurrentHoverItem;
    [System.NonSerialized] public bool isFloatingInventoryItem;
    [System.NonSerialized] public DG_InventoryItem PickedUpItemSlot;

    //Equiped Hotbar Slot
    [System.NonSerialized] public int EquippedHotbarSlot = 0;


    private void Awake()
    {
        QuickFind.GUI_Inventory = this;

        int index = 0;
        int Count = HotbarGrid.childCount + NonHotbarGrid.childCount;
        GuiItemSlots = new DG_InventoryItem[Count];
        for (int i = 0; i < HotbarGrid.childCount; i++)
        { GuiItemSlots[index] = HotbarGrid.GetChild(i).GetComponent<DG_InventoryItem>(); GuiItemSlots[index].SlotID = index; GuiItemSlots[index].isPlayer1 = isPlayer1; index++; }
        for (int i = 0; i < NonHotbarGrid.childCount; i++)
        { GuiItemSlots[index] = NonHotbarGrid.GetChild(i).GetComponent<DG_InventoryItem>(); GuiItemSlots[index].SlotID = index; GuiItemSlots[index].isPlayer1 = isPlayer1; index++; }

        HotbarSlots = new DG_InventoryItem[HotbarMirrorGrid.childCount];
        for (int i = 0; i < HotbarSlots.Length; i++)
        { HotbarSlots[i] = HotbarMirrorGrid.GetChild(i).GetComponent<DG_InventoryItem>(); HotbarSlots[i].isMirror = true; HotbarSlots[i].SlotID = i; HotbarSlots[i].Disabled.enabled = false; HotbarSlots[i].isPlayer1 = isPlayer1; }

        StartingPosition = InventoryFrame.localPosition;
    }



    private void Start()
    {
        QuickFind.EnableCanvas(UICanvas, false);
        transform.localPosition = Vector3.zero;
        AdjustFloatingInventoryUI(false);
    }

    private void Update()
    {
        if (isFloatingInventoryItem)
            HandleFloatingInventoryItem();

        if (QuickFind.NetworkSync != null)
        {
            int PlayerID = QuickFind.NetworkSync.Player1PlayerCharacter;
            if (!isPlayer1) PlayerID = QuickFind.NetworkSync.Player2PlayerCharacter;

            if (!InventoryIsOpen)
            {
                float ZoomAxis = QuickFind.InputController.GetPlayerByPlayerID(PlayerID).CamZoomAxis;
                if (ZoomAxis != 0 && !QuickFind.NetworkSync.GetCharacterLinkByPlayerID(PlayerID).AnimationSync.MidAnimation)
                {
                    bool Add = false;
                    if (ZoomAxis < 0)
                        Add = true;
                    int NextEquippedHotbarSlot = QuickFind.GetNextValueInArray(EquippedHotbarSlot, HotbarSlots.Length, Add, true);
                    SetHotbarSlot(HotbarSlots[NextEquippedHotbarSlot]);
                }
            }
            else 
            {
                bool Quality = QuickFind.TooltipHandler.IsQualitySelection;
                if (Quality)
                {
                    float ZoomAxis = QuickFind.InputController.GetPlayerByPlayerID(PlayerID).CamZoomAxis;
                    if (ZoomAxis != 0)
                        QuickFind.TooltipHandler.UpdateEquippedNum(-(int)ZoomAxis, false);
                }
                if (QuickFind.InputController.GetPlayerByPlayerID(PlayerID).ButtonSet.Action.Up && !QuickFind.ShippingBinGUI.BinUIisOpen)
                {
                    if (isFloatingInventoryItem)
                        QuickFind.InventoryManager.DropOne(PickedUpItemSlot, CurrentHoverItem, PlayerID);
                    else if (!QuickFind.StorageUI.StorageUIOpen && Quality)
                        QuickFind.TooltipHandler.UpdateEquippedNum(1, true);
                }
            }
        }
    }


    public void OpenUI()
    {
        QuickFind.GUI_OverviewTabs.CloseAllTabs();
        QuickFind.EnableCanvas(UICanvas, true);
        UpdateInventoryVisuals();
        InventoryIsOpen = true;
    }
    public void CloseUI()
    {
        InventoryIsOpen = false;
        QuickFind.EnableCanvas(UICanvas, false);
    }



    public void OpenStorageUI() { InventoryFrame.localPosition = StoragePosition; OpenOuterUI(); }
    public void OpenShopUI() { InventoryFrame.localPosition = ShopPosition; OpenOuterUI(); }
    public void OpenShippingUI() { InventoryFrame.localPosition = ShippingBinPosition; OpenOuterUI(); }

    public void CloseStorageUI() { CloseOuterUI(); }
    public void CloseShopUI() { CloseOuterUI(); }
    public void CloseShippingUI() { CloseOuterUI(); }

    void OpenOuterUI()
    {
        int PlayerID = QuickFind.NetworkSync.Player1PlayerCharacter;
        if (!isPlayer1) PlayerID = QuickFind.NetworkSync.Player2PlayerCharacter;
        DG_CharacterLink CharLink = QuickFind.NetworkSync.GetCharacterLinkByPlayerID(PlayerID);

        QuickFind.PlayerCam.EnableCamera(CharLink.PlayerCam, false, true);

        QuickFind.ContextDetectionHandler.COEncountered = null;
        QuickFind.ContextDetectionHandler.LastEncounteredContext = null;
        QuickFind.EnableCanvas(UICanvas, true);
        UpdateInventoryVisuals();
        InventoryIsOpen = true;
        OuterInventoryIsOpen = true;
    }
    void CloseOuterUI()
    {
        int PlayerID = QuickFind.NetworkSync.Player1PlayerCharacter;
        if (!isPlayer1) PlayerID = QuickFind.NetworkSync.Player2PlayerCharacter;
        DG_CharacterLink CharLink = QuickFind.NetworkSync.GetCharacterLinkByPlayerID(PlayerID);

        QuickFind.PlayerCam.EnableCamera(CharLink.PlayerCam, true);

        ClearFloatingObject();
        QuickFind.TooltipHandler.HideToolTip();
        
        InventoryFrame.localPosition = StartingPosition;
        InventoryIsOpen = false;
        OuterInventoryIsOpen = false;
        QuickFind.EnableCanvas(UICanvas, false);
    }







    public void UpdateInventoryVisuals()
    {
        int PlayerID = QuickFind.NetworkSync.Player1PlayerCharacter;
        if (!isPlayer1) PlayerID = QuickFind.NetworkSync.Player2PlayerCharacter;

        DG_PlayerCharacters.CharacterEquipment Equipment = QuickFind.Farm.PlayerCharacters[PlayerID].Equipment;
        for (int i = 0; i < Equipment.RucksackSlots.Length; i++)
        {
            if (i < Equipment.RuckSackUnlockedSize)
                UpdateRucksackSlotVisual(GuiItemSlots[i], Equipment.RucksackSlots[i]);
            else //Disable Locked Rucksack Slots
                GuiItemSlots[i].Disabled.enabled = true;
        }
        UpdateMirrorGrid();
        UpdateFloatingItemVisuals();
    }
    public void UpdateRucksackSlotVisual(DG_InventoryItem GuiSlot, DG_PlayerCharacters.RucksackSlot RucksackSlot)
    {
        if(isFloatingInventoryItem){if (PickedUpItemSlot == GuiSlot) { return; }}

        GuiSlot.Disabled.enabled = false;
        if (RucksackSlot.GetStackValue() == 0)
        {
            GuiSlot.ContainsItem = false;

            GuiSlot.isPlayer1 = isPlayer1;
            GuiSlot.Icon.sprite = DefaultNullSprite;
            GuiSlot.AmountText.text = string.Empty;
            GuiSlot.QualityAmountText.text = string.Empty;
            GuiSlot.QualityLevelOverlay.enabled = false;
        }
        else
        {
            DG_ItemObject Object = QuickFind.ItemDatabase.GetItemFromID(RucksackSlot.ContainedItem);
            GuiSlot.ContainsItem = true;
            GuiSlot.Icon.sprite = Object.GetItemSpriteByQuality(RucksackSlot.CurrentStackActive);
            if (RucksackSlot.GetStackValue() > 1)
            {
                GuiSlot.AmountText.text = RucksackSlot.GetStackValue().ToString();
                GuiSlot.QualityAmountText.text = RucksackSlot.GetNumberOfQuality((DG_ItemObject.ItemQualityLevels)RucksackSlot.CurrentStackActive).ToString();
            }
            else
            {
                GuiSlot.AmountText.text = string.Empty;
                GuiSlot.QualityAmountText.text = string.Empty;
            }

            if (Object.MaxStackSize > 1)
            {
                GuiSlot.QualityLevelOverlay.enabled = true;
                DG_ItemsDatabase.GenericIconDatabaseItem ICD = QuickFind.ItemDatabase.GetGenericIconByString("QualityMarker");
                GuiSlot.QualityLevelOverlay.sprite = ICD.Icon;
                GuiSlot.QualityLevelOverlay.color = ICD.ColorVariations[RucksackSlot.CurrentStackActive];
            }
            else
                GuiSlot.QualityLevelOverlay.enabled = false;
        }
    }


    public void UpdateMirrorGrid()
    {
        for (int i = 0; i < HotbarMirrorGrid.childCount; i++)
            UpdateMirrorSlot(i);
    }
    public void UpdateMirrorSlot(int i)
    {
        DG_InventoryItem Main = GuiItemSlots[i];
        DG_InventoryItem Mirror = HotbarMirrorGrid.GetChild(i).GetComponent<DG_InventoryItem>();
        Mirror.Icon.sprite = Main.Icon.sprite;
        Mirror.AmountText.text = Main.AmountText.text;
        Mirror.ContainsItem = Main.ContainsItem;

        Mirror.QualityLevelOverlay.enabled = Main.QualityLevelOverlay.isActiveAndEnabled;
        Mirror.QualityLevelOverlay.sprite = Main.QualityLevelOverlay.sprite;
        Mirror.QualityLevelOverlay.color = Main.QualityLevelOverlay.color;
        Mirror.QualityAmountText.text = Main.QualityAmountText.text;
    }







    public void InventoryItemPressed(DG_InventoryItem PressedItem)
    {
        //This is a mirror Item, and This is for Setting Current Equipped Item
        if (PressedItem.isMirror) SetHotbarSlot(PressedItem);

        //This is an Inventory Item, and we want to handle user moving item somewhere.
        else
        {
            int PlayerID = QuickFind.NetworkSync.Player1PlayerCharacter;
            if (!isPlayer1) PlayerID = QuickFind.NetworkSync.Player2PlayerCharacter;

            if (isFloatingInventoryItem) //We already have an item held, so we must Swap Items
            {
                AdjustFloatingInventoryUI(false);

                DG_PlayerCharacters.RucksackSlot RucksackSlotA = QuickFind.InventoryManager.GetRuckSackSlotInventoryItem(PickedUpItemSlot, PlayerID);
                DG_PlayerCharacters.RucksackSlot RucksackSlotB = QuickFind.InventoryManager.GetRuckSackSlotInventoryItem(PressedItem, PlayerID);
                int IndexA = PlayerID;
                int IndexB = PlayerID;
                if (PickedUpItemSlot.IsStorageSlot) IndexA = QuickFind.StorageUI.ActiveStorage.transform.GetSiblingIndex();
                if (PressedItem.IsStorageSlot) IndexB = QuickFind.StorageUI.ActiveStorage.transform.GetSiblingIndex();

                if(RucksackSlotA.ContainedItem != RucksackSlotB.ContainedItem)
                    QuickFind.InventoryManager.SwapInventoryItems(RucksackSlotA, RucksackSlotB, PickedUpItemSlot, PressedItem, IndexA, IndexB, PlayerID);
                else
                    QuickFind.InventoryManager.DropStackOntoStack(RucksackSlotA, RucksackSlotB, PickedUpItemSlot, PressedItem, IndexA, IndexB, PlayerID);

                PressedItem.ItemHoverIn();
            }
            else
            {
                DG_PlayerCharacters.RucksackSlot RucksackSlotA = QuickFind.InventoryManager.GetRuckSackSlotInventoryItem(PressedItem, PlayerID);
                if (RucksackSlotA.GetStackValue() != 0)
                { AdjustFloatingInventoryUI(true); PickupItem(PressedItem); }
            }
        }
    }



    public void ClearFloatingObject()
    {
        PickedUpItemSlot = null;
        isFloatingInventoryItem = false;
        FloatingRect.position = new Vector3(8000, 0, 0);
    }


    void UpdateFloatingItemVisuals()
    {
        if (PickedUpItemSlot == null) return;

        int PlayerID = QuickFind.NetworkSync.Player1PlayerCharacter;
        if (!isPlayer1) PlayerID = QuickFind.NetworkSync.Player2PlayerCharacter;

        DG_PlayerCharacters.RucksackSlot RucksackSlot = QuickFind.InventoryManager.GetRuckSackSlotInventoryItem(PickedUpItemSlot, PlayerID);
        DG_ItemObject Object = QuickFind.ItemDatabase.GetItemFromID(RucksackSlot.ContainedItem);
        FloatingItem.AmountText.text = RucksackSlot.GetStackValue().ToString();
        FloatingItem.Icon.sprite = Object.GetItemSpriteByQuality(RucksackSlot.CurrentStackActive);
    }






    void PickupItem(DG_InventoryItem PressedItem)
    {
        if (PressedItem.isTrash) { return; }

        PickedUpItemSlot = PressedItem;

        UpdateFloatingItemVisuals();

        PressedItem.AmountText.text = string.Empty;
        PressedItem.Icon.sprite = DefaultNullSprite;
        PressedItem.QualityLevelOverlay.enabled = false;
        PressedItem.QualityAmountText.text = string.Empty;
    }
    void AdjustFloatingInventoryUI(bool Enabled)
    {
        if (CurrentHoverItem != null && CurrentHoverItem.isTrash) return;

        isFloatingInventoryItem = Enabled;
        if (QuickFind.ShippingBinGUI.BinUIisOpen) QuickFind.ShippingBinGUI.OpenDropPanel(Enabled);
        if (!Enabled)
            FloatingRect.position = new Vector3(8000, 0, 0);
    }

    void HandleFloatingInventoryItem()
    {
        FloatingRect.position = Input.mousePosition;
    }





    public void ResetHotbarSlot()
    {
        SetHotbarSlot(HotbarSlots[EquippedHotbarSlot]);
    }


    public void SetHotbarSlot(DG_InventoryItem PressedItem)
    {
        int PlayerID = QuickFind.NetworkSync.Player1PlayerCharacter;
        if (!isPlayer1) PlayerID = QuickFind.NetworkSync.Player2PlayerCharacter;

        if (QuickFind.NetworkSync.GetCharacterLinkByPlayerID(PlayerID).AnimationSync.DisableWeaponSwitching) return;

        HotbarSlots[EquippedHotbarSlot].ActiveHotbarItem.enabled = false;
        EquippedHotbarSlot = PressedItem.SlotID;
        PressedItem.ActiveHotbarItem.enabled = true;


        DG_PlayerCharacters.CharacterEquipment Equipment = QuickFind.Farm.PlayerCharacters[PlayerID].Equipment;
        if (Equipment.RucksackSlots[EquippedHotbarSlot].GetStackValue() == 0)
            QuickFind.ItemActivateableHandler.SetNoActiveItem();
        else
        {
            DG_ItemObject Object = QuickFind.ItemDatabase.GetItemFromID(Equipment.RucksackSlots[EquippedHotbarSlot].ContainedItem);
            QuickFind.ItemActivateableHandler.SetCurrentActiveItem(Equipment.RucksackSlots[EquippedHotbarSlot], Object, EquippedHotbarSlot, PlayerID);
        }
    }
}
