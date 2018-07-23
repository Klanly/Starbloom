using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Rendering.PostProcessing;

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
        //[ReadOnly]
        public int PlayerID = -1;
        [System.NonSerialized] public Transform CamTrans;
        [System.NonSerialized] public UserCameraMode KnownCameraAngle;

        [System.NonSerialized] public Transform CamHelper;
    }

    public PlayerCamRigg[] CameraRiggs;
    public ThirdPersonControlVariables ThirdPersonVariables;
    public IsometricVariables IsoVariables;
    public float CameraTransitionSpeed;




    private void Awake()
    {
        QuickFind.PlayerCam = this;
        CameraRiggs[0].CamTrans = CameraRiggs[0].MainCam.transform;
        CameraRiggs[1].CamTrans = CameraRiggs[1].MainCam.transform;

        CameraRiggs[0].KnownCameraAngle = CameraRiggs[0].CurrentCameraAngle;
        CameraRiggs[1].KnownCameraAngle = CameraRiggs[1].CurrentCameraAngle;

        CameraRiggs[0].CamHelper = new GameObject().transform;
        CameraRiggs[0].CamHelper.SetParent(transform);
        CameraRiggs[0].CamHelper.eulerAngles = CameraRiggs[0].Rigg.eulerAngles;

        CameraRiggs[1].CamHelper = new GameObject().transform;
        CameraRiggs[1].CamHelper.SetParent(transform);
        CameraRiggs[1].CamHelper.eulerAngles = CameraRiggs[1].Rigg.eulerAngles;
    }
    private void Update()
    {
        for (int i = 0; i < CameraRiggs.Length; i++)
        {
            PlayerCamRigg PCR = CameraRiggs[i];
            if (PCR.PlayerID == -1) continue;

            UserSettings.PlayerSettings PS = QuickFind.UserSettings.SingleSettings;
            if(QuickFind.NetworkSync.Player2PlayerCharacter != -1) PS = QuickFind.UserSettings.CoopSettings[i];

            switch (PCR.CurrentCameraState)
            {
                case CameraState.Disabled: { EnableMouse(true); return; }
                case CameraState.DisabledHideMouse: { EnableMouse(false); return; }
                case CameraState.Thirdperson: { EnableMouse(PS.ThirdPersonCameraControlMode == ControlMode.Mouse); HandleThirdPerson(PCR, PS, i); } break;
                case CameraState.Isometric: { EnableMouse(PS.IsometricCameraControlMode == ControlMode.Mouse); HandleIsometric(PCR, i); } break;
            }
        }
    }





    //Third Person
    void HandleThirdPerson(PlayerCamRigg PCR, UserSettings.PlayerSettings PS, int index)
    {
        DG_PlayerInput.Player MP = QuickFind.InputController.Players[index];

        if (MP.CharLink == null) return;
        PCR.Rigg.position = MP.CharLink.PlayerTrans.position;

        bool AllowRotationChange = false;
        if (MP.CamVerticalAxis != 0 || MP.CamHorizontalAxis != 0) AllowRotationChange = true;
        if (PS.ThirdPersonCameraControlMode == ControlMode.Mouse && !Input.GetMouseButton(2)) AllowRotationChange = false;

        if (AllowRotationChange)
        {
            if (PS.ThirdPersonCameraControlMode == ControlMode.Mouse) { Cursor.visible = false; Cursor.lockState = CursorLockMode.Locked; }

            Vector3 CurAngle = CameraRiggs[index].CamHelper.eulerAngles;

            CurAngle.z = 0;
            CurAngle.y += MP.CamHorizontalAxis * PS.CameraHorizontalPanSpeed;
            CurAngle.x += MP.CamVerticalAxis * PS.CameraVerticalPanSpeed;
            if (CurAngle.x > ThirdPersonVariables.MaxX) CurAngle.x = ThirdPersonVariables.MaxX;
            if (CurAngle.x < ThirdPersonVariables.MinX) CurAngle.x = ThirdPersonVariables.MinX;

            CameraRiggs[index].CamHelper.eulerAngles = CurAngle;
        }

        Quaternion NewRotation = Quaternion.Slerp(PCR.Rigg.rotation, CameraRiggs[index].CamHelper.rotation, ThirdPersonVariables.RotationalSlerp);
        PCR.Rigg.rotation = NewRotation;


        Vector3 CamPos = PCR.CamTrans.localPosition;
        if (CamPos.z != ThirdPersonVariables.CameraDistance)
        {
            CamPos.z = ThirdPersonVariables.CameraDistance;
            PCR.CamTrans.localPosition = Vector3.Slerp(PCR.CamTrans.localPosition, CamPos, CameraTransitionSpeed);
        }

    }

    //Isometric
    void HandleIsometric(PlayerCamRigg PCR, int index)
    {
        Transform PlayerTrans = QuickFind.NetworkSync.GetCharacterLinkByPlayerID(PCR.PlayerID).PlayerTrans;

        if (PlayerTrans == null) return;
        PCR.Rigg.position = PlayerTrans.position;
        CameraRiggs[index].CamHelper.eulerAngles = new Vector3(30, 0, 0);
        Quaternion NewRotation = Quaternion.Slerp(PCR.Rigg.rotation, CameraRiggs[index].CamHelper.rotation, ThirdPersonVariables.RotationalSlerp);
        PCR.Rigg.rotation = NewRotation;


        Vector3 CamPos = PCR.CamTrans.localPosition;
        if (CamPos.z != IsoVariables.CameraDistance)
        {
            CamPos.z = IsoVariables.CameraDistance;
            PCR.CamTrans.localPosition = Vector3.Slerp(PCR.CamTrans.localPosition, CamPos, CameraTransitionSpeed);
        }
    }




    public void InstantSetCameraAngle(float CameraFacing, PlayerCamRigg PCR, int index) { Vector3 Pos = new Vector3(30, CameraFacing, 0); PCR.Rigg.eulerAngles = Pos; CameraRiggs[index].CamHelper.eulerAngles = Pos; }
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
