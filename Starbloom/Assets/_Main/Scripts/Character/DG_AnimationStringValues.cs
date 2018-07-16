using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_AnimationStringValues : MonoBehaviour {

    [System.Serializable]
    public class AnimationBool
    {
        public string BoolName;
        [System.NonSerialized] public bool CurrentState;
    }


    [Header("Player Anim")]
    public string BaseState;
    public string RunVelocityName;
    public string ActionTriggerBoolName;
    public string ActionState;
    public string SubState;
    public string GroundedBool;

    public string AttackTriggerBoolName;
    public string HoldAttackTriggerName;

    [Header("Enemy Anim")]
    public string EnemyActionTriggerBoolName;
    public string EnemyTypeIntName;
    public string EnemyAnimationStateName;


    public AnimationBool[] AnimationBoolValues;


    private void Awake()
    {
        QuickFind.AnimationStringValues = this;
    }
}
