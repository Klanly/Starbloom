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
        PlaceableItem,
        WateringCan
    }


    bool AwaitingActivateable;
    [HideInInspector] public DG_PlayerCharacters.RucksackSlot CurrentRucksackSlot;
    [HideInInspector] public DG_ItemObject CurrentItemDatabaseReference;



    private void Awake()
    {
        QuickFind.ItemActivateableHandler = this;
    }


    private void Update()
    {
        bool AllowSent = false;
        bool UpEvent = false;

        if (QuickFind.GUI_OverviewTabs.UIisOpen || QuickFind.StorageUI.StorageUIOpen)
            return;

        if (QuickFind.InputController.MainPlayer.ButtonSet.Action.Held) AllowSent = true;
        if (QuickFind.InputController.MainPlayer.ButtonSet.Action.Up) { AllowSent = true; UpEvent = true; }

        if (AwaitingActivateable && AllowSent)
        {
            switch(CurrentItemDatabaseReference.ActivateableType)
            {

                case ActivateableTypes.Axe: Debug.Log("Axe " + UpEvent.ToString()); break;
                case ActivateableTypes.FishingPole: QuickFind.FishingHandler.ExternalUpdate(UpEvent); break;
                case ActivateableTypes.Hoe: QuickFind.HoeHandler.InputDetected(UpEvent); break;
                case ActivateableTypes.Pickaxe: Debug.Log("PickAxe " + UpEvent.ToString()); break;
                case ActivateableTypes.RegularItem: Debug.Log("RegularItem " + UpEvent.ToString()); break;
                case ActivateableTypes.PlaceableItem: QuickFind.ObjectPlacementManager.InputDetected(UpEvent); break;
                case ActivateableTypes.WateringCan: Debug.Log("Watering Can " + UpEvent.ToString()); break;
            }
        }
    }











    public void SetCurrentActiveItem(DG_PlayerCharacters.RucksackSlot RucksackSlot, DG_ItemObject ItemDatabaseReference, int Slot)
    {
        AwaitingActivateable = true;
        CurrentRucksackSlot = RucksackSlot;
        CurrentItemDatabaseReference = ItemDatabaseReference;




        if (QuickFind.ObjectPlacementManager.PlacementActive) QuickFind.ObjectPlacementManager.DestroyObjectGhost();
        if (QuickFind.HoeHandler.PlacementActive) QuickFind.HoeHandler.CancelHoeing();

        switch(ItemDatabaseReference.ActivateableType)
        {
            case ActivateableTypes.PlaceableItem: QuickFind.ObjectPlacementManager.SetupItemObjectGhost(DG_ObjectPlacement.PlacementType.ItemObject, RucksackSlot, ItemDatabaseReference, Slot); break;
            case ActivateableTypes.Hoe: QuickFind.HoeHandler.SetupForHoeing(RucksackSlot, ItemDatabaseReference, Slot); break;
        }
    }
    public void SetNoActiveItem()
    {
        AwaitingActivateable = false;
    }
}
