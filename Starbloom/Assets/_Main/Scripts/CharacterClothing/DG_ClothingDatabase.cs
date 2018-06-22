using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_ClothingDatabase : MonoBehaviour {


    [HideInInspector]
    public DG_ClothingObject[] ItemCatagoryList;
    [HideInInspector]
    public int ListCount;



    private void Awake()
    {
        QuickFind.ClothingDatabase = this;
    }


    public DG_ClothingObject GetItemFromID(int ID)
    {
        if (ID < 0) { Debug.Log("Get By ID Failed"); return null; }
        if (ID >= ItemCatagoryList.Length) { Debug.Log("Get By ID Failed"); return null; }

        return ItemCatagoryList[ID];
    }
}
