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
                case ActivateableTypes.PlaceableItem: QuickFind.ObjectPlacementManager.InputDetected(UpEvent); break;

                case ActivateableTypes.Axe: QuickFind.PickaxeHandler.InputDetected(UpEvent); break;
                case ActivateableTypes.FishingPole: QuickFind.FishingHandler.ExternalUpdate(UpEvent); break;
                case ActivateableTypes.Hoe: QuickFind.HoeHandler.InputDetected(UpEvent); break;
                case ActivateableTypes.Pickaxe: QuickFind.PickaxeHandler.InputDetected(UpEvent); break;
                case ActivateableTypes.WateringCan: QuickFind.WateringCanHandler.InputDetected(UpEvent); break;
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
        if (QuickFind.WateringCanHandler.PlacementActive) QuickFind.WateringCanHandler.CancelWatering();
        if (QuickFind.PickaxeHandler.PlacementActive) QuickFind.PickaxeHandler.CancelHittingMode();

        switch (ItemDatabaseReference.ActivateableType)
        {
            case ActivateableTypes.PlaceableItem: QuickFind.ObjectPlacementManager.SetupItemObjectGhost(DG_ObjectPlacement.PlacementType.ItemObject, RucksackSlot, ItemDatabaseReference, Slot); break;
            case ActivateableTypes.Hoe: QuickFind.HoeHandler.SetupForHoeing(RucksackSlot, ItemDatabaseReference, Slot); break;
            case ActivateableTypes.WateringCan: QuickFind.WateringCanHandler.SetupForWatering(RucksackSlot, ItemDatabaseReference, Slot); break;
            case ActivateableTypes.Pickaxe: QuickFind.PickaxeHandler.SetupForHitting(RucksackSlot, ItemDatabaseReference, Slot, ActivateableTypes.Pickaxe); break;
            case ActivateableTypes.Axe: QuickFind.PickaxeHandler.SetupForHitting(RucksackSlot, ItemDatabaseReference, Slot, ActivateableTypes.Axe); break;
        }


    }
    public void SetNoActiveItem()
    {
        AwaitingActivateable = false;
    }

}
