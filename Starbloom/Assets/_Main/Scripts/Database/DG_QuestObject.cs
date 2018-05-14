using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
/////////////////////////////////////////////////////////////////////////////////Editor Extension Buttons
[CustomEditor(typeof(DG_QuestObject))]
class DG_QuestObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        //Buttons

        DG_QuestObject myScript = (DG_QuestObject)target;
        if (GUILayout.Button("FindNextAvailableDatabaseID"))
            myScript.FindNextAvailableDatabaseID();
        if (GUILayout.Button("GenerateNewSaveLocation"))
        {
            if (myScript.DatabaseID == 0)
                myScript.FindNextAvailableDatabaseID();
            string QuestName = "Quest - " + myScript.DatabaseID.ToString();
            DG_DataBoolManager Manager = QuickFindInEditor.GetEditorDataBools();
            int BoolID = Manager.GenerateNewDatabaseItem("Quest", QuestName);
            myScript.BoolSaveLocation = BoolID;
            DG_DataBoolItem BoolItem = Manager.GetBoolFromIDInEditor(BoolID);
            BoolItem.Description = QuestName;
        }
    }
}
//////////////////////////////////////////////////////////////////////////////////
#endif




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

    public int DatabaseID;
    public int BoolSaveLocation;
    public string DevNotes;


    public Requirements[] QuestRequirements;
    public Rewards[] QuestRewards;

    

    public bool QuestIsComplete()
    {
        return QuickFind.DataBools.GetBoolFromID(BoolSaveLocation).BoolValue;
    }

    public bool QuestRequirementsAreMet()
    {
        for(int i = 0; i < QuestRequirements.Length; i++)
        {
            Requirements Req = QuestRequirements[i];
            if(Req.RequireQuestValue)
            {
                int boolDatabaseLocation = QuickFind.QuestDatabase.GetItemFromID(Req.QuestDatabaseID).BoolSaveLocation;
                bool QuestComplete = QuickFind.DataBools.GetBoolFromID(boolDatabaseLocation).BoolValue;

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
                        if (QuickFind.PlayerInventory.TotalInventoryCountOfItem(Req.ReqItemDatabaseID) >= Req.ReqItemValue)
                        {
                            Debug.Log("Quest Not Met");
                            return false;
                        }
                        break;
                    //Greater Than
                    case Requirements.MathConditions.GreaterOrEqual:
                        if (QuickFind.PlayerInventory.TotalInventoryCountOfItem(Req.ReqItemDatabaseID) < Req.ReqItemValue)
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
        QuickFind.DataBools.GetBoolFromID(BoolSaveLocation).BoolValue = true;
        //Quest Rewards
        for(int i = 0; i < QuestRewards.Length; i++)
        {
            Rewards Rew = QuestRewards[i];
            if(Rew.RewardItem) //Quest Rewards an Item
            {
                DG_ItemObject Item = QuickFind.ItemDatabase.GetItemFromID(Rew.RewardItemDatabaseID);
                QuickFind.PlayerInventory.ChangeItemInventorySlot(Item.DatabaseID, Rew.RewardItemValue, true);
            }
        }
        Debug.Log("Quest " + BoolSaveLocation.ToString() + " Completed: Rewards Adjusted");
    }


    public void FindNextAvailableDatabaseID()
    {
        Transform Cat = transform.parent;
        Transform Tracker = Cat.parent;

        int HighestNumber = 0;

        for (int i = 0; i < Tracker.childCount; i++)
        {
            Transform Child = Tracker.GetChild(i);
            for (int iN = 0; iN < Child.childCount; iN++)
            {
                DG_QuestObject Item = Child.GetChild(iN).GetComponent<DG_QuestObject>();
                if (Item.DatabaseID != 0)
                {
                    Debug.Log("This Object Already Has a Database ID");
                    return;
                }
                if (Item.DatabaseID > HighestNumber)
                    HighestNumber = Item.DatabaseID;
            }
        }
        DatabaseID = HighestNumber + 1;
        transform.gameObject.name = DatabaseID.ToString() + " - ";
    }
}
