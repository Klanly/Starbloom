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

    [Button(ButtonSizes.Small)]
    public void GiftPlayerItem()
    {
        QuickFind.InventoryManager.AddItemToRucksack(Player, ItemID, QualityLevel, true);
    }
}
