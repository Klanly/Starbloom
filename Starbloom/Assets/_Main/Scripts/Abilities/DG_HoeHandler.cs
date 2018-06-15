using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_HoeHandler : MonoBehaviour {


    public LayerMask UnSafeGroundDetection;
    public int HoeItemDatabaseNumber = 17;


    DG_PlayerCharacters.RucksackSlot RucksackSlotOpen;
    DG_ItemObject ItemDatabaseReference;
    int ActiveSlot;
    [HideInInspector] public bool PlacementActive;
    bool SafeToPlace = false;



    private void Awake()
    {
        QuickFind.HoeHandler = this;
    }


    private void Update()
    {
        if (PlacementActive)
        {
            if (ThisPlaceisSafeToPlaceObject())
            { QuickFind.GridDetection.GridMesh.enabled = true; SafeToPlace = true; }
            else { QuickFind.GridDetection.GridMesh.enabled = false; SafeToPlace = false; }
        }
        else { SafeToPlace = false; }
    }


    public void InputDetected(bool isUP)
    {
        if (isUP && SafeToPlace)
        {
            QuickFind.NetworkObjectManager.CreateNetSceneObject(QuickFind.NetworkSync.CurrentScene, NetworkObjectManager.NetworkObjectTypes.Item, HoeItemDatabaseNumber, 0, QuickFind.GridDetection.DetectionPoint.position, 0);
        }
    }


    public void SetupForHoeing(DG_PlayerCharacters.RucksackSlot Rucksack = null, DG_ItemObject Item = null, int slot = 0)
    {
        RucksackSlotOpen = Rucksack;
        ItemDatabaseReference = Item;
        ActiveSlot = slot;
        QuickFind.GridDetection.ObjectIsPlacing = true;
        QuickFind.GridDetection.GlobalPositioning = false;
        PlacementActive = true;
    }

    public void CancelHoeing()
    {
        QuickFind.GridDetection.ObjectIsPlacing = false;
        PlacementActive = false;
    }

    public bool ThisPlaceisSafeToPlaceObject()
    {
        RaycastHit m_Hit;
        Vector3 CastPoint = QuickFind.GridDetection.DetectionPoint.position;
        CastPoint.y += 20;

        if (Physics.BoxCast(CastPoint, new Vector3(.5f, .5f, .5f), Vector3.down, out m_Hit, transform.rotation, 21, UnSafeGroundDetection)) return false;
        else return true;
    }
}
