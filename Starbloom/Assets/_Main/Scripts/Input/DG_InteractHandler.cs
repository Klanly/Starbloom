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
                if (CO == null)
                {
                    if (QuickFind.ContextDetectionHandler.LastEncounteredContext != null)
                        QuickFind.ContextDetectionHandler.LastEncounteredContext.SendMessage("OnInteract"); return;
                }

                switch(CO.Type)
                {
                    case DG_ContextObject.ContextTypes.PickupItem: HandlePickUpItem(CO); break;
                    case DG_ContextObject.ContextTypes.Conversation: break;
                    case DG_ContextObject.ContextTypes.Treasure: break;
                    case DG_ContextObject.ContextTypes.MoveableStorage: HandleMoveableStorage(CO); break;
                    case DG_ContextObject.ContextTypes.Pick_And_Break: HandleClusterPick(CO, DG_ContextObject.ContextTypes.Pick_And_Break); break;
                    case DG_ContextObject.ContextTypes.PickOnly: HandleClusterPick(CO, DG_ContextObject.ContextTypes.PickOnly); break;
                    case DG_ContextObject.ContextTypes.ShopInterface: HandleShopInterface(CO); break;
                    case DG_ContextObject.ContextTypes.ShippingBin: QuickFind.ShippingBin.SetStackInShippingBin(CO); break;
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


        if (QuickFind.InventoryManager.AddItemToRucksack(QuickFind.NetworkSync.PlayerCharacterID, ItemID, (DG_ItemObject.ItemQualityLevels)ItemQuality, false))
        {
            QuickFind.NetworkSync.RemoveNetworkSceneObject(QuickFind.NetworkSync.CurrentScene, NO.NetworkObjectID);

            DG_ItemObject IO = QuickFind.ItemDatabase.GetItemFromID(NO.ItemRefID);
            //Pick Up Herb
            if (IO.ItemCat == DG_ItemObject.ItemCatagory.Herb)
                QuickFind.SkillTracker.IncreaseSkillLevel(DG_SkillTracker.SkillTags.Foraging, DG_ItemObject.ItemQualityLevels.Normal);
            //Pick Up Crop
            else if(IO.ItemCat != DG_ItemObject.ItemCatagory.Resource)
                QuickFind.SkillTracker.IncreaseSkillLevel(DG_SkillTracker.SkillTags.Farming, (DG_ItemObject.ItemQualityLevels)ItemQuality);
        }

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
            Growable G = NO.transform.GetChild(0).GetComponent<Growable>();
            G.Harvest();
        }
        else if(Type == DG_ContextObject.ContextTypes.PickOnly)
        {
            int ItemID;
            if (!CO.ThisIsGrowthItem)
                ItemID = NO.ItemRefID;
            else
                ItemID = CO.ContextID;

            int ItemQuality = NO.ItemQualityLevel;
            if (QuickFind.InventoryManager.AddItemToRucksack(QuickFind.NetworkSync.PlayerCharacterID, ItemID, (DG_ItemObject.ItemQualityLevels)ItemQuality, false))
            {
                QuickFind.NetworkSync.RemoveNetworkSceneObject(QuickFind.NetworkSync.CurrentScene, NO.NetworkObjectID);
                Growable G = NO.transform.GetChild(0).GetComponent<Growable>();
                G.Harvest();

                if (IO.ItemCat != DG_ItemObject.ItemCatagory.Resource)
                    QuickFind.SkillTracker.IncreaseSkillLevel(DG_SkillTracker.SkillTags.Farming, (DG_ItemObject.ItemQualityLevels)ItemQuality);
            }
        }
    }
    void HandleShopInterface(DG_ContextObject CO)
    {
        QuickFind.ShopGUI.OpenShopUI(CO.ContextID);
    }
}
