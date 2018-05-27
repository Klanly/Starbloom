using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif



public class NetworkScene : MonoBehaviour {


    public int SceneID;
    [HideInInspector] public List<NetworkObject> NetworkObjectList;

    int wait1 = 0;

    private void Awake()
    {
        NetworkObjectList = new List<NetworkObject>();
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
        for (int i = 0; i < NetworkObjectList.Count; i++)
        {
            GameObject GO = new GameObject();
            GO.transform.SetParent(transform);
            NetworkObject NO = GO.AddComponent<NetworkObject>();
            NetworkObject ListNO = NetworkObjectList[i];
            NO.Clone(NO, ListNO);

            NO.transform.position = NO.Position;
            NO.transform.eulerAngles = new Vector3(0, NO.YFacing, 0);

            NO.SpawnNetworkObject();      
            //
        }
        DestroyObjects();
    }
    public void DestroyObjects()
    {
        for (int i = 0; i < NetworkObjectList.Count; i++)
            Destroy(NetworkObjectList[i]);
        NetworkObjectList.Clear();
    }
    public void AddInitialPlacedObjectsIntoList()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            NetworkObject NO = transform.GetChild(i).GetComponent<NetworkObject>();
            NetworkObject NOT = transform.gameObject.AddComponent<NetworkObject>();
            NO.Clone(NOT, NO);
            NetworkObjectList.Add(NO);
            Destroy(transform.GetChild(i).gameObject);
        }
    }




#if UNITY_EDITOR
    public void InternalGizmos() //Draw Gizmo in Scene view
    {
        int ActiveScene = QuickFindInEditor.GetEditorSceneList().GetSceneIDByString(EditorSceneManager.GetActiveScene().name);
        if (ActiveScene != SceneID) return;

        for(int i = 0; i < transform.childCount; i++)
        {
            NetworkObject NO = transform.GetChild(i).GetComponent<NetworkObject>();
            DG_ItemsDatabase IDB = QuickFindInEditor.GetEditorItemDatabase();
            NO.ItemRefID = QuickFind.GetIfWithinBounds(NO.ItemRefID, 0, IDB.ItemCatagoryList.Length);
            DG_ItemObject IO = IDB.GetItemFromID(NO.ItemRefID);
            if (IO == null) continue;
            NO.ItemGrowthLevel = QuickFind.GetIfWithinBounds(NO.ItemGrowthLevel, 0, IO.GetMax());
            GameObject Prefab = IO.GetPrefabReferenceByQuality(NO.ItemGrowthLevel);
            float Scale = IO.DefaultScale;
            Vector3 localScale = new Vector3(Scale, Scale, Scale);
            NO.DrawMesh(Prefab, localScale);
        }
    }
#endif
}
