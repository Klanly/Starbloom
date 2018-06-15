using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_ShippingBinDropPanel : MonoBehaviour {


    public void ItemPressed()
    {
        if (Input.GetMouseButtonDown(1)) return;

        QuickFind.ShippingBinGUI.FloatingItemDropped();
    }

    public void DragReleased()
    {
        ItemPressed();
    }
}
