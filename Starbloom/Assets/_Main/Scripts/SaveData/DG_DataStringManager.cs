using System.Collections;
using System.Collections.Generic;
using UnityEngine;



#if UNITY_EDITOR
using UnityEditor;
/////////////////////////////////////////////////////////////////////////////////Editor Extension Buttons
[CustomEditor(typeof(DG_DataStringManager))]
class DG_DataStringManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        //Buttons

        DG_DataStringManager myScript = (DG_DataStringManager)target;
        if (GUILayout.Button("SaveStringTracker"))
            myScript.SaveStringTracker();
        if (GUILayout.Button("LoadStringTracker"))
            myScript.LoadStringTracker();
    }
}
//////////////////////////////////////////////////////////////////////////////////
#endif






public class DG_DataStringManager : MonoBehaviour {

    [System.NonSerialized]
    public DG_DataStringItem[] StringList;

    private void Awake()
    {
        QuickFind.DataStrings = this;

        int Counter = 0;
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform Child = transform.GetChild(i);
            for (int iN = 0; iN < Child.childCount; iN++)
                Counter++;
        }

        StringList = new DG_DataStringItem[Counter];
        int index = 0;
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform Child = transform.GetChild(i);
            for (int iN = 0; iN < Child.childCount; iN++)
            {
                StringList[index] = Child.GetChild(iN).GetComponent<DG_DataStringItem>();
                index++;
            }
        }
    }

    public DG_DataStringItem GetStringFromID(int ID)
    {
        DG_DataStringItem ReturnConversation;
        for (int i = 0; i < StringList.Length; i++)
        {
            ReturnConversation = StringList[i];
            if (ReturnConversation.DatabaseID == ID)
                return ReturnConversation;
        }
        Debug.Log("Get By ID Failed");
        return StringList[0];
    }
    public DG_DataStringItem GetStringFromIDInEditor(int ID)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform Child = transform.GetChild(i);
            for (int iN = 0; iN < Child.childCount; iN++)
            {
                DG_DataStringItem StringItem = Child.GetChild(iN).GetComponent<DG_DataStringItem>();
                if (StringItem.DatabaseID == ID)
                    return StringItem;
            }
        }
        Debug.Log("Get By ID Failed");
        return null;
    }


    public void SaveStringTracker()
    {
        string[] StringArray = new string[StringList.Length];
        for (int i = 0; i < StringArray.Length; i++)
            StringArray[i] = StringList[i].StringValue;

        QuickFind.SaveHandler.SaveStringTracker(StringArray);
    }
    public void LoadStringTracker()
    {
        string[] StringArray = QuickFind.SaveHandler.LoadStringTracker();
        for (int i = 0; i < StringArray.Length; i++)
            StringList[i].StringValue = StringArray[i];
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
        NewDatabaseObject.AddComponent<DG_DataStringItem>();

        int AvailableDataID = FindNextAvailableDatabaseID(NewDatabaseObject.transform);
        NewDatabaseObject.name = AvailableDataID.ToString() + " - " + ItemName;

        DG_DataStringItem DataItem = NewDatabaseObject.GetComponent<DG_DataStringItem>();
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
                DG_DataStringItem BoolItem = Child.GetChild(iN).GetComponent<DG_DataStringItem>();
                if (BoolItem.DatabaseID > HighestNumber)
                    HighestNumber = BoolItem.DatabaseID;
            }
        }
        return HighestNumber + 1;
    }
}
