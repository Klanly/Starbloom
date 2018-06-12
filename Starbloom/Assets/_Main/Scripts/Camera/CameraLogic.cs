using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLogic : MonoBehaviour {


    public Camera MainCam;

    [Header("Camera Control")]
    public bool AllowCameraPanning;
    public float MousePanHorSpeed;
    public float MousePanVertSpeed;
    public float JoyPanHorSpeed;
    public float JoyPanVertSpeed;

    [Header("Auto Panning")]
    public float DefaultVertSpeed;
    public float DefaultHorSpeed;


    Transform _T;

    //AutoPaning 
    bool AutoPanVertical; float NewAutoPanValueX; float AutoPanXSpeed;
    bool AutoPanHorizontal; float NewAutoPanValueY; float AutoPanYSpeed;



    private void Awake()
    {
        QuickFind.PlayerCam = this;
        _T = transform;
        AutoPanVertical = false;
        AutoPanHorizontal = false;
    }


    private void Update()
    {
        if (QuickFind.PlayerTrans == null) return;
        _T.position = QuickFind.PlayerTrans.position;

        if (AllowCameraPanning)
        {
            DG_PlayerInput.Player MP = QuickFind.InputController.MainPlayer;
            if (MP.CamVerticalAxis != 0 || MP.CamHorizontalAxis != 0)
            {
                Vector3 CurAngle = _T.eulerAngles;

                float VertMult = -MousePanVertSpeed;
                if (MP.ButtonSet.RJoyVert.Held) VertMult = JoyPanVertSpeed;
                float HorMult = MousePanHorSpeed;
                if (MP.ButtonSet.RJoyHor.Held) HorMult = JoyPanHorSpeed;


                CurAngle.x += MP.CamVerticalAxis * VertMult;
                CurAngle.y += MP.CamHorizontalAxis * HorMult;

                _T.eulerAngles = CurAngle;
            }
        }

        if(AutoPanVertical)
        {
            Vector3 CamStartPos = QuickFind.PlayerCam.transform.localEulerAngles;
            Vector3 CamEndPos = CamStartPos; CamStartPos.x = NewAutoPanValueX;
            QuickFind.PlayerCam.transform.localEulerAngles = QuickFind.AngleLerp(CamEndPos,CamStartPos, AutoPanXSpeed * Time.deltaTime);
            if(QuickFind.PlayerCam.transform.localEulerAngles.x == NewAutoPanValueX) { AutoPanVertical = false; }
        }
        if (AutoPanHorizontal)
        {
            Vector3 CamStartPos = QuickFind.PlayerCam.transform.localEulerAngles;
            Vector3 CamEndPos = CamStartPos; CamStartPos.y = NewAutoPanValueY;
            QuickFind.PlayerCam.transform.localEulerAngles = QuickFind.AngleLerp(CamEndPos, CamStartPos, AutoPanYSpeed * Time.deltaTime);
            if (QuickFind.PlayerCam.transform.localEulerAngles.y == NewAutoPanValueY) { AutoPanHorizontal = false; }
        }
    }

    public void InstantSetCameraAngle(Vector3 Euler)
    {
        transform.eulerAngles = new Vector3(30, Euler.y, 0);
    }


    public void SetNewVerticalPanPosition(float Vertical, float VerticalSpeed = 0){AutoPanVertical = true; NewAutoPanValueX = Vertical;if (VerticalSpeed != 0) AutoPanXSpeed = VerticalSpeed; else AutoPanXSpeed = DefaultVertSpeed;}
    public void SetNewHorizontalPanPosition(float Horizontal, float HorizontalSpeed = 0){AutoPanHorizontal = true; NewAutoPanValueY = Horizontal;if(HorizontalSpeed != 0) AutoPanYSpeed = HorizontalSpeed; else AutoPanYSpeed = DefaultHorSpeed;}
}
