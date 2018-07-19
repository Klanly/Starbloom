﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_AnimationSync : MonoBehaviour {

    public enum AnimationResponseType
    {
        ShowTool,
        DontShowTool
    }

    public enum AnimationActionStates
    {
        None,
        _1_PullToolFromBack,
        _2_PullFromPouch,
        _3_Sword,
        _4_NoTransition
    }
    public enum AnimationSubStates
    {
        None,
        _1_SideSwing,
        _2_OverheadSwing,
        _3_WateringCanPour,
        _4_SwordSlashA,
        _5_SwordSlashB,
        _6_SwordSlashC,
        _7_BendDownPickup,
        _8_HarvestTree,
        _9_SeedPour,
        _10_GenericItemPlace
    }


    public Animator Anim;
    public DG_CharacterLink CharacterLink;
    public Transform LeftFootPoint;
    public Transform RightFootPoint;
    public float FootPrintRadius;


    //Animation
    [System.NonSerialized] public bool DisableWeaponSwitching = false;
    [System.NonSerialized] public bool MidAnimation = false;
    [System.NonSerialized] public bool isPlayer;


    int[] OutData = new int[3];
    float StoredDistance;
    bool isWeapon;
    DG_AnimationObject SavedAO;


    private void Update()
    {
        if (!isPlayer)
        {
            float Distance = CharacterLink.MoveSync.Distance;
            if (Distance > .1f) Distance = 1;
            if (Distance < .1f) Distance = StoredDistance - .1f;

            StoredDistance = Distance;

            Anim.SetFloat(QuickFind.AnimationStringValues.RunVelocityName, StoredDistance);
        }
        else
        {
            for (int i = 0; i < QuickFind.AnimationStringValues.AnimationBoolValues.Length; i++)
            {
                DG_AnimationStringValues.AnimationBool AB = QuickFind.AnimationStringValues.AnimationBoolValues[i];
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

        OutData[0] = CharacterLink.PlayerID;
        OutData[1] = Index;
        OutData[2] = CurrentState;

        QuickFind.NetworkSync.UpdatePlayerAnimationState(OutData);
    }

    public void UpdatePlayerAnimationState(int[] InData)
    {
        bool Value = false;
        if (InData[2] == 1) Value = true;

        Anim.SetBool(QuickFind.AnimationStringValues.AnimationBoolValues[InData[1]].BoolName, Value);
    }





    public void TriggerAnimation(int AnimationID)
    {
        if (!CharacterLink.CharController.isActiveAndEnabled) return;

        DG_AnimationObject AO = QuickFind.AnimationDatabase.GetItemFromID(AnimationID);
        int Random = UnityEngine.Random.Range(0, AO.AnimationSubstateValues.Length);
        int AnimationSubStateNum = (int)AO.AnimationSubstateValues[Random];

        PlayAnimation(CharacterLink.PlayerID, AnimationSubStateNum, AnimationID);

        if (OutData == null) OutData = new int[3];

        OutData[0] = CharacterLink.PlayerID;
        OutData[1] = AnimationSubStateNum;
        OutData[2] = AnimationID;

        QuickFind.NetworkSync.TriggerAnimationSubState(OutData);
    }


    public void ReceiveNetAnimation(int[] InData)
    {
        PlayAnimation(InData[0], InData[1], InData[2]);
    }


    public void PlayAnimation(int PlayerID, int SubStateID, int AnimationID)
    {
        DG_AnimationObject AO = QuickFind.AnimationDatabase.GetItemFromID(AnimationID);
        SavedAO = AO;
        QuickFind.EquipmentFXManager.SetEquipmentAnimationTracker(PlayerID, AnimationID, this);

        Anim.SetInteger(QuickFind.AnimationStringValues.ActionState, (int)AO.AnimationActionState);
        Anim.SetInteger(QuickFind.AnimationStringValues.SubState, SubStateID);
        Anim.SetBool(QuickFind.AnimationStringValues.ActionTriggerBoolName, true);

        isWeapon = AO.isWeapon;
        if (isWeapon) Anim.SetBool(QuickFind.AnimationStringValues.AttackTriggerBoolName, true);

        SetMovementControlState(true);
        if(AO.ResponseType == AnimationResponseType.ShowTool)
            SetAllowWeaponSwitching(true);
    }










    public void SetMovementControlState(bool isTrue)
    {
        if (CharacterLink != QuickFind.NetworkSync.GetCharacterLinkByPlayerID(CharacterLink.PlayerID)) return;
        CharacterLink.EnablePlayerMovement(!isTrue);
        MidAnimation = isTrue;
    }
    public void SetAllowWeaponSwitching(bool isTrue)
    {
        DisableWeaponSwitching = isTrue;      
    }






    ///////////////////////////////////////////////////////////////////////////////////////////
    // EVENTS CALLED FROM MECHANIM

    ///////////
    public void WeaponSwitch()
    {
        if (SavedAO.ResponseType == AnimationResponseType.ShowTool)
        {
            QuickFind.EquipmentFXManager.AnimationEvent(this);
        }
    }

    ///////////
    public void Hit()
    {
        int PlayerID = CharacterLink.PlayerID;

        if (CharacterLink.MoveSync.UserOwner != null) return;

        if (SavedAO.ResponseType == AnimationResponseType.ShowTool)
        {
            QuickFind.BreakableObjectsHandler.HitAction(PlayerID);
            QuickFind.HoeHandler.HitAction(PlayerID);
            QuickFind.WateringCanHandler.HitAction(PlayerID);

            Anim.SetBool(QuickFind.AnimationStringValues.HoldAttackTriggerName, false);
        }
        if (SavedAO.ResponseType == AnimationResponseType.DontShowTool)
        {
            QuickFind.InteractHandler.ReturnInteractionHit(PlayerID);

            QuickFind.ObjectPlacementManager.PlacementHit(QuickFind.ObjectPlacementManager.GetCRPByPlayerID(PlayerID), QuickFind.NetworkSync.GetUserByCharacterLink(CharacterLink).SceneID);
        }

        SetMovementControlState(false);    
    }

    ///////////
    public void ToolActionComplete()
    {
        if (SavedAO.ResponseType == AnimationResponseType.ShowTool)
        {
            QuickFind.EquipmentFXManager.AnimationEvent(this);
            Anim.SetBool(QuickFind.AnimationStringValues.HoldAttackTriggerName, false);
        }
    }

    ///////////
    public void ResetTrigger()
    {
        Anim.SetBool(QuickFind.AnimationStringValues.ActionTriggerBoolName, false);
    }

    ///////////
    public void ResetAttackingTrigger()
    {
        Anim.SetBool(QuickFind.AnimationStringValues.AttackTriggerBoolName, false);
        Anim.SetBool(QuickFind.AnimationStringValues.HoldAttackTriggerName, true);
    }





    ///////////
    public void FootLeft(int Value)
    {
        if(QuickFind.SnowHandler.isActiveAndEnabled) QuickFind.SnowHandler.MarkSnowAt(LeftFootPoint.position, FootPrintRadius);
    }
    public void FootRight(int Value)
    {
        if (QuickFind.SnowHandler.isActiveAndEnabled) QuickFind.SnowHandler.MarkSnowAt(RightFootPoint.position, FootPrintRadius);
    }












    //Get Functions
    public bool CharacterIsGrounded()
    {
        return Anim.GetBool(QuickFind.AnimationStringValues.GroundedBool);
    }
}
