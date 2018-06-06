﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DG_ShopGuiItem : MonoBehaviour {

    
    public Image HoverOverImage = null;

    [Header("Icon Scale Effect")]
    public RectTransform ScaleRect = null;
    public Image Icon = null;
    public float ScaleSize;
    public float ScaleTime;

    [Header("Text")]
    public TMPro.TextMeshProUGUI AmountText = null;
    public TMPro.TextMeshProUGUI NameText = null;

    float Timer;
    bool ScaleUp = true;
    bool EndLoop = false;



    [HideInInspector] public DG_ShopAtlasObject.SeasonalGood SeasonalGoodsRef;




    private void Awake()
    {
        HoverOverImage.enabled = false;
        this.enabled = false;
    }




    public void ItemHoverIn()
    {
        ScaleUp = true;
        Timer = ScaleTime;
        HoverOverImage.enabled = true;
        EndLoop = false;
        this.enabled = true;
    }
    public void ItemHoverOut()
    {
        ScaleUp = false;
        Timer = ScaleTime;
        HoverOverImage.enabled = false;
        EndLoop = true;
        this.enabled = true;
    }

    public void ItemPressed()
    {
        QuickFind.ShopGUI.ShopItemPressed(this);
    }



    private void Update()
    {
        Timer = Timer - Time.deltaTime;

        if (Timer < 0)
        {
            Timer = 0;
            if (EndLoop) this.enabled = false;
            else { ScaleUp = !ScaleUp; Timer = ScaleTime; }
        }

        float TimerPercentage = Timer / ScaleTime;
        if (ScaleUp)
            TimerPercentage = 1 - TimerPercentage;

        float ScaleDiff = ScaleSize - 1;
        float Scale = 1 + (ScaleDiff * TimerPercentage);

        ScaleRect.localScale = new Vector3(Scale, Scale);
    }



    public void UpdateVisuals()
    {
        DG_ItemObject IO = QuickFind.ItemDatabase.GetItemFromID(SeasonalGoodsRef.ItemDatabaseRef);
        Icon.sprite = IO.GetItemSpriteByQuality(SeasonalGoodsRef.QualityLevel);
        AmountText.text = IO.GetBuyPriceByQuality(SeasonalGoodsRef.QualityLevel).ToString();
        NameText.text = QuickFind.WordDatabase.GetWordFromID(IO.ToolTipType.MainLocalizationID);
    }
}
