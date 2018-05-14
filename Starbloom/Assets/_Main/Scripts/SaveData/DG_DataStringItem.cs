using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
/////////////////////////////////////////////////////////////////////////////////Editor Extension Buttons
[CustomEditor(typeof(DG_DataStringItem))]
class DG_DataStringItemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        //Buttons

        DG_DataStringItem myScript = (DG_DataStringItem)target;
        if (GUILayout.Button("FindNextAvailableDatabaseID"))
            myScript.FindNextAvailableDatabaseID();
    }
}
//////////////////////////////////////////////////////////////////////////////////
#endif


public class DG_DataStringItem : MonoBehaviour {

    public int DatabaseID;
    public string StringValue;
    public string Description;




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
                DG_DataStringItem BoolItem = Child.GetChild(iN).GetComponent<DG_DataStringItem>();
                if (BoolItem.DatabaseID != 0)
                {
                    Debug.Log("This Object Already Has a Database ID");
                    return;
                }
                if (BoolItem.DatabaseID > HighestNumber)
                    HighestNumber = BoolItem.DatabaseID;
            }
        }
        DatabaseID = HighestNumber + 1;
        transform.gameObject.name = DatabaseID.ToString() + " - ";
    }
}
