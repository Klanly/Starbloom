using System.Collections;
using System.Collections.Generic;
using UnityEngine;





public class DG_CharacterDatabase : MonoBehaviour {


    [HideInInspector] public DG_CharacterObject[] ItemCatagoryList;
    [HideInInspector] public int ListCount;

    private void Awake()
    {
        QuickFind.CharacterDatabase = this;
    }

    public DG_CharacterObject GetItemFromID(int ID)
    {
        if (ID < 0) { Debug.Log("Get By ID Failed"); return null; }
        if (ID >= ItemCatagoryList.Length) { Debug.Log("Get By ID Failed"); return null; }

        return ItemCatagoryList[ID];
    }
}
