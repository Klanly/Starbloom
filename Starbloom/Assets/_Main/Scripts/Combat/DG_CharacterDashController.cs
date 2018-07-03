using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_CharacterDashController : MonoBehaviour {


    public enum DashTypes
    {
        SimpleDash
    }

    [System.Serializable]
    public class DashTimes
    {
        public DashTypes Type;
        public float Time;
        public float NoTargetDashDistance;
    }

    public DashTimes[] DashTimeRefs;
    public Transform DashHelper;



    float TotalTime;
    float Timer;
    Transform CurrentMovingChar;
    Transform CurrentTarget;
    Transform TrueTarget;
    Transform DashHelper1;
    Transform DashHelper2;





    private void Awake()
    {
        QuickFind.CharacterDashController = this;
        DashHelper1 = new GameObject().transform;
        DashHelper2 = new GameObject().transform;
        DashHelper1.SetParent(transform);
        DashHelper2.SetParent(transform);
        TrueTarget = DashHelper.GetChild(0);
        this.enabled = false;
    }


    private void Update()
    {
        DashHelper.position = CurrentTarget.position;
        DashHelper.LookAt(CurrentMovingChar);

        Timer -= Time.deltaTime;
        if (Timer < 0)
        { Timer = 0; this.enabled = false; }
        float Percentage = 1 - (Timer / TotalTime);
        CurrentMovingChar.position = Vector3.Lerp(DashHelper1.position, TrueTarget.position, Percentage);
    }




    public void DashAction(Transform Character, DashTypes DashType, bool HasTarget = false, Transform Target = null)
    {
        CurrentMovingChar = Character;

        DashTimes DT = null;
        for (int i = 0; i < DashTimeRefs.Length; i++)
        { if (DashTimeRefs[i].Type == DashType) { DT = DashTimeRefs[i]; break; } }

        TotalTime = DT.Time;
        Timer = DT.Time;

        DashHelper1.position = Character.position;
        if (HasTarget)
            CurrentTarget = Target;
        else
        {
            DashHelper2.position = DashHelper1.position;
            DashHelper2.eulerAngles = Character.eulerAngles;

            RaycastHit hit;
            if (Physics.Raycast(DashHelper2.position, DashHelper2.forward, out hit, DT.NoTargetDashDistance))
                DashHelper2.position = hit.point;
            else
                DashHelper2.Translate(Vector3.forward * DT.NoTargetDashDistance);

            CurrentTarget = DashHelper2;
        }

        this.enabled = true;
    }
}
