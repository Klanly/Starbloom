using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_DialogueManager : MonoBehaviour {

    [HideInInspector] public NodeLink[] ItemCatagoryList;
    [HideInInspector] public int ListCount;

    private void Awake()
    {
        QuickFind.DialogueManager = this;
    }

    public NodeLink GetItemFromID(int ID)
    {
        if (ID < 0) { Debug.Log("Get By ID Failed"); return null; }
        if (ID >= ItemCatagoryList.Length) { Debug.Log("Get By ID Failed"); return null; }

        return ItemCatagoryList[ID];
    }
}
