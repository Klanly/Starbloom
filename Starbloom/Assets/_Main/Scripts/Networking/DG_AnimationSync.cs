using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_AnimationSync : MonoBehaviour {

    [System.Serializable]
    public class AnimationBool
    {
        public string BoolName;
        [HideInInspector] public bool CurrentState;
    }


    public Animator Anim;
    public DG_MovementSync MoveSync;
    public AnimationBool[] AnimationBoolValues;
    public string RunVelocityName;


    [HideInInspector] public bool isPlayer;


    int[] OutData = new int[3];


    private void Update()
    {
        if (!isPlayer)
        {
            float Current = Anim.GetFloat(RunVelocityName);
            float Distance = MoveSync.Distance;
            if (Distance > 1) Distance = 1;

            if (Current <= Distance) Current += .1f;
            else Current -= .1f;
            if (Current < .1f) Anim.SetFloat(RunVelocityName, 0);
            else Anim.SetFloat(RunVelocityName, Current);
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

        OutData[0] = QuickFind.NetworkSync.PlayerCharacterID;
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

}
