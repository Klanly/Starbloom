using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_InteractHandler : MonoBehaviour
{

    private void Awake()
    {
        QuickFind.InteractHandler = this;
    }



    private void Update()
    {


        if (QuickFind.InputController.MainPlayer.ButtonSet.Interact.Up)
        {
            if (QuickFind.ContextDetectionHandler.ContextHit)
            {
                DG_ContextObject CO = QuickFind.ContextDetectionHandler.COEncountered;
                switch(CO.Type)
                {
                    case DG_ContextObject.ContextTypes.PickupItem: HandlePickUpItem(CO); break;
                    case DG_ContextObject.ContextTypes.Conversation: break;
                    case DG_ContextObject.ContextTypes.Growable: break;
                    case DG_ContextObject.ContextTypes.Treasure: break;
                }
            }
        }
    }

    void HandlePickUpItem(DG_ContextObject CO)
    {
        NetworkObject NO = CO.transform.parent.GetComponent<NetworkObject>();
        int Scene = NO.transform.parent.GetComponent<NetworkScene>().SceneID;

        QuickFind.NetworkSync.RemoveNetworkSceneObject(Scene, NO.transform.GetSiblingIndex());

        int ItemID = NO.ItemRefID;
        int ItemQuality = NO.ItemGrowthLevel;
        QuickFind.GUI_Inventory.AddItemToRucksack(QuickFind.NetworkSync.PlayerCharacterID, ItemID, (DG_ItemObject.ItemQualityLevels)ItemQuality);
    }
}
