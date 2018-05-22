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
        Max
    }

    public enum ItemCatagory
    {
        Vegetable,
        Herb
    }


    public string Name;
    public int MaxStackSize;
    public HotbarItemHandler.ActivateableTypes ActivateableType;

    [HideInInspector] public int DatabaseID;
    [HideInInspector] public bool LockItem;


    [Header("Default Visuals")]
    public Sprite Icon;
    public GameObject ModelPrefab;
    public float DefaultScale = 1;

    [Header("Localization")]
    public int WordCatagory;
    public int WordValue;


    #region Tool
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

        [Header("Visuals")]
        public bool HasCustomSprite;
        [ShowIf("HasCustomSprite")]
        public Sprite Icon;
        public bool HasCustomModel;
        [ShowIf("HasCustomModel")]
        public GameObject ModelPrefab;
    }
    #endregion


    #region Growable Item
    public bool isGrowableItem = false;
    [ShowIf("isGrowableItem")]
    public GrowableItem[] GrowableItemStages;

    [System.Serializable]
    public class GrowableItem
    {
        public GameObject ModelPrefab;
    }
    #endregion


    #region Item
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







    public int GetMax()
    {
        if (isTool)
            return ToolQualityLevels.Length;
        else if (isGrowableItem)
            return GrowableItemStages.Length;
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
        else if (isGrowableItem)
        {
            if (IQL >= GrowableItemStages.Length) return GrowableItemStages[GrowableItemStages.Length - 1].ModelPrefab;
            else return GrowableItemStages[IQL].ModelPrefab;
        }
        else
            return ModelPrefab;
    }
}
