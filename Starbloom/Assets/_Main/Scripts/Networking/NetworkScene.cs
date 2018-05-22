using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif



public class NetworkScene : MonoBehaviour {


    public int SceneID;





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
