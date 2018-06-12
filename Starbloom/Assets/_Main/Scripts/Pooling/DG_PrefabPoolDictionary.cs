using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_PrefabPoolDictionary : MonoBehaviour {



    [HideInInspector]
    public DG_PrefabPoolItem[] ItemCatagoryList;
    [HideInInspector]
    public int ListCount;


    private void Awake(){QuickFind.PrefabPool = this;}


    public GameObject GetPoolItemByPrefabID(int PrefabID) { return GetItemFromID(PrefabID).GetAvailablePoolObject(); }
    public GameObject GetPoolItemByFXID(int PrefabID) { return GetItemFromID(PrefabID).GetFXObjectByIndex(); }
    public void ReturnPoolItem(DG_PrefabPoolItem.PoolObject PoolObject) { GetItemFromID(PoolObject.PrefabID).ReturnPoolObject(PoolObject); }


    public DG_PrefabPoolItem GetItemFromID(int ID)
    {
        if (ID < 0) { Debug.Log("Get By ID Failed"); return null; }
        if (ID >= ItemCatagoryList.Length) { Debug.Log("Get By ID Failed"); return null; }

        return ItemCatagoryList[ID];
    }



    //Re-Pool, or Destroy
    public void SafeReturnNetworkPoolObject(NetworkObject Object)
    {
        Transform PrefabChild = Object.transform.GetChild(0);
        if (PrefabChild.GetComponent<DG_PoolLink>() != null)
        {
            DG_PoolLink PL = PrefabChild.GetComponent<DG_PoolLink>() ;
            PL.transform.SetParent(GetItemFromID(PL.PoolObjectRef.PrefabID).transform);
            ReturnPoolItem(PL.PoolObjectRef);
        }
    }
    public void SafeReturnNormalObject(GameObject Object)
    {
        if (Object.GetComponent<DG_PoolLink>() != null)
            ReturnPoolItem(Object.GetComponent<DG_PoolLink>().PoolObjectRef);
    }

}
