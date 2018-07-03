using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_AnimationDatabase : MonoBehaviour {

    [HideInInspector]
    public DG_AnimationObject[] ItemCatagoryList;
    [HideInInspector]
    public int ListCount;



    private void Awake()
    {
        QuickFind.AnimationDatabase = this;
    }


    public DG_AnimationObject GetItemFromID(int ID)
    {
        if (ID < 0) { Debug.Log("Get By ID Failed"); return null; }
        if (ID >= ItemCatagoryList.Length) { Debug.Log("Get By ID Failed"); return null; }

        return ItemCatagoryList[ID];
    }
}
