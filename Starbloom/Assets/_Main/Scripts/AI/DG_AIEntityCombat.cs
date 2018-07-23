using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class DG_AIEntityCombat : MonoBehaviour {


    #region Variables

    [System.Serializable]
    public class CombatOptions
    {
        public CombatBehaviour DefaultCombatBehaviour;
        public CombatBehaviour DebugCombatBehaviour;

        [Header("Move To Range")]
        public float MoveWithinRange = 8;

        [Header("Surround Behaviour")]
        public float SurroundRange = 8;
        public float SurroundDefaultDirectionAmount = 10;

        [Header("Attack Warning")]
        public float AttackWarningTime = 1;
        public float PostWarningDelayTime = .5f;

        [Header("Attack Travel Distance")]
        public float AttackTravelTime = .3f;
        public float AttackTravelDistance = 10;

        [Header("Animation Values")]
        public int EnemyAnimationType;
        public int AttackWarningAnim;
        public int AttackAnim;
        public int AttackCompleteAnim;
    }



    //Movement
    public enum CombatBehaviour
    {
        InitialLoad,

        //Idle, or Move Within General Range of Character
        WaitingForDetection,
        MovingToWithinTargetRange,
        
        //Waiting Between Attack Behaviour.
        AwaitingSurroundMovementOrder,
        SurroundMovementReceived,

        //Attack Warning
        AnimationAttackWarning,
        PostWarningDelay,

        //Attack
        AttackTrigger,

        //Attack Complete
        AttackComplete
    }


    //Attack
    [ReadOnly] public CombatBehaviour CurrentCombatBehaviour;

    [System.NonSerialized] public DG_AIEntity Entity;
    [System.NonSerialized] public CombatOptions CombatSettings;
    public GameObject AttackHitbox;
    float WaitTimer;

    #endregion

    private void Awake()
    {
        AttackHitbox.SetActive(false);
        this.enabled = false;
    }


    private void InitialLoad()
    {
        CurrentCombatBehaviour = CombatSettings.DefaultCombatBehaviour;
        ChoseNextAttackState();
    }


    void Update()
    {
        if (CurrentCombatBehaviour == CombatBehaviour.InitialLoad) InitialLoad();
        if (!Entity.CheckIfYouAreOwner()) return;
        HandleCombatUpdate();
    }



    #region Combat

    void HandleCombatUpdate()
    {
        if (!Entity.CheckIfYouAreOwner()) return;
        if (CurrentCombatBehaviour == CombatBehaviour.AwaitingSurroundMovementOrder) return;

        //Awaiting Players in Range
        if (CurrentCombatBehaviour == CombatBehaviour.WaitingForDetection && Entity.Detection.DetectedTarget != null)
        {
            CurrentCombatBehaviour = CombatBehaviour.MovingToWithinTargetRange;
            Entity.Movement.ChangeMovementBehaviourState(DG_AIEntityMovement.MovementBehaviour.RunToDetectedTarget);
        }

        //Moving To Player in Range.
        if (CurrentCombatBehaviour == CombatBehaviour.MovingToWithinTargetRange)
        {
            if (Entity.Detection.RangeFromTarget < CombatSettings.MoveWithinRange)
            {
                CurrentCombatBehaviour = CombatBehaviour.AwaitingSurroundMovementOrder;
                Entity.Movement.ChangeMovementBehaviourState(DG_AIEntityMovement.MovementBehaviour.RunAroundTarget);
            }
        }

        //AwaitingCoolDownBehaviour
        if (CurrentCombatBehaviour == CombatBehaviour.SurroundMovementReceived)
        {
            CurrentCombatBehaviour = CombatBehaviour.AnimationAttackWarning;
            Entity.Movement.ChangeMovementBehaviourState(DG_AIEntityMovement.MovementBehaviour.StopAndRotateToTarget);
            WaitTimer = CombatSettings.AttackWarningTime;

            //Play Warning Animation / FX
            Entity.AnimationSync.PlayAnimation(CombatSettings.AttackWarningAnim);
        }


        //Attack Warning Behaviour
        if (CurrentCombatBehaviour == CombatBehaviour.AnimationAttackWarning)
        {
            if (WaitTimer > 0) WaitTimer -= Time.deltaTime;
            else
            {
                CurrentCombatBehaviour = CombatBehaviour.PostWarningDelay;
                Entity.Movement.ChangeMovementBehaviourState(DG_AIEntityMovement.MovementBehaviour.Stopped);
                WaitTimer = CombatSettings.PostWarningDelayTime;
            }
        }

        //Post Delay Attack Trigger
        if (CurrentCombatBehaviour == CombatBehaviour.PostWarningDelay)
        {
            if (WaitTimer > 0) WaitTimer -= Time.deltaTime;
            else
            {
                CurrentCombatBehaviour = CombatBehaviour.AttackTrigger;
                Entity.AnimationSync.PlayAnimation(CombatSettings.AttackAnim);
                AttackHitbox.SetActive(true);
                QuickFind.CharacterDashController.DashAction(0, Entity._T, CombatSettings.AttackTravelTime, CombatSettings.AttackTravelDistance, false, Entity.gameObject);
            }
        }

        //Attack Behaviour
        if (CurrentCombatBehaviour == CombatBehaviour.AttackTrigger)
        {
            //Debug.Log("Scanning for Counter Attack");
        }

        //Attack Behaviour
        if (CurrentCombatBehaviour == CombatBehaviour.AttackComplete)
        {
            CurrentCombatBehaviour = CombatBehaviour.WaitingForDetection;
            AttackHitbox.SetActive(false);
            Entity.AnimationSync.PlayAnimation(CombatSettings.AttackCompleteAnim);
        }

    }


    public void ChoseNextAttackState()
    {

    }


    public void DashComplete(int EnemyNum)
    {
        CurrentCombatBehaviour = CombatBehaviour.AttackComplete; 
    }


    #endregion

    #region IncomingMessages
    public void TargetLocationReached()
    {
        Debug.Log("We have Reached Attack Range");
        ChoseNextAttackState();
    }

    #endregion


    #region Set Values
    public void SetCombatSettings(CombatOptions Settings) { CombatSettings = Settings; }
    void ChangeCombatBehaviourState(CombatBehaviour Behaviour) { CurrentCombatBehaviour = Behaviour; ChoseNextAttackState(); }

    [Button(ButtonSizes.Small)] [HideInEditorMode] public void SetToDebugCombatState() { ChangeCombatBehaviourState(CombatSettings.DebugCombatBehaviour); }

    #endregion



}
