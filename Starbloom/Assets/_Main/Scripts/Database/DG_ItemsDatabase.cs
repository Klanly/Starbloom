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
<<<<<<< .merge_file_a11876
=======
        public int LocalizationID;
>>>>>>> .merge_file_a09476
        public Color[] ColorVariations;
    }


    [HideInInspector]
    public DG_ItemObject[] ItemCatagoryList;
    [HideInInspector]
    public int ListCount;


    public float PreMagneticObjectWaitTimer;

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

    public GenericIconDatabaseItem GetGenericIconByString(string Requested)
    {
        for(int i = 0; i < GenericIconList.Length; i++)
        {
            if (GenericIconList[i].ItemName == Requested)
                return GenericIconList[i];
        }
        Debug.Log("No Item Found with that name");
        return null;
    }
}
