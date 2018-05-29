using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class DG_FishingLevelStats : MonoBehaviour {

    [System.Serializable]
    public class FishingLevel
    {
        public float FishingStrength;
    }

    public FishingLevel[] FishingLevelStats;
}
