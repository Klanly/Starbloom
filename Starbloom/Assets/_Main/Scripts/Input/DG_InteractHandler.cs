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
                    case DG_ContextObject.ContextTypes.Treasure: break;
                    case DG_ContextObject.ContextTypes.MoveableStorage: HandleMoveableStorage(CO); break;
                    case DG_ContextObject.ContextTypes.Pick_And_Break: HandleClusterPick(CO, DG_ContextObject.ContextTypes.Pick_And_Break); break;
                    case DG_ContextObject.ContextTypes.PickOnly: HandleClusterPick(CO, DG_ContextObject.ContextTypes.PickOnly); break;
                }
            }
        }
    }

    void HandlePickUpItem(DG_ContextObject CO)
    {
        NetworkObject NO = QuickFind.NetworkObjectManager.ScanUpTree(CO.transform);
        int ItemID;
        if (!CO.ThisIsGrowthItem)
            ItemID = NO.ItemRefID;
        else
            ItemID = CO.ContextID;

        int Scene = NO.transform.parent.GetComponent<NetworkScene>().SceneID;
        int ItemQuality = NO.ItemQualityLevel;
            
        

        if (QuickFind.InventoryManager.AddItemToRucksack(QuickFind.NetworkSync.PlayerCharacterID, ItemID, (DG_ItemObject.ItemQualityLevels)ItemQuality))
            Debug.Log("Send Inventory Full Message");
        else
            QuickFind.NetworkSync.RemoveNetworkSceneObject(Scene, NO.NetworkObjectID);
    }
    void HandleMoveableStorage(DG_ContextObject CO)
    {
        NetworkObject NO = QuickFind.NetworkObjectManager.ScanUpTree(CO.transform);
        QuickFind.StorageUI.OpenStorageUI(NO, false);
    }

    void HandleClusterPick(DG_ContextObject CO, DG_ContextObject.ContextTypes Type)
    {
        NetworkObject NO = QuickFind.NetworkObjectManager.ScanUpTree(CO.transform);
        DG_ItemObject IO = QuickFind.ItemDatabase.GetItemFromID(NO.ItemRefID);


        Growable G = NO.transform.GetChild(0).GetComponent<Growable>();
        G.Harvest();

        if (Type == DG_ContextObject.ContextTypes.Pick_And_Break)
        {
            DG_ItemObject.Environment E = IO.EnvironmentValues[0];
            if (E.DropItemsOnInteract)
            {
                int SceneID = QuickFind.NetworkSync.CurrentScene;
                DG_BreakableObjectItem BOI = QuickFind.BreakableObjectsCompendium.GetItemFromID(E.InteractDropID);
                DG_BreakableObjectItem.ItemClump[] IC = BOI.GetBreakReward();

                for (int i = 0; i < IC.Length; i++)
                {
                    DG_BreakableObjectItem.ItemClump Clump = IC[i];
                    for (int iN = 0; iN < Clump.Value; iN++)
                        QuickFind.NetworkObjectManager.CreateNetSceneObject(SceneID, Clump.ItemID, Clump.ItemQuality, CO.GetSpawnPoint(), 0, true, CO.RandomVelocity());
                }
            }
        }
        else if(Type == DG_ContextObject.ContextTypes.PickOnly)
        {
            int ItemID;
            if (!CO.ThisIsGrowthItem)
                ItemID = NO.ItemRefID;
            else
                ItemID = CO.ContextID;

            int ItemQuality = NO.ItemQualityLevel;
            QuickFind.InventoryManager.AddItemToRucksack(QuickFind.NetworkSync.PlayerCharacterID, ItemID, (DG_ItemObject.ItemQualityLevels)ItemQuality);
        }
    }
}
