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
        QuickFind.GUI_Inventory.AddItemToRucksack(Player, ItemID, QualityLevel);
    }

    [Button(ButtonSizes.Large)]
    public void SetItemOnGround()
    {
        QuickFind.NetworkSync.CreateNewNetworkSceneObject(ItemID, GrowthLevel, QuickFind.InputController.MainPlayer.CharLink.CharInput.transform.position, 0);
    }
}
