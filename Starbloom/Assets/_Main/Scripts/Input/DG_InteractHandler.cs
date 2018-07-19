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
        for (int i = 0; i < QuickFind.InputController.Players.Length; i++)
        {
            DG_PlayerInput.Player P = QuickFind.InputController.Players[i];
            if (P.CurrentInputState != DG_PlayerInput.CurrentInputState.Default) continue;

            bool AllowHold = false;
            if (QuickFind.GameSettings.AllowActionsOnHold) { if (P.ButtonSet.SecondaryAction.Held) AllowHold = true; }

            if (P.ButtonSet.SecondaryAction.Up || AllowHold)
            {
                if (!P.CharLink.AnimationSync.CharacterIsGrounded()) return;

                if (QuickFind.ContextDetectionHandler.ContextHit)
                {
                    DG_ContextObject CO = QuickFind.ContextDetectionHandler.COEncountered;
                    if (CO == null)
                    {
                        if (QuickFind.ContextDetectionHandler.LastEncounteredContext != null)
                            QuickFind.ContextDetectionHandler.LastEncounteredContext.SendMessage("OnInteract"); return;
                    }

                    switch (CO.Type)
                    {
                        case DG_ContextObject.ContextTypes.PickupItem: TriggerAnimation(CO, P.CharLink.PlayerID); break;
                        case DG_ContextObject.ContextTypes.Conversation: break;
                        case DG_ContextObject.ContextTypes.Treasure: break;
                        case DG_ContextObject.ContextTypes.MoveableStorage: HandleMoveableStorage(CO); break;
                        case DG_ContextObject.ContextTypes.HarvestablePlant: TriggerAnimation(CO, P.CharLink.PlayerID); break;
                        case DG_ContextObject.ContextTypes.HarvestableTree: TriggerAnimation(CO, P.CharLink.PlayerID); break;
                        case DG_ContextObject.ContextTypes.ShopInterface: HandleShopInterface(CO); break;
                        case DG_ContextObject.ContextTypes.ShippingBin: QuickFind.ShippingBinGUI.OpenBinUI(CO); break;
                        case DG_ContextObject.ContextTypes.ScenePortal: CO.GetComponent<DG_ScenePortalTrigger>().TriggerSceneChange(P.CharLink.PlayerID); break;
                    }
                }
            }
        }
    }


    public void TriggerAnimation(DG_ContextObject CO, int PlayerID)
    {
        AwaitingResponse = true;
        SavedCO = CO;
        SavedNO = QuickFind.NetworkObjectManager.ScanUpTree(CO.transform);
        SavedIO = QuickFind.ItemDatabase.GetItemFromID(SavedNO.ItemRefID);

        if (QuickFind.GameSettings.DisableAnimations)
            ReturnInteractionHit(PlayerID);
        else
        {
            QuickFind.NetworkSync.GetUserByPlayerID(PlayerID).CharacterLink.FacePlayerAtPosition(CO.transform.position);
            QuickFind.NetworkSync.GetUserByPlayerID(PlayerID).CharacterLink.AnimationSync.TriggerAnimation(SavedIO.AnimationInteractID);
        }
    }



    public void ReturnInteractionHit(int PlayerId)
    {
        if (!AwaitingResponse) return; AwaitingResponse = false;

        switch (SavedCO.Type)
        {
            case DG_ContextObject.ContextTypes.PickupItem: HandlePickUpItem(SavedCO, PlayerId); break;
            case DG_ContextObject.ContextTypes.HarvestablePlant: HandleSingleHarvest(SavedCO, PlayerId); break;
            case DG_ContextObject.ContextTypes.HarvestableTree: HandleClusterHarvest(SavedCO); break;
        }
    }



    void HandlePickUpItem(DG_ContextObject CO, int PlayerID)
    {
        NetworkObject NO = QuickFind.NetworkObjectManager.ScanUpTree(CO.transform);
        DG_ItemObject IO = QuickFind.ItemDatabase.GetItemFromID(NO.ItemRefID);
        int ItemID = IO.HarvestItemIndex;

        int Scene = NO.transform.parent.GetComponent<NetworkScene>().SceneID;
        int ItemQuality = NO.ItemQualityLevel;


        if (QuickFind.InventoryManager.AddItemToRucksack(PlayerID, ItemID, (DG_ItemObject.ItemQualityLevels)ItemQuality, false))
        {
            QuickFind.NetworkSync.RemoveNetworkSceneObject(NO.Scene.SceneID, NO.NetworkObjectID);         
            //Pick Up Herb
            if (IO.ItemCat == DG_ItemObject.ItemCatagory.Herb)
                QuickFind.SkillTracker.IncreaseSkillLevel(DG_SkillTracker.SkillTags.Foraging, DG_ItemObject.ItemQualityLevels.Normal, PlayerID);
            //Pick Up Crop
            else if(IO.ItemCat != DG_ItemObject.ItemCatagory.Resource)
                QuickFind.SkillTracker.IncreaseSkillLevel(DG_SkillTracker.SkillTags.Farming, (DG_ItemObject.ItemQualityLevels)ItemQuality, PlayerID);
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

        DG_BreakableObjectItem BOI = QuickFind.BreakableObjectsCompendium.GetItemFromID(IO.HarvestClusterIndex);
        DG_BreakableObjectItem.ItemClump[] IC = BOI.GetBreakReward();

        DG_ScatterPointReference SPR = CO.GetComponent<DG_ScatterPointReference>();
        

        for (int i = 0; i < IC.Length; i++)
        {
            DG_BreakableObjectItem.ItemClump Clump = IC[i];
            for (int iN = 0; iN < Clump.Value; iN++)
                QuickFind.NetworkObjectManager.CreateNetSceneObject(NO.Scene.SceneID, NetworkObjectManager.NetworkObjectTypes.Item, Clump.ItemID, Clump.ItemQuality, SPR.GetSpawnPoint(), 0, true, SPR.RandomVelocity());
        }

        //FX
        int[] OutData = new int[2];
        OutData[0] = NO.Scene.SceneID;
        OutData[1] = NO.NetworkObjectID;
        QuickFind.NetworkSync.PlayClusterPickEffect(OutData);

        QuickFind.NetworkGrowthHandler.Harvest(NO, IO, NO.Scene.SceneID);
    }
    public void HandleSingleHarvest(DG_ContextObject CO, int PlayerID)
    {
        NetworkObject NO = QuickFind.NetworkObjectManager.ScanUpTree(CO.transform);
        DG_ItemObject IO = QuickFind.ItemDatabase.GetItemFromID(NO.ItemRefID);
        int SceneID = NO.Scene.SceneID;
        int ItemID = IO.HarvestItemIndex;

        int ItemQuality = NO.ItemQualityLevel;
        if (QuickFind.InventoryManager.AddItemToRucksack(PlayerID, ItemID, (DG_ItemObject.ItemQualityLevels)ItemQuality, false))
        {
            QuickFind.NetworkGrowthHandler.Harvest(NO, IO, SceneID);

            if (IO.ItemCat != DG_ItemObject.ItemCatagory.Resource)
                QuickFind.SkillTracker.IncreaseSkillLevel(DG_SkillTracker.SkillTags.Farming, (DG_ItemObject.ItemQualityLevels)ItemQuality, PlayerID);
        }
    }
    void HandleShopInterface(DG_ContextObject CO)
    {
        DG_ShopInteractionPoint SIP = CO.GetComponent<DG_ShopInteractionPoint>();
        QuickFind.ShopGUI.OpenShopUI(SIP.ShopID);
    }
}
