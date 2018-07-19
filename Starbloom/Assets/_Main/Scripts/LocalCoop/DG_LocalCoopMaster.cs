using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class DG_LocalCoopMaster : MonoBehaviour {

    [ReadOnly] public bool LocalCoopActive;


    private void Awake()
    {
        QuickFind.LocalCoopController = this;
    }

    [Button(ButtonSizes.Medium)]
    public void DebugToggle()
    {
        TriggerLocalCoop(!LocalCoopActive);
    }

    public void TriggerLocalCoop(bool isEnable)
    {
        LocalCoopActive = isEnable;
    }
}
