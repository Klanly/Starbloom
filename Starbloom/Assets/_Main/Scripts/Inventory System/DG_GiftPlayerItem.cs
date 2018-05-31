using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class DG_GiftPlayerItem : MonoBehaviour {

    public int Player;
    public int ItemID;
    public DG_ItemObject.ItemQualityLevels QualityLevel;
    [Header("Place Object")]
    public int GrowthLevel;

    [Button(ButtonSizes.Large)]
    public void GiftPlayerItem()
    {
        if(QuickFind.InventoryManager.AddItemToRucksack(Player, ItemID, QualityLevel)) QuickFind.TreasureManager.OpenTrashUI(ItemID, QualityLevel);
    }
}
