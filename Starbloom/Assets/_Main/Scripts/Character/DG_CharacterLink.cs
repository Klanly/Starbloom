using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class DG_CharacterLink : MonoBehaviour {

    [System.Serializable]
    public class AttachmentPoints
    {
        public DG_ClothingHairManager.ClothHairType AttachType;
        public Transform AttachmentPoint;
    }

    [Header("References")]
    [ReadOnly] public int PlayerID = -2;
    [ReadOnly] public CameraLogic.PlayerCamRigg PlayerCam;
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
    Transform TargetingHelper;
    Transform TargetingHelper2;

    [System.NonSerialized] public Transform PlayerTrans;


    private void Awake()
    {
        PlayerTrans = MoveSync.transform;
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
            transform.SetParent(QuickFind.CharacterManager.transform);
        else
        {
            QuickFind.PlayerTrans = MoveSync.transform;
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
        if (PlayerID == -2 || QuickFind.NetworkSync == null) return;

        if(!Allow)
        {
            this.enabled = false;
            return;
        }

        CharMovement.enabled = true;
        CharController.enabled = true;
        MagnetAttract.isOwner = true;
        MoveSync.isPlayer = true;

        AnimationSync.isPlayer = true;
        QuickFind.CombatHandler.PlayerDashAttackHitboxes.transform.SetParent(PlayerTrans);
        QuickFind.CombatHandler.PlayerDashAttackHitboxes.transform.localPosition = Vector3.zero;
        QuickFind.CombatHandler.PlayerDashAttackHitboxes.transform.localRotation = Quaternion.identity;
        QuickFind.CombatHandler.PlayerDashAttackHitboxes.SetActive(false);



        DG_PlayerInput.Player MP = QuickFind.InputController.GetPlayerByPlayerID(PlayerID);
        MP.CharLink = this;

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
        TargetingHelper2.position = PlayerTrans.position;

        TargetingHelper2.LookAt(TargetingHelper);

        Vector3 CurRot = PlayerTrans.eulerAngles;
        CurRot.y = TargetingHelper2.eulerAngles.y;
        PlayerTrans.eulerAngles = CurRot;
    }

    public void CenterCharacterX()
    {
        Vector3 PlayerAngle = PlayerTrans.eulerAngles;
        PlayerAngle.x = 0;
        PlayerTrans.eulerAngles = PlayerAngle;
    }
}
