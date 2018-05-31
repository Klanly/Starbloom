using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridPointDetector : MonoBehaviour
{
    public enum GridDetectionMode
    {
        NotDetecting,
        ByCharacterZ,
        ByMousePoint,
        ByMousePointLockDistance
    }



    public GridDetectionMode DetectionMode;
    public LayerMask DetectionMask;

    [Header("Max Distance From Player")]
    public float MaxDistance;

    [Header("Grid Display")]
    public bool ShowGridDisplay;
    public Transform GridDisplay;


    Transform DetectionPoint;
    bool GridPointSet;
    Vector3 LastSafePoint;

    private void Awake()
    {
        QuickFind.GridDetection = this;
        DetectionPoint = new GameObject().transform;
        DetectionPoint.SetParent(transform);
    }



    void Update()
    {
        if (DetectionMode == GridDetectionMode.NotDetecting) return;
        GetDetectionPoint();
        DetectClosestGridPoint();
    }
    void GetDetectionPoint()
    {
        if (DetectionMode == GridDetectionMode.ByMousePoint || DetectionMode == GridDetectionMode.ByMousePointLockDistance)
        {
            RaycastHit hit;
            Ray ray = QuickFind.PlayerCam.MainCam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray.origin, ray.direction, out hit, 200, DetectionMask))
            {
                DetectionPoint.position = hit.point;
                if (DetectionMode == GridDetectionMode.ByMousePointLockDistance && !QuickFind.WithinDistance(DetectionPoint, QuickFind.PlayerTrans, MaxDistance))
                    DetectionPoint.position = LastSafePoint;
            }
        }
        if (DetectionMode == GridDetectionMode.ByCharacterZ)
        {
            DetectionPoint.position = QuickFind.PlayerTrans.position;
            DetectionPoint.rotation = QuickFind.PlayerTrans.rotation;
            DetectionPoint.position += DetectionPoint.forward * .8f;
        }
    }
    void DetectClosestGridPoint()
    {
        Vector3 Pos = DetectionPoint.position;
        DetectionPoint.position = new Vector3(Mathf.RoundToInt(Pos.x), Pos.y, Mathf.RoundToInt(Pos.z));

        if (ShowGridDisplay)
        {
            Vector3 GridPoint = DetectionPoint.position;
            GridPoint.y = GridPoint.y - .45f;
            GridDisplay.position = GridPoint;
        }
        else
            GridDisplay.position = new Vector3(0, 10000, 0);

        LastSafePoint = DetectionPoint.position;
    }




    void OnDrawGizmos() //Draw Gizmo in Scene view
    {
        if (!Application.isPlaying) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(DetectionPoint.position, new Vector3(1, .2f, 1));
    }
}
