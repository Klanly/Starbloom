using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif



public class NetworkScene : MonoBehaviour {


    public int SceneID;
    [HideInInspector]
    public List<NetworkObject> TempNetworkObjectList;
    [HideInInspector]
    public List<NetworkObject> NetworkObjectList;


    [HideInInspector] public GameObject JunkObject;

    int wait1 = 0;

    private void Awake()
    {
        NetworkObjectList = new List<NetworkObject>();
        TempNetworkObjectList = new List<NetworkObject>();
        this.enabled = false;
    }

    private void Update()
    {
        wait1 = wait1 - 1;
        if(wait1 < 0)
        {
            LoadAfterFrame();
            this.enabled = false;
        }
    }







    public void LoadSceneObjects()
    {
        wait1 = 2;
        this.enabled = true;
    }
    void LoadAfterFrame()
    {
        for (int i = 0; i < TempNetworkObjectList.Count; i++)
        {
            GameObject GO = new GameObject();
            GO.transform.SetParent(transform);
            NetworkObject NO = GO.AddComponent<NetworkObject>();
            NetworkObjectList.Add(NO);
            NetworkObject ListNO = TempNetworkObjectList[i];
            NO.Clone(NO, ListNO);
            if (NO.NetworkObjectID == 0) NO.NetworkObjectID = GetValidNextNetworkID();
            NO.transform.position = new Vector3(((float)NO.PositionX / 100), ((float)NO.PositionY / 100), ((float)NO.PositionZ / 100));
            NO.transform.eulerAngles = new Vector3(0, ((float)NO.YFacing / 100), 0);

            if (NO.GrowthValue != 0)
            {
                NO.ActiveVisual = QuickFind.NetworkGrowthHandler.GetCurrentVisualByGrowthValue(NO);
                QuickFind.NetworkGrowthHandler.SetActiveVisual(QuickFind.NetworkObjectManager.GetSceneByID(SceneID), NO, false);
            }
            else
                NO.SpawnNetworkObject(QuickFind.NetworkObjectManager.GetSceneByID(SceneID));
        }
        DestroyTempObjects();
    }
    public void DestroyTempObjects()
    {
        TempNetworkObjectList.Clear();
        if(JunkObject != null) Destroy(JunkObject);
    }
    public void AddInitialPlacedObjectsIntoList()
    {
        JunkObject = new GameObject();
        for (int i = 0; i < transform.childCount; i++)
        {
            NetworkObject NO = transform.GetChild(i).GetComponent<NetworkObject>();
            NetworkObject NOT = JunkObject.AddComponent<NetworkObject>();
            NO.Clone(NOT, NO);
            TempNetworkObjectList.Add(NO);
            Destroy(transform.GetChild(i).gameObject);
        }
    }






    public int GetValidNextNetworkID()
    {
        int index = 0;
        for (int i = 0; i < NetworkObjectList.Count; i++)
        {
            NetworkObject NO = NetworkObjectList[i];
            if (NO.NetworkObjectID > index) index = NO.NetworkObjectID;
        }
        index++;
        return index;
    }
    public NetworkObject GetObjectByID(int ID)
    {
        for (int i = 0; i < NetworkObjectList.Count; i++)
        {
            NetworkObject NO = NetworkObjectList[i];
            if (NO.NetworkObjectID == ID)
                return NO;
        }
        return null;
    }










#if UNITY_EDITOR
    public void InternalGizmos() //Draw Gizmo in Scene view
    {
<<<<<<< .merge_file_a00492
        int ActiveScene = QuickFindInEditor.GetEditorSceneList().GetSceneIDByString(EditorSceneManager.GetActiveScene().name);
=======
        int ActiveScene = QuickFindInEditor.GetEditorSceneList().GetSceneIndexByString(EditorSceneManager.GetActiveScene().name);
>>>>>>> .merge_file_a13488
        if (ActiveScene != SceneID) return;

        for(int i = 0; i < transform.childCount; i++)
        {
            NetworkObject NO = transform.GetChild(i).GetComponent<NetworkObject>();
<<<<<<< .merge_file_a00492
            DG_ItemsDatabase IDB = QuickFindInEditor.GetEditorItemDatabase();
            NO.ItemRefID = QuickFind.GetIfWithinBounds(NO.ItemRefID, 0, IDB.ItemCatagoryList.Length);
            DG_ItemObject IO = IDB.GetItemFromID(NO.ItemRefID);
            if (IO == null) continue;
            NO.ItemQualityLevel = QuickFind.GetIfWithinBounds(NO.ItemQualityLevel, 0, IO.GetMax());
            GameObject Prefab = IO.GetPrefabReferenceByQuality(NO.ItemQualityLevel);
            float Scale = IO.DefaultScale;
            Vector3 localScale = new Vector3(Scale, Scale, Scale);
            NO.DrawMesh(Prefab, localScale);
=======

            if (NO.ObjectType == NetworkObjectManager.NetworkObjectTypes.Item)
            {
                DG_ItemsDatabase IDB = QuickFindInEditor.GetEditorItemDatabase();
                NO.ItemRefID = QuickFind.GetIfWithinBounds(NO.ItemRefID, 0, IDB.ItemCatagoryList.Length);
                DG_ItemObject IO = IDB.GetItemFromID(NO.ItemRefID);
                if (IO == null) continue;
                NO.ItemQualityLevel = QuickFind.GetIfWithinBounds(NO.ItemQualityLevel, 0, IO.GetMax());
                GameObject Prefab = IO.GetPrefabReferenceByQuality(NO.ItemQualityLevel);
                float Scale = IO.DefaultScale;
                Vector3 localScale = new Vector3(Scale, Scale, Scale);
                NO.DrawMesh(Prefab, localScale);
            }
            if (NO.ObjectType == NetworkObjectManager.NetworkObjectTypes.Enemy)
            {
                DG_EnemyDatabase EDB = QuickFindInEditor.GetEnemyDatabase();
                NO.ItemRefID = QuickFind.GetIfWithinBounds(NO.ItemRefID, 0, EDB.ItemCatagoryList.Length);
                DG_EnemyObject EO = EDB.GetItemFromID(NO.ItemRefID);
                if (EO == null) continue;
                GameObject Prefab = EO.PrefabRef;
                Vector3 localScale = new Vector3(1, 1, 1);
                NO.DrawMesh(Prefab, localScale);
            }
>>>>>>> .merge_file_a13488
        }
    }
#endif
}
