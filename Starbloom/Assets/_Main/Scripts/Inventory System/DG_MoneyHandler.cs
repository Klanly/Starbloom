using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_MoneyHandler : MonoBehaviour {






    private void Awake()
    {
        QuickFind.MoneyHandler = this;
    }





    //Check
    public bool CheckIfSubtractMoney(int AmountRequired)
    {
        int CurrentMoney = QuickFind.Farm.SharedMoney;
        if (AmountRequired < CurrentMoney) return true;
        else return false;
    }
    //Add
    public void AddMoney(int AdjustAmount)
    {
        SetFarmMoneyValue(QuickFind.Farm.SharedMoney + AdjustAmount);
    }
    //Subtract
    public bool TrySubtractMoney(int AmountRequired)
    {
        int CurrentMoney = QuickFind.Farm.SharedMoney;
        if (AmountRequired < CurrentMoney)
        {
            SetFarmMoneyValue(QuickFind.Farm.SharedMoney - AmountRequired);
            return true;
        }
        else
            return false;
    }
    //Set
    public void SetFarmMoneyValue(int Amount)
    {
        QuickFind.NetworkSync.SetNewFarmMoneyValue(Amount);
    }

    //Net Incoming
    public void ReceiveFarmMoneyValue(int Amount)
    {
        int PreviousAmount = QuickFind.Farm.SharedMoney;
        QuickFind.Farm.SharedMoney = Amount;
        QuickFind.GUI_MainOverview.SetMoneyValue(PreviousAmount, QuickFind.Farm.SharedMoney, false);
    }
}
