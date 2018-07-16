using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHitboxCollider : MonoBehaviour {

    public enum AttackerType
    {
        Player,
        Enemy
    }
    public AttackerType Type;

    private void OnEnable()
    {
        GetComponent<MeshRenderer>().enabled = QuickFind.GameSettings.ShowAttackHitboxes;
    }


    private void OnTriggerEnter(Collider other)
    {
        if(Type == AttackerType.Enemy)
        {
            QuickFind.CombatHandler.PlayerHitReturn(transform, other);
        }
        if(Type == AttackerType.Player)
        {
            QuickFind.CombatHandler.ObjectHitReturn(other);
        }
    }
}
