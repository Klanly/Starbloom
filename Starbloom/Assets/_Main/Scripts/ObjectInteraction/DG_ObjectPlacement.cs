using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_ObjectPlacement : MonoBehaviour {


    public class CharacterRequestingPlacement
    {
        public int PlayerID;
        public bool PlacementActive = false;
        public int ActiveSlot;

        public Transform ObjectGhost;
        public DG_PlayerCharacters.RucksackSlot RucksackSlotOpen;
        public DG_ItemObject ItemDatabaseReference;

        //Wait for Animation To Complete
        public bool SpawnGhostAfterAnimation = false;
        public bool SafeToPlace = false;
    }


    public enum PlacementType
    {
        ItemObject,
        Structure
    }



    public LayerMask BoxcastDetection;
    public LayerMask SeedPlacementDetection;
    public LayerMask SoilReplaceDetection;
    public int SeedPlantCancelLayer;

    [Header("Debug")]
    public bool ShowDebug = false;

    public CharacterRequestingPlacement[] PlayersPlacement;

    [System.NonSerialized] public bool AwaitingNetResponse;



    PlacementType CurrentPlacementType;
    Collider DetectedInTheWay;
    NetworkObject SoilObject;
    bool AwaitingResponse;

    Vector3 SavedPosition;
    float SavedDirection;






    private void Awake()
    {
        QuickFind.ObjectPlacementManager = this;
        PlayersPlacement = new CharacterRequestingPlacement[2];
        PlayersPlacement[0] = new CharacterRequestingPlacement();
        PlayersPlacement[1] = new CharacterRequestingPlacement();
    }


    private void Update()
    {
        for (int i = 0; i < PlayersPlacement.Length; i++)
        {
            if (QuickFind.InputController.Players[i].CharLink == null) continue;

            CharacterRequestingPlacement CRP = PlayersPlacement[i];

            if (CRP.PlacementActive)
            {
                CRP.ObjectGhost.position = QuickFind.GridDetection.GridDetections[i].DetectionPoint.position;
                bool GoodToPlace = false;
                if (CRP.ItemDatabaseReference.RequireTilledEarth)
                    GoodToPlace = AreaIsOkForSeedPlacement(SeedPlacementDetection, CRP.ObjectGhost.position, .45f);
                else
                    GoodToPlace = AreaIsClear(BoxcastDetection, CRP.ObjectGhost.position, .45f);

                QuickFind.GridDetection.GridDetections[i].GridMesh.enabled = GoodToPlace;
                SetMaterialColors(GoodToPlace, CRP);
                CRP.SafeToPlace = GoodToPlace;
            }
            else CRP.SafeToPlace = false;

            if (CRP.SpawnGhostAfterAnimation)
            {
                if (QuickFind.NetworkSync.GetCharacterLinkByPlayerID(CRP.PlayerID).AnimationSync.MidAnimation) return;
                CRP.SpawnGhostAfterAnimation = false;
                SetupItemObjectGhost(CRP.PlayerID, CRP.RucksackSlotOpen, CRP.ItemDatabaseReference, CRP.ActiveSlot);
            }
        }
    }






    public void InputDetected(bool isUP, int PlayerID)
    {
        bool AllowAction = false;
        if (isUP || QuickFind.GameSettings.AllowActionsOnHold) AllowAction = true;

        CharacterRequestingPlacement CRP = GetCRPByPlayerID(PlayerID);

        if (AllowAction && CRP.SafeToPlace)
        {
            if (!QuickFind.NetworkSync.GetCharacterLinkByPlayerID(PlayerID).AnimationSync.CharacterIsGrounded()) return;



            int SceneID = QuickFind.NetworkSync.GetUserByPlayerID(PlayerID).SceneID;

            if (SoilDetection(CRP, SceneID))
            {
                if (SoilObject != null && SoilObject.SurrogateObjectIndex != 0) return;
                if (AwaitingNetResponse) return;

                AwaitingNetResponse = true;

                if (!CRP.ItemDatabaseReference.isWallItem)
                {
                    AwaitingResponse = true;
                    SavedPosition = CRP.ObjectGhost.position;
                    SavedDirection = CRP.ObjectGhost.eulerAngles.y;

                    if (QuickFind.GameSettings.DisableAnimations)
                        PlacementHit(CRP, SceneID);
                    else
                    {
                        QuickFind.NetworkSync.GetCharacterLinkByPlayerID(PlayerID).FacePlayerAtPosition(SavedPosition);
                        QuickFind.NetworkSync.GetCharacterLinkByPlayerID(PlayerID).AnimationSync.TriggerAnimation(CRP.ItemDatabaseReference.AnimationActionID);
                    }
                }
                else
                    CRP.ObjectGhost.GetComponent<DG_DynamicWall>().TriggerPlaceWall();
                DestroyObjectGhost(PlayerID, CRP);
                QuickFind.InventoryManager.DestroyRucksackItem(CRP.RucksackSlotOpen, CRP.ActiveSlot, PlayerID);
                QuickFind.GUI_Inventory.ResetHotbarSlot(PlayerID);
            }
        }
    }


    public void PlacementHit(CharacterRequestingPlacement CRP, int SceneID)
    {
        if (!AwaitingResponse) return; AwaitingResponse = false;

        QuickFind.NetworkObjectManager.CreateNetSceneObject(SceneID, NetworkObjectManager.NetworkObjectTypes.Item, CRP.ItemDatabaseReference.DatabaseID, CRP.RucksackSlotOpen.CurrentStackActive, SavedPosition, SavedDirection);
    }



    bool SoilDetection(CharacterRequestingPlacement CRP, int SceneID)
    {
        int Array = 0;
        if (CRP.PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) Array = 1;

        if (!AreaIsClear(SoilReplaceDetection, QuickFind.GridDetection.GridDetections[Array].DetectionPoint.position, .45f))
            SoilObject = QuickFind.NetworkObjectManager.ScanUpTree(DetectedInTheWay.transform);
        else
            SoilObject = null;

        if (CRP.ItemDatabaseReference.RequireTilledEarth) { if (SoilObject != null) return true; else return false; }
        else
        {
            if(SoilObject != null && CRP.ItemDatabaseReference.DestroysSoilOnPlacement)
                QuickFind.NetworkSync.RemoveNetworkSceneObject(SceneID, SoilObject.NetworkObjectID);
            return true;
        }
    }



    public void SetupItemObjectGhost(int PlayerID, DG_PlayerCharacters.RucksackSlot Rucksack = null, DG_ItemObject Item = null, int slot = 0)
    {
        CharacterRequestingPlacement CRP = GetCRPByPlayerID(PlayerID);

        if (QuickFind.NetworkSync.GetCharacterLinkByPlayerID(PlayerID).AnimationSync.MidAnimation) { CRP.SpawnGhostAfterAnimation = true; return; }

        int Array = 0;
        if (PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) Array = 1;

        CRP.PlayerID = PlayerID;
        CRP.RucksackSlotOpen = Rucksack;
        CRP.ItemDatabaseReference = Item;
        CRP.ActiveSlot = slot;
        CRP.PlacementActive = true;

        QuickFind.GridDetection.GridDetections[Array].ObjectIsPlacing = true;
        QuickFind.GridDetection.GridDetections[Array].GlobalPositioning = false;

        GameObject ToDestroy = null;
        if (CRP.ObjectGhost != null) ToDestroy = CRP.ObjectGhost.gameObject;

        if(Item.ShowPlacementGhost)
            CRP.ObjectGhost = Instantiate(Item.ModelPrefab).transform;
        else
            CRP.ObjectGhost = new GameObject().transform;

        CRP.ObjectGhost.SetParent(transform);
        CRP.ObjectGhost.localScale = new Vector3(Item.DefaultScale, Item.DefaultScale, Item.DefaultScale);

        TurnOffCollidersLoop(CRP.ObjectGhost);

        if (Item.isWallItem) CRP.ObjectGhost.GetComponent<DG_DynamicWall>().TriggerPlacementMode(Item.DatabaseID, PlayerID);
        if (ToDestroy != null) Destroy(ToDestroy);
    }
    void TurnOffCollidersLoop(Transform T)
    {
        if (T.GetComponent<Collider>()) T.GetComponent<Collider>().enabled = false;
        if(T.childCount > 0)
        {
            for (int i = 0; i < T.childCount; i++)
                TurnOffCollidersLoop(T.GetChild(i));
        }
    }




    public void DestroyObjectGhost(int PlayerID, CharacterRequestingPlacement CRP)
    {
        int Array = 0;
        if (PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) Array = 1;

        QuickFind.GridDetection.GridDetections[Array].ObjectIsPlacing = false;
        Destroy(CRP.ObjectGhost.gameObject);
        CRP.PlacementActive = false;
    }






    public bool AreaIsOkForSeedPlacement(LayerMask DetectionType, Vector3 CastPoint, float Radius)
    {
        //This will have to be adjusted at a later point to fit larger objects.
        Collider[] hitColliders = Physics.OverlapSphere(CastPoint, Radius, DetectionType);

        if (hitColliders.Length > 0)
        {
            for (int i = 0; i < hitColliders.Length; i++)
            { if (hitColliders[i].gameObject.layer == SeedPlantCancelLayer) return false; }
            return true;
        }
        else
        { return false; }
    }
    public bool AreaIsClear(LayerMask DetectionType, Vector3 CastPoint, float Radius)
    {
        //This will have to be adjusted at a later point to fit larger objects.
        Collider[] hitColliders = Physics.OverlapSphere(CastPoint, Radius, DetectionType);

        if (hitColliders.Length > 0)
        { DetectedInTheWay = hitColliders[0]; return false; }
        else
        { return true; }
    }







    public void SendOutSurrogateSearch(NetworkObject Spawn)
    {
        if (!AreaIsClear(SoilReplaceDetection, Spawn.transform.position, .45f))
        {
            DG_ContextObject CO = DetectedInTheWay.GetComponent<DG_ContextObject>();
            NetworkObject NO = QuickFind.NetworkObjectManager.ScanUpTree(CO.transform);

            int[] OutData = new int[3];
            OutData[0] = NO.transform.parent.GetComponent<NetworkScene>().SceneID;
            OutData[1] = NO.NetworkObjectID;
            OutData[2] = Spawn.NetworkObjectID;
            QuickFind.NetworkSync.SetTilledSurrogate(OutData);
        }
        else
        {
            Debug.Log("Critical Error, an object that requires tilled soil was placed in a non tilled place.");
        }
    }

    public void ReceiveTilledSurrogate(int[] InData)
    {
        NetworkObject NO = QuickFind.NetworkObjectManager.GetItemByID(InData[0], InData[1]);
        NO.SurrogateObjectIndex = InData[2];

        NetworkObject NO2 = QuickFind.NetworkObjectManager.GetItemByID(InData[0], InData[2]);
        NO2.SurrogateObjectIndex = InData[1];

        NO2.HasBeenWatered = NO.HasBeenWatered;
    }







    public void SetMaterialColors(bool isGreen, CharacterRequestingPlacement CRP)
    {
        foreach (MeshRenderer c in CRP.ObjectGhost.GetComponents<MeshRenderer>())
        {
            foreach (Material M in c.materials)
            {
                Color C = M.color;
                if (isGreen) { C.g = .3f; C.r = 0f; }
                else { C.g = 0f; C.r = .3f; }
                M.color = C;
            }
        }
    }

    public CharacterRequestingPlacement GetCRPByPlayerID(int PlayerID)
    {
        if (PlayerID == QuickFind.NetworkSync.Player1PlayerCharacter) return PlayersPlacement[0];
        else return PlayersPlacement[1];
    }
}
