using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_CraftingGUI : MonoBehaviour {

    [Header("Canvases")]
    public CanvasGroup UICanvas = null;
    [Header("References")]
    public Transform CraftGrid = null;
    [Header("Coloring")]
    public Color AbleToCraftColor;
    public Color InableToCraftColor;

    [Header("Debug")]
    public bool CanCraft;


    private void Awake() { QuickFind.GUI_Crafting = this; }
    private void Start() { QuickFind.EnableCanvas(UICanvas, false); transform.localPosition = Vector3.zero; }

    [HideInInspector] public DG_CraftButton CurrentHoverItem = null;


    public void OpenUI()
    {
        QuickFind.GUI_OverviewTabs.CloseAllTabs();
        QuickFind.EnableCanvas(UICanvas, true);
        LoadCraftingGUI();
    }
    public void CloseUI()
    {
        QuickFind.EnableCanvas(UICanvas, false);
    }


    void LoadCraftingGUI()
    {
        DG_CraftingDictionaryItem[] CraftingDictionary = QuickFind.CraftingDictionary.ItemCatagoryList;
        int[] PlayerKnownCrafts = QuickFind.Farm.PlayerCharacters[QuickFind.NetworkSync.PlayerCharacterID].CraftsDiscovered;

        int index = 0;
        int GUIChildCount = CraftGrid.childCount;

        for (int i = 0; i < CraftingDictionary.Length; i++)
        {
            if (PlayerKnownCrafts[i] > 0)
            {
                DG_CraftingDictionaryItem CDI = CraftingDictionary[i];

                DG_CraftButton CB = null;
                if (index < GUIChildCount)
                    CB = CraftGrid.GetChild(index).GetComponent<DG_CraftButton>();
                else
                {
                    Transform NewChild = Instantiate(CraftGrid.GetChild(0));
                    NewChild.SetParent(CraftGrid);
                    CB = NewChild.GetComponent<DG_CraftButton>();
                    GUIChildCount++;
                }

                index++;
                CB.CraftDatabaseID = CDI.DatabaseID;
                DG_ItemObject IO = QuickFind.ItemDatabase.GetItemFromID(CDI.ItemCreatedRef);
                CB.Icon.sprite = IO.Icon;
                if (InventoryContainsIngredients(CDI))
                {
                    CB.Icon.color = AbleToCraftColor;
                    CB.AbleToCraft = true;
                }
                else
                {
                    CB.Icon.color = InableToCraftColor;
                    CB.AbleToCraft = false;
                }
            }
        }
    }

    bool InventoryContainsIngredients(DG_CraftingDictionaryItem CDI)
    {
        for(int i = 0; i < CDI.IngredientList.Length; i++)
        {
            DG_CraftingDictionaryItem.Ingredient Ingredient = CDI.IngredientList[i];
            if (QuickFind.InventoryManager.TotalInventoryCountOfItem(Ingredient.ItemDatabaseRef) < Ingredient.Value)
                return false;
        }
        return true;
    }









    public void CraftButtonPressed(DG_CraftButton CraftButton)
    {
        int CraftDatabaseID = CraftButton.CraftDatabaseID;
        DG_CraftingDictionaryItem CDI = QuickFind.CraftingDictionary.GetItemFromID(CraftDatabaseID);

        //Check if Room First, before allowing player to derp themselves.
        if (!QuickFind.InventoryManager.AddItemToRucksack(QuickFind.NetworkSync.PlayerCharacterID, CDI.ItemCreatedRef, DG_ItemObject.ItemQualityLevels.Low, false, true)) return;


        for (int i = 0; i < CDI.IngredientList.Length; i++)
        {
            DG_CraftingDictionaryItem.Ingredient Ingredient = CDI.IngredientList[i];
            QuickFind.InventoryManager.SubtractNumberOfItemFromRucksack(Ingredient.ItemDatabaseRef, Ingredient.Value);
        }

        QuickFind.InventoryManager.AddItemToRucksack(QuickFind.NetworkSync.PlayerCharacterID, CDI.ItemCreatedRef, DG_ItemObject.ItemQualityLevels.Low, false, false);


        if (InventoryContainsIngredients(CDI))
        {
            CraftButton.Icon.color = AbleToCraftColor;
            CraftButton.AbleToCraft = true;
        }
        else
        {
            CraftButton.Icon.color = InableToCraftColor;
            CraftButton.AbleToCraft = false;
        }
    }
}
