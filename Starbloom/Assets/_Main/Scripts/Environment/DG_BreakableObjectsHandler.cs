using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_BreakableObjectsHandler : MonoBehaviour {






    private void Awake()
    { QuickFind.BreakableObjectsHandler = this; }




    public void TryHitObject(DG_ContextObject CO, HotbarItemHandler.ActivateableTypes ActivateableType, DG_ItemObject.ItemQualityLevels ToolLevel, DG_PlayerCharacters.RucksackSlot RucksackSlotOpen)
    {
        NetworkObject NO = QuickFind.NetworkObjectManager.ScanUpTree(CO.transform);
        DG_ItemObject IO = QuickFind.ItemDatabase.GetItemFromID(NO.ItemRefID);
        if (ValidBreakAction(IO, ActivateableType, ToolLevel))
        {
            DG_ItemObject ToolIO = QuickFind.ItemDatabase.GetItemFromID(RucksackSlotOpen.ContainedItem);
            int Hitvalue = ToolIO.ToolQualityLevels[(int)ToolLevel].StrengthValue;
            int newHealthValue = NO.HealthValue - Hitvalue;

            if (newHealthValue <= 0) SendBreak(NO, CO, IO);
            else SendHitData(NO, newHealthValue);
        }
        else return;
    }




    bool ValidBreakAction(DG_ItemObject IO, HotbarItemHandler.ActivateableTypes ActivateableType, DG_ItemObject.ItemQualityLevels ToolLevel)
    {
        if (ActivateableType != IO.EnvironmentValues[0].ActivateableTypeRequired) return false;
        if (ToolLevel < IO.EnvironmentValues[0].QualityLevelRequired) return false;
        return true;
    }




    void SendHitData(NetworkObject NO, int NewHealthValue)
    {
        int[] Sent = new int[3];
        Sent[0] = QuickFind.NetworkSync.CurrentScene;
        Sent[1] = NO.NetworkObjectID;
        Sent[2] = NewHealthValue;

        QuickFind.NetworkSync.SendHitBreakable(Sent);
    }

    public void ReceiveHitData(int[] Data)
    {
        NetworkObject NO = QuickFind.NetworkObjectManager.GetItemByID(Data[0], Data[1]);
        NO.HealthValue = Data[2];
        DG_ItemObject IO = QuickFind.ItemDatabase.GetItemFromID(NO.ItemRefID);

        if(IO.EnvironmentValues[0].ObjectType == DG_BreakableObjectItem.OnHitEffectType.Stone)
            NO.transform.GetChild(0).GetComponent<DG_UI_WobbleAndFade>().enabled = true;
    }




    void SendBreak(NetworkObject NO, DG_ContextObject CO, DG_ItemObject IO)
    {
        QuickFind.NetworkSync.RemoveNetworkSceneObject(QuickFind.NetworkSync.CurrentScene, NO.NetworkObjectID);
        int SceneID = QuickFind.NetworkSync.CurrentScene;
        DG_ItemObject.Environment E = IO.EnvironmentValues[0];

        if (E.DropItemsOnBreak)
        {
            DG_BreakableObjectItem BOI = QuickFind.BreakableObjectsCompendium.GetItemFromID(E.BreakableAtlasID);
            DG_BreakableObjectItem.ItemClump[] IC = BOI.GetBreakReward();

            for (int i = 0; i < IC.Length; i++)
            {
                DG_BreakableObjectItem.ItemClump Clump = IC[i];
                for (int iN = 0; iN < Clump.Value; iN++)
                    QuickFind.NetworkObjectManager.CreateNetSceneObject(SceneID, Clump.ItemID, Clump.ItemQuality, CO.GetSpawnPoint(), 0,true, CO.RandomVelocity());
            }
        }
        else if(E.SwapItemOnBreak)
            QuickFind.NetworkObjectManager.CreateNetSceneObject(SceneID, E.SwapID, 0, NO.transform.position, NO.transform.eulerAngles.y);

        if (E.ObjectType == DG_BreakableObjectItem.OnHitEffectType.Stone)
            QuickFind.SkillTracker.IncreaseSkillLevel(DG_SkillTracker.SkillTags.Mining, DG_ItemObject.ItemQualityLevels.Low);
    }
}
