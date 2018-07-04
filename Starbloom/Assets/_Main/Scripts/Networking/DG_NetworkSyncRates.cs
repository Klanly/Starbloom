using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_NetworkSyncRates : MonoBehaviour {

    public enum SyncRateTypes
    {
        Player,
        EnemyAI
    }


    [System.Serializable]
    public class SyncRateModule
    {
        public SyncRateTypes Type;

        [Header("Network Send Rate")]
        public float SendRate;

        [Header("Catch up Values")]
        public float SlerpMoveRate;
        public float SlerpTurnRate;
        public float MaxDistance;
    }

    public SyncRateModule[] SyncRates;




    private void Awake()
    {
        QuickFind.NetworkSyncRates = this;
    }



    public SyncRateModule GetSyncModuleByType(SyncRateTypes TypeWanted)
    {
        for(int i = 0; i < SyncRates.Length; i++)
        {
            if (SyncRates[i].Type == TypeWanted)
                return SyncRates[i];
        }
        return null;
    }
}
