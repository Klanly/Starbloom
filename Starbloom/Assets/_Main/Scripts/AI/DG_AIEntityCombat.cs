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

    }



    //Movement
    public enum CombatBehaviour
    {
        InitialLoad,

        WaitingForDetection,
        MovingToPosition,
        ChooseAttack,
        AttackTrigger,
        AwaitingAttackDuration,
        AwaitingCooldown
    }


    //Attack
    [ReadOnly] public CombatBehaviour CurrentCombatBehaviour;

    [System.NonSerialized] public DG_AIEntity Entity;
    [System.NonSerialized] public CombatOptions CombatSettings;
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

    }

    public void ChoseNextAttackState()
    {
        

    }
    #endregion


    #region Set Values
    public void SetCombatSettings(CombatOptions Settings) { CombatSettings = Settings; }
    void ChangeCombatBehaviourState(CombatBehaviour Behaviour) { CurrentCombatBehaviour = Behaviour; ChoseNextAttackState(); }

    [Button(ButtonSizes.Small)] [HideInEditorMode] public void SetToDebugCombatState() { ChangeCombatBehaviourState(CombatSettings.DebugCombatBehaviour); }

    #endregion
}
