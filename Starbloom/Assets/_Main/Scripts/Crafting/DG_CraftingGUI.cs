using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_CraftingGUI : MonoBehaviour {


    [System.Serializable]
    public class PlayerCraftingGUI
    {
        [Header("Canvases")]
        public CanvasGroup UICanvas = null;
        public UnityEngine.UI.GraphicRaycaster Raycaster;
        [Header("References")]
        public Transform CraftGrid = null;
    }

    public PlayerCraftingGUI[] CraftingGuis;

    [Header("Coloring")]
    public Color AbleToCraftColor;
    public Color InableToCraftColor;




    private void Awake() { QuickFind.GUI_Crafting = this; }
    private void Start() { QuickFind.EnableCanvas(CraftingGuis[0].UICanvas, false, CraftingGuis[0].Raycaster); QuickFind.EnableCanvas(CraftingGuis[1].UICanvas, false, CraftingGuis[1].Raycaster); transform.localPosition = Vector3.zero; }

    [System.NonSerialized] public DG_CraftButton CurrentHoverItem = null;


    public void OpenUI(int PlayerID)
    {
        int ArrayNum = 0;
        if (PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) ArrayNum = 1;

        QuickFind.GUI_OverviewTabs.CloseAllTabs(ArrayNum, PlayerID);
        QuickFind.EnableCanvas(CraftingGuis[ArrayNum].UICanvas, true, CraftingGuis[ArrayNum].Raycaster);
        LoadCraftingGUI(PlayerID);
    }
    public void CloseUI(int PlayerID)
    {
        int ArrayNum = 0;
        if (PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) ArrayNum = 1;
        QuickFind.EnableCanvas(CraftingGuis[ArrayNum].UICanvas, false, CraftingGuis[ArrayNum].Raycaster);
    }


    void LoadCraftingGUI(int PlayerID)
    {
        int ArrayNum = 0;
        if (PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) ArrayNum = 1;


        DG_CraftingDictionaryItem[] CraftingDictionary = QuickFind.CraftingDictionary.ItemCatagoryList;
        int[] PlayerKnownCrafts = QuickFind.Farm.PlayerCharacters[PlayerID].CraftsDiscovered;

        int index = 0;
        int GUIChildCount = CraftingGuis[ArrayNum].CraftGrid.childCount;

        for (int i = 0; i < CraftingDictionary.Length; i++)
        {
            if (PlayerKnownCrafts[i] > 0)
            {
                DG_CraftingDictionaryItem CDI = CraftingDictionary[i];

                DG_CraftButton CB = null;
                if (index < GUIChildCount)
                    CB = CraftingGuis[ArrayNum].CraftGrid.GetChild(index).GetComponent<DG_CraftButton>();
                else
                {
                    Transform NewChild = Instantiate(CraftingGuis[ArrayNum].CraftGrid.GetChild(0));
                    NewChild.SetParent(CraftingGuis[ArrayNum].CraftGrid);
                    CB = NewChild.GetComponent<DG_CraftButton>();
                    GUIChildCount++;
                }

                index++;
                CB.CraftDatabaseID = CDI.DatabaseID;
                CB.PlayerID = PlayerID;
                DG_ItemObject IO = QuickFind.ItemDatabase.GetItemFromID(CDI.ItemCreatedRef);
                CB.Icon.sprite = IO.Icon;
                if (InventoryContainsIngredients(CDI, PlayerID))
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

    bool InventoryContainsIngredients(DG_CraftingDictionaryItem CDI, int PlayerID)
    {
        for(int i = 0; i < CDI.IngredientList.Length; i++)
        {
            DG_CraftingDictionaryItem.Ingredient Ingredient = CDI.IngredientList[i];
            if (QuickFind.InventoryManager.TotalInventoryCountOfItem(Ingredient.ItemDatabaseRef, PlayerID) < Ingredient.Value)
                return false;
        }
        return true;
    }









    public void CraftButtonPressed(DG_CraftButton CraftButton, int PlayerID)
    {
        int CraftDatabaseID = CraftButton.CraftDatabaseID;
        DG_CraftingDictionaryItem CDI = QuickFind.CraftingDictionary.GetItemFromID(CraftDatabaseID);

        //Check if Room First, before allowing player to derp themselves.
        if (!QuickFind.InventoryManager.AddItemToRucksack(CraftButton.PlayerID, CDI.ItemCreatedRef, DG_ItemObject.ItemQualityLevels.Low, false, true)) return;


        for (int i = 0; i < CDI.IngredientList.Length; i++)
        {
            DG_CraftingDictionaryItem.Ingredient Ingredient = CDI.IngredientList[i];
            QuickFind.InventoryManager.SubtractNumberOfItemFromRucksack(Ingredient.ItemDatabaseRef, Ingredient.Value, PlayerID);
        }

        QuickFind.InventoryManager.AddItemToRucksack(CraftButton.PlayerID, CDI.ItemCreatedRef, DG_ItemObject.ItemQualityLevels.Low, false, false);


        if (InventoryContainsIngredients(CDI, PlayerID))
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
