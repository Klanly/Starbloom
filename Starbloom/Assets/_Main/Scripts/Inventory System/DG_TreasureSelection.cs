using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class DG_TreasureSelection : MonoBehaviour {

    public NetworkObject TreasureObject;

    DG_PlayerCharacters.RucksackSlot FirstAvailableRucksackSlot;


    private void Awake()
    {
        QuickFind.TreasureManager = this;
    }




    public void OpenTrashUI(int ItemID, DG_ItemObject.ItemQualityLevels QualityLevel)
    {
        ClearTreasureChest();
        AddItemToTreasure(ItemID, QualityLevel);
        QuickFind.StorageUI.OpenStorageUI(TreasureObject, true);
    }


    public void ClearTreasureChest()
    {
        for(int i = 0; i < TreasureObject.StorageSlots.Length; i++)
        {
            DG_PlayerCharacters.RucksackSlot RS = TreasureObject.StorageSlots[i];
            RS.ContainedItem = 0;
            RS.CurrentStackActive = 0;
            RS.LowValue = 0;
            RS.NormalValue = 0;
            RS.HighValue = 0;
            RS.MaximumValue = 0;
        }
    }


    public void AddItemToTreasure(int ItemID, DG_ItemObject.ItemQualityLevels QualityLevel)
    {
        DG_ItemObject Object = QuickFind.ItemDatabase.GetItemFromID(ItemID);
        DG_PlayerCharacters.RucksackSlot RucksackSlot = TreasureSlotContainingID(Object);

        if (RucksackSlot == null) //Item Does Not Exist in Bag Yet.
        {
            FirstAvailableRucksackSlot.ContainedItem = Object.DatabaseID;
            FirstAvailableRucksackSlot.AddStackQualityValue(QualityLevel, 1);
            RucksackSlot = FirstAvailableRucksackSlot;
            RucksackSlot.CurrentStackActive = (int)QualityLevel;
        }
        else //Add Item to Rucksack Slot
            RucksackSlot.AddStackQualityValue(QualityLevel, 1);
    }
    DG_PlayerCharacters.RucksackSlot TreasureSlotContainingID(DG_ItemObject Object)
    {
        FirstAvailableRucksackSlot = null;
        int ItemAddSlotPosition;
        int ObjectID = Object.DatabaseID;
        for (int i = 0; i < TreasureObject.StorageSlots.Length; i++)
        {
            DG_PlayerCharacters.RucksackSlot RS = TreasureObject.StorageSlots[i];
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

}
