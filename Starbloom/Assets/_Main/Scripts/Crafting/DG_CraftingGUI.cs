using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_CraftingGUI : MonoBehaviour {

    [Header("Canvases")]
    public CanvasGroup UICanvas = null;



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


        for (int i = 0; i < CraftingDictionary.Length; i++)
        {
            if (PlayerKnownCrafts[i] > 0)
                Debug.Log("Activate, or Spawn Craft Button");
        }
    }



    public void CraftButtonPressed()
    {

    }


}
