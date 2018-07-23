using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_ShippingBinDropPanel : MonoBehaviour {

    public bool isPlayer1;

    public void ItemPressed()
    {
        if (Input.GetMouseButtonDown(1)) return;

        QuickFind.ShippingBinGUI.FloatingItemDropped(isPlayer1);
    }

    public void DragReleased()
    {
        ItemPressed();
    }
}
