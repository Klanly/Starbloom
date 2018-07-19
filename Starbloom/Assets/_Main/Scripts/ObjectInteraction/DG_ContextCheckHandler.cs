using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_ContextCheckHandler : MonoBehaviour {

    [Header("Detection Options")]
    public LayerMask ContextMask;
    [Header("Non-Mouse")]
    public float SphereCastRadius = 5f;
    public float SphereCastMaxLength = 10f;
    [Header("Mouse Distance")]
    public float MouseDistance = 2;


    [System.NonSerialized] public Transform LastEncounteredContext;
    [System.NonSerialized] public DG_UI_WobbleAndFade LastEncounteredWobbleScript;
    [System.NonSerialized] public bool ContextHit = false;
    [System.NonSerialized] public DG_ContextObject COEncountered;
    Transform DetectionPoint;
    

    private void Awake()
    {
        QuickFind.ContextDetectionHandler = this;
        DetectionPoint = transform.GetChild(0);
    }


    void Update()
    {
        for (int i = 0; i < QuickFind.InputController.Players.Length; i++)
        {
            DG_PlayerInput.Player P = QuickFind.InputController.Players[i];
            if (P.CharLink == null) continue;

            if (QuickFind.PlayerTrans == null) return;
            if (QuickFind.GUI_OverviewTabs.UIisOpen) return;
            if (QuickFind.GUI_Inventory.InventoryIsOpen) return;

            CameraLogic.UserCameraMode CamMode = P.CharLink.PlayerCam.CurrentCameraAngle;
            CameraLogic.ContextDetection DetectionMode = CameraLogic.ContextDetection.InfrontPlayer;
            if (CamMode == CameraLogic.UserCameraMode.Isometric) DetectionMode = QuickFind.UserSettings.IsometricInteractMode;
            if (CamMode == CameraLogic.UserCameraMode.Thirdperson) DetectionMode = QuickFind.UserSettings.ThirdPersonInteractionDetection;

            RaycastHit hit;
            if (DetectionMode != CameraLogic.ContextDetection.InfrontPlayer)
            {
                Vector3 Origin;
                Vector3 Direction;
                Ray ray;
                if (DetectionMode == CameraLogic.ContextDetection.MousePosition)
                { ray = P.CharLink.PlayerCam.MainCam.ScreenPointToRay(Input.mousePosition); Origin = ray.origin; Direction = ray.direction; }
                else
                { Origin = P.CharLink.PlayerCam.CamTrans.position; Direction = P.CharLink.PlayerCam.CamTrans.forward; }

                if (Physics.Raycast(Origin, Direction, out hit, 200, ContextMask))
                {
                    DetectionPoint.position = hit.point;
                    if (QuickFind.WithinDistance(DetectionPoint, QuickFind.PlayerTrans, MouseDistance))
                        AddNewContext(hit);
                    else
                    { ContextHit = false; }
                }
                else { ContextHit = false; }
            }
            else
            {
                DetectionPoint.position = QuickFind.PlayerTrans.position;
                DetectionPoint.rotation = QuickFind.PlayerTrans.rotation;

                if (Physics.SphereCast(DetectionPoint.position, SphereCastRadius, DetectionPoint.forward, out hit, SphereCastMaxLength, ContextMask))
                    AddNewContext(hit);
                else
                { ContextHit = false; }
            }

            if (!ContextHit)
            { LastEncounteredContext = null; if (LastEncounteredWobbleScript != null) LastEncounteredWobbleScript.Disable(); QuickFind.GUIPopup.HideToolTip(); }

        }
    }



    void AddNewContext(RaycastHit hit)
    {
        if (hit.transform == LastEncounteredContext) return;

        LastEncounteredContext = hit.transform;
        COEncountered = LastEncounteredContext.GetComponent<DG_ContextObject>();
        ContextHit = true;



        if (COEncountered == null) return;

        bool AllowWobble = false;
        switch (COEncountered.Type)
        {
            case DG_ContextObject.ContextTypes.PickupItem: AllowWobble = true; break;
            case DG_ContextObject.ContextTypes.MoveableStorage: AllowWobble = true; break;
        }

        QuickFind.GUIPopup.ShowPopup(COEncountered);

        if (AllowWobble && LastEncounteredContext.GetComponent<DG_UI_WobbleAndFade>() != null)
        {
            if (LastEncounteredWobbleScript != null) LastEncounteredWobbleScript.Disable();
            LastEncounteredWobbleScript = LastEncounteredContext.GetComponent<DG_UI_WobbleAndFade>();
            LastEncounteredWobbleScript.Enable();
        }     
    }




    void OnDrawGizmos() //Draw Gizmo in Scene view
    {
        if (ContextHit && LastEncounteredContext != null)
        {
            Vector3 Pos = LastEncounteredContext.position;
            Gizmos.color = Color.yellow;
            Gizmos.DrawCube(Pos, new Vector3(1,.2f,1));
        }
    }
}
