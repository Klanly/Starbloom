using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLogic : MonoBehaviour {

    public Transform CharTrans;
    public Camera MainCam;

    [Header("Debug")]
    public bool AllowCameraPanning;
    public float MousePanHorSpeed;
    public float MousePanVertSpeed;


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
            if (Input.GetMouseButton(2))
            {
                Vector3 CurAngle = _T.eulerAngles;
                CurAngle.x += Input.GetAxis("Vertical") * -MousePanHorSpeed;
                CurAngle.y += Input.GetAxis("Horizontal") * MousePanVertSpeed;
                _T.eulerAngles = CurAngle;
            }
        }
    }
}
