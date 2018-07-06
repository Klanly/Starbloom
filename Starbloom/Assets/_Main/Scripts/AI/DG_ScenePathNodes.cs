using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class DG_ScenePathNodes : MonoBehaviour {


    [HideInInspector]
    public DG_ScenePathNode[] ItemCatagoryList;
    [HideInInspector]
    public int ListCount;
    [Header("CAREFUL WITH THIS")]
    public bool AllowNewSceneReset;

    [Button(ButtonSizes.Small)]
    public void ResetForNewScene()
    {
        DG_ScenePathNode SEO = ItemCatagoryList[0];
        int Count = ItemCatagoryList.Length;
        for (int i = 1; i < Count; i++) { if (ItemCatagoryList[1] != null) DestroyImmediate(ItemCatagoryList[1].gameObject); }
        ItemCatagoryList = new DG_ScenePathNode[1];
        ListCount = 1;
        ItemCatagoryList[0] = SEO;
        SEO.DatabaseID = 0;
    }


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
