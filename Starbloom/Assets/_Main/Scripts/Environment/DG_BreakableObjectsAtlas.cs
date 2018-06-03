using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_BreakableObjectsAtlas : MonoBehaviour {



    [HideInInspector] public DG_BreakableObjectItem[] ItemCatagoryList;
    [HideInInspector] public int ListCount;



    private void Awake()
    {
        QuickFind.BreakableObjectsCompendium = this;
    }


    public DG_BreakableObjectItem GetItemFromID(int ID)
    {
        if (ID < 0) { Debug.Log("Get By ID Failed"); return null; }
        if (ID >= ItemCatagoryList.Length) { Debug.Log("Get By ID Failed"); return null; }

        return ItemCatagoryList[ID];
    }
}
