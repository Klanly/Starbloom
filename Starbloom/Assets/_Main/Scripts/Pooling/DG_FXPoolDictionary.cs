using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_FXPoolDictionary : MonoBehaviour {




    [HideInInspector]
    public DG_FXPoolItem[] ItemCatagoryList;
    [HideInInspector]
    public int ListCount;


    private void Awake()
    {
        QuickFind.FXPool = this;
    }


    public DG_FXPoolItem GetItemFromID(int ID)
    {
        if (ID < 0) { Debug.Log("Get By ID Failed"); return null; }
        if (ID >= ItemCatagoryList.Length) { Debug.Log("Get By ID Failed"); return null; }

        return ItemCatagoryList[ID];
    }



}
