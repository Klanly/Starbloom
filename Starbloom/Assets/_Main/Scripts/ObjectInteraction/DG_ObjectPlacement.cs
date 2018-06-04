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
    public LayerMask SoilReplaceDetection;

    [HideInInspector] public Transform ObjectGhost;
    [HideInInspector] public bool PlacementActive;
    [HideInInspector] public bool AwaitingNetResponse;

    PlacementType CurrentPlacementType;
    DG_PlayerCharacters.RucksackSlot RucksackSlotOpen;
    DG_ItemObject ItemDatabaseReference;
    Collider DetectedInTheWay;
    NetworkObject SoilObject;
    int ActiveSlot;

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
            if (AreaIsClear(BoxcastDetection, ObjectGhost.position)) { SetMaterialColors(true); SafeToPlace = true; }
            else { SetMaterialColors(false); SafeToPlace = false; }
        }
        else
            SafeToPlace = false;
    }






    public void InputDetected(bool isUP)
    {
        if(isUP && SafeToPlace)
        {
            if (SoilDetection())
            {
                if (SoilObject != null && SoilObject.SurrogateObjectIndex != 0) return;
                if (AwaitingNetResponse) return;

                AwaitingNetResponse = true;
                QuickFind.NetworkObjectManager.CreateNetSceneObject(QuickFind.NetworkSync.CurrentScene, ItemDatabaseReference.DatabaseID, RucksackSlotOpen.CurrentStackActive, ObjectGhost.position, ObjectGhost.eulerAngles.y);
                DestroyObjectGhost();
                QuickFind.InventoryManager.DestroyRucksackItem(RucksackSlotOpen, ActiveSlot);
                QuickFind.GUI_Inventory.ResetHotbarSlot();
            }
        }
    }



    bool SoilDetection()
    {
        if (!AreaIsClear(SoilReplaceDetection, QuickFind.GridDetection.DetectionPoint.position))
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



    public void SetupItemObjectGhost(PlacementType TypeToPlace, DG_PlayerCharacters.RucksackSlot Rucksack = null, DG_ItemObject Item = null, int slot = 0)
    {
        RucksackSlotOpen = Rucksack;
        ItemDatabaseReference = Item;
        ActiveSlot = slot;
        QuickFind.GridDetection.ObjectIsPlacing = true;
        QuickFind.GridDetection.GlobalPositioning = false;

        GameObject ToDestroy = null;
        if (ObjectGhost != null) ToDestroy = ObjectGhost.gameObject;

        if(!ItemDatabaseReference.isGrowableItem)
            ObjectGhost = Instantiate(Item.ModelPrefab).transform;
        else
            ObjectGhost = Instantiate(Item.PreviewItem).transform;
        ObjectGhost.SetParent(transform);
        ObjectGhost.localScale = new Vector3(Item.DefaultScale, Item.DefaultScale, Item.DefaultScale);

        foreach (Collider c in ObjectGhost.GetComponents<Collider>()) c.enabled = false;

        PlacementActive = true;

        if (ToDestroy != null) Destroy(ToDestroy);
    }




    public void DestroyObjectGhost()
    {
        QuickFind.GridDetection.ObjectIsPlacing = false;
        Destroy(ObjectGhost.gameObject);
        PlacementActive = false;
    }





    public bool AreaIsClear(LayerMask DetectionType, Vector3 CastPoint)
    {
        //This will have to be adjusted at a later point to fit larger objects.

        RaycastHit m_Hit;
        CastPoint.y += 20;

        if (Physics.BoxCast(CastPoint, new Vector3(.5f, .5f, .5f), Vector3.down, out m_Hit, transform.rotation, 21, DetectionType))
        { DetectedInTheWay = m_Hit.collider; return false; }
        else
            return true;
    }







    public void SendOutSurrogateSearch(GameObject Spawn)
    {
        if (!AreaIsClear(SoilReplaceDetection, Spawn.transform.position))
        {
            DG_ContextObject CO = DetectedInTheWay.GetComponent<DG_ContextObject>();
            NetworkObject NO = QuickFind.NetworkObjectManager.ScanUpTree(CO.transform);
            NetworkObject NO2 = QuickFind.NetworkObjectManager.ScanUpTree(Spawn.transform);

            int[] OutData = new int[3];
            OutData[0] = NO.transform.parent.GetComponent<NetworkScene>().SceneID;
            OutData[1] = NO.NetworkObjectID;
            OutData[2] = NO2.NetworkObjectID;
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
