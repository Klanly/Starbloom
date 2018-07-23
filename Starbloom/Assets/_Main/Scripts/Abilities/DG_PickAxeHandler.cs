using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_PickAxeHandler : MonoBehaviour {


    public LayerMask PickaxeObjectDetection;


    DG_PlayerCharacters.RucksackSlot RucksackSlotOpen;
    [System.NonSerialized] public DG_ItemObject ItemDatabaseReference;
    int ActiveSlot;
    HotbarItemHandler.ActivateableTypes CurrentActive;

    GameObject HitObject;

    [System.NonSerialized] public bool PlacementActive;
    bool SafeToPlace = false;




    private void Awake()
    {
        QuickFind.PickaxeHandler = this;
    }



    private void Update()
    {
        for (int i = 0; i < QuickFind.InputController.Players.Length; i++)
        {
            if (QuickFind.InputController.Players[i].CharLink == null) continue;

            if (PlacementActive)
            {
                if (PickaxeObjectFound(i))
                { QuickFind.GridDetection.GridDetections[i].GridMesh.enabled = true; SafeToPlace = true; }
                else { QuickFind.GridDetection.GridDetections[i].GridMesh.enabled = false; SafeToPlace = false; }
            }
            else SafeToPlace = false;
        }
    }



    public void InputDetected(bool isUP, int PlayerID)
    {
        bool AllowAction = false;
        if (isUP || QuickFind.GameSettings.AllowActionsOnHold) AllowAction = true;

        if (AllowAction && SafeToPlace)
        {
            DG_CharacterLink CL = QuickFind.NetworkSync.GetCharacterLinkByPlayerID(PlayerID);

            if (!CL.AnimationSync.CharacterIsGrounded()) return;
            if (HitObject == null) return;

            DG_ContextObject CO = HitObject.GetComponent<DG_ContextObject>();
            QuickFind.BreakableObjectsHandler.TryHitObject(CO, CurrentActive, (DG_ItemObject.ItemQualityLevels)RucksackSlotOpen.CurrentStackActive, RucksackSlotOpen, PlayerID);
        }
    }


    public void SetupForHitting(int PlayerID, DG_PlayerCharacters.RucksackSlot Rucksack = null, DG_ItemObject Item = null, int slot = 0, HotbarItemHandler.ActivateableTypes Current = HotbarItemHandler.ActivateableTypes.Pickaxe)
    {
        int Array = 0;
        if (PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) Array = 1;

        RucksackSlotOpen = Rucksack;
        ItemDatabaseReference = Item;
        ActiveSlot = slot;
        CurrentActive = Current;
        QuickFind.GridDetection.GridDetections[Array].ObjectIsPlacing = true;
        QuickFind.GridDetection.GridDetections[Array].GlobalPositioning = false;
        PlacementActive = true;
    }

    public void CancelHittingMode(int PlayerID)
    {
        int Array = 0;
        if (PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) Array = 1;

        ItemDatabaseReference = null;
        QuickFind.GridDetection.GridDetections[Array].ObjectIsPlacing = false;
        PlacementActive = false;
    }

    public bool PickaxeObjectFound(int i)
    {

        Vector3 CastPoint = QuickFind.GridDetection.GridDetections[i].DetectionPoint.position;
        Collider[] hitColliders = Physics.OverlapSphere(CastPoint, .45f, PickaxeObjectDetection); //DetermineRadiusLater

        if (hitColliders.Length > 0)
        {
            HitObject = hitColliders[0].gameObject;
            DG_ContextObject CO = HitObject.GetComponent<DG_ContextObject>();
            if (CO == null) return false;

            if (CurrentActive == HotbarItemHandler.ActivateableTypes.Pickaxe)
            {
                if (CO.Type == DG_ContextObject.ContextTypes.Breakable)
                    return true;
            }
            if (CurrentActive == HotbarItemHandler.ActivateableTypes.Axe)
            {
                if (CO.Type == DG_ContextObject.ContextTypes.HarvestablePlant || CO.Type == DG_ContextObject.ContextTypes.HarvestableTree || CO.Type == DG_ContextObject.ContextTypes.BreakableTree)
                    return true;
            }
            return false;
        }
        else
        { return false; }
    }
}
