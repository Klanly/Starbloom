using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

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
        Equipement,
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
        UIElement
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









    [Header("Floating Inventory Item")]
    public RectTransform FloatingRect;
    public RectTransform VerticalGridRect;
    public UnityEngine.UI.ContentSizeFitter CSF;
    [ListDrawerSettings(ListElementLabelName = "GroupType", NumberOfItemsPerPage = 16, Expanded = false)]
    public ToolTipGroup[] ToolTipTypes;


    [Header("Debug")]
    public bool DebugON = false;



    bool DisplayTooltip = false;
    float DefaultX;
    List<DG_TooltipModule> Modules;
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
        FloatingRect.position = new Vector3(8000, 0, 0);
        this.enabled = false;
    }
    public void ShowToolTip(ToolTipContainerItem TTContainer)
    {
        if (QuickFind.GUI_Inventory.isFloatingInventoryItem) return;
        ActiveItemObject = TTContainer;
        ActiveToolTipGroup = GetGroupByEnum(TTContainer.GroupType);
        GenerateToolTip();
        DisplayTooltip = true;
        this.enabled = true;
    }
    public void HideToolTip()
    {
        if (QuickFind.GUI_Inventory.isFloatingInventoryItem) return;
        DisplayTooltip = false;
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
                if (Module.isActive)
                {
                    Height = Height + Module.GetComponent<RectTransform>().rect.height;
                    Module.StretchHeightToBounds();
                }
            }
            float OffsetHeight = 0 - (Height / 2); if (!TooltipBelow) OffsetHeight = -OffsetHeight;
            float OffsetWidth = DefaultX; if (MouseRightSide) OffsetWidth = -DefaultX;

            Vector3 CurrentPos = VerticalGridRect.localPosition; CurrentPos.x = OffsetWidth; CurrentPos.y = OffsetHeight;
            VerticalGridRect.localPosition = CurrentPos;
            //
            CSF.enabled = false; CSF.enabled = true;
        }
        else
            { FloatingRect.position = new Vector3(8000, 0, 0); this.enabled = false; }
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
        

        //Desc.TextObject.text = QuickFind.WordDatabase.GetWordFromID(ActiveItemObject.DescriptionID);
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
}
