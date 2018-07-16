using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


#if UNITY_EDITOR
using UnityEditor;
#endif



//Stardew Valleys tooltip system.

//Main Name
//Sub Name(if Applicable)
//Border
//
//[Crafting only] (Icon) + Ingredient
//[Stats only] (Icon) + Ingredient
//
//[All But Stats] Description
//
//[Inventory only](Icon) + Stats
//
//[Acheivements only] Acheivement Values
//[Acheivements only] Acheivement Largest Sale




public class DG_TooltipGUI : MonoBehaviour {

    public enum ToolTipGroups
    {
        NonEdibleItem,
        EdibleItem,
        Tool,
        Weapon,
        Equipment,
        SkillDisplay,
        Person,
        MapItem,
        Craft,
        AcheivementItem,
        AcheivementFish,
        AcheivementArtifact,
        AcheivementStone,
        AcheivementMeal,
        AcheivementGeneral,
        UIElement,
        NoToolTip
    }
    [System.Serializable]
    public class ToolTipGroup
    {
        public ToolTipGroups GroupType;
        public ToolTipModules[] ActiveModules;

        public bool ContainsModule(ToolTipModules M)
        {
            for (int i = 0; i < ActiveModules.Length; i++)
                if (ActiveModules[i] == M) return true;
            return false;
        }
    }

    [System.Serializable]
    public class ToolTipContainerItem
    {
        public ToolTipGroups GroupType;
        public int MainLocalizationID;
        public int SubLocalizationID;
        public int DescriptionID;
        [Header("Context Tooltip ID")]
        public int ContextID;

#if UNITY_EDITOR
        [Button(ButtonSizes.Small)] void GoToMainTextLocation(){Selection.activeGameObject = QuickFindInEditor.GetEditorWordDatabase().GetItemFromID(MainLocalizationID).gameObject;}
        [Button(ButtonSizes.Small)] void GoToSubTextLocation(){Selection.activeGameObject = QuickFindInEditor.GetEditorWordDatabase().GetItemFromID(SubLocalizationID).gameObject;}
        [Button(ButtonSizes.Small)] void GoToDescriptionLocation(){Selection.activeGameObject = QuickFindInEditor.GetEditorWordDatabase().GetItemFromID(DescriptionID).gameObject;}
#endif
    }


    public enum ToolTipModules
    {
        Main,
        SubName,
        Border,
        CraftingIngredient,
        StatsDescriptor,
        Description,
        InventoryStat,
        AchievementValues,
        AchievementLargestSale
    }







    [Header("Floating QualitySelection")]
    public RectTransform FloatingQualitySelect;
    public RectTransform FloatingQualityGrid;
    [Header("Floating Inventory Item")]
    public RectTransform FloatingRect;
    public RectTransform VerticalGridRect;
    public UnityEngine.UI.ContentSizeFitter CSF;
    [ListDrawerSettings(ListElementLabelName = "GroupType", NumberOfItemsPerPage = 16, Expanded = false)]
    public ToolTipGroup[] ToolTipTypes;

    [Header("Debug")]
    public bool DebugON = false;



    [System.NonSerialized] public bool DisplayTooltip = false;
    [System.NonSerialized] public bool IsQualitySelection = false;
    float DefaultX;
    List<DG_TooltipModule> Modules;

    //Context Per UI Type
    [System.NonSerialized] public DG_InventoryItem HoveredInventoryItem;
    [System.NonSerialized] public DG_PlayerCharacters.RucksackSlot ActiveRucksackSlot;
    ToolTipGroup ActiveToolTipGroup;
    ToolTipContainerItem ActiveItemObject;






    private void Awake()
    {
        QuickFind.TooltipHandler = this;
        transform.localPosition = Vector3.zero;
        DefaultX = VerticalGridRect.localPosition.x;
        Modules = new List<DG_TooltipModule>();
        for (int i = 0; i < VerticalGridRect.childCount; i++)
            Modules.Add(VerticalGridRect.GetChild(i).GetComponent<DG_TooltipModule>());
        FloatingQualitySelect.position = new Vector3(8000, 0, 0);
        FloatingRect.position = new Vector3(8000, 0, 0);
        this.enabled = false;
    }
    private void Start()
    {
        SetQualityLevelStars();
    }
    public void ShowToolTip(ToolTipContainerItem TTContainer)
    {
        if (QuickFind.GUI_Inventory.isFloatingInventoryItem) return;
        ActiveItemObject = TTContainer;
        ActiveToolTipGroup = GetGroupByEnum(TTContainer.GroupType);
        GenerateToolTip();
        GenerateQualitySelectionGrid();
        DisplayTooltip = true;
        this.enabled = true;
    }
    public void HideToolTip()
    {
        HoveredInventoryItem = null;
        if (QuickFind.GUI_Inventory.isFloatingInventoryItem) return;
        FloatingQualitySelect.position = new Vector3(8000, 0, 0);
        IsQualitySelection = false;
        DisplayTooltip = false;
    }

    public void UpdateEquippedNum(int isUP, bool CanLoop)
    {
        AddRucksack(isUP, CanLoop, ActiveRucksackSlot.CurrentStackActive);

        DG_PlayerCharacters.CharacterEquipment Equipment = QuickFind.Farm.PlayerCharacters[QuickFind.NetworkSync.PlayerCharacterID].Equipment;
        QuickFind.GUI_Inventory.UpdateRucksackSlotVisual(HoveredInventoryItem, ActiveRucksackSlot);
        GenerateQualitySelectionGrid();
        GenerateToolTip();
        if (HoveredInventoryItem.SlotID < 12)
            QuickFind.GUI_Inventory.UpdateMirrorSlot(HoveredInventoryItem.SlotID);
    }
    void AddRucksack(int isUP, bool CanLoop, int OriginalValue)
    {
        bool PreventInfinite = true;
        ActiveRucksackSlot.CurrentStackActive += isUP;
        if (ActiveRucksackSlot.CurrentStackActive > 3)
            { if (CanLoop) ActiveRucksackSlot.CurrentStackActive = 0; else { ActiveRucksackSlot.CurrentStackActive = OriginalValue; PreventInfinite = false; } }
        if (ActiveRucksackSlot.CurrentStackActive < 0)
            { if (CanLoop) ActiveRucksackSlot.CurrentStackActive = 3; else { ActiveRucksackSlot.CurrentStackActive = OriginalValue; PreventInfinite = false; } }
        if (ActiveRucksackSlot.GetNumberOfQuality((DG_ItemObject.ItemQualityLevels)ActiveRucksackSlot.CurrentStackActive) == 0 && PreventInfinite)
            AddRucksack(isUP, CanLoop, OriginalValue);
    }



    private void Update()
    {
        if (DisplayTooltip || DebugON)
        {
            float Height = 0; bool TooltipBelow = false; bool MouseRightSide = false;

            Vector2 MousePos = Input.mousePosition; if(DebugON) { MousePos.x = 800; MousePos.y = 600; }
            FloatingRect.position = MousePos;
            if (MousePos.y >= Screen.height * 0.2) TooltipBelow = true;
            if (MousePos.x >= Screen.width * 0.8) MouseRightSide = true;
            for (int i = 0; i < Modules.Count; i++)
            {
                DG_TooltipModule Module = Modules[i];
                if (Module.isActive) { Height = Height + Module.GetComponent<RectTransform>().rect.height; Module.StretchHeightToBounds(); }
            }
            float OffsetHeight = 0 - (Height / 2); if (!TooltipBelow) OffsetHeight = -OffsetHeight;
            float OffsetWidth = DefaultX; if (MouseRightSide) OffsetWidth = -DefaultX;

            Vector3 CurrentPos = VerticalGridRect.localPosition; CurrentPos.x = OffsetWidth; CurrentPos.y = OffsetHeight;
            VerticalGridRect.localPosition = CurrentPos;
            //
            CSF.enabled = false; CSF.enabled = true;
        }
        else
        { this.enabled = false; FloatingRect.position = new Vector3(8000, 0, 0); }

        SetQualityHotbarPosition();
    }



    void GenerateToolTip()
    {
        SetCorrectActiveModules();
        switch(ActiveToolTipGroup.GroupType)
        {
            case ToolTipGroups.NonEdibleItem:
                {
                    SetMain();
                    SetSub();
                    SetDescription();
                }
                break;
            case ToolTipGroups.Tool:
                {
                    SetMain();
                    SetSub();
                    SetDescription();
                }
                break;
            case ToolTipGroups.EdibleItem:
                {
                    SetMain();
                    SetSub();
                    SetDescription();
                    SetUpInventoryStats();
                }
                break;
            case ToolTipGroups.Craft:
                {
                    SetMain();
                    SetSub();
                    SetUpCraftIngredients();
                    SetDescription();
                }
                break;
            case ToolTipGroups.Weapon:
                {
                    SetMain();
                    SetSub();
                    SetDescription();
                    SetUpCombatStats();
                }
                break;
        }
    }

    void SetMain()
    {
        DG_TooltipModule Main = GetModuleByType(ToolTipModules.Main);
        Main.TextObject.text = QuickFind.WordDatabase.GetWordFromID(ActiveItemObject.MainLocalizationID);
    }
    void SetSub()
    {
        DG_TooltipModule Sub = GetModuleByType(ToolTipModules.SubName);
        Sub.TextObject.text = QuickFind.WordDatabase.GetWordFromID(ActiveItemObject.SubLocalizationID);
    }
    void SetDescription()
    {
        DG_TooltipModule Desc = GetModuleByType(ToolTipModules.Description);
        Desc.TextObject.text = QuickFind.WordDatabase.GetWordFromID(ActiveItemObject.DescriptionID);
    }
    void SetUpInventoryStats()
    {
        DG_TooltipModule Desc = GetModuleByType(ToolTipModules.InventoryStat);
        Desc.TurnOFFSubs();
        DG_ItemObject IO = QuickFind.ItemDatabase.GetItemFromID(ActiveItemObject.ContextID);
        if (IO == null) Debug.Log("trying to hoverover an edible item that has no context ID, be sure to set the context ID to the Item database ID");
        DG_ItemObject.Item Item = IO.GetItemByQuality(ActiveRucksackSlot.CurrentStackActive);

        int index = 0;
        if (Item.AdjustsHealth)
        {
            int HealthAdjust = Item.HealthAdjustValue;
            DG_TooltipSubItem Sub = Desc.GetSubItem(index); index++;
            DG_ItemsDatabase.GenericIconDatabaseItem ICD = QuickFind.ItemDatabase.GetGenericIconByString("Health");
            Sub.DisplayImage.sprite = ICD.Icon;
            Sub.NumberObject.text = HealthAdjust.ToString(); 
            Sub.TextObject.text =  QuickFind.WordDatabase.GetWordFromID(ICD.LocalizationID);

            Color C = ICD.ColorVariations[0];
            Sub.DisplayImage.color = C;
        }
        if (Item.AdjustsEnergy)
        {
            int EnergyAdjust = Item.EnergyAdjustValue;
            DG_TooltipSubItem Sub = Desc.GetSubItem(index); index++;
            DG_ItemsDatabase.GenericIconDatabaseItem ICD = QuickFind.ItemDatabase.GetGenericIconByString("Energy");
            Sub.DisplayImage.sprite = ICD.Icon;
            Sub.NumberObject.text = EnergyAdjust.ToString();
            Sub.TextObject.text = QuickFind.WordDatabase.GetWordFromID(ICD.LocalizationID);

            Color C = ICD.ColorVariations[0];
            Sub.DisplayImage.color = C;
        }
    }
    void SetUpCraftIngredients()
    {
        DG_TooltipModule Desc = GetModuleByType(ToolTipModules.CraftingIngredient);
        Desc.TurnOFFSubs();

        //Update Static Ingredients Text based on language.
        Desc.transform.GetChild(0).GetChild(0).GetComponent<DG_TextStatic>().ManualLoad();

        DG_CraftingDictionaryItem CDI = QuickFind.CraftingDictionary.GetItemFromID(ActiveItemObject.ContextID);
        DG_ItemObject IO = QuickFind.ItemDatabase.GetItemFromID(CDI.ItemCreatedRef);


        int index = 0;
        for (int i = 0; i < CDI.IngredientList.Length; i++)
        {
            DG_CraftingDictionaryItem.Ingredient I = CDI.IngredientList[i];
            DG_ItemObject IngredientObject = QuickFind.ItemDatabase.GetItemFromID(I.ItemDatabaseRef);
            DG_TooltipSubItem Sub = Desc.GetSubItem(index); index++;
            Sub.DisplayImage.sprite = IngredientObject.Icon;
            Sub.NumberObject.text = I.Value.ToString();
            Sub.TextObject.text = QuickFind.WordDatabase.GetWordFromID(IngredientObject.ToolTipType.MainLocalizationID);

            DG_ItemsDatabase.GenericIconDatabaseItem ICD = QuickFind.ItemDatabase.GetGenericIconByString("TextColor");
            if (QuickFind.InventoryManager.TotalInventoryCountOfItem(IngredientObject.DatabaseID) < I.Value)
            {
                Sub.TextObject.color = ICD.ColorVariations[1];  //Not Enough Color
                Sub.NumberObject.color = ICD.ColorVariations[1];
            }
            else
            {
                Sub.TextObject.color = ICD.ColorVariations[0];  //Has Enough Color
                Sub.NumberObject.color = ICD.ColorVariations[0];
            }
        }
    }
    void SetUpCombatStats()
    {
        DG_TooltipModule Desc = GetModuleByType(ToolTipModules.InventoryStat);
        Desc.TurnOFFSubs();
        DG_ItemObject IO = QuickFind.ItemDatabase.GetItemFromID(ActiveItemObject.ContextID);
        if (IO == null) Debug.Log("trying to hoverover an edible item that has no context ID, be sure to set the context ID to the Item database ID");

        for (int i = 0; i < IO.WeaponValues.Length; i++)
        {
            DG_ItemObject.Weapon Weapon = IO.WeaponValues[i];
            DG_TooltipSubItem Sub = Desc.GetSubItem(i);


            Sub.NumberObject.text = Weapon.DamageValues.DamageMin.ToString() + "-" + Weapon.DamageValues.DamageMax.ToString();


            DG_ItemsDatabase.GenericIconDatabaseItem ICD = null;
            if(Weapon.DamageValues.DamageType == DG_CombatHandler.DamageTypes.Slashing)
                ICD = QuickFind.ItemDatabase.GetGenericIconByString("Slashing");

            Sub.DisplayImage.sprite = ICD.Icon;
            Sub.TextObject.text = QuickFind.WordDatabase.GetWordFromID(ICD.LocalizationID);
            Sub.DisplayImage.color = Color.white;
        }
    }





    void SetCorrectActiveModules()
    {
        for (int i = 0; i < Modules.Count; i++)
        {
            if (ActiveToolTipGroup.ContainsModule(Modules[i].ModuleType))
                Modules[i].gameObject.SetActive(true);
            else Modules[i].gameObject.SetActive(false);
        }
    }

    DG_TooltipModule GetModuleByType(ToolTipModules M)
    { for (int i = 0; i < Modules.Count; i++) { if (Modules[i].ModuleType == M) return Modules[i]; } return null; }

    ToolTipGroup GetGroupByEnum(ToolTipGroups ToolTipType)
    { for(int i = 0; i < ToolTipTypes.Length; i++) { if(ToolTipTypes[i].GroupType == ToolTipType) return ToolTipTypes[i];} return null; }

















    void SetQualityHotbarPosition()
    {
        if (DisplayTooltip && HoveredInventoryItem != null)
        {
            DG_ItemObject IO = QuickFind.ItemDatabase.GetItemFromID(ActiveRucksackSlot.ContainedItem);
            if(QuickFind.GUI_Inventory.isFloatingInventoryItem)
                IsQualitySelection = false;
            if (IO.MaxStackSize < 2 || QuickFind.GUI_Inventory.isFloatingInventoryItem)
            { FloatingQualitySelect.position = new Vector3(8000, 0, 0);  return; }

            IsQualitySelection = true;
            RectTransform InventoryItem = HoveredInventoryItem.GetComponent<RectTransform>();
            FloatingQualitySelect.position = InventoryItem.position;
        }
    }


    void SetQualityLevelStars()
    {
        for (int i = 0; i < FloatingQualityGrid.childCount; i++)
        {
            DG_InventoryItem GuiSlot = FloatingQualityGrid.GetChild(i).GetComponent<DG_InventoryItem>();
            GuiSlot.QualityLevelOverlay.enabled = true;
            DG_ItemsDatabase.GenericIconDatabaseItem ICD = QuickFind.ItemDatabase.GetGenericIconByString("QualityMarker");
            GuiSlot.QualityLevelOverlay.sprite = ICD.Icon;
            GuiSlot.QualityLevelOverlay.color = ICD.ColorVariations[i];
        }
    }

    void GenerateQualitySelectionGrid()
    {
        if (HoveredInventoryItem == null) return;

        for (int i = 0; i < FloatingQualityGrid.childCount; i++)
        {
            DG_InventoryItem GuiSlot = FloatingQualityGrid.GetChild(i).GetComponent<DG_InventoryItem>();
            SetValue(GuiSlot, ActiveRucksackSlot.GetNumberOfQuality((DG_ItemObject.ItemQualityLevels)i));
            if (ActiveRucksackSlot.CurrentStackActive == i)
                GuiSlot.ActiveHotbarItem.enabled = true;
            else
                GuiSlot.ActiveHotbarItem.enabled = false;
        }
    }
    void SetValue(DG_InventoryItem GuiSlot, int Amount)
    {
        if (Amount == 0)
        {
            GuiSlot.Icon.sprite = QuickFind.GUI_Inventory.DefaultNullSprite;
            GuiSlot.QualityAmountText.text = string.Empty;
        }
            else
        {
            GuiSlot.Icon.sprite = HoveredInventoryItem.Icon.sprite;
            GuiSlot.QualityAmountText.text = Amount.ToString();
        }
    }
}
