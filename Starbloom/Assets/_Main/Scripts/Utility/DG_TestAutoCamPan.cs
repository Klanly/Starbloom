using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class DG_TestAutoCamPan : MonoBehaviour {

    [Header("Vertical Pan")]
    public float NewRequestedXPosition;
    public float XSpeed;
    [Header("Horizontal Pan")]
    public float NewRequestedYPosition;
    public float YSpeed;

    [Button(ButtonSizes.Small)]
    public void SetNewRequestedVerticalPosition()
    {
        QuickFind.PlayerCam.SetNewVerticalPanPosition(NewRequestedXPosition, XSpeed);
    }
    [Button(ButtonSizes.Small)]
    public void SetNewRequestedHorizontalPosition()
    {
        QuickFind.PlayerCam.SetNewHorizontalPanPosition(NewRequestedYPosition, YSpeed);
    }

}
