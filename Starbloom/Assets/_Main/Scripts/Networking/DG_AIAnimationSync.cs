using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_AIAnimationSync : MonoBehaviour {


    public Animator Anim;
    public DG_AIMovementSync MovementSync;
    public float CurrentStateMovementAnimationSpeed = .5f;

    //Animation
    [HideInInspector] public bool MidAnimation = false;


    int[] OutData = new int[3];
    float StoredDistance;



    private void Update()
    {
        float Distance = MovementSync.Distance;
        if (Distance > .1f) Distance = CurrentStateMovementAnimationSpeed;
        if (Distance < .1f) Distance = StoredDistance - .01f;

        StoredDistance = Distance;
        Anim.SetFloat(QuickFind.AnimationStringValues.RunVelocityName, StoredDistance);


        //if (MovementSync.isController)
        //{
        //    for (int i = 0; i < QuickFind.AnimationStringValues.AnimationBoolValues.Length; i++)
        //    {
        //        DG_AnimationStringValues.AnimationBool AB = QuickFind.AnimationStringValues.AnimationBoolValues[i];
        //        bool AnimatorState = Anim.GetBool(AB.BoolName);
        //
        //        if (AnimatorState != AB.CurrentState)
        //        {
        //            AB.CurrentState = !AB.CurrentState;
        //            int StateInt = AB.CurrentState ? 1 : 0;
        //            ShiftAnimationState(i, StateInt);
        //        }
        //    }
        //}
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

        Anim.SetBool(QuickFind.AnimationStringValues.AnimationBoolValues[InData[1]].BoolName, Value);
    }


}
