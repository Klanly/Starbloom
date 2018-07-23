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
        for (int i = 0; i < QuickFind.InputController.Players.Length; i++)
        {
            if (QuickFind.InputController.Players[i].CharLink == null) continue;

            if (PlacementActive)
            {
                if (WaterableObjectFound(i))
                { QuickFind.GridDetection.GridDetections[i].GridMesh.enabled = true; SafeToPlace = true; }
                else { QuickFind.GridDetection.GridDetections[i].GridMesh.enabled = false; SafeToPlace = false; }
            }
            else { SafeToPlace = false; }
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

            DG_ContextObject CO = HitObject.GetComponent<DG_ContextObject>();
            if (CO.Type == DG_ContextObject.ContextTypes.Soil)
            {
                KnownCO = CO;
                AwaitingResponse = true;
                if (QuickFind.GameSettings.DisableAnimations)
                    HitAction(PlayerID);
                else
                {
                    CL.FacePlayerAtPosition(CO.transform.position);
                    DG_ClothingObject Cloth = QuickFind.ClothingHairManager.GetAttachedClothingReference(CL, DG_ClothingHairManager.ClothHairType.RightHand).ClothingRef;
                    CL.AnimationSync.TriggerAnimation(Cloth.AnimationDatabaseNumber);
                }
            }
        }
    }
    public void HitAction(int PlayerID)
    {
        if (!AwaitingResponse) return;
        AwaitingResponse = false;

        QuickFind.WateringSystem.WaterObject(KnownCO, PlayerID);
    }




    public void SetupForWatering(int PlayerID, DG_PlayerCharacters.RucksackSlot Rucksack = null, DG_ItemObject Item = null, int slot = 0)
    {
        int Array = 0;
        if (PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) Array = 1;

        RucksackSlotOpen = Rucksack;
        ItemDatabaseReference = Item;
        ActiveSlot = slot;
        QuickFind.GridDetection.GridDetections[Array].ObjectIsPlacing = true;
        QuickFind.GridDetection.GridDetections[Array].GlobalPositioning = false;
        PlacementActive = true;
    }

    public void CancelWatering(int PlayerID)
    {
        int Array = 0;
        if (PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) Array = 1;

        QuickFind.GridDetection.GridDetections[Array].ObjectIsPlacing = false;
        PlacementActive = false;
    }

    public bool WaterableObjectFound(int i)
    {
        Vector3 CastPoint = QuickFind.GridDetection.GridDetections[i].DetectionPoint.position;
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
