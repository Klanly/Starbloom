using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLogic : MonoBehaviour {

    public Transform CharTrans;
    public Camera MainCam;

    [Header("Camera Control")]
    public bool AllowCameraPanning;
    public float MousePanHorSpeed;
    public float MousePanVertSpeed;
    public float JoyPanHorSpeed;
    public float JoyPanVertSpeed;


    Transform _T;


    private void Awake()
    {
        QuickFind.PlayerCam = this;
        _T = transform;       
    }


    private void Update()
    {
        if (CharTrans == null)
            return;
        _T.position = CharTrans.position;

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
    }
}
