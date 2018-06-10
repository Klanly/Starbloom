using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class DG_PrefabPoolItem : MonoBehaviour {

    [HideInInspector]
    public int DatabaseID;
    [HideInInspector]
    public bool LockItem;


    public class PoolObject
    {
        public bool IsActive;
        public int PoolID;
        public GameObject GameObjectRef;

        public int PrefabID;
    }



    public string Name;
    public GameObject PrefabReference;

    [Header("Pool Size")]
    public bool UseMaxPoolValue;
    [ShowIf("UseMaxPoolValue")]
    public int PoolMaxValue;

    [Header("GenerateOnStart")]
    public bool GeneratePoolOnStart;
    [ShowIf("GeneratePoolOnStart")]
    public int GenerationValue;

    [Header("Debug")]
    public bool PrintDebugWhenHitPoolMax;



    List<PoolObject> PrefabPool;



    private void Awake()
    {
        PrefabPool = new List<PoolObject>();
    }
    private void Start()
    {
        if (QuickFind.GameSettings.DisableAllPoolSpawningAtStart) return;
        if (GeneratePoolOnStart) { for (int i = 0; i < GenerationValue; i++) GenerateNewPoolObject(false); }
    }



    PoolObject GenerateNewPoolObject(bool SpawnActive)
    {
        PoolObject PO = new PoolObject();
        PO.PoolID = PrefabPool.Count;
        PO.GameObjectRef = SpawnNewPrefab(SpawnActive, PO);
        PO.PrefabID = DatabaseID;
        PrefabPool.Add(PO);
        return PO;
    }
    GameObject SpawnNewPrefab(bool SpawnActive, PoolObject PO)
    {
        GameObject GO = Instantiate(PrefabReference);
        GO.transform.SetParent(transform);
        DG_PoolLink PL = GO.AddComponent<DG_PoolLink>();
        PL.PoolObjectRef = PO;
        if(!SpawnActive) GO.SetActive(false);
        return GO;
    }




    public GameObject GetAvailablePoolObject()
    {
        for(int i = 0; i < PrefabPool.Count; i++)
        {
            PoolObject PO = PrefabPool[i];
            if (PO.IsActive) continue;
            else { PO.IsActive = true; PO.GameObjectRef.SetActive(true); return PO.GameObjectRef; }
        }
        if (!UseMaxPoolValue || PrefabPool.Count < PoolMaxValue)
        {
            PoolObject PO = GenerateNewPoolObject(true);
            PO.IsActive = true;
            return PO.GameObjectRef;
        }

        if (PrintDebugWhenHitPoolMax) Debug.Log("We have reached our Max Pool Length");
        return null;
    }

    public void ReturnPoolObject(PoolObject ReturnObject)
    {
        ReturnObject.GameObjectRef.SetActive(false);
        ReturnObject.IsActive = false;
    }
}
