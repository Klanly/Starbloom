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


    public List<DG_BreakableObjectItem> FetchListOfSpawnableObjects(HotbarItemHandler.ActivateableTypes Activateable, DG_ItemObject.ItemQualityLevels ToolLevel)
    {
        List<DG_BreakableObjectItem> ReturnList = new List<DG_BreakableObjectItem>();

        for (int i = 0; i < ItemCatagoryList.Length; i++)
        {
            DG_BreakableObjectItem FD = ItemCatagoryList[i];
            if (FD.ActivateableTypeRequired != Activateable) continue;
            if (ToolLevel >= FD.QualityLevelRequired) continue;

            ReturnList.Add(FD);
        }
        return ReturnList;
    }
}
