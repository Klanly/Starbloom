using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class DG_ScenePathNodes : MonoBehaviour {


    [HideInInspector]
    public DG_ScenePathNode[] ItemCatagoryList;
    [HideInInspector]
    public int ListCount;


    private void Awake()
    {
        QuickFind.ScenePathNodes = this;
    }

    public DG_ScenePathNode GetItemFromID(int ID)
    {
        if (ID < 0) { Debug.Log("Get By ID Failed"); return null; }
        if (ID >= ItemCatagoryList.Length) { Debug.Log("Get By ID Failed"); return null; }

        return ItemCatagoryList[ID];
    }
}
