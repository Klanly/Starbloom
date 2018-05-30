using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class DG_ItemsDatabase : MonoBehaviour {


    [System.Serializable]
    public class GenericIconDatabaseItem
    {
        [Header("--------------------------------------")]
        public string ItemName;
        public Sprite Icon;
        public Color[] ColorVariations;
    }


    [HideInInspector] public DG_ItemObject[] ItemCatagoryList;
    [HideInInspector] public int ListCount;


    [ListDrawerSettings(ListElementLabelName = "ItemName")]
    public GenericIconDatabaseItem[] GenericIconList;




    private void Awake()
    {
        QuickFind.ItemDatabase = this;
    }




    public DG_ItemObject GetItemFromID(int ID)
    {
        if (ID < 0) { Debug.Log("Get By ID Failed"); return null; }
        if (ID >= ItemCatagoryList.Length) { Debug.Log("Get By ID Failed"); return null; }
        return ItemCatagoryList[ID];
    }
}
