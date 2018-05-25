using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_TooltipModule : MonoBehaviour {

    public DG_TooltipGUI.ToolTipModules ModuleType;
    public TMPro.TextMeshProUGUI TextObject;


    public bool isActive = true;

    public void StretchHeightToBounds()
    {
        if (TextObject == null)
            return;

        RectTransform Rect = this.GetComponent<RectTransform>();
        Vector2 CurSize = Rect.sizeDelta;
        float TextHeight = TextObject.renderedHeight;
        CurSize.y = TextHeight;
        Rect.sizeDelta = CurSize;
    }

}
