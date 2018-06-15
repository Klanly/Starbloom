using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_SwordHandler : MonoBehaviour {


    [HideInInspector] public bool SwordActive = true;



    private void Awake()
    {
        QuickFind.SwordHandler = this;
    }


    public void InputDetected(bool isUP)
    {
        if (isUP)
        {
            //Temp until animations are in.
            transform.GetChild(0).gameObject.SetActive(true);
        }
    }


    public void SetupForHitting(DG_PlayerCharacters.RucksackSlot Rucksack = null, DG_ItemObject Item = null, int slot = 0)
    {
        QuickFind.CombatHandler.RucksackSlotOpen = Rucksack;
        QuickFind.CombatHandler.ItemDatabaseReference = Item;
        QuickFind.CombatHandler.ActiveSlot = slot;
        SwordActive = true;
    }

    public void CancelHittingMode()
    {
        SwordActive = false;
    }


}
