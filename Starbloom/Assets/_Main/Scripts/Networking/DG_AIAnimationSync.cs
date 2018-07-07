using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_AIAnimationSync : MonoBehaviour {


    public Animator Anim;
    DG_AIEntity AIObject;

    int[] OutData = new int[3];
    float StoredSpeed;

    private void Awake()
    {
        AIObject = transform.GetComponent<DG_AIEntity>();
    }

    private void Start() { if (!QuickFind.GameStartHandler.GameHasStarted) { Debug.Log("AI Object Left In Scene, Destroying"); Destroy(gameObject); return; } }


    private void Update()
    {
        if (AIObject.DestinationReached) StoredSpeed -= 0.01f;
        else if (AIObject.agent.speed == AIObject.MovementSettings.walkSpeed) StoredSpeed = .5f;
        else if (AIObject.agent.speed == AIObject.MovementSettings.RunSpeed) StoredSpeed = 1;

        if (StoredSpeed < 0) StoredSpeed = 0;
        Anim.SetFloat(QuickFind.AnimationStringValues.RunVelocityName, StoredSpeed);


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
