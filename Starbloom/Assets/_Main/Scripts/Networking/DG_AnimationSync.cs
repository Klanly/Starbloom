using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_AnimationSync : MonoBehaviour {

    [System.Serializable]
    public class AnimationStringValues
    {
        public string BaseState;
        public string RunVelocityName;
        public string ActionTriggerBoolName;
        public string ActionState;
        public string SubState;
    }


    public enum AnimationActionStates
    {
        None,
        Tool2Handed,
        WateringCan
    }
    public enum AnimationSubStates
    {
        None,
        SideSwing,
        OverheadSwing,
        WateringCanPour
    }



    [System.Serializable]
    public class AnimationBool
    {
        public string BoolName;
        [HideInInspector] public bool CurrentState;
    }


    public Animator Anim;
    public DG_CharacterLink CharacterLink;
    public AnimationBool[] AnimationBoolValues;
    public AnimationStringValues StringValues;

    //Animation
    [HideInInspector] public bool MidAnimation = false;
    [HideInInspector] public bool isPlayer;


    int[] OutData = new int[3];
    float StoredDistance;




    private void Update()
    {
        if (!isPlayer)
        {
            float Distance = CharacterLink.MoveSync.Distance;
            if (Distance > .2f) Distance = 1;
            if (Distance < .2f) Distance = StoredDistance - .1f;

            StoredDistance = Distance;

            Anim.SetFloat(StringValues.RunVelocityName, StoredDistance);
        }
        else
        {
            for (int i = 0; i < AnimationBoolValues.Length; i++)
            {
                AnimationBool AB = AnimationBoolValues[i];
                bool AnimatorState = Anim.GetBool(AB.BoolName);

                if (AnimatorState != AB.CurrentState)
                {
                    AB.CurrentState = !AB.CurrentState;
                    int StateInt = AB.CurrentState ? 1 : 0;
                    ShiftAnimationState(i, StateInt);
                }
            }
        }
    }


    void ShiftAnimationState(int Index, int CurrentState)
    {
        if (OutData == null) OutData = new int[3];

        OutData[0] = QuickFind.NetworkSync.UserID;
        OutData[1] = Index;
        OutData[2] = CurrentState;

        QuickFind.NetworkSync.UpdatePlayerAnimationState(OutData);
    }

    public void UpdatePlayerAnimationState(int[] InData)
    {
        bool Value = false;
        if (InData[2] == 1) Value = true;

        Anim.SetBool(AnimationBoolValues[InData[1]].BoolName, Value);
    }









    public void TriggerToolAnimation()
    {
        if (!CharacterLink.CharController.isActiveAndEnabled) return;

        DG_ClothingObject CO = QuickFind.ClothingHairManager.GetAttachedClothingReference(QuickFind.NetworkSync.CharacterLink, DG_ClothingHairManager.ClothHairType.RightHand).ClothingRef;
        DG_ClothingObject.ClothingAnimationNumbers CAN = CO.AnimationNumbers[0];
        int Random = UnityEngine.Random.Range(0, CAN.AnimationSubstateValues.Length);
        int AnimationSubStateNum = (int)CAN.AnimationSubstateValues[Random];

        int UserID = QuickFind.NetworkSync.UserID;

        PlayAnimation(UserID, AnimationSubStateNum, CO.DatabaseID);

        if (OutData == null) OutData = new int[3];

        OutData[0] = UserID;
        OutData[1] = AnimationSubStateNum;
        OutData[2] = CO.DatabaseID;

        QuickFind.NetworkSync.TriggerAnimationSubState(OutData);
    }


    public void ReceiveNetAnimation(int[] InData)
    {
        PlayAnimation(InData[0], InData[1], InData[2]);
    }


    public void PlayAnimation(int UserID, int SubStateID, int ClothingObjectID)
    {
        DG_ClothingObject CO = QuickFind.ClothingDatabase.GetItemFromID(ClothingObjectID);
        DG_ClothingObject.ClothingAnimationNumbers CAN = CO.AnimationNumbers[0];

        QuickFind.EquipmentFXManager.SetEquipmentAnimationTracker(UserID, ClothingObjectID, this);

        Anim.SetInteger(StringValues.ActionState, (int)CAN.AnimationActionState);
        Anim.SetInteger(StringValues.SubState, SubStateID);
        Anim.SetBool(StringValues.ActionTriggerBoolName, true);

        SetAnimationState(true);
    }








    public void SetAnimationState(bool isTrue)
    {
        MidAnimation = isTrue;

        if (CharacterLink != QuickFind.NetworkSync.CharacterLink) return;
        CharacterLink.EnablePlayerMovement(!isTrue);
    }





    ///////////////////////////////////////////////////////////////////////////////////////////
    // EVENTS CALLED FROM MECHANIM
    public void WeaponSwitch()
    {
        QuickFind.EquipmentFXManager.AnimationEvent(this);
    }
    public void Hit()
    {
        if (CharacterLink.MoveSync.UserOwner != null) return; 

        QuickFind.BreakableObjectsHandler.HitAction();
        QuickFind.HoeHandler.HitAction();
        QuickFind.WateringCanHandler.HitAction();
    }

    public void ToolActionComplete()
    {
        QuickFind.EquipmentFXManager.AnimationEvent(this);
    }

    public void ResetTrigger()
    {
        Anim.SetBool(StringValues.ActionTriggerBoolName, false);
    }



}
