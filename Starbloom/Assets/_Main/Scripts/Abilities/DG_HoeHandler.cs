using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_HoeHandler : MonoBehaviour {


    public LayerMask UnSafeGroundDetection;
    public int HoeItemDatabaseNumber = 17;


    DG_PlayerCharacters.RucksackSlot RucksackSlotOpen;
    DG_ItemObject ItemDatabaseReference;
    int ActiveSlot;
    [System.NonSerialized] public bool PlacementActive;
    bool SafeToPlace = false;
    bool AwaitingResponse;
    Vector3 StoredPosition;


    private void Awake()
    {
        QuickFind.HoeHandler = this;
    }


    private void Update()
    {
        for (int i = 0; i < QuickFind.InputController.Players.Length; i++)
        {
            if (QuickFind.InputController.Players[i].CharLink == null) continue;

            if (PlacementActive)
            {
                if (ThisPlaceisSafeToPlaceObject(i))
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

        int Array = 0;
        if (PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) Array = 1;

        if (AllowAction && SafeToPlace)
        {
            DG_CharacterLink CLink = QuickFind.NetworkSync.GetCharacterLinkByPlayerID(PlayerID);

            if (!CLink.AnimationSync.CharacterIsGrounded()) return;

            AwaitingResponse = true;
            StoredPosition = QuickFind.GridDetection.GridDetections[Array].DetectionPoint.position;
            if (QuickFind.GameSettings.DisableAnimations)
                HitAction(PlayerID);
            else
            {
                CLink.FacePlayerAtPosition(StoredPosition);
                DG_ClothingObject Cloth = QuickFind.ClothingHairManager.GetAttachedClothingReference(CLink, DG_ClothingHairManager.ClothHairType.RightHand).ClothingRef;
                CLink.AnimationSync.TriggerAnimation(Cloth.AnimationDatabaseNumber);
            }
        }
    }


    public void HitAction(int PlayerID)
    {
        if (!AwaitingResponse) return;
        AwaitingResponse = false;



        QuickFind.NetworkObjectManager.CreateNetSceneObject(QuickFind.NetworkSync.GetUserByPlayerID(PlayerID).SceneID, NetworkObjectManager.NetworkObjectTypes.Item, HoeItemDatabaseNumber, 0, StoredPosition, 0);
    }





    public void SetupForHoeing(int PlayerID, DG_PlayerCharacters.RucksackSlot Rucksack = null, DG_ItemObject Item = null, int slot = 0)
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

    public void CancelHoeing(int PlayerID)
    {
        int Array = 0;
        if (PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) Array = 1;

        QuickFind.GridDetection.GridDetections[Array].ObjectIsPlacing = false;
        PlacementActive = false;
    }

    public bool ThisPlaceisSafeToPlaceObject(int Index)
    {
        
        Vector3 CastPoint = QuickFind.GridDetection.GridDetections[Index].DetectionPoint.position;

        Collider[] hitColliders = Physics.OverlapSphere(CastPoint, .45f, UnSafeGroundDetection); //DetermineRadiusLater

        if (hitColliders.Length > 0) return false;
        else return true;




        //Old Method

        //RaycastHit m_Hit;
        //CastPoint.y += 20;
        //
        //if (Physics.BoxCast(CastPoint, new Vector3(.5f, .5f, .5f), Vector3.down, out m_Hit, transform.rotation, 21, UnSafeGroundDetection)) return false;
        //else return true;
    }
}
