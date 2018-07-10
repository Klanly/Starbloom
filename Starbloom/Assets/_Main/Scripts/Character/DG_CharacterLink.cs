using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_CharacterLink : MonoBehaviour {

    [System.Serializable]
    public class AttachmentPoints
    {
        public DG_ClothingHairManager.ClothHairType AttachType;
        public Transform AttachmentPoint;
    }

    [Header("References")]
    public DG_MovementSync MoveSync;
    public DG_MagnetAttraction MagnetAttract;
    public DG_AnimationSync AnimationSync;
    public ECM.Components.CharacterMovement CharMovement;
    public ECM.Examples.EthanPlatformerController CharController;

    [Header("CharacterBodyReferences")]
    public DG_PlayerCharacters.GenderValue Gender;
    public Transform MainBodyRef;
    public Transform SkeletonRoot;
    [Header("AttachPoints")]
    public AttachmentPoints[] GearAttachPoints;
    [Header("Hair")]
    public List<CapsuleCollider> HairColliders;
    [Header("AttachedClothingItems")]
    public List<DG_ClothingHairManager.AttachedClothing> AttachedClothes;


    [Header("Debug")]
    public bool DoNotDisableOnStart = false;

    bool Allow = false;
    [System.NonSerialized] public Transform _T;
    Transform TargetingHelper;
    Transform TargetingHelper2;


    private void Awake()
    {
        _T = transform;
        AttachedClothes = new List<DG_ClothingHairManager.AttachedClothing>();
        if (!DoNotDisableOnStart)
        {
            CharMovement.enabled = false;
            CharController.enabled = false;
        }
    }


    private void Start()
    {
        if (!DoNotDisableOnStart)
            _T.SetParent(QuickFind.CharacterManager.transform);
        else
        {
            QuickFind.PlayerTrans = _T;
            MoveSync.enabled = false;
            AnimationSync.enabled = false;
        }

        TargetingHelper = new GameObject().transform;
        TargetingHelper2 = new GameObject().transform;
        TargetingHelper.SetParent(QuickFind.ContextDetectionHandler.transform);
        TargetingHelper2.SetParent(QuickFind.ContextDetectionHandler.transform);
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

        CharMovement.enabled = true;
        CharController.enabled = true;
        MagnetAttract.isOwner = true;
        MoveSync.isPlayer = true;

        QuickFind.PlayerTrans = _T;
        QuickFind.WeatherController.Player = gameObject;
        AnimationSync.isPlayer = true;

        QuickFind.InputController.MainPlayer.CharLink = this;

        this.enabled = false;
    }


    public AttachmentPoints GetAttachmentByType(DG_ClothingHairManager.ClothHairType Type)
    {
        for(int i = 0; i < GearAttachPoints.Length; i++)
        {
            AttachmentPoints AP = GearAttachPoints[i];
            if (AP.AttachType == Type)
                return AP;
        }
        return null;
    }






    public void EnablePlayerMovement(bool isTrue)
    {
        CharMovement.enabled = isTrue;
        CharController.enabled = isTrue;

        if(!isTrue)
        {
            CharMovement.velocity = Vector3.zero;
        }

    }









    public void FacePlayerAtPosition(Vector3 NewPosition)
    {
        TargetingHelper.position = NewPosition;
        TargetingHelper2.position = _T.position;

        TargetingHelper2.LookAt(TargetingHelper);

        Vector3 CurRot = _T.eulerAngles;
        CurRot.y = TargetingHelper2.eulerAngles.y;
        _T.eulerAngles = CurRot;
    }

    public void CenterCharacterX()
    {
        Vector3 PlayerAngle = _T.eulerAngles;
        PlayerAngle.x = 0;
        _T.eulerAngles = PlayerAngle;
    }
}
