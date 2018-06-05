using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_ShopAtlas : MonoBehaviour {

    [HideInInspector]
    public DG_ShopAtlasObject[] ItemCatagoryList;
    [HideInInspector]
    public int ListCount;



    private void Awake()
    {
        QuickFind.ShopAtlas = this;
    }


    public DG_ShopAtlasObject GetItemFromID(int ID)
    {
        if (ID < 0) { Debug.Log("Get By ID Failed"); return null; }
        if (ID >= ItemCatagoryList.Length) { Debug.Log("Get By ID Failed"); return null; }

        return ItemCatagoryList[ID];
    }
}
