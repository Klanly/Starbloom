using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_SFXDatabase : MonoBehaviour {

    [HideInInspector]
    public DG_SFXObject[] ItemCatagoryList;
    [HideInInspector]
    public int ListCount;



    private void Awake()
    {
        QuickFind.SFXDatabase = this;
    }


    public DG_SFXObject GetItemFromID(int ID)
    {
        if (ID < 0) { Debug.Log("Get By ID Failed"); return null; }
        if (ID >= ItemCatagoryList.Length) { Debug.Log("Get By ID Failed"); return null; }

        return ItemCatagoryList[ID];
    }
}
