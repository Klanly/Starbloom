using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class GiftMoney : MonoBehaviour {

    public int Value;

    [Button(ButtonSizes.Small)]
    public void AddMoney()
    {
        QuickFind.MoneyHandler.AddMoney(Value);
    }
    [Button(ButtonSizes.Small)]
    public void SubtractMoney()
    {
        if (!QuickFind.MoneyHandler.TrySubtractMoney(Value))
            Debug.Log("Not Enough Money");
    }
}
