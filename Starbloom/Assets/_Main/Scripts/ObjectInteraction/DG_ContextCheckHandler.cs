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
    

    public class PlayerContext
    {
        [System.NonSerialized] public Transform LastEncounteredContext;
        [System.NonSerialized] public DG_UI_WobbleAndFade LastEncounteredWobbleScript;
        [System.NonSerialized] public bool ContextHit = false;
        [System.NonSerialized] public DG_ContextObject COEncountered;
        [System.NonSerialized] public Transform DetectionPoint;
    }

    [System.NonSerialized] public PlayerContext[] Contexts;


    private void Awake()
    {
        QuickFind.ContextDetectionHandler = this;
        Contexts = new PlayerContext[2];
        Contexts[0] = new PlayerContext();
        Contexts[1] = new PlayerContext();
        Contexts[0].DetectionPoint = transform.GetChild(0);
        Contexts[1].DetectionPoint = transform.GetChild(1);
    }


    void Update()
    {
        for (int i = 0; i < QuickFind.InputController.Players.Length; i++)
        {
            DG_PlayerInput.Player P = QuickFind.InputController.Players[i];
            if (P.CharLink == null) continue;

            if (P.CharLink.PlayerTrans == null) return;
            if (QuickFind.GUI_OverviewTabs.OverviewTabs[i].UIisOpen) return;
            if (QuickFind.GUI_Inventory.PlayersInventory[i].InventoryIsOpen) return;

            UserSettings.PlayerSettings PS = QuickFind.UserSettings.SingleSettings;
            if (QuickFind.NetworkSync.Player2PlayerCharacter != -1) PS = QuickFind.UserSettings.CoopSettings[i];

            CameraLogic.UserCameraMode CamMode = P.CharLink.PlayerCam.CurrentCameraAngle;
            CameraLogic.ContextDetection DetectionMode = CameraLogic.ContextDetection.InfrontPlayer;
            if (CamMode == CameraLogic.UserCameraMode.Isometric) DetectionMode = PS.IsometricInteractMode;
            if (CamMode == CameraLogic.UserCameraMode.Thirdperson) DetectionMode = PS.ThirdPersonInteractionDetection;

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
                    Contexts[i].DetectionPoint.position = hit.point;
                    if (QuickFind.WithinDistance(Contexts[i].DetectionPoint, P.CharLink.PlayerTrans, MouseDistance))
                        AddNewContext(hit, P.CharLink.PlayerID);
                    else
                    { Contexts[i].ContextHit = false; }
                }
                else { Contexts[i].ContextHit = false; }
            }
            else
            {
                Contexts[i].DetectionPoint.position = P.CharLink.PlayerTrans.position;
                Contexts[i].DetectionPoint.rotation = P.CharLink.PlayerTrans.rotation;

                if (Physics.SphereCast(Contexts[i].DetectionPoint.position, SphereCastRadius, Contexts[i].DetectionPoint.forward, out hit, SphereCastMaxLength, ContextMask))
                    AddNewContext(hit, P.CharLink.PlayerID);
                else
                { Contexts[i].ContextHit = false; }
            }

            if (!Contexts[i].ContextHit)
            { Contexts[i].LastEncounteredContext = null; if (Contexts[i].LastEncounteredWobbleScript != null) Contexts[i].LastEncounteredWobbleScript.Disable(); QuickFind.GUIPopup.HideToolTip(P.CharLink.PlayerID); }

        }
    }



    void AddNewContext(RaycastHit hit, int PlayerID)
    {
        int Array = 0;
        if (PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) Array = 1;

        if (hit.transform == Contexts[Array].LastEncounteredContext) return;

        Contexts[Array].LastEncounteredContext = hit.transform;
        Contexts[Array].COEncountered = Contexts[Array].LastEncounteredContext.GetComponent<DG_ContextObject>();
        Contexts[Array].ContextHit = true;



        if (Contexts[Array].COEncountered == null) return;

        bool AllowWobble = false;
        switch (Contexts[Array].COEncountered.Type)
        {
            case DG_ContextObject.ContextTypes.PickupItem: AllowWobble = true; break;
            case DG_ContextObject.ContextTypes.MoveableStorage: AllowWobble = true; break;
        }

        QuickFind.GUIPopup.ShowPopup(Contexts[Array].COEncountered, PlayerID);

        if (AllowWobble && Contexts[Array].LastEncounteredContext.GetComponent<DG_UI_WobbleAndFade>() != null)
        {
            if (Contexts[Array].LastEncounteredWobbleScript != null) Contexts[Array].LastEncounteredWobbleScript.Disable();
            Contexts[Array].LastEncounteredWobbleScript = Contexts[Array].LastEncounteredContext.GetComponent<DG_UI_WobbleAndFade>();
            Contexts[Array].LastEncounteredWobbleScript.Enable();
        }     
    }
}
