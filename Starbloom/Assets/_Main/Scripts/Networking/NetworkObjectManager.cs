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
            GenerateSceneObjects(QuickFind.NetworkSync.CurrentScene);
        }
        else
        {
            for(int i = 0; i < transform.childCount; i++)
            {
                Transform Child = transform.GetChild(i);
                for(int iN = 0; iN < Child.childCount; iN++)
                    Destroy(Child.GetChild(iN).gameObject);
            }

            QuickFind.NetworkSync.RequestWorldObjects();
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
                for(int iN = 0; iN < Child.childCount; iN++)
                    Child.GetChild(iN).GetComponent<NetworkObject>().SpawnNetworkObject();
                return;
            }
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
