using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class DG_FXContextObjectReference : MonoBehaviour {


    public FXTriggers[] ImpactEffects;
    public FXTriggers[] BreakEffects;
    public FXTriggers[] PickEffects;


    [System.Serializable]
    public class FXTriggers
    {
        public Transform SpawnPointReference;
        public int FXID;
    }



    [Button(ButtonSizes.Small)]
    public void TriggerImpact(){LoopThroughFXTriggers(ImpactEffects);}
    [Button(ButtonSizes.Small)]
    public void TriggerBreak(){LoopThroughFXTriggers(BreakEffects);}
    [Button(ButtonSizes.Small)]
    public void TriggerPick() { LoopThroughFXTriggers(PickEffects); }



    void LoopThroughFXTriggers(FXTriggers[] Array)
    {
        if (Array == null || Array.Length == 0) Debug.Log("There is no effects assigned to this Array");
        for (int i = 0; i < Array.Length; i++)
        {
            FXTriggers FXT = Array[i];
            QuickFind.FXHandler.PlayEffect(FXT.FXID, FXT.SpawnPointReference.position, FXT.SpawnPointReference.eulerAngles);
        }
    }
}
