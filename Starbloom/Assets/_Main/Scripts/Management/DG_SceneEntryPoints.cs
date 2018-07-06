﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class DG_SceneEntryPoints : MonoBehaviour {


    [HideInInspector]
    public DG_SceneEntryObject[] ItemCatagoryList;
    [HideInInspector]
    public int ListCount;
    [Header("CAREFUL WITH THIS")]
    public bool AllowNewSceneReset;

    [Button(ButtonSizes.Small)]
    public void ResetForNewScene()
    {
        DG_SceneEntryObject SEO = ItemCatagoryList[0];
        int Count = ItemCatagoryList.Length;
        for (int i = 1; i < Count; i++) { if (ItemCatagoryList[1] != null ) DestroyImmediate(ItemCatagoryList[1].gameObject); }
        ItemCatagoryList = new DG_SceneEntryObject[1];
        ListCount = 1;
        ItemCatagoryList[0] = SEO;
        SEO.DatabaseID = 0;
    }



    private void Awake()
    {
        QuickFind.SceneEntryPoints = this;
    }
#if UNITY_EDITOR
    private void Start()
    {
        QuickFind.GameSettings.GetComponent<DG_SceneJumpTool>().LoadScenePortals(this);
    }
#endif

    public DG_SceneEntryObject GetItemFromID(int ID)
    {
        if (ID < 0) { Debug.Log("Get By ID Failed"); return null; }
        if (ID >= ItemCatagoryList.Length) { Debug.Log("Get By ID Failed"); return null; }

        return ItemCatagoryList[ID];
    }
}
