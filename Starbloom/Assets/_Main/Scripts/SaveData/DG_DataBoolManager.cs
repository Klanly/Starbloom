using System.Collections;
using System.Collections.Generic;
using UnityEngine;




#if UNITY_EDITOR
using UnityEditor;
/////////////////////////////////////////////////////////////////////////////////Editor Extension Buttons
[CustomEditor(typeof(DG_DataBoolManager))]
class DG_DataBoolManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        //Buttons

        DG_DataBoolManager myScript = (DG_DataBoolManager)target;
        if (GUILayout.Button("SaveBoolTracker"))
            myScript.SaveBoolTracker();
        if (GUILayout.Button("LoadBoolTracker"))
            myScript.LoadBoolTracker();
    }
}
//////////////////////////////////////////////////////////////////////////////////
#endif







public class DG_DataBoolManager : MonoBehaviour {

    [System.NonSerialized]
    public DG_DataBoolItem[] BoolList;

    private void Awake()
    {
        QuickFind.DataBools = this;

        int Counter = 0;
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform Child = transform.GetChild(i);
            for (int iN = 0; iN < Child.childCount; iN++)
                Counter++;
        }


        BoolList = new DG_DataBoolItem[Counter];
        int index = 0;
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform Child = transform.GetChild(i);
            for (int iN = 0; iN < Child.childCount; iN++)
            {
                BoolList[index] = Child.GetChild(iN).GetComponent<DG_DataBoolItem>();
                index++;
            }
        }
    }

    public DG_DataBoolItem GetBoolFromID(int ID)
    {
        DG_DataBoolItem ReturnConversation;
        for (int i = 0; i < BoolList.Length; i++)
        {
            ReturnConversation = BoolList[i];
            if (ReturnConversation.DatabaseID == ID)
                return ReturnConversation;
        }
        Debug.Log("Get By ID Failed");
        return BoolList[0];
    }
    public DG_DataBoolItem GetBoolFromIDInEditor(int ID)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform Child = transform.GetChild(i);
            for (int iN = 0; iN < Child.childCount; iN++)
            {
                DG_DataBoolItem BoolItem = Child.GetChild(iN).GetComponent<DG_DataBoolItem>();
                if (BoolItem.DatabaseID == ID)
                    return BoolItem;
            }
        }
        Debug.Log("Get By ID Failed");
        return null;
    }

    public void SaveBoolTracker()
    {
        bool[] BoolArray = new bool[BoolList.Length];
        for (int i = 0; i < BoolArray.Length; i++)
            BoolArray[i] = BoolList[i].BoolValue;

        QuickFind.SaveHandler.SaveBoolTracker(BoolArray);
    }
    public void LoadBoolTracker()
    {
        bool[] BoolArray = QuickFind.SaveHandler.LoadBoolTracker();
        for (int i = 0; i < BoolArray.Length; i++)
            BoolList[i].BoolValue = BoolArray[i];
    }



    public int GenerateNewDatabaseItem(string CatagoryName, string ItemName)
    {
        string CatagorySearch = "Catagory - " + CatagoryName;
        int value = -1;
        for (int i = 0; i < transform.childCount; i++)
        {
            string childname = transform.GetChild(i).name;
            if (childname == CatagorySearch)
            {
                value = i;
                break;
            }
        }
        Transform Catagory;
        if (value == -1) //Generate New Catagory
        {
            GameObject newObject = new GameObject();
            newObject.transform.SetParent(transform);
            newObject.name = CatagorySearch;
            Catagory = newObject.transform;
        }
        else
            Catagory = transform.GetChild(value);


        GameObject NewDatabaseObject = new GameObject();
        NewDatabaseObject.transform.SetParent(Catagory);
        NewDatabaseObject.AddComponent<DG_DataBoolItem>();

        int AvailableDataID = FindNextAvailableDatabaseID(NewDatabaseObject.transform);
        NewDatabaseObject.name = AvailableDataID.ToString() + " - " + ItemName;

        DG_DataBoolItem DataItem = NewDatabaseObject.GetComponent<DG_DataBoolItem>();
        DataItem.DatabaseID = AvailableDataID;

        return AvailableDataID;
    }
    int FindNextAvailableDatabaseID(Transform NewItem)
    {
        Transform Cat = NewItem.parent;
        Transform Tracker = Cat.parent;

        int HighestNumber = 0;

        for (int i = 0; i < Tracker.childCount; i++)
        {
            Transform Child = Tracker.GetChild(i);
            for (int iN = 0; iN < Child.childCount; iN++)
            {
                DG_DataBoolItem BoolItem = Child.GetChild(iN).GetComponent<DG_DataBoolItem>();
                if (BoolItem.DatabaseID > HighestNumber)
                    HighestNumber = BoolItem.DatabaseID;
            }
        }
        return HighestNumber + 1;
    }
}
