using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
using UnityEditor;
#endif


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
        Fruit,
        Misc
    }

    public bool DatabaseUsesNameInsteadOfPrefab;
    public string Name;
#if UNITY_EDITOR
    public NameChange ChangeName;
#endif


    public int MaxStackSize;

    [Header("Type")]
    public HotbarItemHandler.ActivateableTypes ActivateableType;
    [ShowIf("ActivateableType", HotbarItemHandler.ActivateableTypes.PlaceableItem)]
    public bool DestroysSoilOnPlacement;
    [ShowIf("ActivateableType", HotbarItemHandler.ActivateableTypes.PlaceableItem)]
    public bool ShowPlacementGhost;

    [Header("ToolTip Data")]
    public DG_TooltipGUI.ToolTipContainerItem ToolTipType;

    [HideInInspector]
    public int DatabaseID;
    [HideInInspector]
    public bool LockItem;


    [Header("Default Visuals")]
    public Sprite Icon;
    public GameObject ModelPrefab;
    public bool UsePoolIDForSpawn;
    [ShowIf("UsePoolIDForSpawn")]
    public int PoolID;
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
        public Sprite Icon;
        public int EquipmentID;
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
    public bool IsWaterable;
    [ShowIf("isGrowableItem")]
    public bool RequireTilledEarth = false;
    [ShowIf("isGrowableItem")]
    public int ResetIndex;
    [ShowIf("isGrowableItem")]
    public int BreakIndex;

    [ShowIf("isGrowableItem")]
    public GrowthStage[] GrowthStages;



    [System.Serializable]
    public class GrowthStage
    {
        public int GrowthLevelRequired;
        public GameObject StagePrefabReference;
    }
    #endregion


    #region Breakable
    [Header("----------------------------------")]
    public bool isBreakable = false;
    [Header("Breakable")]
    [ShowIf("isBreakable")]
    public Environment[] EnvironmentValues;

    [System.Serializable]
    public class Environment
    {
        public DG_BreakableObjectItem.OnHitEffectType OnHitFXType;
        public HotbarItemHandler.ActivateableTypes ActivateableTypeRequired;
        public DG_ItemObject.ItemQualityLevels QualityLevelRequired;
        public int ObjectHealth;
    }
    #endregion


    #region Weapon
    [Header("----------------------------------")]
    public bool isWeapon = false;
    [Header("Weapon")]
    [ShowIf("isWeapon")]
    public Weapon[] WeaponValues;

    [System.Serializable]
    public class Weapon
    {
        [Header("Damage")]
        public DG_CombatHandler.Damages DamageValues;

        [Header("Energy")]
        public int EnergyBaseCost;
        public int ManaCost;

        [Header("Visuals")]
        public int EquipmentID;

    }
    #endregion



    [Header("----------------------------------")]
    public bool HarvestableItem;
    [ShowIf("HarvestableItem")]
    public int HarvestItemIndex;
    [ShowIf("HarvestableItem")]
    public int HarvestClusterIndex;
   


    [Header("----------------------------------")]
    public bool isWallItem;
    [Header("----------------------------------")]
    public int AnimationActionID;
    public int AnimationInteractID;  //Since Some Objects Have a "plant", and a "Harvest" Animation








    #region Get Helpers

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
        if (UsePoolIDForSpawn && Application.isPlaying)
            return QuickFind.PrefabPool.GetPoolItemByPrefabID(PoolID);
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
    public Sprite GetItemSpriteByQuality(int IQL)
    {
        if (isTool)
        {
            Tool T = GetToolByQuality(IQL);
            return T.Icon;
        }

        return Icon;
    }
    public int GetClothingID(DG_PlayerCharacters.RucksackSlot RucksackSlot)
    {
        if (isTool)
            return GetToolByQuality(RucksackSlot.CurrentStackActive).EquipmentID;
        else if (isWeapon)
            return WeaponValues[0].EquipmentID;
        else
            return -1;
    }

    public int GetBuyPriceByQuality(int IQL)
    {
        if (isTool) return GetToolByQuality(IQL).BuyPrice;
        if(isItem) return GetItemByQuality(IQL).BuyPrice;

        Debug.Log("This Item is not listed as an item or tool.");
        return 0;
    }

    public int GetSellPriceByQuality(int IQL)
    {
        if (isTool) return GetToolByQuality(IQL).SellPrice;
        if (isItem) return GetItemByQuality(IQL).SellPrice;

        Debug.Log("This Item is not listed as an item or tool.");
        return 0;
    }


    #endregion





#if UNITY_EDITOR
    [System.Serializable]
    public class NameChange
    {
        [Button(ButtonSizes.Small)]
        public void SetName()
        {

            DG_ItemObject IO = null;
            IO = Selection.activeGameObject.GetComponent<DG_ItemObject>();
            if (QuickFindInEditor.GetEditorUserSettings().CurrentLanguage != 0) { Debug.Log("Language Not set to English."); return; }

            if (Application.isPlaying) return;
            

            string NewName = string.Empty;
            NewName += QuickFindInEditor.GetEditorWordDatabase().GetWordFromID(IO.ToolTipType.MainLocalizationID);
            NewName += " - ";
            NewName += QuickFindInEditor.GetEditorWordDatabase().GetWordFromID(IO.ToolTipType.SubLocalizationID);

            IO.Name = NewName;
        }
    }
#endif


}
