﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NetworkObjectManager : MonoBehaviour {



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
            GenerateSceneObjects(QuickFind.NetworkSync.CurrentScene);
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
            Child.GetComponent<NetworkScene>().DestroyObjects();
            for (int iN = 0; iN < Child.childCount; iN++)
                Destroy(Child.GetChild(iN).gameObject);
        }
    }


    public void GenerateSceneObjects(int Scene)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform Child = transform.GetChild(i);
            NetworkScene NS = Child.GetComponent<NetworkScene>();
            if (NS.SceneID == Scene)
            {
                NS.LoadSceneObjects();
                return;
            }
        }
    }
    public NetworkObject GetItemByID(int Scene, int index)
    {
        return FindObject(Scene, index).GetComponent<NetworkObject>();
    }

    public GameObject FindObject(int Scene, int index)
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            Transform Child = transform.GetChild(i);
            NetworkScene NS = Child.GetComponent<NetworkScene>();
            if (NS.SceneID == Scene)
                return Child.GetChild(index).gameObject;
        }
        return null;
    }
    public NetworkScene GetSceneByID(int index)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform Child = transform.GetChild(i);
            NetworkScene NS = Child.GetComponent<NetworkScene>();
            if (NS.SceneID == index)
                return NS;
        }
        return null;
    }




    //Outgoing
    public void CreateNetSceneObject(int SceneID, int ObjectID, int ItemLevel, Vector3 Position, float Facing, bool GenerateVelocity = false, Vector3 Velocity = new Vector3())
    {
        List<int> IntData = new List<int>();

        IntData.Add(SceneID);
        IntData.Add(ObjectID);
        IntData.Add(ItemLevel);

        IntData.Add(QuickFind.ConvertFloatToInt(Position.x));
        IntData.Add(QuickFind.ConvertFloatToInt(Position.y));
        IntData.Add(QuickFind.ConvertFloatToInt(Position.z));
        IntData.Add(QuickFind.ConvertFloatToInt(Facing));

        DG_ItemObject IO = QuickFind.ItemDatabase.GetItemFromID(ObjectID);

        if(IO.isEnvironment && IO.EnvironmentValues[0].IsWaterable) IntData.Add(1); else IntData.Add(0);
        if (IO.isStorage) IntData.Add(1); else IntData.Add(0);

        if (GenerateVelocity) IntData.Add(1); else IntData.Add(0);
        if(GenerateVelocity)
        {
            IntData.Add(QuickFind.ConvertFloatToInt(Velocity.x));
            IntData.Add(QuickFind.ConvertFloatToInt(Velocity.y));
            IntData.Add(QuickFind.ConvertFloatToInt(Velocity.z));
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

        NO.ItemRefID = IncomingData[index]; index++;
        NO.ItemGrowthLevel = IncomingData[index]; index++;
        NO.PositionX = IncomingData[index]; index++;
        NO.PositionY = IncomingData[index]; index++;
        NO.PositionZ = IncomingData[index]; index++;
        NO.YFacing = IncomingData[index]; index++;

        int Waterable = IncomingData[index]; index++;
        if (Waterable == 1){NO.isWaterable = true;}

        int isStorage = IncomingData[index]; index++;
        if (isStorage == 1)
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

            NO.SpawnNetworkObject(true, Velo);
        }


        else
            NO.SpawnNetworkObject();
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
