using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_BreakableObjectsHandler : MonoBehaviour {



    DG_ContextObject KnownCO;
    NetworkObject KnownNO;
    DG_ItemObject KnownIO;
    int KnownToolLevel;
    DG_PlayerCharacters.RucksackSlot KnownRucksackSlotOpen;
    bool AwaitingResponse;


    private void Awake()
    { QuickFind.BreakableObjectsHandler = this; }




    public void TryHitObject(DG_ContextObject CO, HotbarItemHandler.ActivateableTypes ActivateableType, DG_ItemObject.ItemQualityLevels ToolLevel, DG_PlayerCharacters.RucksackSlot RucksackSlotOpen, int PlayerID)
    {
        NetworkObject NO = QuickFind.NetworkObjectManager.ScanUpTree(CO.transform);
        DG_ItemObject IO = QuickFind.ItemDatabase.GetItemFromID(NO.ItemRefID);
        if (ValidBreakAction(IO, ActivateableType, ToolLevel))
        {
            KnownCO = CO;
            KnownNO = NO;
            KnownIO = IO;
            KnownToolLevel = (int)ToolLevel;
            KnownRucksackSlotOpen = RucksackSlotOpen;
            AwaitingResponse = true;

            if (QuickFind.GameSettings.DisableAnimations)
                HitAction(PlayerID);
            else
            {
                DG_CharacterLink CL = QuickFind.NetworkSync.GetCharacterLinkByPlayerID(PlayerID);

                CL.FacePlayerAtPosition(CO.transform.position);
                DG_ClothingObject Cloth = QuickFind.ClothingHairManager.GetAttachedClothingReference(CL, DG_ClothingHairManager.ClothHairType.RightHand).ClothingRef;
                CL.AnimationSync.TriggerAnimation(Cloth.AnimationDatabaseNumber);
            }
        }
        else return;
    }
    public void HitAction(int PlayerID)
    {
        if (!AwaitingResponse) return;
        AwaitingResponse = false;

        if (KnownCO.Type == DG_ContextObject.ContextTypes.HarvestableTree)
            QuickFind.InteractHandler.HandleClusterHarvest(KnownCO);
        else
        {
            DG_ItemObject ToolIO = QuickFind.ItemDatabase.GetItemFromID(KnownRucksackSlotOpen.ContainedItem);
            int Hitvalue = ToolIO.ToolQualityLevels[KnownToolLevel].StrengthValue;
            int newHealthValue = KnownNO.HealthValue - Hitvalue;

            if (newHealthValue <= 0) { SendBreak(KnownNO, KnownCO, KnownIO, PlayerID); }
            else { SendHitData(KnownCO, KnownNO, newHealthValue); }
        }
    }




    bool ValidBreakAction(DG_ItemObject IO, HotbarItemHandler.ActivateableTypes ActivateableType, DG_ItemObject.ItemQualityLevels ToolLevel)
    {
        if (ActivateableType != IO.EnvironmentValues[0].ActivateableTypeRequired) return false;
        if (ToolLevel < IO.EnvironmentValues[0].QualityLevelRequired) return false;
        return true;
    }




    void SendHitData(DG_ContextObject CO, NetworkObject NO, int NewHealthValue)
    {
        int[] Sent = new int[3];
        Sent[0] = NO.Scene.SceneID;
        Sent[1] = NO.NetworkObjectID;
        Sent[2] = NewHealthValue;

        QuickFind.NetworkSync.SendHitBreakable(Sent);
    }

    public void ReceiveHitData(int[] Data)
    {
        NetworkObject NO = QuickFind.NetworkObjectManager.GetItemByID(Data[0], Data[1]);
        NO.HealthValue = Data[2];
        DG_ItemObject IO = QuickFind.ItemDatabase.GetItemFromID(NO.ItemRefID);

        //FX
        Transform Child = NO.transform.GetChild(0);
        if (IO.EnvironmentValues[0].OnHitFXType == DG_BreakableObjectItem.OnHitEffectType.Stone) Child.GetComponent<DG_UI_WobbleAndFade>().enabled = true;
        Child.GetComponent<DG_FXContextObjectReference>().TriggerImpact();
    }




    void SendBreak(NetworkObject NO, DG_ContextObject CO, DG_ItemObject IO, int PlayerID)
    {
        if(CO.Type == DG_ContextObject.ContextTypes.BreakableTree)
            CO.GetComponent<DG_TreeFall>().BreakMessage();
        else
        {
            int[] OutData = new int[2];
            OutData[0] = NO.Scene.SceneID;
            OutData[1] = NO.NetworkObjectID;

            QuickFind.NetworkSync.PlayDestroyEffect(OutData);
            QuickFind.NetworkSync.RemoveNetworkSceneObject(NO.Scene.SceneID, NO.NetworkObjectID);
            DG_BreakableObjectItem BOI = QuickFind.BreakableObjectsCompendium.GetItemFromID(IO.HarvestClusterIndex);
            DG_BreakableObjectItem.ItemClump[] IC = BOI.GetBreakReward();

            DG_ScatterPointReference SPR = CO.GetComponent<DG_ScatterPointReference>();
            for (int i = 0; i < IC.Length; i++)
            {
                DG_BreakableObjectItem.ItemClump Clump = IC[i];
                for (int iN = 0; iN < Clump.Value; iN++)
                    QuickFind.NetworkObjectManager.CreateNetSceneObject(NO.Scene.SceneID, NetworkObjectManager.NetworkObjectTypes.Item, Clump.ItemID, Clump.ItemQuality, SPR.GetSpawnPoint(), 0, true, SPR.RandomVelocity());
            }

            //EXP
            if (IO.EnvironmentValues[0].ActivateableTypeRequired == HotbarItemHandler.ActivateableTypes.Pickaxe)
                QuickFind.SkillTracker.IncreaseSkillLevel(DG_SkillTracker.SkillTags.Mining, DG_ItemObject.ItemQualityLevels.Low, PlayerID);

            //FX
            CO.GetComponent<DG_FXContextObjectReference>().TriggerBreak();
        }
    }
}
