using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
/////////////////////////////////////////////////////////////////////////////////Editor Extension Buttons
[CustomEditor(typeof(DG_ItemObject))]
class DG_ItemObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        //Buttons

        DG_ItemObject myScript = (DG_ItemObject)target;
        if (GUILayout.Button("FindNextAvailableDatabaseID"))
            myScript.FindNextAvailableDatabaseID();
    }
}
//////////////////////////////////////////////////////////////////////////////////
#endif


public class DG_ItemObject : MonoBehaviour {

    public int DatabaseID;
    public string DevNotes;
    public int WordCatagory;
    public int WordValue;
    [Header("Item Stats")]
    public bool ItemHasStats;
    public ItemStats[] ItemStat;

    [System.Serializable]
    public class ItemStats
    {

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
                DG_ItemObject Item = Child.GetChild(iN).GetComponent<DG_ItemObject>();
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
