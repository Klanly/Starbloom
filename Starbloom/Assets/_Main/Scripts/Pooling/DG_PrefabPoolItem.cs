using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Audio;

public class DG_PrefabPoolItem : MonoBehaviour {

    public enum MaxPoolReached
    {
        ReplaceOldest,
        JustDontSpawn,
        ReplaceByDistance
    }


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

    [Header("Generation Reference")]
    public GameObject PrefabReference;
    public bool UseSFXObject;
    [ShowIf("UseSFXObject")]
    public AudioClip AudioFile;

    [Header("FX Pool Size")]
    public bool UseMaxPoolValue;
    [ShowIf("UseMaxPoolValue")]
    public int PoolMaxValue;
    [ShowIf("UseMaxPoolValue")]
    public MaxPoolReached HowToHandleMaxPool;
  

    [Header("GenerateOnStart")]
    public bool GeneratePoolOnStart;
    [ShowIf("GeneratePoolOnStart")]
    public int GenerationValue;



    List<PoolObject> PrefabPool;
    int FXIndex = 0;


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
        GameObject GO = null;
        if (!UseSFXObject) GO = Instantiate(PrefabReference);
        else
        {
            GO = new GameObject();
            AudioSource AS = GO.AddComponent<AudioSource>();
            AS.playOnAwake = false;
            AS.clip = AudioFile;
        }
        GO.transform.SetParent(transform);
        DG_PoolLink PL = GO.AddComponent<DG_PoolLink>();
        PL.PoolObjectRef = PO;
        if(!SpawnActive) GO.SetActive(false);
        return GO;
    }




    public GameObject GetAvailablePoolObject()
    {
        if (!UseMaxPoolValue) return GetPoolPrefabByActive(false);
        else
        {
            if (HowToHandleMaxPool == MaxPoolReached.ReplaceOldest) return GetObjectByIndex();
            if (HowToHandleMaxPool == MaxPoolReached.JustDontSpawn) return GetPoolPrefabByActive(true);
            if (HowToHandleMaxPool == MaxPoolReached.ReplaceByDistance) Debug.Log("Todo, replace by distance logic");
        }

        return null;
    }
    GameObject GetPoolPrefabByActive(bool DontSpawnPastMax)
    {
        PoolObject PO;
        for (int i = 0; i < PrefabPool.Count; i++)
        {
            PO = PrefabPool[i];
            if (PO.IsActive) continue;
            else { PO.IsActive = true; PO.GameObjectRef.SetActive(true); return PO.GameObjectRef; }
        }
        if (DontSpawnPastMax && PrefabPool.Count == PoolMaxValue) return null;

        PO = GenerateNewPoolObject(true);
        PO.IsActive = true;
        return PO.GameObjectRef;     
    }
    GameObject GetObjectByIndex()
    {
        if (PoolMaxValue == 0) Debug.Log("Pool Max Value Has not been set for an FX Object " + Name);
        PoolObject PO;
        if (FXIndex < PrefabPool.Count)
            PO = PrefabPool[FXIndex];
        else
            PO = GenerateNewPoolObject(true);
        FXIndex++;
        if (FXIndex == PoolMaxValue) FXIndex = 0;

        return PO.GameObjectRef;
    }


    public void ReturnPoolObject(PoolObject ReturnObject)
    {
        ReturnObject.GameObjectRef.SetActive(false);
        ReturnObject.IsActive = false;
    }



    [Button(ButtonSizes.Small)] public void SyncNameToPrefabRef() { Name = PrefabReference.name; }
    [Button(ButtonSizes.Small)] public void SyncNameToAudioFileName() { Name = "SFX - " + AudioFile.name; }
}
