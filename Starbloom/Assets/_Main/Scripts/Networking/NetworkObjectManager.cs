using System.Collections;
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
