using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NetworkObjectManager : MonoBehaviour {

    public enum NetworkObjectTypes
    {
        Item,
        Enemy
    }




    private void Awake()
    {
        QuickFind.NetworkObjectManager = this;
    }








    public void GenerateObjectData()
    {
        if(PhotonNetwork.isMasterClient)
        {
            //Check if Save File Available.

            //if not, keep all things as they are.

            for (int i = 0; i < transform.childCount; i++)
                transform.GetChild(i).GetComponent<NetworkScene>().AddInitialPlacedObjectsIntoList();
            GenerateSceneObjects();
        }
        else
        {
            ClearObjects();
            QuickFind.NetworkSync.RequestWorldObjects();
        }
    }
    public void ClearObjects()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform Child = transform.GetChild(i);
            NetworkScene NS = Child.GetComponent<NetworkScene>();
            NS.DestroyTempObjects();
            for (int iN = 0; iN < Child.childCount; iN++)
                Destroy(Child.GetChild(iN).gameObject);

            NS.NetworkObjectList.Clear();
        }
    }
    public void GenerateSceneObjects()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            NetworkScene NS = transform.GetChild(i).GetComponent<NetworkScene>();
            NS.LoadSceneObjects();
        }
    }














    public NetworkObject GetItemByID(int Scene, int index)
    {
        return GetSceneByID(Scene).GetObjectByID(index);    
    }
    public NetworkObject ScanUpTree(Transform T)
    {
        if (T == null) { Debug.Log("Network Object Transform is Null, this should not happen."); return null; }
        NetworkObject NO = T.GetComponent<NetworkObject>();
        if (NO == null)
            return ScanUpTree(T.parent);
        else
            return NO;
    }
    public NetworkScene GetSceneByID(int index)
    {
        return QuickFind.SceneList.GetSceneByID(index);
    }












    //Outgoing
    public void CreateNetSceneObject(   int SceneID, NetworkObjectTypes ObjectType, int ObjectID, int ItemLevel, Vector3 Position, float Facing, 
                                        bool GenerateVelocity = false, Vector3 Velocity = new Vector3())
    {
        List<int> IntData = new List<int>();

        IntData.Add(SceneID);
        int NewObjectID = QuickFind.NetworkObjectManager.GetSceneByID(SceneID).GetValidNextNetworkID();
        IntData.Add(NewObjectID);
        IntData.Add(QuickFind.ConvertFloatToInt(Position.x));
        IntData.Add(QuickFind.ConvertFloatToInt(Position.y));
        IntData.Add(QuickFind.ConvertFloatToInt(Position.z));
        IntData.Add(QuickFind.ConvertFloatToInt(Facing));
        IntData.Add((int)ObjectType);
        IntData.Add(ObjectID);

        if (ObjectType == NetworkObjectTypes.Item)
        {
            IntData.Add(ItemLevel);

            if (GenerateVelocity) IntData.Add(1); else IntData.Add(0);
            if (GenerateVelocity)
            {
                IntData.Add(QuickFind.ConvertFloatToInt(Velocity.x));
                IntData.Add(QuickFind.ConvertFloatToInt(Velocity.y));
                IntData.Add(QuickFind.ConvertFloatToInt(Velocity.z));
            }
        }


        QuickFind.NetworkSync.AddNetworkSceneObject(IntData.ToArray());
    }
    //Incoming
    public void CreateSceneObject(int[] IncomingData)
    {
        int index = 0;
        NetworkScene NS = GetSceneByID(IncomingData[index]); index++;
        Transform NewObject = new GameObject().transform;
        NewObject.SetParent(NS.transform);

        NetworkObject NO = NewObject.gameObject.AddComponent<NetworkObject>();
        NS.NetworkObjectList.Add(NO);

        NO.NetworkObjectID = IncomingData[index]; index++;
        NO.PositionX = IncomingData[index]; index++;
        NO.PositionY = IncomingData[index]; index++;
        NO.PositionZ = IncomingData[index]; index++;
        NO.YFacing = IncomingData[index]; index++;
        NO.ObjectType = (NetworkObjectTypes)IncomingData[index]; index++;
        NO.ItemRefID = IncomingData[index]; index++;


        if (NO.ObjectType == NetworkObjectTypes.Item)
        {
            NO.ItemQualityLevel = IncomingData[index]; index++;

            DG_ItemObject IO = QuickFind.ItemDatabase.GetItemFromID(NO.ItemRefID);

            if (IO.IsWaterable) { NO.isWaterable = true; }
            if (IO.isStorage)
            {
                NO.isStorageContainer = true;
                NO.StorageSlots = new DG_PlayerCharacters.RucksackSlot[36];
                for (int i = 0; i < 36; i++) NO.StorageSlots[i] = new DG_PlayerCharacters.RucksackSlot();
            }

            NewObject.position = QuickFind.ConvertIntsToPosition(NO.PositionX, NO.PositionY, NO.PositionZ);
            NewObject.eulerAngles = new Vector3(0, QuickFind.ConvertIntToFloat(NO.YFacing), 0);


            int GenerateVelocity = IncomingData[index]; index++;
            if (GenerateVelocity == 1)
            {
                Vector3 Velo;
                Velo.x = QuickFind.ConvertIntToFloat(IncomingData[index]); index++;
                Velo.y = QuickFind.ConvertIntToFloat(IncomingData[index]); index++;
                Velo.z = QuickFind.ConvertIntToFloat(IncomingData[index]); index++;

                NO.SpawnNetworkObject(NS, true, Velo);
            }
            else
                NO.SpawnNetworkObject(NS);

            QuickFind.ObjectPlacementManager.AwaitingNetResponse = false;
        }




    }








#if UNITY_EDITOR
    [Header("Editor Debug")]
    public bool DisplayPreviewsInSceneView = true;
    void OnDrawGizmos() //Draw Gizmo in Scene view
    {
        if (Application.isPlaying)
            return;

        if (!DisplayPreviewsInSceneView) return;
        else
        {
            for (int i = 0; i < transform.childCount; i++)
                transform.GetChild(0).GetComponent<NetworkScene>().InternalGizmos();
        }
    }
#endif
}
