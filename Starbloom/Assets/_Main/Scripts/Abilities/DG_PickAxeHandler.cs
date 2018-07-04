using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_PickAxeHandler : MonoBehaviour {


    public LayerMask PickaxeObjectDetection;


    DG_PlayerCharacters.RucksackSlot RucksackSlotOpen;
    DG_ItemObject ItemDatabaseReference;
    int ActiveSlot;
    HotbarItemHandler.ActivateableTypes CurrentActive;

    GameObject HitObject;

    [HideInInspector] public bool PlacementActive;
    bool SafeToPlace = false;




    private void Awake()
    {
        QuickFind.PickaxeHandler = this;
    }



    private void Update()
    {
        if (PlacementActive)
        { if (PickaxeObjectFound())
            { QuickFind.GridDetection.GridMesh.enabled = true; SafeToPlace = true; }
            else { QuickFind.GridDetection.GridMesh.enabled = false; SafeToPlace = false; } }
        else SafeToPlace = false;
    }



    public void InputDetected(bool isUP)
    {
        bool AllowAction = false;
        if (isUP || QuickFind.GameSettings.AllowActionsOnHold) AllowAction = true;

        if (AllowAction && SafeToPlace)
        {
            if (!QuickFind.NetworkSync.CharacterLink.AnimationSync.CharacterIsGrounded()) return;

            DG_ContextObject CO = HitObject.GetComponent<DG_ContextObject>();
            QuickFind.BreakableObjectsHandler.TryHitObject(CO, CurrentActive, (DG_ItemObject.ItemQualityLevels)RucksackSlotOpen.CurrentStackActive, RucksackSlotOpen);
        }
    }


    public void SetupForHitting(DG_PlayerCharacters.RucksackSlot Rucksack = null, DG_ItemObject Item = null, int slot = 0, HotbarItemHandler.ActivateableTypes Current = HotbarItemHandler.ActivateableTypes.Pickaxe)
    {
        RucksackSlotOpen = Rucksack;
        ItemDatabaseReference = Item;
        ActiveSlot = slot;
        CurrentActive = Current;
        QuickFind.GridDetection.ObjectIsPlacing = true;
        QuickFind.GridDetection.GlobalPositioning = false;
        PlacementActive = true;
    }

    public void CancelHittingMode()
    {
        QuickFind.GridDetection.ObjectIsPlacing = false;
        PlacementActive = false;
    }

    public bool PickaxeObjectFound()
    {
        RaycastHit m_Hit;
        Vector3 CastPoint = QuickFind.GridDetection.DetectionPoint.position;
        CastPoint.y += 20;

        if (Physics.BoxCast(CastPoint, new Vector3(.5f, .5f, .5f), Vector3.down, out m_Hit, transform.rotation, 21, PickaxeObjectDetection))
        {
            HitObject = m_Hit.collider.gameObject;
            DG_ContextObject CO = HitObject.GetComponent<DG_ContextObject>();
            if (CO == null) return false;

            if(CurrentActive == HotbarItemHandler.ActivateableTypes.Pickaxe)
            {
                if (CO.Type == DG_ContextObject.ContextTypes.Breakable)
                    return true;
            }
            if (CurrentActive == HotbarItemHandler.ActivateableTypes.Axe)
            {
                if ( CO.Type == DG_ContextObject.ContextTypes.HarvestablePlant || CO.Type == DG_ContextObject.ContextTypes.HarvestableTree || CO.Type == DG_ContextObject.ContextTypes.BreakableTree)
                    return true;
            }
            return false;
        }
        else
        {
            return false;
        }
    }
}
