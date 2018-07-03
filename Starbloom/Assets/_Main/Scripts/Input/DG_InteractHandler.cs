using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_InteractHandler : MonoBehaviour
{






    DG_ContextObject SavedCO;
    NetworkObject SavedNO;
    DG_ItemObject SavedIO;
    bool AwaitingResponse;


    private void Awake()
    {
        QuickFind.InteractHandler = this;
    }



    private void Update()
    {

        if (QuickFind.InputController.InputState != DG_PlayerInput.CurrentInputState.Default) return;

        if (QuickFind.InputController.MainPlayer.ButtonSet.Interact.Up)
        {
            if (!QuickFind.NetworkSync.CharacterLink.AnimationSync.CharacterIsGrounded()) return;

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
                    case DG_ContextObject.ContextTypes.PickupItem: TriggerAnimation(CO); break;
                    case DG_ContextObject.ContextTypes.Conversation: break;
                    case DG_ContextObject.ContextTypes.Treasure: break;
                    case DG_ContextObject.ContextTypes.MoveableStorage: HandleMoveableStorage(CO); break;
                    case DG_ContextObject.ContextTypes.HarvestablePlant: TriggerAnimation(CO); break;
                    case DG_ContextObject.ContextTypes.HarvestableTree: TriggerAnimation(CO); break;
                    case DG_ContextObject.ContextTypes.ShopInterface: HandleShopInterface(CO); break;
                    case DG_ContextObject.ContextTypes.ShippingBin: QuickFind.ShippingBinGUI.OpenBinUI(CO); break;
                    case DG_ContextObject.ContextTypes.ScenePortal: CO.GetComponent<DG_ScenePortalTrigger>().TriggerSceneChange(); break;
                }
            }
        }
    }


    public void TriggerAnimation(DG_ContextObject CO)
    {
        AwaitingResponse = true;
        SavedCO = CO;
        SavedNO = QuickFind.NetworkObjectManager.ScanUpTree(CO.transform);
        SavedIO = QuickFind.ItemDatabase.GetItemFromID(SavedNO.ItemRefID);
        QuickFind.NetworkSync.CharacterLink.FacePlayerAtPosition(CO.transform.position);
        QuickFind.NetworkSync.CharacterLink.AnimationSync.TriggerAnimation(SavedIO.AnimationInteractID);
    }



    public void ReturnInteractionHit()
    {
        if (!AwaitingResponse) return; AwaitingResponse = false;

        switch (SavedCO.Type)
        {
            case DG_ContextObject.ContextTypes.PickupItem: HandlePickUpItem(SavedCO); break;
            case DG_ContextObject.ContextTypes.HarvestablePlant: HandleSingleHarvest(SavedCO); break;
            case DG_ContextObject.ContextTypes.HarvestableTree: HandleClusterHarvest(SavedCO); break;
        }
    }



    void HandlePickUpItem(DG_ContextObject CO)
    {
        NetworkObject NO = QuickFind.NetworkObjectManager.ScanUpTree(CO.transform);
        DG_ItemObject IO = QuickFind.ItemDatabase.GetItemFromID(NO.ItemRefID);
        int ItemID = IO.HarvestItemIndex;

        int Scene = NO.transform.parent.GetComponent<NetworkScene>().SceneID;
        int ItemQuality = NO.ItemQualityLevel;


        if (QuickFind.InventoryManager.AddItemToRucksack(QuickFind.NetworkSync.PlayerCharacterID, ItemID, (DG_ItemObject.ItemQualityLevels)ItemQuality, false))
        {
            QuickFind.NetworkSync.RemoveNetworkSceneObject(QuickFind.NetworkSync.CurrentScene, NO.NetworkObjectID);         
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

    public void HandleClusterHarvest(DG_ContextObject CO)
    {
        NetworkObject NO = QuickFind.NetworkObjectManager.ScanUpTree(CO.transform);
        DG_ItemObject IO = QuickFind.ItemDatabase.GetItemFromID(NO.ItemRefID);
        int SceneID = QuickFind.NetworkSync.CurrentScene;

        DG_BreakableObjectItem BOI = QuickFind.BreakableObjectsCompendium.GetItemFromID(IO.HarvestClusterIndex);
        DG_BreakableObjectItem.ItemClump[] IC = BOI.GetBreakReward();

        DG_ScatterPointReference SPR = CO.GetComponent<DG_ScatterPointReference>();
        

        for (int i = 0; i < IC.Length; i++)
        {
            DG_BreakableObjectItem.ItemClump Clump = IC[i];
            for (int iN = 0; iN < Clump.Value; iN++)
                QuickFind.NetworkObjectManager.CreateNetSceneObject(SceneID, NetworkObjectManager.NetworkObjectTypes.Item, Clump.ItemID, Clump.ItemQuality, SPR.GetSpawnPoint(), 0, true, SPR.RandomVelocity());
        }

        //FX
        int[] OutData = new int[2];
        NetworkScene NS = NO.transform.parent.GetComponent<NetworkScene>();
        OutData[0] = NS.SceneID;
        OutData[1] = NO.NetworkObjectID;
        QuickFind.NetworkSync.PlayClusterPickEffect(OutData);

        QuickFind.NetworkGrowthHandler.Harvest(NO, IO, SceneID);
    }
    public void HandleSingleHarvest(DG_ContextObject CO)
    {
        NetworkObject NO = QuickFind.NetworkObjectManager.ScanUpTree(CO.transform);
        DG_ItemObject IO = QuickFind.ItemDatabase.GetItemFromID(NO.ItemRefID);
        int SceneID = QuickFind.NetworkSync.CurrentScene;
        int ItemID = IO.HarvestItemIndex;

        int ItemQuality = NO.ItemQualityLevel;
        if (QuickFind.InventoryManager.AddItemToRucksack(QuickFind.NetworkSync.PlayerCharacterID, ItemID, (DG_ItemObject.ItemQualityLevels)ItemQuality, false))
        {
            QuickFind.NetworkGrowthHandler.Harvest(NO, IO, SceneID);

            if (IO.ItemCat != DG_ItemObject.ItemCatagory.Resource)
                QuickFind.SkillTracker.IncreaseSkillLevel(DG_SkillTracker.SkillTags.Farming, (DG_ItemObject.ItemQualityLevels)ItemQuality);
        }
    }
    void HandleShopInterface(DG_ContextObject CO)
    {
        DG_ShopInteractionPoint SIP = CO.GetComponent<DG_ShopInteractionPoint>();
        QuickFind.ShopGUI.OpenShopUI(SIP.ShopID);
    }
}
