using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_WeaponImpactDetection : MonoBehaviour {

    public int Index;

    private void OnCollisionEnter(Collision collision)
    {
        DG_ContextObject CO = collision.transform.GetComponent<DG_ContextObject>();
        if (CO == null) return;
        if (CO.Type == DG_ContextObject.ContextTypes.Enemy)
            QuickFind.CombatHandler.MeleeHitTrigger(Index,CO);
    }
    private void OnTriggerEnter(Collider other)
    {
        DG_ContextObject CO = other.transform.GetComponent<DG_ContextObject>();
        if (CO == null) return;
        if (CO.Type == DG_ContextObject.ContextTypes.Enemy)
            QuickFind.CombatHandler.MeleeHitTrigger(Index,CO);
    }
}
