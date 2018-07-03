using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_AnimationStringValues : MonoBehaviour {

    [System.Serializable]
    public class AnimationBool
    {
        public string BoolName;
        [HideInInspector] public bool CurrentState;
    }



    public string BaseState;
    public string RunVelocityName;
    public string ActionTriggerBoolName;
    public string ActionState;
    public string SubState;
    public string GroundedBool;

    public string AttackTriggerBoolName;
    public string HoldAttackTriggerName;



    public AnimationBool[] AnimationBoolValues;


    private void Awake()
    {
        QuickFind.AnimationStringValues = this;
    }
}
