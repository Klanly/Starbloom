using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class DG_SceneEntryPoints : MonoBehaviour {


    [HideInInspector]
    public DG_SceneEntryObject[] ItemCatagoryList;
    [HideInInspector]
    public int ListCount;



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
