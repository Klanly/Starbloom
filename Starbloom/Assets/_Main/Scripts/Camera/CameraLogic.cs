using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class CameraLogic : MonoBehaviour {

    public enum CameraState
    {
        Disabled,
        DisabledHideMouse,
        Thirdperson,
        Isometric
    }
    public enum UserCameraMode
    {
        Thirdperson,
        Isometric
    }
    public enum ControlMode
    {
        Mouse,
        AutoPan
    }
    public enum ContextDetection
    {
        InfrontPlayer,
        MiddleScreenPosition,
        MousePosition
    }



    [System.Serializable]
    public class ThirdPersonControlVariables
    {
        [Header("True Pan Speeds")]
        public float MousePanHorSpeed;
        public float MousePanVertSpeed;
        public float JoyPanHorSpeed;
        public float JoyPanVertSpeed;
        [Header("Slerp Speed")]
        public float RotationalSlerp;
        [Header("X Restriction")]
        public float MaxX;
        public float MinX;
        [Header("Distance")]
        public float CameraDistance;
    }

    [System.Serializable]
    public class IsometricVariables
    {
        [Header("Variables")]
        public float CameraDistance;
    }

    [System.Serializable]
    public class PlayerCamRigg
    {
        public Transform Rigg;
        public Camera MainCam;
        [Header("State")]
        public UserCameraMode CurrentCameraAngle;
        [ReadOnly] public CameraState CurrentCameraState;
        [ReadOnly] public ContextDetection CurrentDetectionStyle;
        [ReadOnly] public int PlayerID = -1;
        [System.NonSerialized] public Transform CamTrans;
        [System.NonSerialized] public UserCameraMode KnownCameraAngle;
    }

    public PlayerCamRigg[] CameraRiggs;
    public ThirdPersonControlVariables ThirdPersonVariables;
    public IsometricVariables IsoVariables;
    public float CameraTransitionSpeed;
    Transform CamHelper;
    bool isPlayer1 = true;



    private void Awake()
    {
        QuickFind.PlayerCam = this;
        CameraRiggs[0].CamTrans = CameraRiggs[0].MainCam.transform;
        CameraRiggs[1].CamTrans = CameraRiggs[1].MainCam.transform;

        CameraRiggs[0].KnownCameraAngle = CameraRiggs[0].CurrentCameraAngle;
        CameraRiggs[1].KnownCameraAngle = CameraRiggs[1].CurrentCameraAngle;

        CamHelper = new GameObject().transform;
        CamHelper.SetParent(transform);
        CamHelper.eulerAngles = CameraRiggs[0].Rigg.eulerAngles;
    }
    private void Update()
    {
        for (int i = 0; i < CameraRiggs.Length; i++)
        {
            PlayerCamRigg PCR = CameraRiggs[i];
            if (PCR.PlayerID == -1) continue;

            switch (PCR.CurrentCameraState)
            {
                case CameraState.Disabled: { EnableMouse(true); return; }
                case CameraState.DisabledHideMouse: { EnableMouse(false); return; }
                case CameraState.Thirdperson: { EnableMouse(QuickFind.UserSettings.ThirdPersonCameraControlMode == ControlMode.Mouse); HandleThirdPerson(PCR); } break;
                case CameraState.Isometric: { EnableMouse(QuickFind.UserSettings.IsometricCameraControlMode == ControlMode.Mouse); HandleIsometric(PCR); } break;
            }
        }
    }





    //Third Person
    void HandleThirdPerson(PlayerCamRigg PCR)
    {
        if (QuickFind.PlayerTrans == null) return;
        PCR.Rigg.position = QuickFind.PlayerTrans.position;

        DG_PlayerInput.Player MP;
        if(isPlayer1) MP = QuickFind.InputController.Players[0];
        else MP = QuickFind.InputController.Players[1];

        bool AllowRotationChange = false;
        if (MP.CamVerticalAxis != 0 || MP.CamHorizontalAxis != 0) AllowRotationChange = true;
        if (QuickFind.UserSettings.ThirdPersonCameraControlMode == ControlMode.Mouse && !Input.GetMouseButton(2)) AllowRotationChange = false;

        if (AllowRotationChange)
        {
            if (QuickFind.UserSettings.ThirdPersonCameraControlMode == ControlMode.Mouse) { Cursor.visible = false; Cursor.lockState = CursorLockMode.Locked; }

            Vector3 CurAngle = CamHelper.eulerAngles;

            float VertMult = -ThirdPersonVariables.MousePanVertSpeed;
            if (MP.ButtonSet.RJoyVert.Held) VertMult = ThirdPersonVariables.JoyPanVertSpeed;
            float HorMult = ThirdPersonVariables.MousePanHorSpeed;
            if (MP.ButtonSet.RJoyHor.Held) HorMult = ThirdPersonVariables.JoyPanHorSpeed;

            CurAngle.z = 0;
            CurAngle.y += MP.CamHorizontalAxis * HorMult * QuickFind.UserSettings.CameraSensitivity;
            CurAngle.x += MP.CamVerticalAxis * VertMult * QuickFind.UserSettings.CameraSensitivity;
            if (CurAngle.x > ThirdPersonVariables.MaxX) CurAngle.x = ThirdPersonVariables.MaxX;
            if (CurAngle.x < ThirdPersonVariables.MinX) CurAngle.x = ThirdPersonVariables.MinX;

            CamHelper.eulerAngles = CurAngle;
        }

        Quaternion NewRotation = Quaternion.Slerp(PCR.Rigg.rotation, CamHelper.rotation, ThirdPersonVariables.RotationalSlerp);
        PCR.Rigg.rotation = NewRotation;


        Vector3 CamPos = PCR.CamTrans.localPosition;
        if (CamPos.z != ThirdPersonVariables.CameraDistance)
        {
            CamPos.z = ThirdPersonVariables.CameraDistance;
            PCR.CamTrans.localPosition = Vector3.Slerp(PCR.CamTrans.localPosition, CamPos, CameraTransitionSpeed);
        }

    }

    //Isometric
    void HandleIsometric(PlayerCamRigg PCR)
    {
        if (QuickFind.PlayerTrans == null) return;
        PCR.Rigg.position = QuickFind.PlayerTrans.position;
        CamHelper.eulerAngles = new Vector3(30, 0, 0);
        Quaternion NewRotation = Quaternion.Slerp(PCR.Rigg.rotation, CamHelper.rotation, ThirdPersonVariables.RotationalSlerp);
        PCR.Rigg.rotation = NewRotation;


        Vector3 CamPos = PCR.CamTrans.localPosition;
        if (CamPos.z != IsoVariables.CameraDistance)
        {
            CamPos.z = IsoVariables.CameraDistance;
            PCR.CamTrans.localPosition = Vector3.Slerp(PCR.CamTrans.localPosition, CamPos, CameraTransitionSpeed);
        }
    }




    public void InstantSetCameraAngle(float CameraFacing, PlayerCamRigg PCR) { Vector3 Pos = new Vector3(30, CameraFacing, 0); PCR.Rigg.eulerAngles = Pos; CamHelper.eulerAngles = Pos; }
    public void EnableCamera(PlayerCamRigg PCR, bool isTrue, bool ShowMouse = false)
    {
        if (isTrue)
        {
            if (PCR.CurrentCameraAngle == UserCameraMode.Thirdperson) PCR.CurrentCameraState = CameraState.Thirdperson;
            if (PCR.CurrentCameraAngle == UserCameraMode.Isometric) PCR.CurrentCameraState = CameraState.Isometric;
        }
        else
        {
            if (ShowMouse) PCR.CurrentCameraState = CameraState.Disabled;
            else PCR.CurrentCameraState = CameraState.DisabledHideMouse;
        }
    }
    void EnableMouse(bool isTrue)
    {
        Cursor.visible = isTrue;
        if (isTrue)
        {
            if (Application.isEditor || !Application.isFocused || QuickFind.GameSettings.DisableLockMouseToScreen) Cursor.lockState = CursorLockMode.None;
            else Cursor.lockState = CursorLockMode.Confined;
        }
        else
            Cursor.lockState = CursorLockMode.Locked;
    }
}
