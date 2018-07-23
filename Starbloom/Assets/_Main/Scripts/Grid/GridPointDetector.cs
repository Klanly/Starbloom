using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridPointDetector : MonoBehaviour
{

    public LayerMask DetectionMask;

    [Header("Max Distance From Player")]
    public float MaxDistance;


    [System.Serializable]
    public class PlayerGridDetection
    {
        [Header("Grid Display")]
        public Transform GridDisplay;
        public MeshRenderer GridMesh = null;

        [System.NonSerialized] public Transform PlayerPointHelper;
        [System.NonSerialized] public Transform PlayerPointHelper2;
        [System.NonSerialized] public bool GridPointSet;

        [System.NonSerialized] public Transform DetectionPoint;
        [System.NonSerialized] public bool ObjectIsPlacing = false;
        [System.NonSerialized] public bool GlobalPositioning = false;
    }

    public PlayerGridDetection[] GridDetections;


    private void Awake()
    {
        QuickFind.GridDetection = this;

        for (int i = 0; i < GridDetections.Length; i++)
        {
            PlayerGridDetection PGD = GridDetections[i];
            PGD.DetectionPoint = new GameObject().transform;
            PGD.PlayerPointHelper = new GameObject().transform;
            PGD.DetectionPoint.SetParent(transform);
            PGD.PlayerPointHelper.SetParent(transform);
            PGD.GridDisplay.position = new Vector3(0, 10000, 0);
            PGD.GridMesh.enabled = false;
        }
    }



    void Update()
    {
        for (int i = 0; i < QuickFind.InputController.Players.Length; i++)
        {
            DG_PlayerInput.Player P = QuickFind.InputController.Players[i];
            if (P.CharLink == null) continue;
            GetDetectionPoint(P, i);
        }
    }
    void GetDetectionPoint(DG_PlayerInput.Player P, int Index)
    {
        if (P.CharLink.PlayerTrans == null) return;
        if (QuickFind.GUI_OverviewTabs.OverviewTabs[Index].UIisOpen) return;
        if (QuickFind.GUI_Inventory.PlayersInventory[Index].InventoryIsOpen) return;

        int Array = 0;
        if (P.CharLink.PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) Array = 1;

        GridDetections[Array].PlayerPointHelper.position = P.CharLink.PlayerTrans.position;
        AlignTransToClosestGridPoint(GridDetections[Array].PlayerPointHelper);

        CameraLogic.UserCameraMode CamMode = P.CharLink.PlayerCam.CurrentCameraAngle;
        CameraLogic.ContextDetection DetectionMode = CameraLogic.ContextDetection.InfrontPlayer;

        UserSettings.PlayerSettings PS = QuickFind.UserSettings.SingleSettings;
        if (QuickFind.NetworkSync.Player2PlayerCharacter != -1) PS = QuickFind.UserSettings.CoopSettings[Index];

        if (QuickFind.ItemActivateableHandler.Hotbars[Index].CurrentItemDatabaseReference.ActivateableType == HotbarItemHandler.ActivateableTypes.Axe
            || QuickFind.ItemActivateableHandler.Hotbars[Index].CurrentItemDatabaseReference.ActivateableType == HotbarItemHandler.ActivateableTypes.Pickaxe)
        {
            if (CamMode == CameraLogic.UserCameraMode.Isometric) DetectionMode = PS.IsometricBreakableDetectionMode;
            if (CamMode == CameraLogic.UserCameraMode.Thirdperson) DetectionMode = PS.ThirdPersonBreakableDetectionMode;
        }
        else
        {
            if (CamMode == CameraLogic.UserCameraMode.Isometric) DetectionMode = PS.IsometricObjectPlacementDetectionMode;
            if (CamMode == CameraLogic.UserCameraMode.Thirdperson) DetectionMode = PS.ThirdPersonObjectPlacementDetectionMode;
        }

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

            if (Physics.Raycast(Origin, Direction, out hit, 200, DetectionMask))
            {
                GridDetections[Array].DetectionPoint.position = hit.point;
                if (!GridDetections[Array].GlobalPositioning && !QuickFind.WithinDistance(GridDetections[Array].DetectionPoint, GridDetections[Array].PlayerPointHelper, 1f))
                {
                    GridDetections[Array].PlayerPointHelper.LookAt(GridDetections[Array].DetectionPoint);
                    GridDetections[Array].PlayerPointHelper.position += GridDetections[Array].PlayerPointHelper.forward * MaxDistance;
                    GridDetections[Array].DetectionPoint.position = GridDetections[Array].PlayerPointHelper.position;
                }
            }
        }
        else
        {
            GridDetections[Array].DetectionPoint.position = GridDetections[Array].PlayerPointHelper.position;
            GridDetections[Array].DetectionPoint.rotation = P.CharLink.PlayerTrans.rotation;
            GridDetections[Array].DetectionPoint.position += GridDetections[Array].DetectionPoint.forward * .8f;
        }
        AlignTransToClosestGridPoint(GridDetections[Array].DetectionPoint);
        SetGridDisplay(GridDetections[Array].DetectionPoint, Array);
    }



    void AlignTransToClosestGridPoint(Transform T)
    {
        Vector3 Pos = T.position;
        T.position = new Vector3(Mathf.RoundToInt(Pos.x), Pos.y, Mathf.RoundToInt(Pos.z));
    }
    void SetGridDisplay(Transform T, int Array)
    {
        if (GridDetections[Array].ObjectIsPlacing)
        {
            Vector3 GridPoint = T.position;
            GridPoint.y = GridPoint.y - .45f;
            GridDetections[Array].GridDisplay.position = GridPoint;
        }
        else
            GridDetections[Array].GridDisplay.position = new Vector3(0, 10000, 0);
    }
}
