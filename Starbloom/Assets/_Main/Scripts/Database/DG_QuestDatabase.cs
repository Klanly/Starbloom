using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_QuestDatabase : MonoBehaviour {

    [HideInInspector] public DG_QuestObject[] ItemCatagoryList;
    [HideInInspector] public int ListCount;

    private void Awake()
    {
        QuickFind.QuestDatabase = this;
    }


    public DG_QuestObject GetItemFromID(int ID)
    {
        if (ID < 0) { Debug.Log("Get By ID Failed"); return null; }
        if (ID >= ItemCatagoryList.Length) { Debug.Log("Get By ID Failed"); return null; }

        return ItemCatagoryList[ID];
    }
}
