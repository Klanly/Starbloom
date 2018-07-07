using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class DG_TooltipModule : MonoBehaviour {

    [System.NonSerialized] public bool isActive = true;

    public DG_TooltipGUI.ToolTipModules ModuleType;
    public TMPro.TextMeshProUGUI TextObject;
    public bool HasSubItems = false;
    [ShowIf("HasSubItems")]
    public bool SkipFirst = false;
    [ShowIf("HasSubItems")]
    public bool SkipLast = false;

    //[System.NonSerialized]
    public List<DG_TooltipSubItem> SubList;








    private void Awake()
    {
        if (HasSubItems)
            SubList = new List<DG_TooltipSubItem>();
        for (int i = 0; i < transform.childCount; i++)
        {
            if (i == 0 && SkipFirst) continue;
            if (i == transform.childCount - 1 && SkipLast) continue;
            SubList.Add(transform.GetChild(i).GetComponent<DG_TooltipSubItem>());
        }
    }


    public void TurnOFFSubs()
    {
        for (int i = 0; i < SubList.Count; i++) { if(SubList[i] != null) SubList[i].gameObject.SetActive(false); }
    }

    public DG_TooltipSubItem GetSubItem(int index)
    {
        if (index < SubList.Count)
        {
            SubList[index].gameObject.SetActive(true);
            return SubList[index];
        }
        else
            return AddNewSubItem();
    }
    public DG_TooltipSubItem AddNewSubItem()
    {
        GameObject New = Instantiate(SubList[0].gameObject);
        New.transform.SetParent(transform);
        if(SkipLast)
        {
            int CurrentIndex = New.transform.GetSiblingIndex();
            New.transform.SetSiblingIndex(CurrentIndex - 1);
        }
        New.SetActive(true);
        DG_TooltipSubItem ReturnItem = New.GetComponent<DG_TooltipSubItem>();
        SubList.Add(ReturnItem);
        return ReturnItem;
    }


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
