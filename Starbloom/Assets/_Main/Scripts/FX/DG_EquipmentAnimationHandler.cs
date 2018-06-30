using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_EquipmentAnimationHandler : MonoBehaviour {

    public enum AnimationType
    {
        OnOff,
        ScaleUpDown
    }
    public enum AnimationStates
    {
        PreUnSheath,
        UnSheath,
        Idle,
        ReSheath
    }
    public class ActiveEquipmentAnimation
    {
        public int UserID;
        public int ClothingID;

        public DG_AnimationSync AnimationSyncRef;
        public AnimationStates CurrentAnimationState;
        public AnimationType Type;

        public GameObject GameObjectLink;
        public DG_ClothingObject.CharacterOffsetPoints OffsetReference;
        public bool ActiveTrackingTime;
        public float TransitionTime;
        public float Timer;
    }

    [System.Serializable]
    public class EquipmentAnimationData
    {
        public AnimationType Type;
        public float TransitionTime;
    }





    List<ActiveEquipmentAnimation> ActiveAnimations;





    private void Awake()
    {
        QuickFind.EquipmentFXManager = this;
        ActiveAnimations = new List<ActiveEquipmentAnimation>();
    }

    private void Update()
    {
        for (int i = 0; i < ActiveAnimations.Count; i++)
        {
            ActiveEquipmentAnimation AEA = ActiveAnimations[i];
            if (AEA.ActiveTrackingTime)
            {
                switch (AEA.CurrentAnimationState)
                {
                    case AnimationStates.UnSheath: AEA.GameObjectLink.SetActive(true); HandleSheathAnimation(AEA, true); break;
                    case AnimationStates.ReSheath: HandleSheathAnimation(AEA, false); break;
                }
            }
        }
    }
    void HandleSheathAnimation(ActiveEquipmentAnimation AEA, bool isUnsheath)
    {
        float NewTime = AEA.Timer - Time.deltaTime;
        if (NewTime < 0)
        {
            AEA.ActiveTrackingTime = false;
            if (!isUnsheath)
            {
                switch (AEA.Type)
                {
                    case AnimationType.OnOff: AEA.GameObjectLink.SetActive(false); break;
                    case AnimationType.ScaleUpDown: AEA.GameObjectLink.SetActive(false); break;
                }
                AEA.AnimationSyncRef.SetAnimationState(false);
                ActiveAnimations.Remove(AEA);             
            }
            else
            {
                AEA.CurrentAnimationState = AnimationStates.Idle;
                switch (AEA.Type)
                {
                    case AnimationType.OnOff: break;
                    case AnimationType.ScaleUpDown: AEA.GameObjectLink.transform.localScale = AEA.OffsetReference.OffsetData.Scale; break;
                }
            }
        }
        else
        {
            //Handle Other Forms of Transistion besides On/Off
        }
    }











    //Setup
    public void SetEquipmentAnimationTracker(int UserID, int ClothingDatabaseID, DG_AnimationSync AnimationSync)
    {
        DG_ClothingObject CO = QuickFind.ClothingDatabase.GetItemFromID(ClothingDatabaseID);
        DG_ClothingObject.ClothingAnimationNumbers CAN = CO.AnimationNumbers[0];
        ActiveEquipmentAnimation AEA = GetAnimationTracker(ClothingDatabaseID, UserID, AnimationSync);

        AEA.AnimationSyncRef = AnimationSync;
        AEA.CurrentAnimationState = AnimationStates.PreUnSheath;
        AEA.Type = CAN.AnimationTransition.Type;
        AEA.ActiveTrackingTime = false;
        AEA.TransitionTime = CAN.AnimationTransition.TransitionTime;
        AEA.Timer = AEA.TransitionTime;

        DG_CharacterLink CL = QuickFind.NetworkSync.GetCharacterLinkByUserID(UserID);
        DG_ClothingHairManager.AttachedClothing AC = QuickFind.ClothingHairManager.GetAttachedClothingReference(CL, DG_ClothingHairManager.ClothHairType.RightHand);
        AEA.OffsetReference = CO.GetCharOffsetRefByGender(CL.Gender);
        AEA.GameObjectLink = AC.ClothingPieces[0];
    }

    //Get Animation Tracker
    ActiveEquipmentAnimation GetAnimationTracker(int ClothingDatabaseID, int UserID, DG_AnimationSync AnimationSync)
    {
        ActiveEquipmentAnimation Return = null;

        //Clear Old
        for (int i = 0; i < ActiveAnimations.Count; i++)
        {
            ActiveEquipmentAnimation AEA = ActiveAnimations[i];
            if (AEA.AnimationSyncRef == AnimationSync)
            {
                ActiveAnimations.Remove(AEA);
                break;
            }
        }

        //Create New
        for (int i = 0; i < ActiveAnimations.Count; i++)
        {
            ActiveEquipmentAnimation AEA = ActiveAnimations[i];
            if (AEA.ClothingID == ClothingDatabaseID && AEA.UserID == UserID)
            {
                Return = ActiveAnimations[i];
                break;
            }
        }
        if (Return == null)
        {
            Return = new ActiveEquipmentAnimation();
            ActiveAnimations.Add(Return);
            Return.ClothingID = ClothingDatabaseID;
            Return.UserID = UserID;
        }
        return Return;
    }




    //Event
    public void AnimationEvent(DG_AnimationSync AnimationSync)
    {
        ActiveEquipmentAnimation AEA = null;
        for (int i = 0; i < ActiveAnimations.Count; i++)
        {
            ActiveEquipmentAnimation NewAEA;
            NewAEA = ActiveAnimations[i];
            if (NewAEA.AnimationSyncRef == AnimationSync) { AEA = NewAEA; break; }
        }


        AEA.ActiveTrackingTime = true;

        switch (AEA.CurrentAnimationState)
        {
            case AnimationStates.PreUnSheath: AEA.CurrentAnimationState = AnimationStates.UnSheath; break;
            case AnimationStates.Idle: AEA.CurrentAnimationState = AnimationStates.ReSheath; break;
        }
    }
}
