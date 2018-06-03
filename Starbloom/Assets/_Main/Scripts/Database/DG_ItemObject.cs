using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;


public class DG_ItemObject : MonoBehaviour
{
    public enum ItemQualityLevels
    {
        Low,
        Normal,
        High,
        Max,
        All
    }

    public enum ItemCatagory
    {
        Vegetable,
        Herb,
        Fish,
        Resource,
        Seeds,
        Fruit
    }


    public string Name;
    public int MaxStackSize;
    public HotbarItemHandler.ActivateableTypes ActivateableType;
    public bool DestroysSoilOnPlacement;
    
    [Header("ToolTip Data")]
    public DG_TooltipGUI.ToolTipContainerItem ToolTipType;

    [HideInInspector] public int DatabaseID;
    [HideInInspector] public bool LockItem;


    [Header("Default Visuals")]
    public Sprite Icon;
    public GameObject ModelPrefab;
    public float DefaultScale = 1;



    #region Tool
    [Header("----------------------------------")]
    public bool isTool = false;
    [Header("Tool")]
    [ShowIf("isTool")]
    public Tool[] ToolQualityLevels;

    [System.Serializable]
    public class Tool
    {
        [Header("------------------------------------------------------")]
        public ItemQualityLevels Quality;
        [Header("Energy")]
        public int EnergyUsageValue;

        [Header("Shop")]
        public int BuyPrice;
        public int SellPrice;

        [Header("Strength")]
        public int StrengthValue;

        [Header("Visuals")]
        public bool HasCustomSprite;
        [ShowIf("HasCustomSprite")]
        public Sprite Icon;
        public bool HasCustomModel;
        [ShowIf("HasCustomModel")]
        public GameObject ModelPrefab;
    }
    #endregion


    #region Item
    [Header("----------------------------------")]
    public bool isItem = false;
    [ShowIf("isItem")]
    [Header("Item")]
    public ItemCatagory ItemCat;
    [ShowIf("isItem")]
    public Item[] ItemQualities;

    [System.Serializable]
    public class Item
    {
        public ItemQualityLevels Quality;

        [Header("Health")]
        public bool AdjustsHealth;
        [ShowIf("AdjustsHealth")]
        public int HealthAdjustValue;

        [Header("Energy")]
        public bool AdjustsEnergy;
        [ShowIf("AdjustsEnergy")]
        public int EnergyAdjustValue;

        [Header("Shop")]
        public int BuyPrice;
        public int SellPrice;
    }
    #endregion


    #region Storage
    [Header("----------------------------------")]
    public bool isStorage = false;
    [Header("Storage")]
    [ShowIf("isStorage")]
    public Storage[] StorageList;

    [System.Serializable]
    public class Storage
    {
        public int TotalStorageSlots;
    }
    #endregion


    #region Growable Item
    [Header("----------------------------------")]
    public bool isGrowableItem = false;
    [ShowIf("isGrowableItem")]
    public bool RequireTilledEarth = false;
    [ShowIf("isGrowableItem")]
    public GameObject PreviewItem = null;

    //[ShowIf("isGrowableItem")]
    //public GrowthStage[] GrowthStages;
    //
    //[System.Serializable]
    //public class GrowthStage
    //{
    //    public int GrowthLevelRequired;
    //    public GameObject StagePrefabReference;
    //}
    #endregion


    #region Environment
    [Header("----------------------------------")]
    public bool isEnvironment = false;
    [Header("Environment")]
    [ShowIf("isEnvironment")]
    public Environment[] EnvironmentValues;

    [System.Serializable]
    public class Environment
    {
        public bool IsWaterable;
        public bool IsBreakable;
        [ShowIf("IsBreakable")]
        public DG_BreakableObjectItem.OnHitEffectType ObjectType;
        [ShowIf("IsBreakable")]
        public HotbarItemHandler.ActivateableTypes ActivateableTypeRequired;
        [ShowIf("IsBreakable")]
        public DG_ItemObject.ItemQualityLevels QualityLevelRequired;
        [ShowIf("IsBreakable")]
        public int ObjectHealth;

        [ShowIf("IsBreakable")]
        public bool DropItemsOnBreak;
        [ShowIf("DropItemsOnBreak")]
        public int BreakableAtlasID;

        [ShowIf("IsBreakable")]
        public bool DropItemsOnInteract;
        [ShowIf("DropItemsOnInteract")]
        public int InteractDropID;

        [ShowIf("IsBreakable")]
        public bool SwapItemOnBreak;
        [ShowIf("SwapItemOnBreak")]
        public int SwapID;
    }
    #endregion




    public int GetMax()
    {
        if (isTool)
            return ToolQualityLevels.Length;
        else if (isItem)
            return ItemQualities.Length;

        return 0;
    }

    public GameObject GetPrefabReferenceByQuality(int IQL)
    {
        if (isTool)
        {
            if (ToolQualityLevels[IQL].HasCustomModel) return ToolQualityLevels[IQL].ModelPrefab;
            else return ModelPrefab;
        }
        else
            return ModelPrefab;
    }
    public Item GetItemByQuality(int IQL)
    {
        ItemQualityLevels QualityNeeded = (ItemQualityLevels)IQL;
        for (int i = 0; i < ItemQualities.Length; i++)
        {
            if (ItemQualities[i].Quality == QualityNeeded)
                return ItemQualities[i];
        }
        return null;
    }
    public Tool GetToolByQuality(int IQL)
    {
        ItemQualityLevels QualityNeeded = (ItemQualityLevels)IQL;
        for (int i = 0; i < ToolQualityLevels.Length; i++)
        {
            if(ToolQualityLevels[i].Quality == QualityNeeded)
                return ToolQualityLevels[i];
        }
        return null;
    }
}
