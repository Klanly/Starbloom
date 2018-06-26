using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_CharacterLink : MonoBehaviour {

    [Header("CharacterBodyReferences")]
    public Transform MainBodyRef;
    public Transform SkeletonRoot;
    [Header("Hair")]
    public Transform HairAttachpoint;
    public Vector9 TransformData;
    public List<CapsuleCollider> HairColliders;
    [Header("AttachedClothingItems")]
    public List<DG_ClothingHairManager.AttachedClothing> AttachedClothes;


    [Header("Debug")]
    public bool DoNotDisableOnStart = false;

    bool Allow = false;



    private void Awake()
    {
        AttachedClothes = new List<DG_ClothingHairManager.AttachedClothing>();
        if (!DoNotDisableOnStart)
        {
            transform.GetComponent<ECM.Components.CharacterMovement>().enabled = false;
            transform.GetComponent<ECM.Examples.EthanPlatformerController>().enabled = false;
        }
    }


    private void Start()
    {
        if (!DoNotDisableOnStart)
            transform.SetParent(QuickFind.CharacterManager.transform);
        else
        {
            QuickFind.PlayerTrans = transform;
            transform.GetComponent<DG_MovementSync>().enabled = false;
            QuickFind.PlayerTrans.GetChild(0).GetComponent<DG_AnimationSync>().enabled = false;
        }
    }



    public void ActivatePlayer()
    {
        Allow = true;
        this.enabled = true;
    }
    private void Update()
    {
        if (QuickFind.NetworkSync == null || QuickFind.NetworkSync.UserID == 0)
            return;

        if(!Allow)
        {
            this.enabled = false;
            return;
        }

        transform.GetComponent<ECM.Components.CharacterMovement>().enabled = true;
        transform.GetComponent<ECM.Examples.EthanPlatformerController>().enabled = true;
        transform.GetComponent<DG_MagnetAttraction>().isOwner = true;
        transform.GetComponent<DG_MovementSync>().isPlayer = true;

        QuickFind.PlayerTrans = transform;
        QuickFind.PlayerTrans.GetChild(0).GetComponent<DG_AnimationSync>().isPlayer = true;

        QuickFind.InputController.MainPlayer.CharLink = this;

        this.enabled = false;
    }
}
