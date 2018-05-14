using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_ItemsDatabase : MonoBehaviour {

    [System.NonSerialized]
    public DG_ItemObject[] ItemList;

    private void Awake()
    {
        QuickFind.ItemDatabase = this;

        int Counter = 0;
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform Child = transform.GetChild(i);
            for (int iN = 0; iN < Child.childCount; iN++)
                Counter++;
        }


        ItemList = new DG_ItemObject[Counter];
        int index = 0;
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform Child = transform.GetChild(i);
            for (int iN = 0; iN < Child.childCount; iN++)
            {
                ItemList[index] = Child.GetChild(iN).GetComponent<DG_ItemObject>();
                index++;
            }
        }
    }


    public DG_ItemObject GetItemFromID(int ID)
    {
        DG_ItemObject ReturnItem;
        for (int i = 0; i < ItemList.Length; i++)
        {
            ReturnItem = ItemList[i];
            if (ReturnItem.DatabaseID == ID)
                return ReturnItem;
        }
        Debug.Log("Get By ID Failed");
        return ItemList[0];
    }
    public DG_ItemObject GetItemFromIDInEditor(int ID)
    {
        DG_ItemObject ReturnItem;

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform Child = transform.GetChild(i);

            for (int iN = 0; iN < Child.childCount; iN++)
            {
                ReturnItem = Child.GetChild(iN).GetComponent<DG_ItemObject>();
                if (ReturnItem.DatabaseID == ID)
                    return ReturnItem;
            }
        }
        Debug.Log("Get By ID Failed");
        return null;
    }




    public DG_ItemObject GenerateNewDatabaseItem(string CatagoryName, string ItemName, string Identifier)
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
        NewDatabaseObject.AddComponent<DG_ItemObject>();

        int AvailableDataID = FindNextAvailableDatabaseID(NewDatabaseObject.transform);
        NewDatabaseObject.name = AvailableDataID.ToString() + " - " + Identifier + " - " + ItemName;

        DG_ItemObject DataItem = NewDatabaseObject.GetComponent<DG_ItemObject>();
        DataItem.DatabaseID = AvailableDataID;

        return DataItem;
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
                DG_ItemObject BoolItem = Child.GetChild(iN).GetComponent<DG_ItemObject>();
                if (BoolItem.DatabaseID > HighestNumber)
                    HighestNumber = BoolItem.DatabaseID;
            }
        }
        return HighestNumber + 1;
    }
}
