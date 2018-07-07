using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_ObjectPlacement : MonoBehaviour {

    public enum PlacementType
    {
        ItemObject,
        Structure
    }



    public LayerMask BoxcastDetection;
    public LayerMask SeedPlacementDetection;
    public LayerMask SoilReplaceDetection;

    [Header("Debug")]
    public bool ShowDebug = false;


    [System.NonSerialized] public Transform ObjectGhost;
    [System.NonSerialized] public bool PlacementActive;
    [System.NonSerialized] public bool AwaitingNetResponse;

    [System.NonSerialized] public DG_PlayerCharacters.RucksackSlot RucksackSlotOpen;
    [System.NonSerialized] public DG_ItemObject ItemDatabaseReference;

    PlacementType CurrentPlacementType;
    Collider DetectedInTheWay;
    NetworkObject SoilObject;
    int ActiveSlot;
    bool AwaitingResponse;



    Vector3 SavedPosition;
    float SavedDirection;

    //Wait for Animation To Complete
    bool SpawnGhostAfterAnimation;

    bool SafeToPlace = false;


    private void Awake()
    {
        QuickFind.ObjectPlacementManager = this;
    }


    private void Update()
    {
        if (PlacementActive)
        {
            ObjectGhost.position = QuickFind.GridDetection.DetectionPoint.position;
            bool GoodToPlace = false;
            if (ItemDatabaseReference.ItemCat == DG_ItemObject.ItemCatagory.Seeds)
                GoodToPlace = AreaIsClear(SeedPlacementDetection, ObjectGhost.position, .45f);
            else
                GoodToPlace = AreaIsClear(BoxcastDetection, ObjectGhost.position, .45f);

            QuickFind.GridDetection.GridMesh.enabled = GoodToPlace;
            SetMaterialColors(GoodToPlace);
            SafeToPlace = GoodToPlace;
        }
        else SafeToPlace = false;

        if (SpawnGhostAfterAnimation)
        {
            if (QuickFind.NetworkSync.CharacterLink.AnimationSync.MidAnimation) return;
            SpawnGhostAfterAnimation = false;
            SetupItemObjectGhost(RucksackSlotOpen, ItemDatabaseReference, ActiveSlot);
        }
    }






    public void InputDetected(bool isUP)
    {
        bool AllowAction = false;
        if (isUP || QuickFind.GameSettings.AllowActionsOnHold) AllowAction = true;

        if (AllowAction && SafeToPlace)
        {
            if (!QuickFind.NetworkSync.CharacterLink.AnimationSync.CharacterIsGrounded()) return;

            if (SoilDetection())
            {
                if (SoilObject != null && SoilObject.SurrogateObjectIndex != 0) return;
                if (AwaitingNetResponse) return;

                AwaitingNetResponse = true;

                if (!ItemDatabaseReference.isWallItem)
                {
                    AwaitingResponse = true;
                    SavedPosition = ObjectGhost.position;
                    SavedDirection = ObjectGhost.eulerAngles.y;

                    if (QuickFind.GameSettings.DisableAnimations)
                        PlacementHit();
                    else
                    {
                        QuickFind.NetworkSync.CharacterLink.FacePlayerAtPosition(SavedPosition);
                        QuickFind.NetworkSync.CharacterLink.AnimationSync.TriggerAnimation(ItemDatabaseReference.AnimationActionID);
                    }
                }
                else
                    ObjectGhost.GetComponent<DG_DynamicWall>().TriggerPlaceWall();
                DestroyObjectGhost();
                QuickFind.InventoryManager.DestroyRucksackItem(RucksackSlotOpen, ActiveSlot);
                QuickFind.GUI_Inventory.ResetHotbarSlot();
            }
        }
    }


    public void PlacementHit()
    {
        if (!AwaitingResponse) return; AwaitingResponse = false;

        QuickFind.NetworkObjectManager.CreateNetSceneObject(QuickFind.NetworkSync.CurrentScene, NetworkObjectManager.NetworkObjectTypes.Item, ItemDatabaseReference.DatabaseID, RucksackSlotOpen.CurrentStackActive, SavedPosition, SavedDirection);
    }



    bool SoilDetection()
    {
        if (!AreaIsClear(SoilReplaceDetection, QuickFind.GridDetection.DetectionPoint.position, .45f))
            SoilObject = QuickFind.NetworkObjectManager.ScanUpTree(DetectedInTheWay.transform);
        else
            SoilObject = null;

        if (ItemDatabaseReference.RequireTilledEarth) { if (SoilObject != null) return true; else return false; }
        else
        {
            if(SoilObject != null && ItemDatabaseReference.DestroysSoilOnPlacement)
                QuickFind.NetworkSync.RemoveNetworkSceneObject(QuickFind.NetworkSync.CurrentScene, SoilObject.NetworkObjectID);
            return true;
        }
    }



    public void SetupItemObjectGhost(DG_PlayerCharacters.RucksackSlot Rucksack = null, DG_ItemObject Item = null, int slot = 0)
    {
        RucksackSlotOpen = Rucksack;
        ItemDatabaseReference = Item;
        ActiveSlot = slot;

        if (QuickFind.NetworkSync.CharacterLink.AnimationSync.MidAnimation) { SpawnGhostAfterAnimation = true; return; }

        PlacementActive = true;
        QuickFind.GridDetection.ObjectIsPlacing = true;
        QuickFind.GridDetection.GlobalPositioning = false;

        GameObject ToDestroy = null;
        if (ObjectGhost != null) ToDestroy = ObjectGhost.gameObject;

        if(Item.ShowPlacementGhost)
            ObjectGhost = Instantiate(Item.ModelPrefab).transform;
        else
            ObjectGhost = new GameObject().transform;

        ObjectGhost.SetParent(transform);
        ObjectGhost.localScale = new Vector3(Item.DefaultScale, Item.DefaultScale, Item.DefaultScale);

        TurnOffCollidersLoop(ObjectGhost);

        if (Item.isWallItem) ObjectGhost.GetComponent<DG_DynamicWall>().TriggerPlacementMode();
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




    public void DestroyObjectGhost()
    {
        QuickFind.GridDetection.ObjectIsPlacing = false;
        Destroy(ObjectGhost.gameObject);
        PlacementActive = false;
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







    public void SetMaterialColors(bool isGreen)
    {
        foreach (MeshRenderer c in ObjectGhost.GetComponents<MeshRenderer>())
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
}
