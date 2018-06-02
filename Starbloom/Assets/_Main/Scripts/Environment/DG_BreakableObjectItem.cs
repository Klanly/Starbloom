using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_BreakableObjectItem : MonoBehaviour {

    public enum BreakableObjectType
    {
        Stone,
        Tree
    }

    [System.Serializable]
    public class ItemRoll
    {
        [Header("-----------------------------------------------------------")]
        public float RollPercent;
        public ItemClump[] ItemGifts;    
    }
    [System.Serializable]
    public class ItemClump
    {
        public int ItemID;
        public int ItemQuality;
        public int Value;
    }


    [HideInInspector] public int DatabaseID;
    [HideInInspector] public bool LockItem;
    

    public string Name;
    public int ItemObjectRefDatabaseID;

    public BreakableObjectType ObjectType;
    public HotbarItemHandler.ActivateableTypes ActivateableTypeRequired;
    public DG_ItemObject.ItemQualityLevels QualityLevelRequired;

    public int ObjectHealth;

    public ItemRoll[] RewardRolls;
}
