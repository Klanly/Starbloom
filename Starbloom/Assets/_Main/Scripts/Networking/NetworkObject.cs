using System.Collections;
using System.Collections.Generic;
using UnityEngine;



#if UNITY_EDITOR
using UnityEditor;
#endif



public class NetworkObject : MonoBehaviour {

    public int ItemRefID;
    [Range(0,5)]
    public int ItemGrowthLevel;
    public Vector3 Position;
    public float YFacing;




    public void SpawnNetworkObject()
    {
        DG_ItemsDatabase IDB = QuickFind.ItemDatabase;
        DG_ItemObject IO = IDB.GetItemFromID(ItemRefID);
        GameObject Prefab = IO.GetPrefabReferenceByQuality(ItemGrowthLevel);

        GameObject Spawn = Instantiate(Prefab);
        Transform T = Spawn.transform;
        T.SetParent(transform);
        T.localPosition = Vector3.zero;
        T.localEulerAngles = Vector3.zero;
        float Scale = IO.DefaultScale;
        T.localScale = new Vector3(Scale, Scale, Scale);
    }








#if UNITY_EDITOR
    void OnDrawGizmos() //Draw Gizmo in Scene view
    {
        if (Application.isPlaying)
            return;

        Position = transform.position;
        YFacing = transform.eulerAngles.y;
    }

    public void DrawMesh(GameObject Prefab, Vector3 localScale)
    {
        Mesh M = Prefab.GetComponent<MeshFilter>().sharedMesh;
        Gizmos.DrawMesh(M, 0, transform.position, transform.rotation, localScale);
        transform.name = Prefab.name;
    }
#endif
}
