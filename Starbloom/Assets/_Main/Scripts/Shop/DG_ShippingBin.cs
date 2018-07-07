﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_ShippingBin : MonoBehaviour {

    [System.Serializable]
    public class ShippingBinItem
    {
        public int ItemRef;
        public int Low;
        public int Normal;
        public int High;
        public int Max;

        public int GetStackValue()
        { return Low + Normal + High + Max; }
    }

    [System.NonSerialized] public List<ShippingBinItem> DailyShippingItems;
    [System.NonSerialized] public DG_ContextObject ActiveBinObject;
    int NetID = 0;



    private void Awake()
    {
        QuickFind.ShippingBin = this;
        DailyShippingItems = new List<ShippingBinItem>();
    }



    public void SetStackInShippingBin(DG_PlayerCharacters.RucksackSlot RucksackSlot)
    {
        ShippingBinItem SBI = null;
        for (int i = 0; i < DailyShippingItems.Count; i++)
        {
            if (DailyShippingItems[i].ItemRef == RucksackSlot.ContainedItem)
                SBI = DailyShippingItems[i];
        }

        int[] SendData = new int[5];

        SendData[0] = RucksackSlot.ContainedItem;
        SendData[1] = RucksackSlot.LowValue;
        SendData[2] = RucksackSlot.NormalValue;
        SendData[3] = RucksackSlot.HighValue;
        SendData[4] = RucksackSlot.MaximumValue;

        if (SBI != null)
        {
            SendData[1] += SBI.Low;
            SendData[2] += SBI.Normal;
            SendData[3] += SBI.High;
            SendData[4] += SBI.Max;
        }

        QuickFind.NetworkSync.SetItemInShippingBin(SendData);
        //FX
        ActiveBinObject.GetComponent<DG_UI_WobbleAndFade>().enabled = true;
    }


    public void ItemSetInShippingBin(int[] InData)
    {
        ShippingBinItem SBI = null;
        int ItemID = InData[0];
        for (int i = 0; i < DailyShippingItems.Count; i++)
        {
            if (DailyShippingItems[i].ItemRef == ItemID)
                SBI = DailyShippingItems[i];
        }

        if (SBI == null) { SBI = new ShippingBinItem(); DailyShippingItems.Add(SBI); SBI.ItemRef = ItemID; }

        SBI.Low += InData[1];
        SBI.Normal += InData[2];
        SBI.High += InData[3];
        SBI.Max += InData[4];
    }


    public void DayEndTallyMoney()
    {
        int MoneyMade = 0;
        for(int i = 0; i < DailyShippingItems.Count; i++)
            MoneyMade += CalculateTotalOfStack(DailyShippingItems[i]);

        DailyShippingItems.Clear();
        Debug.Log("Total Daily Money Made == " + MoneyMade.ToString());
        if (PhotonNetwork.isMasterClient)
        {
            if(MoneyMade != 0)
                QuickFind.MoneyHandler.AddMoney(MoneyMade);
        }
    }


    public int CalculateTotalOfStack(ShippingBinItem SBI)
    {
        int MoneyMade = 0;
        DG_ItemObject IO = QuickFind.ItemDatabase.GetItemFromID(SBI.ItemRef);
        if (SBI.Low > 0)
            MoneyMade += SBI.Low * IO.GetSellPriceByQuality(0);
        if (SBI.Normal > 0)
            MoneyMade += SBI.Normal * IO.GetSellPriceByQuality(1);
        if (SBI.High > 0)
            MoneyMade += SBI.High * IO.GetSellPriceByQuality(2);
        if (SBI.Max > 0)
            MoneyMade += SBI.Max * IO.GetSellPriceByQuality(3);

        return MoneyMade;
    }





    public void UpdateBinItem(DG_ShippingBin.ShippingBinItem BinItem)
    {
        for(int i = 0; i < DailyShippingItems.Count; i++)
        {
            if (DailyShippingItems[i] == BinItem) { QuickFind.NetworkSync.ClearBinItem(i); break; }
        }
        if (BinItem.ItemRef != 0)
        {
            int[] SendData = new int[5];

            SendData[0] = BinItem.ItemRef;
            SendData[1] += BinItem.Low;
            SendData[2] += BinItem.Normal;
            SendData[3] += BinItem.High;
            SendData[4] += BinItem.Max;

            QuickFind.NetworkSync.SetItemInShippingBin(SendData);
        }
    }

    public void ClearBinItem(int ItemIndex)
    {
        DailyShippingItems.RemoveAt(ItemIndex);
    }
}
