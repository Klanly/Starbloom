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
        }
        return Value;
    }

    public bool ChangeItemInventorySlot(int ItemID, int Value, bool Add)
    {
        return false;
    }
    InventorySlot FindItemSlot(int ItemID)
    {

        return null;
    }
    bool AddOrSubtract(InventorySlot IS, bool Add, int Value)
    {
        return false;
    }
}
