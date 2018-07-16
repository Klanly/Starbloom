using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_CharacterDashController : MonoBehaviour {


    public class DashingObject
    {
        public bool inUse;
        public float TotalTime;
        public float Timer;
        public Transform CurrentMovingChar;
        public Transform CurrentTarget;
        public Transform DashHelper1;
        public Transform DashHelper2;
        public GameObject MessageReturn;
    }

    public Transform DashHelper;
    Transform TrueTarget;
    List<DashingObject> DashingObjects;
    public LayerMask DashBlockingMask;






    private void Awake()
    {
        DashingObjects = new List<DashingObject>();
        QuickFind.CharacterDashController = this;
        TrueTarget = DashHelper.GetChild(0);
    }


    private void Update()
    {

        for(int i = 0; i < DashingObjects.Count; i++)
        {
            DashingObject DO = DashingObjects[i];
            if (!DO.inUse) continue;
            if(DO.CurrentMovingChar == null) { DO.inUse = false; continue; }

            DashHelper.position = DO.CurrentTarget.position;
            DashHelper.LookAt(DO.CurrentMovingChar);

            DO.Timer -= Time.deltaTime;
            if (DO.Timer < 0)
            {
                DO.Timer = 0;
                DO.inUse = false;
                if(DO.MessageReturn != null) DO.MessageReturn.SendMessage("DashComplete");
            }
            float Percentage = 1 - (DO.Timer / DO.TotalTime);
            DO.CurrentMovingChar.position = Vector3.Lerp(DO.DashHelper1.position, TrueTarget.position, Percentage);
        }
    }




    public void DashAction(Transform Character, float DashTime, float DashDistance, bool DoRaycast = false, GameObject ReturnObject = null)
    {
        DashingObject DO = GetDashObjectController(Character);
        DO.inUse = true;
        DO.MessageReturn = ReturnObject;
        DO.CurrentMovingChar = Character;
        DO.TotalTime = DashTime;
        DO.Timer = DashTime;
        DO.DashHelper1.position = Character.position;
        DO.DashHelper2.position = DO.DashHelper1.position;
        DO.DashHelper2.eulerAngles = Character.eulerAngles;

        RaycastHit hit;
        if (DoRaycast && Physics.Raycast(DO.DashHelper2.position, DO.DashHelper2.forward, out hit, DashDistance, DashBlockingMask))
            DO.DashHelper2.position = hit.point;
        else
            DO.DashHelper2.Translate(Vector3.forward * DashDistance);

        DO.CurrentTarget = DO.DashHelper2;
    }

    public DashingObject GetDashObjectController(Transform Character)
    {
        DashingObject DO;
        for (int i = 0; i < DashingObjects.Count; i++)
        {
            DO = DashingObjects[i];
            if (!DO.inUse || Character == DO.CurrentMovingChar) return DO;
        }

        DO = new DashingObject();

        DO.DashHelper1 = new GameObject().transform;
        DO.DashHelper2 = new GameObject().transform;
        DO.DashHelper1.SetParent(transform);
        DO.DashHelper2.SetParent(transform);

        DashingObjects.Add(DO);
        return DO;
    }

}
