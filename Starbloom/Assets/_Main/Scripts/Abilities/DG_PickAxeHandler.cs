using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_PickAxeHandler : MonoBehaviour {


    public LayerMask PickaxeObjectDetection;


    DG_PlayerCharacters.RucksackSlot RucksackSlotOpen;
    DG_ItemObject ItemDatabaseReference;
    int ActiveSlot;
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
        { if (PickaxeObjectFound()) { SafeToPlace = true; } else { SafeToPlace = false; } }
        else SafeToPlace = false;
    }



    public void InputDetected(bool isUP)
    {
        if (isUP && SafeToPlace)
        {
            DG_ContextObject CO = HitObject.GetComponent<DG_ContextObject>();
            if (CO.Type == DG_ContextObject.ContextTypes.Breakable || CO.Type == DG_ContextObject.ContextTypes.Pick_And_Break)
                QuickFind.BreakableObjectsHandler.TryHitObject(CO, HotbarItemHandler.ActivateableTypes.Pickaxe,(DG_ItemObject.ItemQualityLevels)RucksackSlotOpen.CurrentStackActive, RucksackSlotOpen);
        }
    }


    public void SetupForHitting(DG_PlayerCharacters.RucksackSlot Rucksack = null, DG_ItemObject Item = null, int slot = 0)
    {
        RucksackSlotOpen = Rucksack;
        ItemDatabaseReference = Item;
        ActiveSlot = slot;
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

        if (Physics.BoxCast(CastPoint, new Vector3(.5f, .5f, .5f), Vector3.down, out m_Hit, transform.rotation, 21, PickaxeObjectDetection)) { HitObject = m_Hit.collider.gameObject; return true; }
        else { return false; }
    }
}
