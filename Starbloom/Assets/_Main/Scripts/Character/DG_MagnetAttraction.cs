﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_MagnetAttraction : MonoBehaviour {

    public class MagneticTracker
    {
        public int OwnerID;
        public Transform Trans;
    }

    public LayerMask MagneticMask;
    public float BaseMagnetRange;
    public float PullSpeed;

    [HideInInspector] public bool isOwner = false;
    [HideInInspector] public List<MagneticTracker> MagnetObjects;
    Transform AdjustedHeight;


    private void Awake()
    {
        MagnetObjects = new List<MagneticTracker>();
        AdjustedHeight = new GameObject().transform;
        AdjustedHeight.SetParent(transform);
        AdjustedHeight.localPosition = new Vector3(0, .7f, 0);
    }


    public void Update()
    {
        if (isOwner) CheckCollision();

        for (int i = 0; i < MagnetObjects.Count; i++)
        {
            MagneticTracker MT = MagnetObjects[i];
            if(MT.Trans == null)
            {
                MagnetObjects.Remove(MT);
                continue;
            }


            AttractMotion(MT.Trans);


            //If Close then destroy
            if (MT.OwnerID == QuickFind.NetworkSync.PlayerCharacterID && QuickFind.WithinDistance(MT.Trans, AdjustedHeight, .05f))
            {
                NetworkObject NO = QuickFind.NetworkObjectManager.ScanUpTree(MT.Trans);
                if(QuickFind.InventoryManager.AddItemToRucksack(QuickFind.NetworkSync.PlayerCharacterID, NO.ItemRefID, (DG_ItemObject.ItemQualityLevels)NO.ItemQualityLevel, false))
                    QuickFind.NetworkSync.RemoveNetworkSceneObject(NO.transform.parent.GetComponent<NetworkScene>().SceneID, NO.NetworkObjectID);
            }
        }
    }

    //Move Towards Player
    void AttractMotion(Transform MagnetTransform)
    {

        MagnetTransform.position = Vector3.MoveTowards(MagnetTransform.position, AdjustedHeight.position, PullSpeed * Time.deltaTime);
    }








    void CheckCollision()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, GetMagnetRadius(), MagneticMask);
        for (int i = 0; i < hitColliders.Length; i++)
        {
            DG_MagneticItem MI = hitColliders[i].GetComponent<DG_MagneticItem>();
            if (MI.AllowMagnetic && !MI.Claimed) RequestObject(MI);
        }
    }

    public float GetMagnetRadius()
    {
        //Add equipment buffs here.
        return BaseMagnetRange;
    }

    void RequestObject(DG_MagneticItem MI)
    {
        NetworkObject NO = QuickFind.NetworkObjectManager.ScanUpTree(MI.transform);
        if (!QuickFind.InventoryManager.AddItemToRucksack(QuickFind.NetworkSync.PlayerCharacterID, NO.ItemRefID, (DG_ItemObject.ItemQualityLevels)NO.ItemQualityLevel, false, true)) return;
        
        int[] Sent = new int[3];
        Sent[0] = QuickFind.NetworkSync.PlayerCharacterID;
        Sent[1] = QuickFind.NetworkSync.CurrentScene;
        Sent[2] = NO.NetworkObjectID;

        QuickFind.NetworkSync.ClaimMagneticObject(Sent);
    }

    public void ClaimObject(int[] Data)
    {
        NetworkObject NO = QuickFind.NetworkObjectManager.GetItemByID(Data[1], Data[2]);
        DG_MagneticItem MI = NO.transform.GetChild(0).GetComponent<DG_MagneticItem>();

        if (MI.Claimed) return; //if two charaters standing close to spawn point, and send simultanious request.
        else if (!QuickFind.InventoryManager.AddItemToRucksack(QuickFind.NetworkSync.PlayerCharacterID, NO.ItemRefID, (DG_ItemObject.ItemQualityLevels)NO.ItemQualityLevel, false)) return;

        MI.Claimed = true;

        MagneticTracker MT = new MagneticTracker();
        MT.Trans = MI.transform;
        MT.OwnerID = Data[0];
        MagnetObjects.Add(MT);
    }
}
