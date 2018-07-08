using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif



public class NetworkScene : MonoBehaviour {


    public int SceneID;
    public int SceneOwnerID;
    [System.NonSerialized]
    public List<NetworkObject> TempNetworkObjectList;
    [System.NonSerialized]
    public List<NetworkObject> NetworkObjectList;


    [System.NonSerialized] public GameObject JunkObject;
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
            NetworkObject ListNO = TempNetworkObjectList[i];
            GameObject GO = new GameObject();
            GO.transform.SetParent(transform);
            NetworkObject NO = GO.AddComponent<NetworkObject>();
            NetworkObjectList.Add(NO);
            
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
            {
                bool InitializeAI = false;
                if (SceneID == QuickFind.NetworkSync.CurrentScene) InitializeAI = true;
                NO.SpawnNetworkObject(QuickFind.NetworkObjectManager.GetSceneByID(SceneID), InitializeAI);
            }
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
        Debug.Log("Network Object Not Found");
        return null;
    }









    //Scene Ownership
    public void SelfEnteredScene()
    {
        if (SceneOwnerID == 0 && !SomeoneElseIsInThisScene()) RequestsSceneMaster(QuickFind.NetworkSync.UserID);
    }
    public void UserLeftScene(DG_NetworkSync.Users U)
    {
        if (SceneOwnerID == U.ID) SceneOwnerID = 0;
        if (QuickFind.NetworkSync.UserID != SceneOwnerID && QuickFind.NetworkSync.CurrentScene == SceneID) RequestsSceneMaster(QuickFind.NetworkSync.UserID);
    }



    public void RequestsSceneMaster(int UserID)
    {
        int[] IntGroup = new int[2];
        IntGroup[0] = SceneID;
        IntGroup[1] = UserID;
        QuickFind.NetworkSync.UserRequestingOwnership(IntGroup);
    }
    public void ReceivedMasterRequest(int NewOwner)
    {
        if (SceneOwnerID == 0)
            SceneOwnerID = NewOwner;
    }









    public bool SomeoneElseIsInThisScene()
    {
        for (int i = 0; i < QuickFind.NetworkSync.UserList.Count; i++)
        {
            DG_NetworkSync.Users U = QuickFind.NetworkSync.UserList[i];
            if (U.ID == QuickFind.NetworkSync.UserID) continue;
            if (U.SceneID == SceneID) return true;
        }
        return false;
    }
    public bool AnyoneIsInThisScene()
    {
        for (int i = 0; i < QuickFind.NetworkSync.UserList.Count; i++)
        {
            DG_NetworkSync.Users U = QuickFind.NetworkSync.UserList[i];
            if (U.SceneID == SceneID) return true;
        }
        return false;
    }






#if UNITY_EDITOR
    public void InternalGizmos() //Draw Gizmo in Scene view
    {
        int ActiveScene = QuickFindInEditor.GetEditorSceneList().GetSceneIndexByString(EditorSceneManager.GetActiveScene().name);
        if (ActiveScene != SceneID) return;

        for(int i = 0; i < transform.childCount; i++)
        {
            NetworkObject NO = transform.GetChild(i).GetComponent<NetworkObject>();

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
        }
    }
#endif
}
