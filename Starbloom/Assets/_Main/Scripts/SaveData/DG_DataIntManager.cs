using System.Collections;
using System.Collections.Generic;
using UnityEngine;




#if UNITY_EDITOR
using UnityEditor;
/////////////////////////////////////////////////////////////////////////////////Editor Extension Buttons
[CustomEditor(typeof(DG_DataIntManager))]
class DG_DataIntManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        //Buttons

        DG_DataIntManager myScript = (DG_DataIntManager)target;
        if (GUILayout.Button("SaveIntTracker"))
            myScript.SaveIntTracker();
        if (GUILayout.Button("LoadIntTracker"))
            myScript.LoadIntTracker();
    }
}
//////////////////////////////////////////////////////////////////////////////////
#endif





public class DG_DataIntManager : MonoBehaviour {

    [System.NonSerialized]
    public DG_DataIntItem[] IntList;

    private void Awake()
    {
        QuickFind.DataInts = this;

        int Counter = 0;
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform Child = transform.GetChild(i);
            for (int iN = 0; iN < Child.childCount; iN++)
                Counter++;
        }

        IntList = new DG_DataIntItem[Counter];
        int index = 0;
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform Child = transform.GetChild(i);
            for (int iN = 0; iN < Child.childCount; iN++)
            {
                IntList[index] = Child.GetChild(iN).GetComponent<DG_DataIntItem>();
                index++;
            }
        }
    }

    public DG_DataIntItem GetIntFromID(int ID)
    {
        DG_DataIntItem ReturnConversation;
        for (int i = 0; i < IntList.Length; i++)
        {
            ReturnConversation = IntList[i];
            if (ReturnConversation.DatabaseID == ID)
                return ReturnConversation;
        }
        Debug.Log("Get By ID Failed");
        return IntList[0];
    }
    public DG_DataIntItem GetItemFromIDInEditor(int ID)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform Child = transform.GetChild(i);
            for (int iN = 0; iN < Child.childCount; iN++)
            {
                DG_DataIntItem Item = Child.GetChild(iN).GetComponent<DG_DataIntItem>();
                if (Item.DatabaseID == ID)
                    return Item;
            }
        }
        Debug.Log("Get By ID Failed");
        return null;
    }


    public void SaveIntTracker()
    {
        int[] IntArray = new int[IntList.Length];
        for (int i = 0; i < IntArray.Length; i++)
            IntArray[i] = IntList[i].IntValue;

        QuickFind.SaveHandler.SaveIntTracker(IntArray);
    }
    public void LoadIntTracker()
    {
        int[] IntArray = QuickFind.SaveHandler.LoadIntTracker();
        for (int i = 0; i < IntArray.Length; i++)
            IntList[i].IntValue = IntArray[i];
    }




    public void DeleteGameObjectFromID(int ID)
    {
        List<GameObject> DestroyList = new List<GameObject>();

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform Child = transform.GetChild(i);
            for (int iN = 0; iN < Child.childCount; iN++)
            {
                DG_DataIntItem IntItem = Child.GetChild(iN).GetComponent<DG_DataIntItem>();
                if (IntItem.DatabaseID != 0 && IntItem.DatabaseID == ID)
                    DestroyList.Add(Child.GetChild(iN).gameObject);
            }
        }

        foreach(GameObject Child in DestroyList)
        {
            GameObject.DestroyImmediate(Child);
        }
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
        NewDatabaseObject.AddComponent<DG_DataIntItem>();

        int AvailableDataID = FindNextAvailableDatabaseID(NewDatabaseObject.transform);
        NewDatabaseObject.name = AvailableDataID.ToString() + " - " + ItemName;

        DG_DataIntItem DataItem = NewDatabaseObject.GetComponent<DG_DataIntItem>();
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
                DG_DataIntItem BoolItem = Child.GetChild(iN).GetComponent<DG_DataIntItem>();
                if (BoolItem.DatabaseID > HighestNumber)
                    HighestNumber = BoolItem.DatabaseID;
            }
        }
        return HighestNumber + 1;
    }
}
