using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_ContextCheckHandler : MonoBehaviour {

    [HideInInspector] public DG_CharacterLink CharacterLink = null;

    [Header("Detection Options")]
    public LayerMask ContextMask;
    [Header("Non-Mouse")]
    public float SphereCastRadius = 5f;
    public float SphereCastMaxLength = 10f;
    [Header("Mouse Distance")]
    public float MouseDistance = 2;
    [Header("Display to Player")]
    public Transform DetectionMeshTrans;
    public MeshFilter DetectionMesh;
    public MeshRenderer DetectionRenderer;


    [System.NonSerialized] public Transform LastEncounteredContext;
    [System.NonSerialized] public bool ContextHit = false;
    [System.NonSerialized] public DG_ContextObject COEncountered;
    Transform DetectionPoint;
    MeshRenderer LastHoveredMesh;

    Vector3 KnownScale;
    float Timer;
    bool ScaleUp = true;
    bool AllowScale = true;


    private void Awake()
    {
        QuickFind.ContextDetectionHandler = this;
        DetectionPoint = transform.GetChild(0);
    }


    void Update()
    {
        if (CharacterLink == null)
            return;

        RaycastHit hit;
        DG_PlayerInput.Player MP = QuickFind.InputController.MainPlayer;

        if (MP.Context == DG_PlayerInput.ContextDetection.MousePosition)
        {
            Ray ray = QuickFind.PlayerCam.MainCam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray.origin, ray.direction, out hit, 200, ContextMask))
            {
                DetectionPoint.position = hit.point;
                if (QuickFind.WithinDistance(DetectionPoint, CharacterLink.PlayerT, MouseDistance))
                    AddNewContext(hit);
                else
                    ContextHit = false;
            }
            else ContextHit = false;
        }
        else
        {
            DetectionPoint.position = CharacterLink.PlayerT.position;
            DetectionPoint.rotation = CharacterLink.PlayerT.rotation;

            if (Physics.SphereCast(DetectionPoint.position, SphereCastRadius, DetectionPoint.forward, out hit, SphereCastMaxLength, ContextMask))
                AddNewContext(hit);
            else
                ContextHit = false;
        }

        if (!ContextHit)
        {
            LastEncounteredContext = null;
            DetectionMesh.transform.position = new Vector3(0, 8000, 0);
            if (LastHoveredMesh != null)
                LastHoveredMesh.enabled = true;
        }
        else
            ScaleItem();
    }

    void ScaleItem()
    {
        if (!AllowScale) return;

        Timer = Timer - Time.deltaTime;

        if (Timer < 0)
        {
            Timer = COEncountered.ScaleSpeed;
            ScaleUp = !ScaleUp;
        }

        float TimerPercentage = Timer / COEncountered.ScaleSpeed;
        if (ScaleUp) TimerPercentage = 1 - TimerPercentage;

        float ScaleDiff = COEncountered.ScaleAmount - 1;
        float Scale = 1 + (ScaleDiff * TimerPercentage);
        Vector3 NewScale = KnownScale * Scale;

        DetectionMeshTrans.localScale = NewScale;
    }



    void AddNewContext(RaycastHit hit)
    {
        if (hit.transform == LastEncounteredContext) return;

        LastEncounteredContext = hit.transform;
        COEncountered = LastEncounteredContext.GetComponent<DG_ContextObject>();

        if(LastHoveredMesh != null) LastHoveredMesh.enabled = true;
        ContextHit = true;


        if (COEncountered.AllowScaling)
        {
            AllowScale = true;

            LastHoveredMesh = LastEncounteredContext.transform.GetComponent<MeshRenderer>();
            LastHoveredMesh.enabled = false;
            DetectionMesh.mesh = LastEncounteredContext.GetComponent<MeshFilter>().mesh;

            DetectionRenderer.materials = LastHoveredMesh.materials;

            DetectionMesh.transform.position = LastEncounteredContext.position;
            DetectionMesh.transform.rotation = LastEncounteredContext.rotation;
            KnownScale = LastEncounteredContext.localScale;
            ScaleUp = true;
            Timer = COEncountered.ScaleSpeed / 2;
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
