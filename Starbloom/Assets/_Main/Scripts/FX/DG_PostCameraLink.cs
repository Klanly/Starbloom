using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_PostCameraLink : MonoBehaviour {

    public int Cameraindex;

    void OnPreCull()
    {
        CameraLogic.PlayerCamRigg PCR = QuickFind.PlayerCam.CameraRiggs[Cameraindex];
        QuickFind.SnowHandler.ExternalOnPreCull(PCR, Cameraindex);
    }

    void OnPreRender()
    {
        CameraLogic.PlayerCamRigg PCR = QuickFind.PlayerCam.CameraRiggs[Cameraindex];
        QuickFind.SnowHandler.ExternalOnPreRender(PCR);
    }

    void OnPostRender()
    {
        CameraLogic.PlayerCamRigg PCR = QuickFind.PlayerCam.CameraRiggs[Cameraindex];
    }
}
