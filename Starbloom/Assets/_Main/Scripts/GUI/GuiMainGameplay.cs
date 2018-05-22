using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuiMainGameplay : MonoBehaviour {


    [Header("Inventory Hotbar")]
    public Transform InventoryHotbarGrid = null;

    [Header("Canvases")]
    public CanvasGroup UICanvas = null;
    public CanvasGroup InventoryHotbar = null;



    private void Awake()
    {
        QuickFind.GUI_MainOverview = this;
    }



    private void Start()
    {
        QuickFind.EnableCanvas(UICanvas, false);
        transform.localPosition = Vector3.zero;
    }


    public void OpenUI(bool UIisOpen)
    {
        QuickFind.EnableCanvas(UICanvas, UIisOpen);
    }




    public void SetGuiDayValue(int Year, int Month, int Day)
    {
        
    }

    public void SetGuiMoneyValue(int NewMoneyValueToDisplay)
    {
        
    }

    public void SetGuiEnergyValue(int NewEnergyValue)
    {

    }

}
