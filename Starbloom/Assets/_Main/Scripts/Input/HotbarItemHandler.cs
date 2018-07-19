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
        WateringCan,
        Weapon
    }


    bool AwaitingActivateable;
    [System.NonSerialized] public DG_PlayerCharacters.RucksackSlot CurrentRucksackSlot;
    [System.NonSerialized] public DG_ItemObject CurrentItemDatabaseReference;


    private void Awake()
    {
        QuickFind.ItemActivateableHandler = this;
    }


    private void Update()
    {
        bool AllowSent = false;
        bool UpEvent = false;

        if (QuickFind.GUI_OverviewTabs == null) return;
        if (QuickFind.NetworkSync == null) return;

        for (int i = 0; i < QuickFind.InputController.Players.Length; i++)
        {
            DG_PlayerInput.Player P = QuickFind.InputController.Players[i];

            if (P.CharLink == null) return;

            if (P.CharLink.AnimationSync.MidAnimation) return;
            if (QuickFind.GUI_OverviewTabs.UIisOpen || QuickFind.StorageUI.StorageUIOpen) return;


            if (P.ButtonSet.Action.Held) AllowSent = true;
            if (P.ButtonSet.Action.Up) { AllowSent = true; UpEvent = true; }

            if (AwaitingActivateable && AllowSent)
            {
                switch (CurrentItemDatabaseReference.ActivateableType)
                {
                    case ActivateableTypes.PlaceableItem: QuickFind.ObjectPlacementManager.InputDetected(UpEvent, P.CharLink.PlayerID); break;
                    case ActivateableTypes.Axe: QuickFind.PickaxeHandler.InputDetected(UpEvent, P.CharLink.PlayerID); break;
                    case ActivateableTypes.FishingPole: QuickFind.FishingHandler.ExternalUpdate(UpEvent, P.CharLink.PlayerID); break;
                    case ActivateableTypes.Hoe: QuickFind.HoeHandler.InputDetected(UpEvent, P.CharLink.PlayerID); break;
                    case ActivateableTypes.Pickaxe: QuickFind.PickaxeHandler.InputDetected(UpEvent, P.CharLink.PlayerID); break;
                    case ActivateableTypes.WateringCan: QuickFind.WateringCanHandler.InputDetected(UpEvent, P.CharLink.PlayerID); break;
                    case ActivateableTypes.Weapon: QuickFind.CombatHandler.InputDetected(UpEvent, P.CharLink.PlayerID); break;
                }
            }
        }
    }






    public void SetCurrentActiveItem(DG_PlayerCharacters.RucksackSlot RucksackSlot, DG_ItemObject ItemDatabaseReference, int Slot, int PlayerID)
    {
        AwaitingActivateable = true;
        CurrentRucksackSlot = RucksackSlot;
        CurrentItemDatabaseReference = ItemDatabaseReference;


        if (QuickFind.ObjectPlacementManager.GetCRPByPlayerID(PlayerID).PlacementActive) QuickFind.ObjectPlacementManager.DestroyObjectGhost(QuickFind.ObjectPlacementManager.GetCRPByPlayerID(PlayerID));
        if (QuickFind.HoeHandler.PlacementActive) QuickFind.HoeHandler.CancelHoeing();
        if (QuickFind.WateringCanHandler.PlacementActive) QuickFind.WateringCanHandler.CancelWatering();
        if (QuickFind.PickaxeHandler.PlacementActive) QuickFind.PickaxeHandler.CancelHittingMode();
        if (QuickFind.CombatHandler.WeaponActive) QuickFind.CombatHandler.CancelHittingMode();

        QuickFind.TargetingController.TargetingCanvas.gameObject.SetActive(false);


        switch (ItemDatabaseReference.ActivateableType)
        {
            case ActivateableTypes.PlaceableItem: QuickFind.ObjectPlacementManager.SetupItemObjectGhost(PlayerID, RucksackSlot, ItemDatabaseReference, Slot); break;
            case ActivateableTypes.Hoe: QuickFind.HoeHandler.SetupForHoeing(RucksackSlot, ItemDatabaseReference, Slot); break;
            case ActivateableTypes.WateringCan: QuickFind.WateringCanHandler.SetupForWatering(RucksackSlot, ItemDatabaseReference, Slot); break;
            case ActivateableTypes.Pickaxe: QuickFind.PickaxeHandler.SetupForHitting(RucksackSlot, ItemDatabaseReference, Slot, ActivateableTypes.Pickaxe); break;
            case ActivateableTypes.Axe: QuickFind.PickaxeHandler.SetupForHitting(RucksackSlot, ItemDatabaseReference, Slot, ActivateableTypes.Axe); break;
            case ActivateableTypes.Weapon: QuickFind.CombatHandler.SetupForHitting(RucksackSlot, ItemDatabaseReference, Slot); break;
        }


        if (ItemDatabaseReference.isTool || ItemDatabaseReference.isWeapon)
            QuickFind.ClothingHairManager.AddClothingItem(QuickFind.NetworkSync.GetCharacterLinkByPlayerID(PlayerID), ItemDatabaseReference.GetClothingID(RucksackSlot));
    }
    public void SetNoActiveItem()
    {
        AwaitingActivateable = false;
    }

}
