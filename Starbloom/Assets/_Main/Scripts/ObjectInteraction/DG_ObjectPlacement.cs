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

    [HideInInspector] public Transform ObjectGhost;
    [HideInInspector] public bool PlacementActive;


    PlacementType CurrentPlacementType;
    DG_PlayerCharacters.RucksackSlot RucksackSlotOpen;
    DG_ItemObject ItemDatabaseReference;
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
            if (ThisPlaceisSafeToPlaceObject()) { SetMaterialColors(true); SafeToPlace = true; }
            else { SetMaterialColors(false); SafeToPlace = false; }
        }
        else
            SafeToPlace = false;
    }






    public void InputDetected(bool isUP)
    {
        if(isUP && SafeToPlace)
        {
            QuickFind.NetworkObjectManager.CreateNetSceneObject(QuickFind.NetworkSync.CurrentScene, ItemDatabaseReference.DatabaseID, RucksackSlotOpen.CurrentStackActive, ObjectGhost.position, ObjectGhost.eulerAngles.y);
            DestroyObjectGhost();
            QuickFind.InventoryManager.DestroyRucksackItem(RucksackSlotOpen, ActiveSlot);
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

        ObjectGhost = Instantiate(Item.ModelPrefab).transform;
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





    public bool ThisPlaceisSafeToPlaceObject()
    {
        //This will have to be adjusted at a later point to fit larger objects.


        RaycastHit m_Hit;
        Vector3 CastPoint = QuickFind.GridDetection.DetectionPoint.position;
        CastPoint.y += 20;

        if (Physics.BoxCast(CastPoint, new Vector3(.5f, .5f, .5f), Vector3.down, out m_Hit, transform.rotation, 21, BoxcastDetection))
            return false;
        else
            return true;
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
