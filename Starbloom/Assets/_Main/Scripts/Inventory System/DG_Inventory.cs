using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_Inventory : MonoBehaviour {



    public class ExchangeItem
    {
        public DG_PlayerCharacters.RucksackSlot RS;
        public int index;
    }



    //RuckSack Check Stuff
    DG_PlayerCharacters.RucksackSlot FirstAvailableRucksackSlot;
    int ItemAddSlotPosition;
    List<ExchangeItem> ExchangeItems;


    private void Awake()
    {
        QuickFind.InventoryManager = this;
        ExchangeItems = new List<ExchangeItem>();
    }







    public bool AddItemToRucksack(int PlayerID, int ItemID, DG_ItemObject.ItemQualityLevels QualityLevel)
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
            }
            else //Add Item to First Available Rucksack Slot
            {
                FirstAvailableRucksackSlot.ContainedItem = Object.DatabaseID;
                FirstAvailableRucksackSlot.AddStackQualityValue(QualityLevel, 1);
                RucksackSlot = FirstAvailableRucksackSlot;
                RucksackSlot.CurrentStackActive = (int)QualityLevel;
            }
        }
        else //Add Item to Rucksack Slot
        { RucksackSlot.AddStackQualityValue(QualityLevel, 1); }

        if (!TrashSwap)
            QuickFind.NetworkSync.SetRucksackValue(PlayerID, ItemAddSlotPosition, RucksackSlot.ContainedItem, RucksackSlot.CurrentStackActive,
            RucksackSlot.LowValue, RucksackSlot.NormalValue, RucksackSlot.HighValue, RucksackSlot.MaximumValue);

        QuickFind.SystemMessageGUI.GenerateSystemMessage(ItemID);

        return TrashSwap;
    }



    public DG_PlayerCharacters.RucksackSlot RucksackSlotContainingID(DG_PlayerCharacters.CharacterEquipment Equipment, DG_ItemObject Object)
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
            else if (FirstAvailableRucksackSlot == null && RS.GetStackValue() == 0)
            { FirstAvailableRucksackSlot = RS; ItemAddSlotPosition = i; }
        }
        return null;
    }

    public void SwapInventoryItems(DG_PlayerCharacters.RucksackSlot RucksackSlotA, DG_PlayerCharacters.RucksackSlot RucksackSlotB, DG_InventoryItem SlotA, DG_InventoryItem SlotB, int ObjectIndexA, int ObjectIndexB)
    {
        int ContainedItem = RucksackSlotA.ContainedItem;
        int CurrentStackActive = RucksackSlotA.CurrentStackActive;
        int LowValue = RucksackSlotA.LowValue;
        int NormalValue = RucksackSlotA.NormalValue;
        int HighValue = RucksackSlotA.HighValue;
        int MaximumValue = RucksackSlotA.MaximumValue;

        if (!SlotB.isTrash)
            SetItemValueInRucksack(RucksackSlotA, ObjectIndexA, SlotA.SlotID, RucksackSlotB.ContainedItem, RucksackSlotB.CurrentStackActive, RucksackSlotB.LowValue, RucksackSlotB.NormalValue, RucksackSlotB.HighValue, RucksackSlotB.MaximumValue, SlotA.IsStorageSlot);
        else
        {
            SetItemValueInRucksack(RucksackSlotA, ObjectIndexA, SlotA.SlotID, 0, 0, 0, 0, 0, 0, SlotA.IsStorageSlot);
            QuickFind.GUI_Inventory.ClearFloatingObject();
        }

        if (!SlotB.isTrash) SetItemValueInRucksack(RucksackSlotB, ObjectIndexB, SlotB.SlotID, ContainedItem, CurrentStackActive, LowValue, NormalValue, HighValue, MaximumValue, SlotB.IsStorageSlot);
    }

    public void SetItemValueInRucksack(DG_PlayerCharacters.RucksackSlot RucksackSlot, int ObjectIndex, int Slot,
        int ContainedItem, int CurrentStackActive, int LowValue, int NormalValue, int HighValue, int MaximumValue, bool isStorage)
    {
        RucksackSlot.ContainedItem = ContainedItem;
        RucksackSlot.CurrentStackActive = CurrentStackActive;
        RucksackSlot.LowValue = LowValue;
        RucksackSlot.NormalValue = NormalValue;
        RucksackSlot.HighValue = HighValue;
        RucksackSlot.MaximumValue = MaximumValue;

        if(!isStorage)
            QuickFind.NetworkSync.SetRucksackValue(ObjectIndex, Slot, ContainedItem, CurrentStackActive, LowValue, NormalValue, HighValue, MaximumValue);
        else if(!QuickFind.StorageUI.isTreasureUI)
            QuickFind.NetworkSync.SetStorageValue(QuickFind.NetworkSync.CurrentScene, ObjectIndex, Slot, ContainedItem, CurrentStackActive, LowValue, NormalValue, HighValue, MaximumValue);
        else
            QuickFind.GUI_Inventory.UpdateInventoryVisuals();
    }








    public DG_PlayerCharacters.RucksackSlot GetRuckSackSlotInventoryItem(DG_InventoryItem II)
    {
        if (!II.IsStorageSlot)
        {
            DG_PlayerCharacters.CharacterEquipment Equipment = QuickFind.Farm.PlayerCharacters[QuickFind.NetworkSync.PlayerCharacterID].Equipment;
            return Equipment.RucksackSlots[II.SlotID];
        }
        else
            return QuickFind.StorageUI.ActiveStorage.StorageSlots[II.SlotID];
    }
    public DG_ItemObject GetItemByInventoryItem(DG_InventoryItem II)
    {
        DG_PlayerCharacters.RucksackSlot RucksackSlot = null;
        if (!II.IsStorageSlot)
        {
            DG_PlayerCharacters.CharacterEquipment Equipment = QuickFind.Farm.PlayerCharacters[QuickFind.NetworkSync.PlayerCharacterID].Equipment;
            RucksackSlot = Equipment.RucksackSlots[II.SlotID];     
        }
        else
            RucksackSlot = QuickFind.StorageUI.ActiveStorage.StorageSlots[II.SlotID];

        return QuickFind.ItemDatabase.GetItemFromID(RucksackSlot.ContainedItem);
    }








    public int TotalInventoryCountOfItem(int ItemID)
    {
        return 0;
    }






    public void DropOne(DG_InventoryItem FromItem, DG_InventoryItem ToItem)
    {
        DG_PlayerCharacters.RucksackSlot From = GetRuckSackSlotInventoryItem(FromItem);
        DG_PlayerCharacters.RucksackSlot To = GetRuckSackSlotInventoryItem(ToItem);

        int ID = QuickFind.NetworkSync.PlayerCharacterID;
        int IndexA = ID;
        int IndexB = ID;
        if (FromItem.IsStorageSlot) IndexA = QuickFind.StorageUI.ActiveStorage.transform.GetSiblingIndex();
        if (ToItem.IsStorageSlot) IndexB = QuickFind.StorageUI.ActiveStorage.transform.GetSiblingIndex();

        int ItemID = From.ContainedItem;
        if (To.ContainedItem == ItemID || To.GetStackValue() == 0)
        {
            To.ContainedItem = ItemID;

            DG_ItemObject.ItemQualityLevels QualityLevel = (DG_ItemObject.ItemQualityLevels)From.CurrentStackActive;
            if (From.GetNumberOfQuality(QualityLevel) > 0)
            {
                From.AddStackQualityValue(QualityLevel, -1);
                To.AddStackQualityValue(QualityLevel, 1);
            }

            SetItemValueInRucksack(From, IndexA, FromItem.SlotID, From.ContainedItem, From.CurrentStackActive, From.LowValue, From.NormalValue, From.HighValue, From.MaximumValue, FromItem.IsStorageSlot);
            if (!ToItem.isTrash && To != null) SetItemValueInRucksack(To, IndexB, ToItem.SlotID, To.ContainedItem, To.CurrentStackActive, To.LowValue, To.NormalValue, To.HighValue, To.MaximumValue, ToItem.IsStorageSlot);
        }
    }

    public void DestroyRucksackItem(DG_PlayerCharacters.RucksackSlot From, int SlotID)
    {
        int ID = QuickFind.NetworkSync.PlayerCharacterID;
        int IndexA = ID;
        //int ItemID = From.ContainedItem;
        DG_ItemObject.ItemQualityLevels QualityLevel = (DG_ItemObject.ItemQualityLevels)From.CurrentStackActive;
        if (From.GetNumberOfQuality(QualityLevel) > 0)
        {
            From.AddStackQualityValue(QualityLevel, -1);
            SetItemValueInRucksack(From, IndexA, SlotID, From.ContainedItem, From.CurrentStackActive, From.LowValue, From.NormalValue, From.HighValue, From.MaximumValue, false);
        }
    }


    public void DropStackOntoStack(DG_PlayerCharacters.RucksackSlot From, DG_PlayerCharacters.RucksackSlot To, DG_InventoryItem SlotA, DG_InventoryItem SlotB, int ObjectIndexA, int ObjectIndexB)
    {
        DG_ItemObject IO = QuickFind.ItemDatabase.GetItemFromID(From.ContainedItem);
        int MaxStack = IO.MaxStackSize;
        int StackValue = To.GetStackValue();
        int count;
        count = From.LowValue; for (int iN = 0; iN < count; iN++) { if (StackValue == MaxStack) break; To.LowValue++; From.LowValue--; StackValue++;}
        count = From.NormalValue; for (int iN = 0; iN < count; iN++) { if (StackValue == MaxStack) break; To.NormalValue++; From.NormalValue--; StackValue++;}
        count = From.HighValue; for (int iN = 0; iN < count; iN++) { if (StackValue == MaxStack) break; To.HighValue++; From.HighValue--; StackValue++;}
        count = From.MaximumValue; for (int iN = 0; iN < count; iN++) { if (StackValue == MaxStack) break; To.MaximumValue++; From.MaximumValue--; StackValue++;}


        SetItemValueInRucksack(From, ObjectIndexA, SlotA.SlotID, From.ContainedItem, From.CurrentStackActive, From.LowValue, From.NormalValue, From.HighValue, From.MaximumValue, SlotA.IsStorageSlot);
        if (!SlotB.isTrash) SetItemValueInRucksack(To, ObjectIndexB, SlotB.SlotID, To.ContainedItem, To.CurrentStackActive, To.LowValue, To.NormalValue, To.HighValue, To.MaximumValue, SlotB.IsStorageSlot);
    }







    public void ShiftStackToFromStorage()
    {
        ExchangeItems.Clear();

        DG_InventoryItem CurrentHoverItem = QuickFind.GUI_Inventory.CurrentHoverItem;
        DG_InventoryItem[] InventorySlots = QuickFind.GUI_Inventory.GuiItemSlots;


        DG_PlayerCharacters.RucksackSlot MoveStack = GetRuckSackSlotInventoryItem(CurrentHoverItem);
        DG_PlayerCharacters.RucksackSlot[] AlternateSide;
        bool IsStorage;
        if (CurrentHoverItem.IsStorageSlot)
        { IsStorage = true; AlternateSide = QuickFind.Farm.PlayerCharacters[QuickFind.NetworkSync.PlayerCharacterID].Equipment.RucksackSlots; }
        else
        { IsStorage = false; AlternateSide = QuickFind.StorageUI.ActiveStorage.StorageSlots; InventorySlots = QuickFind.StorageUI.StorageSlots; }

        DG_ItemObject IO = GetItemByInventoryItem(QuickFind.GUI_Inventory.CurrentHoverItem);

        int MaxStack = IO.MaxStackSize;
        int MoveStackValue = MoveStack.GetStackValue();
        int PlayerID = QuickFind.NetworkSync.PlayerCharacterID;
        int StorageID = QuickFind.StorageUI.ActiveStorage.transform.GetSiblingIndex();
        int LoopIndex = PlayerID;
        int FinalIndex = StorageID;

        if (!IsStorage) { LoopIndex = StorageID; FinalIndex = PlayerID; }

        for (int i = 0; i < AlternateSide.Length; i++)
        {
            DG_PlayerCharacters.RucksackSlot RS = AlternateSide[i];
            if (RS.ContainedItem == MoveStack.ContainedItem)
            {
                ExchangeItem EI = new ExchangeItem();
                EI.RS = RS;
                EI.index = i;

                ExchangeItems.Add(EI);
                int StackValue = RS.GetStackValue();
                int count;

                count = MoveStack.LowValue; for (int iN = 0; iN < count; iN++) { if (StackValue == MaxStack) break; RS.LowValue++; MoveStack.LowValue--; StackValue++; MoveStackValue--; }
                count = MoveStack.NormalValue; for (int iN = 0; iN < count; iN++) { if (StackValue == MaxStack) break; RS.NormalValue++; MoveStack.NormalValue--; StackValue++; MoveStackValue--; }
                count = MoveStack.HighValue; for (int iN = 0; iN < count; iN++) { if (StackValue == MaxStack) break; RS.HighValue++; MoveStack.HighValue--; StackValue++; MoveStackValue--; }
                count = MoveStack.MaximumValue; for (int iN = 0; iN < count; iN++) { if (StackValue == MaxStack) break; RS.MaximumValue++; MoveStack.MaximumValue--; StackValue++; MoveStackValue--; }

                if (MoveStackValue == 0)
                { MoveStack.ContainedItem = 0; }
            }
        }
        if (MoveStackValue > 0)
        {
            for (int i = 0; i < AlternateSide.Length; i++)
            {
                DG_PlayerCharacters.RucksackSlot RS = AlternateSide[i];
                if (RS.GetStackValue() == 0)
                {
                    ExchangeItem EI = new ExchangeItem();
                    EI.RS = RS;
                    EI.index = i;
                    ExchangeItems.Add(EI);

                    RS.ContainedItem = MoveStack.ContainedItem;
                    MoveStack.ContainedItem = 0;
                    int count;

                    count = MoveStack.LowValue; for (int iN = 0; iN < count; iN++) { RS.LowValue++; MoveStack.LowValue--; RS.CurrentStackActive = 0; }
                    count = MoveStack.NormalValue; for (int iN = 0; iN < count; iN++) { RS.NormalValue++; MoveStack.NormalValue--; RS.CurrentStackActive = 1; }
                    count = MoveStack.HighValue; for (int iN = 0; iN < count; iN++) { RS.HighValue++; MoveStack.HighValue--; RS.CurrentStackActive = 0; }
                    count = MoveStack.MaximumValue; for (int iN = 0; iN < count; iN++) { RS.MaximumValue++; MoveStack.MaximumValue--; RS.CurrentStackActive = 0; }
                    break;
                }
            }
        }


        for (int i = 0; i < ExchangeItems.Count; i++)
        {
            DG_PlayerCharacters.RucksackSlot RS = ExchangeItems[i].RS;
            SetItemValueInRucksack(RS, LoopIndex, ExchangeItems[i].index, RS.ContainedItem, RS.CurrentStackActive, RS.LowValue, RS.NormalValue, RS.HighValue, RS.MaximumValue, !IsStorage);
        }

        SetItemValueInRucksack(MoveStack, FinalIndex, CurrentHoverItem.SlotID, MoveStack.ContainedItem, MoveStack.CurrentStackActive, MoveStack.LowValue, MoveStack.NormalValue, MoveStack.HighValue, MoveStack.MaximumValue, IsStorage);
    }
}
