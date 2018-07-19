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
        if (PlacementActive)
        {
            if (ThisPlaceisSafeToPlaceObject())
            { QuickFind.GridDetection.GridMesh.enabled = true; SafeToPlace = true; }
            else { QuickFind.GridDetection.GridMesh.enabled = false; SafeToPlace = false; }
        }
        else { SafeToPlace = false; }
    }


    public void InputDetected(bool isUP, int PlayerID)
    {
        bool AllowAction = false;
        if (isUP || QuickFind.GameSettings.AllowActionsOnHold) AllowAction = true;

        if (AllowAction && SafeToPlace)
        {
            DG_CharacterLink CLink = QuickFind.NetworkSync.GetCharacterLinkByPlayerID(PlayerID);

            if (!CLink.AnimationSync.CharacterIsGrounded()) return;

            AwaitingResponse = true;
            StoredPosition = QuickFind.GridDetection.DetectionPoint.position;
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
        
        Vector3 CastPoint = QuickFind.GridDetection.DetectionPoint.position;

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
