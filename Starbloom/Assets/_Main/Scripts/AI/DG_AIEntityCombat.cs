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

        [Header("Surround")]
        public float MoveWithinRange = 8;
        public float SurroundUpdateRate = .6f;

        [Header("AttackPosition")]
        public float AttackRange = 4;
        public float MoveToUpdateRate = .3f;
    }



    //Movement
    public enum CombatBehaviour
    {
        InitialLoad,

        WaitingForDetection,
        MovingToSurroundArea,
        MovingToAttackRange,
        ChooseAttack,
        AttackTrigger,
        AwaitingAttackDuration,
        AwaitingCooldown
    }


    //Attack
    [ReadOnly] public CombatBehaviour CurrentCombatBehaviour;

    [System.NonSerialized] public DG_AIEntity Entity;
    [System.NonSerialized] public CombatOptions CombatSettings;
    bool WaitingForTimer;
    float WaitTimer;

    #endregion

    private void Awake()
    {
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
        if (WaitingForTimer) { WaitTimer -= Time.deltaTime; if (WaitTimer < 0) { WaitingForTimer = false; } }
        else ChoseNextAttackState();
    }

    public void ChoseNextAttackState()
    {
        if (!Entity.CheckIfYouAreOwner()) return;
        if (CurrentCombatBehaviour == CombatBehaviour.WaitingForDetection && Entity.Detection.DetectedTarget != null) CurrentCombatBehaviour = CombatBehaviour.MovingToSurroundArea;
        if (CurrentCombatBehaviour == CombatBehaviour.MovingToSurroundArea)
        {
            if (!QuickFind.WithinDistance(Entity.Detection.DetectedTarget, Entity._T, CombatSettings.MoveWithinRange))
            {
                Vector3 NewPos = Entity.FindRandomNavMeshPoint(Entity.Detection.DetectedTarget.position, CombatSettings.MoveWithinRange);
                Entity.Movement.RequestedMoveLocation = NewPos;
                Entity.Movement.ChangeMovementBehaviourState(DG_AIEntityMovement.MovementBehaviour.RunToTargetLocation);
                WaitingForTimer = true; WaitTimer = CombatSettings.SurroundUpdateRate;
            }
            else CurrentCombatBehaviour = CombatBehaviour.MovingToAttackRange;
        }
        if (CurrentCombatBehaviour == CombatBehaviour.MovingToAttackRange)
        {
            if (!QuickFind.WithinDistance(Entity.Detection.DetectedTarget, Entity._T, CombatSettings.AttackRange))
            {
                Entity.Movement.ChangeMovementBehaviourState(DG_AIEntityMovement.MovementBehaviour.WalkToDetectedTarget);
                WaitingForTimer = true; WaitTimer = CombatSettings.MoveToUpdateRate;
            }
            else CurrentCombatBehaviour = CombatBehaviour.ChooseAttack;
        }
        if (CurrentCombatBehaviour == CombatBehaviour.ChooseAttack)
        {
            Entity.Movement.ChangeMovementBehaviourState(DG_AIEntityMovement.MovementBehaviour.Stopped);

        }
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
