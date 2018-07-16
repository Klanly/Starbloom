using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_WateringCan : MonoBehaviour {


    public LayerMask WaterableObjectDetection;


    DG_PlayerCharacters.RucksackSlot RucksackSlotOpen;
    DG_ItemObject ItemDatabaseReference;
    int ActiveSlot;
    GameObject HitObject;
    DG_ContextObject KnownCO;

    [System.NonSerialized] public bool PlacementActive;
    bool SafeToPlace = false;
    bool AwaitingResponse;





    private void Awake()
    {
        QuickFind.WateringCanHandler = this;
    }



    private void Update()
    {
        if (PlacementActive)
        {
            if (WaterableObjectFound())
            { QuickFind.GridDetection.GridMesh.enabled = true; SafeToPlace = true; }
            else { QuickFind.GridDetection.GridMesh.enabled = false; SafeToPlace = false; }
        }
        else { SafeToPlace = false; }
    }


    public void InputDetected(bool isUP)
    {
        bool AllowAction = false;
        if (isUP || QuickFind.GameSettings.AllowActionsOnHold) AllowAction = true;

        if (AllowAction && SafeToPlace)
        {
            if (!QuickFind.NetworkSync.CharacterLink.AnimationSync.CharacterIsGrounded()) return;

            DG_ContextObject CO = HitObject.GetComponent<DG_ContextObject>();
            if (CO.Type == DG_ContextObject.ContextTypes.Soil)
            {
                KnownCO = CO;
                AwaitingResponse = true;
                if (QuickFind.GameSettings.DisableAnimations)
                    HitAction();
                else
                {
                    QuickFind.NetworkSync.CharacterLink.FacePlayerAtPosition(CO.transform.position);
                    DG_ClothingObject Cloth = QuickFind.ClothingHairManager.GetAttachedClothingReference(QuickFind.NetworkSync.CharacterLink, DG_ClothingHairManager.ClothHairType.RightHand).ClothingRef;
                    QuickFind.NetworkSync.CharacterLink.AnimationSync.TriggerAnimation(Cloth.AnimationDatabaseNumber);
                }
            }
        }
    }
    public void HitAction()
    {
        if (!AwaitingResponse) return;
        AwaitingResponse = false;

        QuickFind.WateringSystem.WaterObject(KnownCO);
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
        Vector3 CastPoint = QuickFind.GridDetection.DetectionPoint.position;
        Collider[] hitColliders = Physics.OverlapSphere(CastPoint, .45f, WaterableObjectDetection); //DetermineRadiusLater

        if (hitColliders.Length > 0)
        { HitObject = hitColliders[0].gameObject; return true; }
        else return false;


        //Old Method


        //RaycastHit m_Hit;
        //CastPoint.y += 20;
        //
        //if (Physics.BoxCast(CastPoint, new Vector3(.5f, .5f, .5f), Vector3.down, out m_Hit, transform.rotation, 21, WaterableObjectDetection)) { HitObject = m_Hit.collider.gameObject; return true; }
        //else return false;
    }
}
