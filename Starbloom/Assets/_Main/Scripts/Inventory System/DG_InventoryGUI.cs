using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_InventoryGUI : MonoBehaviour
{


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


    bool InventoryIsOpen;
    //RuckSack Check Stuff
    DG_PlayerCharacters.RucksackSlot FirstAvailableRucksackSlot;
    int ItemAddSlotPosition;
    DG_InventoryItem[] GuiItemSlots;
    [HideInInspector] public DG_InventoryItem[] HotbarSlots;

    //InventorySwap Stuff
    [HideInInspector] public DG_InventoryItem CurrentHoverItem;
    [HideInInspector] public bool isFloatingInventoryItem;
    int PickedUpItemSlot;

    //Equiped Hotbar Slot
    [HideInInspector] public int EquippedHotbarSlot = 0;



    private void Awake()
    {
        QuickFind.GUI_Inventory = this;

        int index = 0;
        int Count = HotbarGrid.childCount + NonHotbarGrid.childCount;
        GuiItemSlots = new DG_InventoryItem[Count];
        for (int i = 0; i < HotbarGrid.childCount; i++)
        { GuiItemSlots[index] = HotbarGrid.GetChild(i).GetComponent<DG_InventoryItem>(); GuiItemSlots[index].SlotID = index; index++; }
        for (int i = 0; i < NonHotbarGrid.childCount; i++)
        { GuiItemSlots[index] = NonHotbarGrid.GetChild(i).GetComponent<DG_InventoryItem>(); GuiItemSlots[index].SlotID = index; index++; }

        HotbarSlots = new DG_InventoryItem[HotbarMirrorGrid.childCount];
        for (int i = 0; i < HotbarSlots.Length; i++)
        { HotbarSlots[i] = HotbarMirrorGrid.GetChild(i).GetComponent<DG_InventoryItem>(); HotbarSlots[i].isMirror = true; HotbarSlots[i].SlotID = i; HotbarSlots[i].Disabled.enabled = false; }
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

        if (!InventoryIsOpen && QuickFind.NetworkSync != null)
        {
            float ZoomAxis = QuickFind.InputController.MainPlayer.CamZoomAxis;
            if (ZoomAxis != 0)
            {
                bool Add = false;
                if (ZoomAxis < 0)
                    Add = true;
                int NextEquippedHotbarSlot = QuickFind.GetNextValueInArray(EquippedHotbarSlot, HotbarSlots.Length, Add, true);
                SetHotbarSlot(HotbarSlots[NextEquippedHotbarSlot]);
            }
        }
    }


    public void OpenUI()
    {
        InventoryIsOpen = true;
        QuickFind.GUI_OverviewTabs.CloseAllTabs();
        QuickFind.EnableCanvas(UICanvas, true);
        UpdateInventoryVisuals();
    }
    public void CloseUI()
    {
        InventoryIsOpen = false;
        QuickFind.EnableCanvas(UICanvas, false);
    }








    public void UpdateInventoryVisuals()
    {
        DG_PlayerCharacters.CharacterEquipment Equipment = QuickFind.Farm.PlayerCharacters[QuickFind.NetworkSync.PlayerCharacterID].Equipment;
        for (int i = 0; i < Equipment.RucksackSlots.Length; i++)
        {
            if (i < Equipment.RuckSackUnlockedSize)
            {
                DG_PlayerCharacters.RucksackSlot RucksackSlot = Equipment.RucksackSlots[i];
                DG_InventoryItem GuiSlot = GuiItemSlots[i];

                GuiSlot.Disabled.enabled = false;
                if (RucksackSlot.GetStackValue() == 0)
                    GuiSlot.Icon.sprite = DefaultNullSprite;
                else
                {
                    DG_ItemObject Object = QuickFind.ItemDatabase.GetItemFromID(RucksackSlot.ContainedItem);
                    GuiSlot.Icon.sprite = Object.Icon;
                    if (RucksackSlot.GetStackValue() > 1)
                        GuiSlot.AmountText.text = RucksackSlot.GetStackValue().ToString();
                    else
                        GuiSlot.AmountText.text = string.Empty;
                }
            }
            else //Disable Locked Rucksack Slots
                GuiItemSlots[i].Disabled.enabled = true;
        }
        UpdateMirrorGrid();
    }
    void UpdateMirrorGrid()
    {
        for (int i = 0; i < HotbarMirrorGrid.childCount; i++)
        {
            DG_InventoryItem Main = GuiItemSlots[i];
            DG_InventoryItem Mirror = HotbarMirrorGrid.GetChild(i).GetComponent<DG_InventoryItem>();
            Mirror.Icon.sprite = Main.Icon.sprite;
            Mirror.AmountText.text = Main.AmountText.text;
        }
    }



    public void AddItemToRucksack(int PlayerID, int ItemID, DG_ItemObject.ItemQualityLevels QualityLevel)
    {
        DG_PlayerCharacters.CharacterEquipment Equipment = QuickFind.Farm.PlayerCharacters[PlayerID].Equipment;
        DG_ItemObject Object = QuickFind.ItemDatabase.GetItemFromID(ItemID);
        DG_PlayerCharacters.RucksackSlot RucksackSlot = RucksackSlotContainingID(Equipment, Object);

        bool TrashSwap = false;

        if (RucksackSlot == null) //Item Does Not Exist in Bag Yet.
        {
            if (FirstAvailableRucksackSlot == null) // There is no Available Room in Bag.
            {
                TrashSwap = true;
                Debug.Log("Trigger Trash Select GUI");
            }
            else //Add Item to First Available Rucksack Slot
            {
                FirstAvailableRucksackSlot.ContainedItem = Object.DatabaseID;
                FirstAvailableRucksackSlot.AddStackQualityValue(QualityLevel, 1);
                RucksackSlot = FirstAvailableRucksackSlot;
                RucksackSlot.CurrentStackActive = 1;
            }
        }
        else //Add Item to Rucksack Slot
        { RucksackSlot.AddStackQualityValue(QualityLevel, 1); }

        if (!TrashSwap)
            QuickFind.NetworkSync.SetRucksackValue(PlayerID, ItemAddSlotPosition, RucksackSlot.ContainedItem, RucksackSlot.CurrentStackActive,
            RucksackSlot.LowValue, RucksackSlot.NormalValue, RucksackSlot.HighValue, RucksackSlot.MaximumValue);
    }

    DG_PlayerCharacters.RucksackSlot RucksackSlotContainingID(DG_PlayerCharacters.CharacterEquipment Equipment, DG_ItemObject Object)
    {
        FirstAvailableRucksackSlot = null;
        ItemAddSlotPosition = 0;
        int ObjectID = Object.DatabaseID;
        int count = Equipment.RuckSackUnlockedSize;
        for (int i = 0; i < count; i++)
        {
            DG_PlayerCharacters.RucksackSlot RS = Equipment.RucksackSlots[i];
            if (RS.ContainedItem == ObjectID)
            {
                if (RS.GetStackValue() < Object.MaxStackSize)
                { ItemAddSlotPosition = i; return RS; }
            }
            else if (FirstAvailableRucksackSlot == null && RS.ContainedItem == 0)
            { FirstAvailableRucksackSlot = RS; ItemAddSlotPosition = i; }
        }
        return null;
    }



    void SwapInventoryItems(int SlotA, int SlotB, int PlayerIDA, int PlayerIDB)
    {
        DG_PlayerCharacters.CharacterEquipment EquipmentA = QuickFind.Farm.PlayerCharacters[PlayerIDA].Equipment;
        DG_PlayerCharacters.CharacterEquipment EquipmentB = QuickFind.Farm.PlayerCharacters[PlayerIDB].Equipment;

        DG_PlayerCharacters.RucksackSlot RucksackSlotA = EquipmentA.RucksackSlots[SlotA];
        DG_PlayerCharacters.RucksackSlot RucksackSlotB = EquipmentB.RucksackSlots[SlotB];

        int ContainedItem = RucksackSlotA.ContainedItem;
        int CurrentStackActive = RucksackSlotA.CurrentStackActive;
        int LowValue = RucksackSlotA.LowValue;
        int NormalValue = RucksackSlotA.NormalValue;
        int HighValue = RucksackSlotA.HighValue;
        int MaximumValue = RucksackSlotA.MaximumValue;

        SetItemValueInRucksack(RucksackSlotA, PlayerIDA, SlotA, RucksackSlotB.ContainedItem, RucksackSlotB.CurrentStackActive,
            RucksackSlotB.LowValue, RucksackSlotB.NormalValue, RucksackSlotB.HighValue, RucksackSlotB.MaximumValue);
        SetItemValueInRucksack(RucksackSlotB, PlayerIDB, SlotB, ContainedItem, CurrentStackActive, LowValue, NormalValue, HighValue, MaximumValue);
    }

    public void SetItemValueInRucksack(DG_PlayerCharacters.RucksackSlot RucksackSlot, int PlayerID, int Slot,
        int ContainedItem, int CurrentStackActive, int LowValue, int NormalValue, int HighValue, int MaximumValue)
    {
        RucksackSlot.ContainedItem = ContainedItem;
        RucksackSlot.CurrentStackActive = CurrentStackActive;
        RucksackSlot.LowValue = LowValue;
        RucksackSlot.NormalValue = NormalValue;
        RucksackSlot.HighValue = HighValue;
        RucksackSlot.MaximumValue = MaximumValue;

        QuickFind.NetworkSync.SetRucksackValue(PlayerID, Slot, ContainedItem, CurrentStackActive, LowValue, NormalValue, HighValue, MaximumValue);
    }






    public void InventoryItemPressed(DG_InventoryItem PressedItem)
    {
        //This is a mirror Item, and This is for Setting Current Equipped Item
        if (PressedItem.isMirror)
            SetHotbarSlot(PressedItem);

        //This is an Inventory Item, and we want to handle user moving item somewhere.
        else
        {
            if (isFloatingInventoryItem) //We already have an item held, so we must Swap Items
            {
                int ID = QuickFind.NetworkSync.PlayerCharacterID;
                AdjustFloatingInventoryUI(false); SwapInventoryItems(PickedUpItemSlot, PressedItem.SlotID, ID, ID);
            }
            else { AdjustFloatingInventoryUI(true); PickupItem(PressedItem); }
        }
    }

    void PickupItem(DG_InventoryItem PressedItem)
    {
        PickedUpItemSlot = PressedItem.SlotID;
        FloatingItem.AmountText.text = PressedItem.AmountText.text;
        FloatingItem.Icon.sprite = PressedItem.Icon.sprite;
        PressedItem.AmountText.text = string.Empty;
        PressedItem.Icon.sprite = DefaultNullSprite;
    }
    void AdjustFloatingInventoryUI(bool Enabled)
    {
        isFloatingInventoryItem = Enabled;
        if (!Enabled)
            FloatingRect.position = new Vector3(8000, 0, 0);
    }

    void HandleFloatingInventoryItem()
    {
        FloatingRect.position = Input.mousePosition;
    }







    public void SetHotbarSlot(DG_InventoryItem PressedItem)
    {
        HotbarSlots[EquippedHotbarSlot].ActiveHotbarItem.enabled = false;
        EquippedHotbarSlot = PressedItem.SlotID;
        PressedItem.ActiveHotbarItem.enabled = true;


        DG_PlayerCharacters.CharacterEquipment Equipment = QuickFind.Farm.PlayerCharacters[QuickFind.NetworkSync.PlayerCharacterID].Equipment;
        if (Equipment.RucksackSlots[EquippedHotbarSlot].GetStackValue() == 0)
            QuickFind.ItemActivateableHandler.SetNoActiveItem();
        else
        {
            DG_ItemObject Object = QuickFind.ItemDatabase.GetItemFromID(Equipment.RucksackSlots[EquippedHotbarSlot].ContainedItem);
            QuickFind.ItemActivateableHandler.SetCurrentActiveItem(Equipment.RucksackSlots[EquippedHotbarSlot], Object);
        }
    }










    public int TotalInventoryCountOfItem(int ItemID)
    {
        return 0;
    }
}
