using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_ShopHandler : MonoBehaviour {




    private void Awake()
    {
        QuickFind.ShopHandler = this;
    }


    public void LoadShop(int ShopID)
    {
        //Load Data from Atlas.

        QuickFind.ShopGUI.OpenShopUI();
    }
}
