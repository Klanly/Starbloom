using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DG_InventoryItem : MonoBehaviour {

    public Image Icon = null;
    public RectTransform ScaleRect = null;
    public Image HoverOverImage = null;
    public Image Disabled = null;
    public Image ActiveHotbarItem = null;
    public TMPro.TextMeshProUGUI AmountText = null;

    [Header("Icon Scale Effect")]
    public float ScaleSize;
    public float ScaleTime;

    [Header("Drag Item")]
    public bool isDragDisplay;

    [HideInInspector] public bool isMirror;
    [HideInInspector] public int SlotID;

    float Timer;
    bool ScaleUp = true;




    private void Awake()
    {
        HoverOverImage.enabled = false;
        ActiveHotbarItem.enabled = false;
        AmountText.text = string.Empty;
    }
    private void Start()
    {
        if(!isDragDisplay)
            this.enabled = false;
        Icon.sprite = QuickFind.GUI_Inventory.DefaultNullSprite;
    }

    public void ItemHoverIn()
    {
        ScaleUp = true;
        Timer = ScaleTime;
        HoverOverImage.enabled = true;
        this.enabled = true;

        QuickFind.GUI_Inventory.CurrentHoverItem = this;
    }
    public void ItemHoverOut()
    {
        ScaleUp = false;
        Timer = ScaleTime;
        HoverOverImage.enabled = false;
        this.enabled = true;

    }

    public void ItemPressed()
    {
        QuickFind.GUI_Inventory.InventoryItemPressed(this);
    }

    public void OnDrag()
    {
        if (isMirror) return;
        if(!QuickFind.GUI_Inventory.isFloatingInventoryItem) ItemPressed();
    }
    public void DragReleased()
    {
        if (isMirror) return;
        ItemPressed();
    }



    private void Update()
    {
        Timer = Timer - Time.deltaTime;

        if (Timer < 0)
        {
            Timer = 0;
            if (!isDragDisplay) this.enabled = false;
            else { ScaleUp = !ScaleUp; Timer = ScaleTime; }
        }

        float TimerPercentage = Timer / ScaleTime;
        if (ScaleUp)
            TimerPercentage = 1 - TimerPercentage;

        float ScaleDiff = ScaleSize - 1;
        float Scale = 1 + (ScaleDiff * TimerPercentage);

        ScaleRect.localScale = new Vector3(Scale, Scale);
    }
}
