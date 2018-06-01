using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_WateringCan : MonoBehaviour {


    public LayerMask WaterableObjectDetection;


    DG_PlayerCharacters.RucksackSlot RucksackSlotOpen;
    DG_ItemObject ItemDatabaseReference;
    int ActiveSlot;
    GameObject HitObject;

    [HideInInspector] public bool PlacementActive;
    bool SafeToPlace = false;





    private void Awake()
    {
        QuickFind.WateringCanHandler = this;
    }



    private void Update()
    {
        if (PlacementActive)
        { if (WaterableObjectFound()) { SafeToPlace = true; } else { SafeToPlace = false; } }
        else SafeToPlace = false;
    }


    public void InputDetected(bool isUP)
    {
        if (isUP && SafeToPlace)
        {
            DG_ContextObject CO = HitObject.GetComponent<DG_ContextObject>();
            if(CO.Type == DG_ContextObject.ContextTypes.Soil)
                QuickFind.WateringSystem.WaterObject(CO);
        }
    }


    public void SetupForWatering(DG_PlayerCharacters.RucksackSlot Rucksack = null, DG_ItemObject Item = null, int slot = 0)
    {
        RucksackSlotOpen = Rucksack;
        ItemDatabaseReference = Item;
        ActiveSlot = slot;
        QuickFind.GridDetection.ObjectIsPlacing = true;
        QuickFind.GridDetection.GlobalPositioning = false;
        PlacementActive = true;
    }

    public void CancelWatering()
    {
        QuickFind.GridDetection.ObjectIsPlacing = false;
        PlacementActive = false;
    }

    public bool WaterableObjectFound()
    {
        RaycastHit m_Hit;
        Vector3 CastPoint = QuickFind.GridDetection.DetectionPoint.position;
        CastPoint.y += 20;

        if (Physics.BoxCast(CastPoint, new Vector3(.5f, .5f, .5f), Vector3.down, out m_Hit, transform.rotation, 21, WaterableObjectDetection)) { HitObject = m_Hit.collider.gameObject; return true; }
        else return false;
    }
}
