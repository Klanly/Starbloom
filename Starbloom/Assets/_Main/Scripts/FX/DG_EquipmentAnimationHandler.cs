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
    public class ActiveAnimation
    {
        public int PlayerID;
        public int AnimationID;

        public DG_AnimationSync AnimationSyncRef;
        public AnimationStates CurrentAnimationState;
        public AnimationType Type;

        //Tool Activate/Deactivate Animation Data
        public GameObject GameObjectLink;
        public Vector3 OriginalScale;
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





    List<ActiveAnimation> ActiveAnimations;





    private void Awake()
    {
        QuickFind.EquipmentFXManager = this;
        ActiveAnimations = new List<ActiveAnimation>();
    }

    private void Update()
    {
        for (int i = 0; i < ActiveAnimations.Count; i++)
        {
            ActiveAnimation AEA = ActiveAnimations[i];
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
    void HandleSheathAnimation(ActiveAnimation AEA, bool isUnsheath)
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
                AEA.AnimationSyncRef.SetMovementControlState(false);
                AEA.AnimationSyncRef.SetAllowWeaponSwitching(false);
                ActiveAnimations.Remove(AEA);             
            }
            else
            {
                AEA.CurrentAnimationState = AnimationStates.Idle;
                switch (AEA.Type)
                {
                    case AnimationType.OnOff: break;
                    case AnimationType.ScaleUpDown: AEA.GameObjectLink.transform.localScale = AEA.OriginalScale; break;
                }
            }
        }
        else
        {
            //Handle Other Forms of Transistion besides On/Off
        }
    }











    //Setup
    public void SetEquipmentAnimationTracker(int PlayerID, int AnimationID, DG_AnimationSync AnimationSync)
    {
        DG_AnimationObject AO = QuickFind.AnimationDatabase.GetItemFromID(AnimationID);
        ActiveAnimation AEA = GetAnimationTracker(AnimationID, PlayerID, AnimationSync);

        AEA.AnimationSyncRef = AnimationSync;
        AEA.CurrentAnimationState = AnimationStates.PreUnSheath;
        AEA.Type = AO.SheathUnSheathTransition.Type;
        AEA.ActiveTrackingTime = false;
        AEA.TransitionTime = AO.SheathUnSheathTransition.TransitionTime;
        AEA.Timer = AEA.TransitionTime;

        DG_CharacterLink CL = QuickFind.NetworkSync.GetCharacterLinkByPlayerID(PlayerID);
        DG_ClothingHairManager.AttachedClothing AC = QuickFind.ClothingHairManager.GetAttachedClothingReference(CL, DG_ClothingHairManager.ClothHairType.RightHand);
        AEA.OriginalScale = AC.ClothingRef.GetCharOffsetRefByGender(CL.Gender).OffsetData.Scale;
        AEA.GameObjectLink = AC.ClothingPieces[0];
    }

    //Get Animation Tracker
    ActiveAnimation GetAnimationTracker(int AnimationID, int PlayerID, DG_AnimationSync AnimationSync)
    {
        ActiveAnimation Return = null;

        //Clear Old
        for (int i = 0; i < ActiveAnimations.Count; i++)
        {
            ActiveAnimation AEA = ActiveAnimations[i];
            if (AEA.AnimationSyncRef == AnimationSync)
            {
                ActiveAnimations.Remove(AEA);
                break;
            }
        }

        //Create New
        for (int i = 0; i < ActiveAnimations.Count; i++)
        {
            ActiveAnimation AEA = ActiveAnimations[i];
            if (AEA.AnimationID == AnimationID && AEA.PlayerID == PlayerID)
            {
                Return = ActiveAnimations[i];
                break;
            }
        }
        if (Return == null)
        {
            Return = new ActiveAnimation();
            ActiveAnimations.Add(Return);
            Return.AnimationID = AnimationID;
            Return.PlayerID = PlayerID;
        }
        return Return;
    }




    //Event
    public void AnimationEvent(DG_AnimationSync AnimationSync)
    {
        ActiveAnimation AEA = null;
        for (int i = 0; i < ActiveAnimations.Count; i++)
        {
            ActiveAnimation NewAEA;
            NewAEA = ActiveAnimations[i];
            if (NewAEA.AnimationSyncRef == AnimationSync) { AEA = NewAEA; break; }
        }


        if (AEA == null) return;

        AEA.ActiveTrackingTime = true;

        switch (AEA.CurrentAnimationState)
        {
            case AnimationStates.PreUnSheath: AEA.CurrentAnimationState = AnimationStates.UnSheath; break;
            case AnimationStates.Idle: AEA.CurrentAnimationState = AnimationStates.ReSheath; break;
        }
    }
}
