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
        DG_BreakableObjectItem BOI = QuickFind.BreakableObjectsCompendium.GetItemFromID(IO.EnvironmentValues[0].BreakableAtlasID);
        if (ValidBreakAction(IO, BOI, ActivateableType, ToolLevel))
        {
            DG_ItemObject ToolIO = QuickFind.ItemDatabase.GetItemFromID(RucksackSlotOpen.ContainedItem);
            int Hitvalue = ToolIO.ToolQualityLevels[(int)ToolLevel].StrengthValue;
            int newHealthValue = NO.HealthValue - Hitvalue;
            if (newHealthValue <= 0) SendBreak(NO, BOI, GetBreakReward(BOI));
            else SendHitData(NO, newHealthValue, BOI);
        }
        else return;
    }

    bool ValidBreakAction(DG_ItemObject IO, DG_BreakableObjectItem BOI, HotbarItemHandler.ActivateableTypes ActivateableType, DG_ItemObject.ItemQualityLevels ToolLevel)
    {
        if (ActivateableType != IO.EnvironmentValues[0].ActivateableTypeRequired) return false;
        if (ToolLevel < IO.EnvironmentValues[0].QualityLevelRequired) return false;
        return true;
    }

    int GetBreakReward(DG_BreakableObjectItem BOI)
    {
        float RewardRoll = Random.Range(0f, 1f);
        for (int i = 0; i < BOI.RewardRolls.Length; i++)
            if (RewardRoll < BOI.RewardRolls[i].RollPercent) return i;
        return 0;
    }











    void SendHitData(NetworkObject NO, int NewHealthValue, DG_BreakableObjectItem BOI)
    {
        int[] Sent = new int[3];
        Sent[0] = QuickFind.NetworkSync.CurrentScene;
        Sent[1] = NO.transform.GetSiblingIndex();
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


    void SendBreak(NetworkObject NO, DG_BreakableObjectItem BOI, int RewardNum)
    {
        QuickFind.NetworkSync.RemoveNetworkSceneObject(QuickFind.NetworkSync.CurrentScene, NO.transform.GetSiblingIndex());
        int SceneID = QuickFind.NetworkSync.CurrentScene;
        DG_BreakableObjectItem.ItemClump[] IC = BOI.RewardRolls[RewardNum].ItemGifts;
        Vector3 SpawnPoint = NO.transform.position;

        bool Randomize = false;
        Vector3 Vec = Vector3.zero;

        for (int i = 0; i < IC.Length; i++)
        {
            DG_BreakableObjectItem.ItemClump IO = IC[i];
            DG_ItemObject O = QuickFind.ItemDatabase.GetItemFromID(IO.ItemID);
            for (int iN = 0; iN < IO.Value; iN++)
            {
                Randomize = false;
                if (O.RandomizeVelocityOnSpawn) Randomize = true;
                if(!Randomize) Vec = Vector3.zero;
                else
                {
                    Vec.x = Random.Range(-O.RandomVelocityMax, O.RandomVelocityMax);
                    Vec.y = Random.Range(0, O.RandomVelocityMax);
                    Vec.z = Random.Range(-O.RandomVelocityMax, O.RandomVelocityMax);
                }

                QuickFind.NetworkObjectManager.CreateNetSceneObject(SceneID, IO.ItemID, IO.ItemQuality, SpawnPoint, 0, Randomize, Vec);
            }
        }
    }
}
