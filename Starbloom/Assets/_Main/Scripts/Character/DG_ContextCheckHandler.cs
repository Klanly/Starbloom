using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_ContextCheckHandler : MonoBehaviour {

    public DG_CharacterLink CharacterLink = null;

    [Header("Detection Options")]
    public LayerMask ContextMask;
    [Header("Non-Mouse")]
    public float SphereCastRadius = 5f;
    public float SphereCastMaxLength = 10f;
    [Header("Mouse")]
    public float MouseCastRadius = 1.5f;
    public float MouseCastLength = 100f;


    [System.NonSerialized] public Transform LastEncounteredContext;
    [System.NonSerialized] public bool ContextHit = false;
    [System.NonSerialized] public NA_ContextObject COEncountered;
    Transform DetectionPoint;
    


    private void Awake()
    {
        DetectionPoint = transform.GetChild(0);
    }


    void Update()
    {
        RaycastHit hit;
        DG_PlayerInput.Player MP = QuickFind.InputController.MainPlayer;

        if (MP.Context == DG_PlayerInput.ContextDetection.MousePosition)
        {
            Ray ray = QuickFind.PlayerCam.MainCam.ScreenPointToRay(Input.mousePosition);
            if (Physics.SphereCast(ray.origin, MouseCastRadius, ray.direction, out hit, MouseCastLength, ContextMask))
            {
                LastEncounteredContext = hit.transform;
                COEncountered = LastEncounteredContext.GetComponent<NA_ContextObject>();
                ContextHit = true;
            }
            else
                ContextHit = false;
        }
        else
        {
            DetectionPoint.position = CharacterLink.PlayerT.position;
            DetectionPoint.rotation = CharacterLink.PlayerT.rotation;

            if (Physics.SphereCast(DetectionPoint.position, SphereCastRadius, DetectionPoint.forward, out hit, SphereCastMaxLength, ContextMask))
            {
                LastEncounteredContext = hit.transform;
                COEncountered = LastEncounteredContext.GetComponent<NA_ContextObject>();
                ContextHit = true;
            }
            else
                ContextHit = false;
        }
    }


    void OnDrawGizmos() //Draw Gizmo in Scene view
    {
        if (ContextHit)
        {
            Vector3 Pos = LastEncounteredContext.position;
            Gizmos.color = Color.yellow;
            Gizmos.DrawCube(Pos, new Vector3(2,.2f,2));
        }
    }
}
