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

        if (QuickFind.InputController.InputState != DG_PlayerInput.CurrentInputState.Default) return;


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
                    case DG_ContextObject.ContextTypes.MoveableStorage: HandleMoveableStorage(CO); break;
                }
            }
        }
    }

    void HandlePickUpItem(DG_ContextObject CO)
    {
        NetworkObject NO = CO.transform.parent.GetComponent<NetworkObject>();
        int Scene = NO.transform.parent.GetComponent<NetworkScene>().SceneID;
        int ItemID = NO.ItemRefID;
        int ItemQuality = NO.ItemGrowthLevel;

        if (QuickFind.InventoryManager.AddItemToRucksack(QuickFind.NetworkSync.PlayerCharacterID, ItemID, (DG_ItemObject.ItemQualityLevels)ItemQuality))
            Debug.Log("Send Inventory Full Message");
        else
            QuickFind.NetworkSync.RemoveNetworkSceneObject(Scene, NO.transform.GetSiblingIndex());
    }
    void HandleMoveableStorage(DG_ContextObject CO)
    {
        NetworkObject NO = CO.transform.parent.GetComponent<NetworkObject>();
        QuickFind.StorageUI.OpenStorageUI(NO, false);
    }
}
