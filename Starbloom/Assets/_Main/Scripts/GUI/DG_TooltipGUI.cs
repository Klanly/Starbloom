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




    [System.Serializable]
    public class PlayerTooltipGUI
    {
        [System.NonSerialized] public bool Enabled;
        [Header("Floating QualitySelection")]
        public RectTransform FloatingQualitySelect;
        public RectTransform FloatingQualityGrid;
        [Header("Floating Inventory Item")]
        public RectTransform FloatingRect;
        public RectTransform VerticalGridRect;
        public UnityEngine.UI.ContentSizeFitter CSF;

        [System.NonSerialized] public bool DisplayTooltip = false;
        [System.NonSerialized] public bool IsQualitySelection = false;

        //Context Per UI Type
        [System.NonSerialized] public DG_InventoryItem HoveredInventoryItem;
        [System.NonSerialized] public DG_PlayerCharacters.RucksackSlot ActiveRucksackSlot;
        [System.NonSerialized] public ToolTipGroup ActiveToolTipGroup;
        [System.NonSerialized] public ToolTipContainerItem ActiveItemObject;
        [System.NonSerialized] public float DefaultX;

        [System.NonSerialized] public List<DG_TooltipModule> Modules;
    }

    public PlayerTooltipGUI[] TooltipGuis;

    [ListDrawerSettings(ListElementLabelName = "GroupType", NumberOfItemsPerPage = 16, Expanded = false)]
    public ToolTipGroup[] ToolTipTypes;

    [Header("Debug")]
    public bool DebugON = false;







    private void Awake()
    {
        QuickFind.TooltipHandler = this;

    }
    private void Start()
    {
        for (int iN = 0; iN < TooltipGuis.Length; iN++)
            SetQualityLevelStars(iN);

        transform.localPosition = Vector3.zero;

        for (int iN = 0; iN < TooltipGuis.Length; iN++)
        {
            PlayerTooltipGUI PTG = TooltipGuis[iN];

            PTG.DefaultX = PTG.VerticalGridRect.localPosition.x;
            PTG.Modules = new List<DG_TooltipModule>();
            for (int i = 0; i < PTG.VerticalGridRect.childCount; i++)
                PTG.Modules.Add(PTG.VerticalGridRect.GetChild(i).GetComponent<DG_TooltipModule>());
            PTG.FloatingQualitySelect.localPosition = new Vector3(8000, 0, 0);
            PTG.FloatingRect.localPosition = new Vector3(8000, 0, 0);
        }
        this.enabled = false;
    }
    public void ShowToolTip(ToolTipContainerItem TTContainer, int PlayerID)
    {
        int ArrayNum = 0;
        if (PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) ArrayNum = 1;

        if (QuickFind.GUI_Inventory.PlayersInventory[ArrayNum].isFloatingInventoryItem) return;
        TooltipGuis[ArrayNum].ActiveItemObject = TTContainer;
        TooltipGuis[ArrayNum].ActiveToolTipGroup = GetGroupByEnum(TTContainer.GroupType);
        GenerateToolTip(PlayerID);
        GenerateQualitySelectionGrid(PlayerID);
        TooltipGuis[ArrayNum].DisplayTooltip = true;
        TooltipGuis[ArrayNum].Enabled = true;
        this.enabled = true;
    }
    public void HideToolTip(int PlayerID)
    {
        int ArrayNum = 0;
        if (PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) ArrayNum = 1;

        TooltipGuis[ArrayNum].HoveredInventoryItem = null;
        if (QuickFind.GUI_Inventory.PlayersInventory[ArrayNum].isFloatingInventoryItem) return;
        TooltipGuis[ArrayNum].FloatingQualitySelect.position = new Vector3(8000, 0, 0);
        TooltipGuis[ArrayNum].IsQualitySelection = false;
        TooltipGuis[ArrayNum].DisplayTooltip = false;
    }

    public void UpdateEquippedNum(int isUP, bool CanLoop, int PlayerID)
    {
        int ArrayNum = 0;
        if (PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) ArrayNum = 1;

        AddRucksack(isUP, CanLoop, TooltipGuis[ArrayNum].ActiveRucksackSlot.CurrentStackActive, PlayerID);

        DG_PlayerCharacters.CharacterEquipment Equipment = QuickFind.Farm.PlayerCharacters[PlayerID].Equipment;
        QuickFind.GUI_Inventory.UpdateRucksackSlotVisual(TooltipGuis[ArrayNum].HoveredInventoryItem, TooltipGuis[ArrayNum].ActiveRucksackSlot, PlayerID);
        GenerateQualitySelectionGrid(PlayerID);
        GenerateToolTip(PlayerID);
        if (TooltipGuis[ArrayNum].HoveredInventoryItem.SlotID < 12)
            QuickFind.GUI_Inventory.UpdateMirrorSlot(TooltipGuis[ArrayNum].HoveredInventoryItem.SlotID, ArrayNum);
    }
    void AddRucksack(int isUP, bool CanLoop, int OriginalValue, int PlayerID)
    {
        int ArrayNum = 0;
        if (PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) ArrayNum = 1;

        bool PreventInfinite = true;
        TooltipGuis[ArrayNum].ActiveRucksackSlot.CurrentStackActive += isUP;
        if (TooltipGuis[ArrayNum].ActiveRucksackSlot.CurrentStackActive > 3)
            { if (CanLoop) TooltipGuis[ArrayNum].ActiveRucksackSlot.CurrentStackActive = 0; else { TooltipGuis[ArrayNum].ActiveRucksackSlot.CurrentStackActive = OriginalValue; PreventInfinite = false; } }
        if (TooltipGuis[ArrayNum].ActiveRucksackSlot.CurrentStackActive < 0)
            { if (CanLoop) TooltipGuis[ArrayNum].ActiveRucksackSlot.CurrentStackActive = 3; else { TooltipGuis[ArrayNum].ActiveRucksackSlot.CurrentStackActive = OriginalValue; PreventInfinite = false; } }
        if (TooltipGuis[ArrayNum].ActiveRucksackSlot.GetNumberOfQuality((DG_ItemObject.ItemQualityLevels)TooltipGuis[ArrayNum].ActiveRucksackSlot.CurrentStackActive) == 0 && PreventInfinite)
            AddRucksack(isUP, CanLoop, OriginalValue, PlayerID);
    }



    private void Update()
    {
        for (int iN = 0; iN < TooltipGuis.Length; iN++)
        {
            if (QuickFind.InputController.Players[iN].CharLink == null) continue;

            PlayerTooltipGUI PTG = TooltipGuis[iN];

            if (PTG.DisplayTooltip || DebugON)
            {
                float Height = 0; bool TooltipBelow = false; bool MouseRightSide = false;

                Vector2 MousePos = Input.mousePosition; if (DebugON) { MousePos.x = 800; MousePos.y = 600; }
                PTG.FloatingRect.position = MousePos;
                if (MousePos.y >= Screen.height * 0.2) TooltipBelow = true;
                if (MousePos.x >= Screen.width * 0.8) MouseRightSide = true;
                for (int i = 0; i < PTG.Modules.Count; i++)
                {
                    DG_TooltipModule Module = PTG.Modules[i];
                    if (Module.isActive) { Height = Height + Module.GetComponent<RectTransform>().rect.height; Module.StretchHeightToBounds(); }
                }
                float OffsetHeight = 0 - (Height / 2); if (!TooltipBelow) OffsetHeight = -OffsetHeight;
                float OffsetWidth = PTG.DefaultX; if (MouseRightSide) OffsetWidth = -PTG.DefaultX;

                Vector3 CurrentPos = PTG.VerticalGridRect.localPosition; CurrentPos.x = OffsetWidth; CurrentPos.y = OffsetHeight;
                PTG.VerticalGridRect.localPosition = CurrentPos;
                //
                PTG.CSF.enabled = false; PTG.CSF.enabled = true;
            }
            else
            {
                PTG.Enabled = false; PTG.FloatingRect.position = new Vector3(8000, 0, 0);
            }

            int PlayerID = QuickFind.NetworkSync.Player1PlayerCharacter;
            if(iN == 1) PlayerID = QuickFind.NetworkSync.Player2PlayerCharacter;

            SetQualityHotbarPosition(PlayerID);
        }

        if (!TooltipGuis[0].Enabled && !TooltipGuis[1].Enabled) this.enabled = false;
    }



    void GenerateToolTip(int PlayerID)
    {
        int ArrayNum = 0;
        if (PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) ArrayNum = 1;

        SetCorrectActiveModules(PlayerID);
        switch(TooltipGuis[ArrayNum].ActiveToolTipGroup.GroupType)
        {
            case ToolTipGroups.NonEdibleItem:
                {
                    SetMain(PlayerID);
                    SetSub(PlayerID);
                    SetDescription(PlayerID);
                }
                break;
            case ToolTipGroups.Tool:
                {
                    SetMain(PlayerID);
                    SetSub(PlayerID);
                    SetDescription(PlayerID);
                }
                break;
            case ToolTipGroups.EdibleItem:
                {
                    SetMain(PlayerID);
                    SetSub(PlayerID);
                    SetDescription(PlayerID);
                    SetUpInventoryStats(PlayerID);
                }
                break;
            case ToolTipGroups.Craft:
                {
                    SetMain(PlayerID);
                    SetSub(PlayerID);
                    SetUpCraftIngredients(PlayerID);
                    SetDescription(PlayerID);
                }
                break;
            case ToolTipGroups.Weapon:
                {
                    SetMain(PlayerID);
                    SetSub(PlayerID);
                    SetDescription(PlayerID);
                    SetUpCombatStats(PlayerID);
                }
                break;
        }
    }

    void SetMain(int PlayerID)
    {
        int ArrayNum = 0;
        if (PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) ArrayNum = 1;

        DG_TooltipModule Main = GetModuleByType(ToolTipModules.Main, TooltipGuis[ArrayNum]);
        Main.TextObject.text = QuickFind.WordDatabase.GetWordFromID(TooltipGuis[ArrayNum].ActiveItemObject.MainLocalizationID);
    }
    void SetSub(int PlayerID)
    {
        int ArrayNum = 0;
        if (PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) ArrayNum = 1;

        DG_TooltipModule Sub = GetModuleByType(ToolTipModules.SubName, TooltipGuis[ArrayNum]);
        Sub.TextObject.text = QuickFind.WordDatabase.GetWordFromID(TooltipGuis[ArrayNum].ActiveItemObject.SubLocalizationID);
    }
    void SetDescription(int PlayerID)
    {
        int ArrayNum = 0;
        if (PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) ArrayNum = 1;

        DG_TooltipModule Desc = GetModuleByType(ToolTipModules.Description, TooltipGuis[ArrayNum]);
        Desc.TextObject.text = QuickFind.WordDatabase.GetWordFromID(TooltipGuis[ArrayNum].ActiveItemObject.DescriptionID);
    }
    void SetUpInventoryStats(int PlayerID)
    {
        int ArrayNum = 0;
        if (PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) ArrayNum = 1;

        DG_TooltipModule Desc = GetModuleByType(ToolTipModules.InventoryStat, TooltipGuis[ArrayNum]);
        Desc.TurnOFFSubs();
        DG_ItemObject IO = QuickFind.ItemDatabase.GetItemFromID(TooltipGuis[ArrayNum].ActiveItemObject.ContextID);
        if (IO == null) Debug.Log("trying to hoverover an edible item that has no context ID, be sure to set the context ID to the Item database ID");
        DG_ItemObject.Item Item = IO.GetItemByQuality(TooltipGuis[ArrayNum].ActiveRucksackSlot.CurrentStackActive);

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
    void SetUpCraftIngredients(int PlayerID)
    {
        int ArrayNum = 0;
        if (PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) ArrayNum = 1;

        DG_TooltipModule Desc = GetModuleByType(ToolTipModules.CraftingIngredient, TooltipGuis[ArrayNum]);
        Desc.TurnOFFSubs();

        //Update Static Ingredients Text based on language.
        Desc.transform.GetChild(0).GetChild(0).GetComponent<DG_TextStatic>().ManualLoad();

        DG_CraftingDictionaryItem CDI = QuickFind.CraftingDictionary.GetItemFromID(TooltipGuis[ArrayNum].ActiveItemObject.ContextID);
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
            if (QuickFind.InventoryManager.TotalInventoryCountOfItem(IngredientObject.DatabaseID, PlayerID) < I.Value)
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
    void SetUpCombatStats(int PlayerID)
    {
        int ArrayNum = 0;
        if (PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) ArrayNum = 1;

        DG_TooltipModule Desc = GetModuleByType(ToolTipModules.InventoryStat, TooltipGuis[ArrayNum]);
        Desc.TurnOFFSubs();
        DG_ItemObject IO = QuickFind.ItemDatabase.GetItemFromID(TooltipGuis[ArrayNum].ActiveItemObject.ContextID);
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





    void SetCorrectActiveModules(int PlayerID)
    {
        int ArrayNum = 0;
        if (PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) ArrayNum = 1;

        PlayerTooltipGUI PTG = TooltipGuis[ArrayNum];

        for (int i = 0; i < PTG.Modules.Count; i++)
        {
            if (TooltipGuis[ArrayNum].ActiveToolTipGroup.ContainsModule(PTG.Modules[i].ModuleType))
                PTG.Modules[i].gameObject.SetActive(true);
            else PTG.Modules[i].gameObject.SetActive(false);
        }
    }

    DG_TooltipModule GetModuleByType(ToolTipModules M, PlayerTooltipGUI PTG)
    { for (int i = 0; i < PTG.Modules.Count; i++) { if (PTG.Modules[i].ModuleType == M) return PTG.Modules[i]; } return null; }

    ToolTipGroup GetGroupByEnum(ToolTipGroups ToolTipType)
    { for(int i = 0; i < ToolTipTypes.Length; i++) { if(ToolTipTypes[i].GroupType == ToolTipType) return ToolTipTypes[i];} return null; }

















    void SetQualityHotbarPosition(int PlayerID)
    {
        int ArrayNum = 0;
        if (PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) ArrayNum = 1;

        if (TooltipGuis[ArrayNum].DisplayTooltip && TooltipGuis[ArrayNum].HoveredInventoryItem != null)
        {
            DG_ItemObject IO = QuickFind.ItemDatabase.GetItemFromID(TooltipGuis[ArrayNum].ActiveRucksackSlot.ContainedItem);
            if(QuickFind.GUI_Inventory.PlayersInventory[ArrayNum].isFloatingInventoryItem)
                TooltipGuis[ArrayNum].IsQualitySelection = false;
            if (IO.MaxStackSize < 2 || QuickFind.GUI_Inventory.PlayersInventory[ArrayNum].isFloatingInventoryItem)
            { TooltipGuis[ArrayNum].FloatingQualitySelect.position = new Vector3(8000, 0, 0);  return; }

            TooltipGuis[ArrayNum].IsQualitySelection = true;
            RectTransform InventoryItem = TooltipGuis[ArrayNum].HoveredInventoryItem.GetComponent<RectTransform>();
            TooltipGuis[ArrayNum].FloatingQualitySelect.position = InventoryItem.position;
        }
    }


    void SetQualityLevelStars(int Index)
    {
        int ArrayNum = 0;
        if (Index == 1) ArrayNum = 1;

        for (int i = 0; i < TooltipGuis[ArrayNum].FloatingQualityGrid.childCount; i++)
        {
            DG_InventoryItem GuiSlot = TooltipGuis[ArrayNum].FloatingQualityGrid.GetChild(i).GetComponent<DG_InventoryItem>();
            GuiSlot.QualityLevelOverlay.enabled = true;
            DG_ItemsDatabase.GenericIconDatabaseItem ICD = QuickFind.ItemDatabase.GetGenericIconByString("QualityMarker");
            GuiSlot.QualityLevelOverlay.sprite = ICD.Icon;
            GuiSlot.QualityLevelOverlay.color = ICD.ColorVariations[i];
        }
    }

    void GenerateQualitySelectionGrid(int PlayerID)
    {
        int ArrayNum = 0;
        if (PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) ArrayNum = 1;

        if (TooltipGuis[ArrayNum].HoveredInventoryItem == null) return;

        for (int i = 0; i < TooltipGuis[ArrayNum].FloatingQualityGrid.childCount; i++)
        {
            DG_InventoryItem GuiSlot = TooltipGuis[ArrayNum].FloatingQualityGrid.GetChild(i).GetComponent<DG_InventoryItem>();
            SetValue(GuiSlot, TooltipGuis[ArrayNum].ActiveRucksackSlot.GetNumberOfQuality((DG_ItemObject.ItemQualityLevels)i), PlayerID);
            if (TooltipGuis[ArrayNum].ActiveRucksackSlot.CurrentStackActive == i)
                GuiSlot.ActiveHotbarItem.enabled = true;
            else
                GuiSlot.ActiveHotbarItem.enabled = false;
        }
    }
    void SetValue(DG_InventoryItem GuiSlot, int Amount, int PlayerID)
    {
        int ArrayNum = 0;
        if (PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) ArrayNum = 1;

        if (Amount == 0)
        {
            GuiSlot.Icon.sprite = QuickFind.GUI_Inventory.DefaultNullSprite;
            GuiSlot.QualityAmountText.text = string.Empty;
        }
        else
        {
            GuiSlot.Icon.sprite = TooltipGuis[ArrayNum].HoveredInventoryItem.Icon.sprite;
            GuiSlot.QualityAmountText.text = Amount.ToString();
        }
    }
}
