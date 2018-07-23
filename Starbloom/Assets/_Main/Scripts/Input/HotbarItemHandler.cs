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



    public class PlayerHotbar
    {
        [System.NonSerialized] public bool AwaitingActivateable;
        [System.NonSerialized] public DG_PlayerCharacters.RucksackSlot CurrentRucksackSlot;
        [System.NonSerialized] public DG_ItemObject CurrentItemDatabaseReference;
    }

    public PlayerHotbar[] Hotbars;


    private void Awake()
    {
        Hotbars = new PlayerHotbar[2];
        Hotbars[0] = new PlayerHotbar();
        Hotbars[1] = new PlayerHotbar();
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
            if (QuickFind.GUI_OverviewTabs.OverviewTabs[i].UIisOpen || QuickFind.StorageUI.StorageGuis[i].StorageUIOpen) return;


            if (P.ToolButton == DG_GameButtons.ButtonState.Held) AllowSent = true;
            if (P.ToolButton == DG_GameButtons.ButtonState.Up) { AllowSent = true; UpEvent = true; }

            if (Hotbars[i].AwaitingActivateable && AllowSent)
            {
                switch (Hotbars[i].CurrentItemDatabaseReference.ActivateableType)
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
        int Array = 0;
        if (PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) Array = 1;

        Hotbars[Array].AwaitingActivateable = true;
        Hotbars[Array].CurrentRucksackSlot = RucksackSlot;
        Hotbars[Array].CurrentItemDatabaseReference = ItemDatabaseReference;


        if (QuickFind.ObjectPlacementManager.GetCRPByPlayerID(PlayerID).PlacementActive) QuickFind.ObjectPlacementManager.DestroyObjectGhost(PlayerID, QuickFind.ObjectPlacementManager.GetCRPByPlayerID(PlayerID));
        if (QuickFind.HoeHandler.PlacementActive) QuickFind.HoeHandler.CancelHoeing(PlayerID);
        if (QuickFind.WateringCanHandler.PlacementActive) QuickFind.WateringCanHandler.CancelWatering(PlayerID);
        if (QuickFind.PickaxeHandler.PlacementActive) QuickFind.PickaxeHandler.CancelHittingMode(PlayerID);
        if (QuickFind.CombatHandler.Combats[Array].WeaponActive) QuickFind.CombatHandler.CancelHittingMode(PlayerID);

        QuickFind.TargetingController.Targeting[Array].TargetingCanvas.gameObject.SetActive(false);


        switch (ItemDatabaseReference.ActivateableType)
        {
            case ActivateableTypes.PlaceableItem: QuickFind.ObjectPlacementManager.SetupItemObjectGhost(PlayerID, RucksackSlot, ItemDatabaseReference, Slot); break;
            case ActivateableTypes.Hoe: QuickFind.HoeHandler.SetupForHoeing(PlayerID, RucksackSlot, ItemDatabaseReference, Slot); break;
            case ActivateableTypes.WateringCan: QuickFind.WateringCanHandler.SetupForWatering(PlayerID, RucksackSlot, ItemDatabaseReference, Slot); break;
            case ActivateableTypes.Pickaxe: QuickFind.PickaxeHandler.SetupForHitting(PlayerID, RucksackSlot, ItemDatabaseReference, Slot, ActivateableTypes.Pickaxe); break;
            case ActivateableTypes.Axe: QuickFind.PickaxeHandler.SetupForHitting(PlayerID, RucksackSlot, ItemDatabaseReference, Slot, ActivateableTypes.Axe); break;
            case ActivateableTypes.Weapon: QuickFind.CombatHandler.SetupForHitting(PlayerID, RucksackSlot, ItemDatabaseReference, Slot); break;
        }


        if (ItemDatabaseReference.isTool || ItemDatabaseReference.isWeapon)
            QuickFind.ClothingHairManager.AddClothingItem(QuickFind.NetworkSync.GetCharacterLinkByPlayerID(PlayerID), ItemDatabaseReference.GetClothingID(RucksackSlot));
    }
    public void SetNoActiveItem(int Array)
    {
        Hotbars[Array].AwaitingActivateable = false;
    }

}
