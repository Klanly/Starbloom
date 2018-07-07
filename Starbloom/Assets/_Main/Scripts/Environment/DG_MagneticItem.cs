using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_MagneticItem : MonoBehaviour {

    [System.NonSerialized] public bool Claimed = false;
    [System.NonSerialized] public bool AllowMagnetic = false;

    public bool RandomizeRotationOnEnable;
    float WaitTimer;


    public void TriggerStart(Vector3 Force)
    {
        if (RandomizeRotationOnEnable)
            transform.rotation = Random.rotation;

        Rigidbody RB = transform.GetComponent<Rigidbody>();
        RB.AddForce(Force, ForceMode.VelocityChange);
        WaitTimer = QuickFind.ItemDatabase.PreMagneticObjectWaitTimer;
        AllowMagnetic = false;
        Claimed = false;
        this.enabled = true;
    }


    private void Update()
    {
        if (!AllowMagnetic)
        {
            WaitTimer -= Time.deltaTime;
            if (WaitTimer < 0)
            {
                AllowMagnetic = true;
                this.enabled = false;
            }
        }
    }
}
