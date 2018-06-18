using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_EnemyDatabase : MonoBehaviour {

    [HideInInspector]
    public DG_EnemyObject[] ItemCatagoryList;
    [HideInInspector]
    public int ListCount;



    private void Awake()
    {
        QuickFind.EnemyDatabase = this;
    }


    public DG_EnemyObject GetItemFromID(int ID)
    {
        if (ID < 0) { Debug.Log("Get By ID Failed"); return null; }
        if (ID >= ItemCatagoryList.Length) { Debug.Log("Get By ID Failed"); return null; }

        return ItemCatagoryList[ID];
    }
}
