using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_ShopGUI : MonoBehaviour {


    [Header("Grid")]
    public RectTransform DisplayGrid = null;
    public DG_UICustomGridScroll GridScroll;
    [Header("Canvases")]
    public CanvasGroup UICanvas = null;
    [HideInInspector] public bool ShopUIisOpen = false;




    private void Awake()
    {
        QuickFind.ShopGUI = this;
    }

    private void Start()
    {
        QuickFind.EnableCanvas(UICanvas, false);
        transform.localPosition = Vector3.zero;
    }



    public void OpenShopUI(int ShopID)
    {
        GridScroll.ResetGrid();
        LoadShopData(ShopID);

        ShopUIisOpen = true;
        QuickFind.EnableCanvas(UICanvas, true);
        QuickFind.GUI_Inventory.OpenShopUI();
    }
    public void CloseShopUI()
    {
        ShopUIisOpen = false;
        QuickFind.EnableCanvas(UICanvas, false);
        QuickFind.GUI_Inventory.CloseShopUI();
    }



    void LoadShopData(int ShopID)
    {
        //Load Data from Atlas.
        DG_ShopAtlasObject SAO = QuickFind.ShopAtlas.GetItemFromID(ShopID);
        WeatherHandler.Seasons CurrentSeason = QuickFind.WeatherHandler.CurrentSeason;

        int index = 0;
        int GridCount = DisplayGrid.childCount;

        for (int i = 0; i < SAO.Seasons.Length; i++)
        {
            DG_ShopAtlasObject.SeasonalSelection SS = SAO.Seasons[i];
            if (SS.Season == CurrentSeason || SS.Season == WeatherHandler.Seasons.All)
            {
                for (int iN = 0; iN < SS.Items.Length; iN++)
                {
                    DG_ShopAtlasObject.SeasonalGood SG = SS.Items[iN];

                    DG_ShopGuiItem SGI;
                    if (index < GridCount)
                    {
                        SGI = DisplayGrid.GetChild(index).GetComponent<DG_ShopGuiItem>();
                        SGI.gameObject.SetActive(true);
                    }
                    else
                    {
                        Transform New = Instantiate(DisplayGrid.GetChild(0));
                        New.SetParent(DisplayGrid);
                        SGI = New.GetComponent<DG_ShopGuiItem>();
                        SGI.gameObject.SetActive(true);
                        GridCount++;
                    }

                    SGI.SeasonalGoodsRef = SG;
                    SGI.UpdateVisuals();
                    index++;
                }
            }
        }

        if (index < (GridCount - 1))
        {
            for (int i = index; i < GridCount; i++)
                DisplayGrid.GetChild(i).gameObject.SetActive(false);
        }
    }







    public void ShopItemPressed(DG_ShopGuiItem ShopItem)
    {
        if (QuickFind.GUI_Inventory.isFloatingInventoryItem) return;

        if (!QuickFind.InventoryManager.AddItemToRucksack(QuickFind.NetworkSync.PlayerCharacterID, ShopItem.SeasonalGoodsRef.ItemDatabaseRef, (DG_ItemObject.ItemQualityLevels)ShopItem.SeasonalGoodsRef.QualityLevel, false, true)) return;
        if (!QuickFind.MoneyHandler.CheckIfSubtractMoney(QuickFind.ItemDatabase.GetItemFromID(ShopItem.SeasonalGoodsRef.ItemDatabaseRef).GetBuyPriceByQuality(ShopItem.SeasonalGoodsRef.QualityLevel))) return;
        MakePurchase(ShopItem);
    }
    void MakePurchase(DG_ShopGuiItem ShopItem)
    {
        DG_ItemObject IO = QuickFind.ItemDatabase.GetItemFromID(ShopItem.SeasonalGoodsRef.ItemDatabaseRef);
        QuickFind.MoneyHandler.TrySubtractMoney(IO.GetBuyPriceByQuality(ShopItem.SeasonalGoodsRef.QualityLevel));
        QuickFind.InventoryManager.AddItemToRucksack(QuickFind.NetworkSync.PlayerCharacterID,
                                                    ShopItem.SeasonalGoodsRef.ItemDatabaseRef,
                                                    (DG_ItemObject.ItemQualityLevels)ShopItem.SeasonalGoodsRef.QualityLevel,
                                                    false);
    }
}
