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

    [HideInInspector] public Transform DetectionPoint;
    [HideInInspector] public bool ObjectIsPlacing = false;
    [HideInInspector] public bool GlobalPositioning = false;


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
        GetDetectionPoint();
    }
    void GetDetectionPoint()
    {
        if (QuickFind.GUI_OverviewTabs == null) return;
        if (QuickFind.GUI_OverviewTabs.UIisOpen) return;
        if (QuickFind.GUI_Inventory.InventoryIsOpen) return;

        if (QuickFind.PlayerTrans != null)
            PlayerPointHelper.position = QuickFind.PlayerTrans.position;

        AlignTransToClosestGridPoint(PlayerPointHelper);

        if (QuickFind.InputController.MainPlayer.Context == DG_PlayerInput.ContextDetection.MousePosition)
        {
            RaycastHit hit;
            Ray ray = QuickFind.PlayerCam.MainCam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray.origin, ray.direction, out hit, 200, DetectionMask))
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
