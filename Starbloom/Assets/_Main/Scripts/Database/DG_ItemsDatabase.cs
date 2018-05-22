using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_ItemsDatabase : MonoBehaviour {

    [HideInInspector] public DG_ItemObject[] ItemCatagoryList;
    [HideInInspector] public int ListCount;


    private void Awake()
    {
        QuickFind.ItemDatabase = this;
    }

    public DG_ItemObject GetItemFromID(int ID)
    {
        if (ID < 0) { Debug.Log("Get By ID Failed"); return null; }
        if (ID >= ItemCatagoryList.Length) { Debug.Log("Get By ID Failed"); return null; }
        return ItemCatagoryList[ID];
    }
}
