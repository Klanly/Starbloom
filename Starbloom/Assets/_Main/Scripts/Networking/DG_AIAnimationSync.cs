using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_AIAnimationSync : MonoBehaviour {


    public DG_AIEntity Entity;
    public Animator Anim;


    int[] OutData = new int[3];
    float StoredSpeed;


    private void Awake()
    {
        Entity.AnimationSync = this;
    }

    private void Start() { if (gameObject.scene.name != "Networking") { Debug.Log("AI Object Left In Scene, Destroying"); Destroy(Entity.gameObject); return; } }


    private void Update()
    {
        if (Entity.RelayNetworkObject.AICharData[0].DestinationReached || Entity.Movement.CurrentMovementBehaviour == DG_AIEntityMovement.MovementBehaviour.Stopped) StoredSpeed -= 0.01f;
        else if (Entity.Movement.agent.speed == Entity.Movement.MovementSettings.walkSpeed) StoredSpeed = .5f;
        else if (Entity.Movement.agent.speed == Entity.Movement.MovementSettings.RunSpeed) StoredSpeed = 1;
        else if (Entity.Movement.agent.speed == Entity.Movement.MovementSettings.SurroundSpeed) StoredSpeed = 1;

        if (StoredSpeed < 0) StoredSpeed = 0;
        Anim.SetFloat(QuickFind.AnimationStringValues.RunVelocityName, StoredSpeed);
    }

    public void PlayAnimation(int AnimationState)
    {
        Anim.SetInteger(QuickFind.AnimationStringValues.EnemyTypeIntName, Entity.Combat.CombatSettings.EnemyAnimationType);
        Anim.SetInteger(QuickFind.AnimationStringValues.EnemyAnimationStateName, AnimationState);
        Anim.SetBool(QuickFind.AnimationStringValues.EnemyActionTriggerBoolName, true);      
    }



    public void RemoveActionTrigger()
    {
        Anim.SetBool(QuickFind.AnimationStringValues.EnemyActionTriggerBoolName, false);
    }

}
