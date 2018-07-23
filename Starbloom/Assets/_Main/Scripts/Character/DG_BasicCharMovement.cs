using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_BasicCharMovement : MonoBehaviour
{
    public DG_CharacterLink CharLink;
    public Animator Anim = null;
    public Rigidbody rb = null;

    [Header("Vars")]
    public float BaseSpeed;
    public float LockOnBaseRange = 10f;

    [Header("Targeting")]
    public Transform Targeter = null;
    public bool LockOn = false;
    public Transform LockOnTarget = null;
    Vector3 KnownHeading;

    Transform _T;




    private void Awake()
    {
        _T = transform;
    }

    private void FixedUpdate()
    {
        bool inputDetected = false;

        DG_PlayerInput.Player MP = QuickFind.InputController.GetPlayerByPlayerID(CharLink.PlayerID);

        if (MP.VerticalAxis != 0 || MP.HorizontalAxis != 0) inputDetected = true;

        if (inputDetected)
        {
            Transform Camera = MP.CharLink.PlayerCam.CamTrans;
            Vector3 forward = Camera.transform.forward;
            var right = Camera.transform.right;

            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            Vector3 desiredMoveDirection = (forward * MP.VerticalAxis + right * MP.HorizontalAxis) * GetRunSpeed();
            Vector3 CurrentVel = rb.velocity;
            desiredMoveDirection.y = CurrentVel.y;
            rb.velocity = desiredMoveDirection;

            KnownHeading = desiredMoveDirection;
            Anim.SetBool("Moving", true);
        }
        else
        {
            Vector3 CurrentVel = rb.velocity;
            Vector3 NewVel = Vector3.zero;
            NewVel.y = CurrentVel.y;
            rb.velocity = NewVel;
            Anim.SetBool("Moving", false);
        }
        SetFacing();
    }


    void SetFacing()
    {
        if (LockOn)
            Targeter.position = LockOnTarget.position;
        else
            Targeter.position = _T.position + KnownHeading;

        _T.LookAt(Targeter);
    }




    float GetRunSpeed()
    {
        return BaseSpeed;
    }

    void OnDrawGizmos() //Draw Gizmo in Scene view
    {
        Vector3 Pos = transform.position;
        Pos.y += 5;
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(Pos, 1f);
    }
}
