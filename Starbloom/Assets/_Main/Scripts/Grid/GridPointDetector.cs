using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridPointDetector : MonoBehaviour
{

    public LayerMask DetectionMask;

    [Header("Max Distance From Player")]
    public float MaxDistance;

    [Header("Grid Display")]
    public bool ShowGridDisplay;
    public Transform GridDisplay;
    public MeshRenderer GridMesh = null;



    Transform PlayerPointHelper;
    Transform PlayerPointHelper2;
    bool GridPointSet;

    [System.NonSerialized] public Transform DetectionPoint;
    [System.NonSerialized] public bool ObjectIsPlacing = false;
    [System.NonSerialized] public bool GlobalPositioning = false;


    private void Awake()
    {
        QuickFind.GridDetection = this;
        DetectionPoint = new GameObject().transform;
        PlayerPointHelper = new GameObject().transform;
        DetectionPoint.SetParent(transform);
        PlayerPointHelper.SetParent(transform);
        GridDisplay.position = new Vector3(0, 10000, 0);
        GridMesh.enabled = false;
    }



    void Update()
    {
        for (int i = 0; i < QuickFind.InputController.Players.Length; i++)
        {
            DG_PlayerInput.Player P = QuickFind.InputController.Players[i];
            if (P.CharLink == null) continue;
            GetDetectionPoint(P);
        }
    }
    void GetDetectionPoint(DG_PlayerInput.Player P)
    {
        if (QuickFind.PlayerTrans == null) return;
        if (QuickFind.GUI_OverviewTabs.UIisOpen) return;
        if (QuickFind.GUI_Inventory.InventoryIsOpen) return;

        
        PlayerPointHelper.position = QuickFind.PlayerTrans.position;
        AlignTransToClosestGridPoint(PlayerPointHelper);

        CameraLogic.UserCameraMode CamMode = P.CharLink.PlayerCam.CurrentCameraAngle;
        CameraLogic.ContextDetection DetectionMode = CameraLogic.ContextDetection.InfrontPlayer;

        if (QuickFind.ItemActivateableHandler.CurrentItemDatabaseReference.ActivateableType == HotbarItemHandler.ActivateableTypes.Axe
            || QuickFind.ItemActivateableHandler.CurrentItemDatabaseReference.ActivateableType == HotbarItemHandler.ActivateableTypes.Pickaxe)
        {
            if (CamMode == CameraLogic.UserCameraMode.Isometric) DetectionMode = QuickFind.UserSettings.IsometricBreakableDetectionMode;
            if (CamMode == CameraLogic.UserCameraMode.Thirdperson) DetectionMode = QuickFind.UserSettings.ThirdPersonBreakableDetectionMode;
        }
        else
        {
            if (CamMode == CameraLogic.UserCameraMode.Isometric) DetectionMode = QuickFind.UserSettings.IsometricObjectPlacementDetectionMode;
            if (CamMode == CameraLogic.UserCameraMode.Thirdperson) DetectionMode = QuickFind.UserSettings.ThirdPersonObjectPlacementDetectionMode;
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
                DetectionPoint.position = hit.point;
                if (!GlobalPositioning && !QuickFind.WithinDistance(DetectionPoint, PlayerPointHelper, 1f))
                {
                    PlayerPointHelper.LookAt(DetectionPoint);
                    PlayerPointHelper.position += PlayerPointHelper.forward * MaxDistance;
                    DetectionPoint.position = PlayerPointHelper.position;
                }
            }
        }
        else
        {
            DetectionPoint.position = PlayerPointHelper.position;
            DetectionPoint.rotation = QuickFind.PlayerTrans.rotation;
            DetectionPoint.position += DetectionPoint.forward * .8f;
        }
        AlignTransToClosestGridPoint(DetectionPoint);
        SetGridDisplay(DetectionPoint);
    }



    void AlignTransToClosestGridPoint(Transform T)
    {
        Vector3 Pos = T.position;
        T.position = new Vector3(Mathf.RoundToInt(Pos.x), Pos.y, Mathf.RoundToInt(Pos.z));
    }
    void SetGridDisplay(Transform T)
    {
        if (ObjectIsPlacing)
        {
            Vector3 GridPoint = T.position;
            GridPoint.y = GridPoint.y - .45f;
            GridDisplay.position = GridPoint;
        }
        else
            GridDisplay.position = new Vector3(0, 10000, 0);
    }
}
