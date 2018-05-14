using System.Collections;
using System.Collections.Generic;
using UnityEngine;



#if UNITY_EDITOR
using UnityEditor;
/////////////////////////////////////////////////////////////////////////////////Editor Extension Buttons
[CustomEditor(typeof(DG_PlayerInventory))]
class DG_PlayerInventoryEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        //Buttons

        DG_PlayerInventory myScript = (DG_PlayerInventory)target;
        if (GUILayout.Button("GenerateDatabaseIDs"))
        {
            DG_DataIntManager IntManager = QuickFindInEditor.GetEditorDataInts();

            for (int i = 0; i < myScript.InventorySlots.Length; i++)
            {
                if (i == 0)
                {
                    IntManager.GenerateNewDatabaseItem("Inventory", "Current Gold ID");
                    myScript.InventorySlots[i].ItemDatabaseId = 1;
                    myScript.InventorySlots[i].ValueDatabaseId = 1;
                }
                else
                {
                    if (myScript.InventorySlots[i].ItemDatabaseId == 0)
                    {
                        int NewDatabasePosition = IntManager.GenerateNewDatabaseItem("Inventory", "Item Slot " + i.ToString() + " ID");
                        myScript.InventorySlots[i].ItemDatabaseId = NewDatabasePosition;
                    }
                }
                if (myScript.InventorySlots[i].ValueDatabaseId == 0)
                {
                    int NewDatabasePosition = IntManager.GenerateNewDatabaseItem("Inventory", "Item Slot " + i.ToString() + " Value");
                    myScript.InventorySlots[i].ValueDatabaseId = NewDatabasePosition;
                }
            }
        }
    }
}
//////////////////////////////////////////////////////////////////////////////////
#endif




public class DG_PlayerInventory : MonoBehaviour {

    [System.Serializable]
    public class InventorySlot
    {
        [Header("Inventory Slot -------------------------------------------------------------------------------")]
        public int ItemDatabaseId;
        public int ValueDatabaseId;
    }

    public InventorySlot[] InventorySlots;


    private void Awake()
    {
        QuickFind.PlayerInventory = this;
    }




    public int TotalInventoryCountOfItem(int ItemID)
    {
        int Value = 0;
        for (int i = 0; i < InventorySlots.Length; i++)
        {
            InventorySlot IS = InventorySlots[i];
            DG_DataIntItem IntObjectItem = QuickFind.DataInts.GetIntFromID(IS.ItemDatabaseId);

            if (IntObjectItem.IntValue == ItemID)
                Value += QuickFind.DataInts.GetIntFromID(IS.ValueDatabaseId).IntValue;
        }
        return Value;
    }

    public bool ChangeItemInventorySlot(int ItemID, int Value, bool Add)
    {
        //Find if there is a slot where the Item Can Stack.
        InventorySlot ContainingSlot = FindItemSlot(ItemID);
        if (ContainingSlot != null)
            return AddOrSubtract(ContainingSlot, Add, Value);

        //Find an available Slot if no Item already exists.
        ContainingSlot = FindItemSlot(0);
        if (ContainingSlot != null)
        {
            QuickFind.DataInts.GetIntFromID(ContainingSlot.ItemDatabaseId).IntValue = ItemID;
            return AddOrSubtract(ContainingSlot, Add, Value);
        }

        Debug.Log("No Available Space in Inventory");
        return false;
    }
    InventorySlot FindItemSlot(int ItemID)
    {
        for (int i = 0; i < InventorySlots.Length; i++)
        {
            InventorySlot IS = InventorySlots[i];
            DG_DataIntItem IntObjectItem = QuickFind.DataInts.GetIntFromID(IS.ItemDatabaseId);

            if (IntObjectItem.IntValue == ItemID)
                return IS;
        }
        return null;
    }
    bool AddOrSubtract(InventorySlot IS, bool Add, int Value)
    {
        DG_DataIntItem IntValueItem = QuickFind.DataInts.GetIntFromID(IS.ValueDatabaseId);

        if (Add)
        {
            //Check if it will go over max stack
            Debug.Log("Add a check for going over max here.");

            //Add
            IntValueItem.IntValue += Value;
            return true;
        }
        else
        {
            if (IntValueItem.IntValue - Value < 0) //Will take item value below 0
            {
                Debug.Log("Subtracting Value will Take it below 0");
                //Check if there is another stack somewhere.
                Debug.Log("Add a check for if there is another stack here.");
                //
                return false;
            }
            else
            {
                IntValueItem.IntValue -= Value;
                return true;
            }
        }
    }
}
