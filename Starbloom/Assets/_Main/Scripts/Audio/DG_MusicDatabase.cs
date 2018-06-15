using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_MusicDatabase : MonoBehaviour {

    [HideInInspector]
    public DG_MusicObject[] ItemCatagoryList;
    [HideInInspector]
    public int ListCount;



    private void Awake()
    {
        QuickFind.MusicDatabase = this;
    }


    public DG_MusicObject GetItemFromID(int ID)
    {
        if (ID < 0) { Debug.Log("Get By ID Failed"); return null; }
        if (ID >= ItemCatagoryList.Length) { Debug.Log("Get By ID Failed"); return null; }

        return ItemCatagoryList[ID];
    }
}
