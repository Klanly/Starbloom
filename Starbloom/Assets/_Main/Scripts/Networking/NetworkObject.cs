using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;



#if UNITY_EDITOR
using UnityEditor;
#endif



public class NetworkObject : MonoBehaviour {

    [HideInInspector]
    public int NetworkObjectID;
    [Header("---------------------------------------------------------------")]
<<<<<<< .merge_file_a04064
    public int ItemRefID;
    public int ItemQualityLevel;
=======
>>>>>>> .merge_file_a02220
    public int PositionX;
    public int PositionY;
    public int PositionZ;
    public int YFacing;
<<<<<<< .merge_file_a04064

=======
    public NetworkObjectManager.NetworkObjectTypes ObjectType;
    public int ItemRefID;


    [Header("Health -----------------------------------------------------")]
    public bool HasHealth = false;
    [ShowIf("HasHealth")]
    public int HealthValue;


    /////////////////////////////////////////////////////////////////////////////////////
    [Header("ITEM ----------------------------------------------------------")]
    public int ItemQualityLevel;
>>>>>>> .merge_file_a02220
    [Header("Watering -----------------------------------------------------")]
    public bool isWaterable = false;
    [ShowIf("isWaterable")]
    public int SurrogateObjectIndex;
    [ShowIf("isWaterable")]
    public bool HasBeenWatered = false;

<<<<<<< .merge_file_a04064
    [Header("Breaking -----------------------------------------------------")]
    public bool HasHealth = false;
    [ShowIf("HasHealth")]
    public int HealthValue;
    [ShowIf("HasHealth")]
    public int GrowthValue;
    [ShowIf("HasHealth")]
    public int ActiveVisual;
=======
    [ShowIf("HasHealth")]
    public int ActiveVisual;
    [ShowIf("HasHealth")]
    public int GrowthValue;
>>>>>>> .merge_file_a02220

    [Header("Storage -----------------------------------------------------")]
    public bool isStorageContainer = false;
    [ShowIf("isStorageContainer")]
    public bool isTreasureList;
    [ShowIf("isStorageContainer")]
    [ListDrawerSettings(NumberOfItemsPerPage = 12)]
    public DG_PlayerCharacters.RucksackSlot[] StorageSlots;
<<<<<<< .merge_file_a04064

=======
    /////////////////////////////////////////////////////////////////////////////////////
>>>>>>> .merge_file_a02220






    public void SpawnNetworkObject(NetworkScene NS, bool GenerateVelocity = false, Vector3 Velocity = new Vector3())
    {
<<<<<<< .merge_file_a04064
        DG_ItemsDatabase IDB = QuickFind.ItemDatabase;
        DG_ItemObject IO = IDB.GetItemFromID(ItemRefID);
        GameObject Prefab = IO.GetPrefabReferenceByQuality(ItemQualityLevel);

        GameObject Spawn;
        if (IO.UsePoolIDForSpawn && Application.isPlaying) Spawn = Prefab;
        else Spawn = Instantiate(Prefab);
=======
        GameObject Prefab = null;
        GameObject Spawn = null;
        float Scale = 1;

        if (ObjectType == NetworkObjectManager.NetworkObjectTypes.Item)
        {
            DG_ItemsDatabase IDB = QuickFind.ItemDatabase;
            DG_ItemObject IO = IDB.GetItemFromID(ItemRefID);
            Prefab = IO.GetPrefabReferenceByQuality(ItemQualityLevel);

            if (IO.UsePoolIDForSpawn && Application.isPlaying) Spawn = Prefab;
            else Spawn = Instantiate(Prefab);

            Scale = IO.DefaultScale;

            if (IO.isBreakable) { HasHealth = true; HealthValue = IO.EnvironmentValues[0].ObjectHealth; }
            if (HasBeenWatered) QuickFind.WateringSystem.AdjustWateredObjectVisual(this, true);
            if (IO.isWallItem) Spawn.GetComponent<DG_DynamicWall>().DetermineActiveBoolsByID(ItemQualityLevel);
            if (GenerateVelocity) { DG_MagneticItem MI = Spawn.GetComponent<DG_MagneticItem>(); MI.TriggerStart(Velocity); }
            if (IO.RequireTilledEarth) { if (PhotonNetwork.isMasterClient) QuickFind.ObjectPlacementManager.SendOutSurrogateSearch(Spawn); }
        }
        else if(ObjectType == NetworkObjectManager.NetworkObjectTypes.Enemy)
        {
            DG_EnemyObject EO = QuickFind.EnemyDatabase.GetItemFromID(ItemRefID);
            Prefab = EO.PrefabRef;
            if (EO.UsePoolIDForSpawn && Application.isPlaying) Spawn = Prefab;
            else Spawn = Instantiate(Prefab);
            Scale = EO.DefaultScale;

            HasHealth = true; HealthValue = EO.HealthValue;
        }

>>>>>>> .merge_file_a02220

        Transform T = Spawn.transform;
        T.SetParent(transform);
        T.localPosition = Vector3.zero;
        T.localEulerAngles = Vector3.zero;
<<<<<<< .merge_file_a04064
        float Scale = IO.DefaultScale;
        T.localScale = new Vector3(Scale, Scale, Scale);

        if (NS.SceneID != QuickFind.NetworkSync.CurrentScene) transform.gameObject.SetActive(false);
        if (IO.isBreakable) { HasHealth = true; HealthValue = IO.EnvironmentValues[0].ObjectHealth; }
        if (HasBeenWatered) QuickFind.WateringSystem.AdjustWateredObjectVisual(this, true);
        if (IO.isWallItem) Spawn.GetComponent<DG_DynamicWall>().DetermineActiveBoolsByID(ItemQualityLevel);

        if (GenerateVelocity)
        {
            DG_MagneticItem MI = Spawn.GetComponent<DG_MagneticItem>();
            MI.TriggerStart(Velocity);
        }

        transform.name = Spawn.name;


        if(IO.RequireTilledEarth) {if (PhotonNetwork.isMasterClient) QuickFind.ObjectPlacementManager.SendOutSurrogateSearch(Spawn);}
=======
        T.localScale = new Vector3(Scale, Scale, Scale);

        if (NS.SceneID != QuickFind.NetworkSync.CurrentScene) transform.gameObject.SetActive(false);
        transform.name = Spawn.name;
>>>>>>> .merge_file_a02220
    }





    public void Clone(NetworkObject NO, NetworkObject ListNO)
    {
        NO.NetworkObjectID = ListNO.NetworkObjectID;
<<<<<<< .merge_file_a04064
        NO.ItemRefID = ListNO.ItemRefID;
        NO.ItemQualityLevel = ListNO.ItemQualityLevel;
=======

>>>>>>> .merge_file_a02220
        NO.PositionX = ListNO.PositionX;
        NO.PositionY = ListNO.PositionY;
        NO.PositionZ = ListNO.PositionZ;
        NO.YFacing = ListNO.YFacing;

<<<<<<< .merge_file_a04064
=======
        NO.ObjectType = ListNO.ObjectType;
        NO.ItemRefID = ListNO.ItemRefID;

        NO.HasHealth = ListNO.HasHealth;
        NO.HealthValue = ListNO.HealthValue;


        //Item
        NO.ItemQualityLevel = ListNO.ItemQualityLevel;

>>>>>>> .merge_file_a02220
        NO.isWaterable = ListNO.isWaterable;
        NO.HasBeenWatered = ListNO.HasBeenWatered;
        NO.SurrogateObjectIndex = ListNO.SurrogateObjectIndex;

        NO.GrowthValue = ListNO.GrowthValue;

<<<<<<< .merge_file_a04064
        NO.HasHealth = ListNO.HasHealth;
        NO.HealthValue = ListNO.HealthValue;

=======
>>>>>>> .merge_file_a02220
        NO.isStorageContainer = ListNO.isStorageContainer;
        NO.StorageSlots = ListNO.StorageSlots;
        NO.isTreasureList = ListNO.isTreasureList;
    }









#if UNITY_EDITOR
    void OnDrawGizmos() //Draw Gizmo in Scene view
    {
        if (Application.isPlaying)
            return;

        Vector3 Pos = transform.position;
        PositionX = QuickFind.ConvertFloatToInt(Pos.x);
        PositionY = QuickFind.ConvertFloatToInt(Pos.y);
        PositionZ = QuickFind.ConvertFloatToInt(Pos.z);
        YFacing = QuickFind.ConvertFloatToInt(transform.eulerAngles.y);
    }

    public void DrawMesh(GameObject Prefab, Vector3 localScale)
    {
        MeshFilter MF = Prefab.GetComponent<MeshFilter>();
        if (MF != null)
        {
            Mesh M = MF.sharedMesh;
            Gizmos.DrawMesh(M, 0, transform.position, transform.rotation, localScale);
        }
        else
        {
            Vector3 Position = transform.position;
            Position.y += .5f;
            Gizmos.DrawCube(Position, localScale);
        }

        transform.name = Prefab.name;
    }
#endif
}
