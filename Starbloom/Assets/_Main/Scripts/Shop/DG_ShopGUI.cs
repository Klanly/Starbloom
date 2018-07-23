using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_ShopGUI : MonoBehaviour {

    [System.Serializable]
    public class PlayerShopGUI
    {
        [Header("Grid")]
        public RectTransform DisplayGrid = null;
        public DG_UICustomGridScroll GridScroll;
        [Header("Canvases")]
        public CanvasGroup UICanvas = null;
        public UnityEngine.UI.GraphicRaycaster Raycaster;
        [System.NonSerialized] public bool ShopUIisOpen = false;
    }

    public PlayerShopGUI[] ShopGuis;

    private void Awake()
    {
        QuickFind.ShopGUI = this;
    }

    private void Start()
    {
        QuickFind.EnableCanvas(ShopGuis[0].UICanvas, false, ShopGuis[0].Raycaster);
        QuickFind.EnableCanvas(ShopGuis[1].UICanvas, false, ShopGuis[1].Raycaster);
        transform.localPosition = Vector3.zero;
    }



    public void OpenShopUI(int ShopID, int PlayerID)
    {
        int ArrayNum = 0;
        if (PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) ArrayNum = 1;

        ShopGuis[ArrayNum].GridScroll.ResetGrid();
        LoadShopData(ShopID, PlayerID);

        ShopGuis[ArrayNum].ShopUIisOpen = true;
        QuickFind.EnableCanvas(ShopGuis[ArrayNum].UICanvas, true, ShopGuis[ArrayNum].Raycaster);
        QuickFind.GUI_Inventory.OpenShopUI(PlayerID);
    }

    public void ClosePlayer1Shop()
    {
        ShopGuis[0].ShopUIisOpen = false;
        QuickFind.EnableCanvas(ShopGuis[0].UICanvas, false, ShopGuis[0].Raycaster);
        QuickFind.GUI_Inventory.CloseShopUI(QuickFind.NetworkSync.Player1PlayerCharacter);
    }
    public void ClosePlayer2Shop()
    {
        ShopGuis[1].ShopUIisOpen = false;
        QuickFind.EnableCanvas(ShopGuis[1].UICanvas, false, ShopGuis[1].Raycaster);
        QuickFind.GUI_Inventory.CloseShopUI(QuickFind.NetworkSync.Player2PlayerCharacter);
    }



    void LoadShopData(int ShopID, int PlayerID)
    {
        int ArrayNum = 0;
        bool Player1 = true;
        if (PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) { ArrayNum = 1; Player1 = false; }

        PlayerShopGUI PSG = ShopGuis[ArrayNum];

        //Load Data from Atlas.
        DG_ShopAtlasObject SAO = QuickFind.ShopAtlas.GetItemFromID(ShopID);
        WeatherHandler.Seasons CurrentSeason = QuickFind.WeatherHandler.CurrentSeason;

        int index = 0;
        int GridCount = PSG.DisplayGrid.childCount;

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
                        SGI = PSG.DisplayGrid.GetChild(index).GetComponent<DG_ShopGuiItem>();
                        SGI.gameObject.SetActive(true);
                    }
                    else
                    {
                        Transform New = Instantiate(PSG.DisplayGrid.GetChild(0));
                        New.SetParent(PSG.DisplayGrid);
                        SGI = New.GetComponent<DG_ShopGuiItem>();
                        SGI.gameObject.SetActive(true);
                        GridCount++;
                    }

                    SGI.SeasonalGoodsRef = SG;
                    SGI.UpdateVisuals(Player1);
                    index++;
                }
            }
        }

        if (index < (GridCount - 1))
        {
            for (int i = index; i < GridCount; i++)
                PSG.DisplayGrid.GetChild(i).gameObject.SetActive(false);
        }
    }







    public void ShopItemPressed(DG_ShopGuiItem ShopItem)
    {
        int ArrayNum = 0;
        if (!ShopItem.Player1) ArrayNum = 1;

        if (QuickFind.GUI_Inventory.PlayersInventory[ArrayNum].isFloatingInventoryItem) return;

        int PlayerID = QuickFind.NetworkSync.Player1PlayerCharacter;
        if (!ShopItem.Player1) PlayerID = QuickFind.NetworkSync.Player2PlayerCharacter;

        if (!QuickFind.InventoryManager.AddItemToRucksack(PlayerID, ShopItem.SeasonalGoodsRef.ItemDatabaseRef, (DG_ItemObject.ItemQualityLevels)ShopItem.SeasonalGoodsRef.QualityLevel, false, true)) return;
        if (!QuickFind.MoneyHandler.CheckIfSubtractMoney(QuickFind.ItemDatabase.GetItemFromID(ShopItem.SeasonalGoodsRef.ItemDatabaseRef).GetBuyPriceByQuality(ShopItem.SeasonalGoodsRef.QualityLevel))) return;
        MakePurchase(ShopItem);
    }
    void MakePurchase(DG_ShopGuiItem ShopItem)
    {
        int PlayerID = QuickFind.NetworkSync.Player1PlayerCharacter;
        if (!ShopItem.Player1) PlayerID = QuickFind.NetworkSync.Player2PlayerCharacter;

        DG_ItemObject IO = QuickFind.ItemDatabase.GetItemFromID(ShopItem.SeasonalGoodsRef.ItemDatabaseRef);
        QuickFind.MoneyHandler.TrySubtractMoney(IO.GetBuyPriceByQuality(ShopItem.SeasonalGoodsRef.QualityLevel));
        QuickFind.InventoryManager.AddItemToRucksack(PlayerID,
                                                    ShopItem.SeasonalGoodsRef.ItemDatabaseRef,
                                                    (DG_ItemObject.ItemQualityLevels)ShopItem.SeasonalGoodsRef.QualityLevel,
                                                    false);
    }
}
