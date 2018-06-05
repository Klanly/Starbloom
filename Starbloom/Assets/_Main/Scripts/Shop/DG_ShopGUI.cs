using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_ShopGUI : MonoBehaviour {



    [Header("Canvases")]
    public CanvasGroup UICanvas = null;
    public bool ShopUIisOpen = false;


    private void Awake()
    {
        QuickFind.ShopGUI = this;
    }

    private void Start()
    {
        QuickFind.EnableCanvas(UICanvas, false);
        transform.localPosition = Vector3.zero;
    }




    public void OpenShopUI()
    {
        QuickFind.EnableCanvas(UICanvas, true);
        QuickFind.GUI_Inventory.OpenShopUI();
    }

    public void CloseStorageUI()
    {
        QuickFind.EnableCanvas(UICanvas, false);
        QuickFind.GUI_Inventory.CloseShopUI();
    }
}
