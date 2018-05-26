using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotbarItemHandler : MonoBehaviour {


    public enum ActivateableTypes
    {
        Pickaxe,
        Axe,
        Hoe,
        FishingPole,

        RegularItem,
        PlaceableItem
    }


    bool AwaitingActivateable;
    DG_PlayerCharacters.RucksackSlot CurrentRucksackSlot;
    DG_ItemObject CurrentItemDatabaseReference;




    private void Awake()
    {
        QuickFind.ItemActivateableHandler = this;
    }


    private void Update()
    {
        bool AllowSent = false;
        bool UpEvent = false;

        if (QuickFind.InputController.MainPlayer.ButtonSet.Action.Held) AllowSent = true;
        if (QuickFind.InputController.MainPlayer.ButtonSet.Action.Up) { AllowSent = true; UpEvent = true; }

        if (AwaitingActivateable && AllowSent)
        {
            switch(CurrentItemDatabaseReference.ActivateableType)
            {

                case ActivateableTypes.Axe: AxeEvent(UpEvent); break;
                case ActivateableTypes.FishingPole: FishingPoleEvent(UpEvent); break;
                case ActivateableTypes.Hoe: HoeEvent(UpEvent); break;
                case ActivateableTypes.Pickaxe: PickaxeEvent(UpEvent); break;
                case ActivateableTypes.RegularItem: RegularItemEvent(UpEvent); break;
                case ActivateableTypes.PlaceableItem: PlaceableItemEvent(UpEvent); break;

            }
        }
    }


    void AxeEvent(bool isUp)
    {
        if(isUp)
            Debug.Log("Active Axe UP Event");
        else
            Debug.Log("Active Axe HELD Event");
    }
    void FishingPoleEvent(bool isUp)
    {
        if (isUp)
            Debug.Log("Active FishingPole UP Event");
        else
            Debug.Log("Active FishingPole HELD Event");
    }
    void HoeEvent(bool isUp)
    {
        if (isUp)
            Debug.Log("Active Hoe UP Event");
        else
            Debug.Log("Active Hoe HELD Event");
    }
    void PickaxeEvent(bool isUp)
    {
        if (isUp)
            Debug.Log("Active Pickaxe UP Event");
        else
            Debug.Log("Active Pickaxe HELD Event");
    }
    void RegularItemEvent(bool isUp)
    {
        if (isUp)
            Debug.Log("Active RegularItem UP Event");
        else
            Debug.Log("Active RegularItem HELD Event");
    }
    void PlaceableItemEvent(bool isUP)
    {
        if (isUP)
            Debug.Log("Placeable Item UP Event");
        else
            Debug.Log("Placeable Item HELD Event");
    }












    public void SetCurrentActiveItem(DG_PlayerCharacters.RucksackSlot RucksackSlot, DG_ItemObject ItemDatabaseReference)
    {
        AwaitingActivateable = true;
        CurrentRucksackSlot = RucksackSlot;
        CurrentItemDatabaseReference = ItemDatabaseReference;
    }
    public void SetNoActiveItem()
    {
        AwaitingActivateable = false;
    }
}
