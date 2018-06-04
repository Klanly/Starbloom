using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class DG_QuestObject : MonoBehaviour {

    [System.Serializable]
    public class Requirements
    {
        public enum MathConditions
        {
            LessThan,
            GreaterOrEqual
        }

        [Header("Quest Required")]
        public bool RequireQuestValue;
        public int QuestDatabaseID;
        public bool QuestShouldBeComplete;
        //
        [Header("Item Value Required")]
        public bool RequireItem;
        public int ReqItemDatabaseID;
        public int ReqItemValue;
        public MathConditions MathCond;
    }
    [System.Serializable]
    public class Rewards
    {
        [Header("Item Value Adjusted")]
        public bool RewardItem;
        public int RewardItemDatabaseID;
        public int RewardItemValue;
    }

    [HideInInspector] public int DatabaseID;
    [HideInInspector] public bool LockItem;
    public string ObjectName;

    public int BoolSaveLocation;


    public Requirements[] QuestRequirements;
    public Rewards[] QuestRewards;

    

    public bool QuestIsComplete()
    {
        Debug.Log("This Has been removed, to be fixed");
        return false;
    }

    public bool QuestRequirementsAreMet()
    {
        for(int i = 0; i < QuestRequirements.Length; i++)
        {
            Requirements Req = QuestRequirements[i];
            if(Req.RequireQuestValue)
            {
                int boolDatabaseLocation = QuickFind.QuestDatabase.GetItemFromID(Req.QuestDatabaseID).BoolSaveLocation;
                bool QuestComplete = false;

                if (Req.QuestShouldBeComplete && !QuestComplete)
                {
                    Debug.Log("Quest Not Met");
                    return false;
                }
                else if (!Req.QuestShouldBeComplete && QuestComplete)
                {
                    Debug.Log("Quest Not Met");
                    return false;
                }
            }
            if(Req.RequireItem)
            {
                switch (Req.MathCond)
                {   //Less Than
                    case Requirements.MathConditions.LessThan:
                        if (QuickFind.InventoryManager.TotalInventoryCountOfItem(Req.ReqItemDatabaseID) >= Req.ReqItemValue)
                        {
                            Debug.Log("Quest Not Met");
                            return false;
                        }
                        break;
                    //Greater Than
                    case Requirements.MathConditions.GreaterOrEqual:
                        if (QuickFind.InventoryManager.TotalInventoryCountOfItem(Req.ReqItemDatabaseID) < Req.ReqItemValue)
                        {
                            Debug.Log("Quest Not Met");
                            return false;
                        }
                        break;
                }
            }
        }

        return true;
    }
    public void CompleteQuest()
    {
        //Quest Rewards
        for(int i = 0; i < QuestRewards.Length; i++)
        {
            Rewards Rew = QuestRewards[i];
            if (Rew.RewardItem) //Quest Rewards an Item
            {
                Debug.Log("Quality Level Not set dynamically");
                DG_ItemObject.ItemQualityLevels RewardLevel = DG_ItemObject.ItemQualityLevels.Low;

                QuickFind.InventoryManager.AddItemToRucksack(QuickFind.NetworkSync.PlayerCharacterID, Rew.RewardItemDatabaseID, RewardLevel, true);
            }
        }
        Debug.Log("Quest " + BoolSaveLocation.ToString() + " Completed: Rewards Adjusted");
    }
}
