using UnityEngine;
using System.Collections;
using System.Collections.Generic;





#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif





public static class QuickFind
{
    //InputController
    public static DG_PlayerInput InputController = null;
    public static DG_CharacterControllers CharacterManager = null;

    //GUI
    public static DG_GUI_FadeScreen FadeScreen = null;

    //Cameras
    public static CameraLogic PlayerCam;


    //Network
    public static ConnectAndJoinRandom NetworkMaster = null;
    public static DG_NetworkSync IDMaster = null;














    //Reference Utilities
    public static Transform FindTransform(Transform parent, string name)
    {
        Transform[] children = parent.GetComponentsInChildren<Transform>();
        foreach (Transform child in children)
        {
            if (child.name == name)
                return child;
        }
        return null;
    }
    public static bool WithinDistance(Transform Object, Transform Target, float MasterMinDistance)
    {
        if (Vector3.Distance(Object.position, Target.position) < MasterMinDistance)
            return true;
        else
            return false;
    }
    public static Vector3 AngleLerp(Vector3 StartAngle, Vector3 FinishAngle, float t)
    {
        float xLerp = Mathf.LerpAngle(StartAngle.x, FinishAngle.x, t);
        float yLerp = Mathf.LerpAngle(StartAngle.y, FinishAngle.y, t);
        float zLerp = Mathf.LerpAngle(StartAngle.z, FinishAngle.z, t);
        Vector3 Lerped = new Vector3(xLerp, yLerp, zLerp);
        return Lerped;
    }
    public static int GetNextValueInArray(int current, int ArrayLength, bool Add, bool CanLoop)
    {
        int Return = current;
        if(Add)
        {
            Return++;
            if (Return == ArrayLength)
            {
                if (CanLoop)
                    Return = 0;
                else
                    Return--;
            }
        }
        else
        {
            Return--;
            if (Return < 0)
            {
                if (CanLoop)
                    Return = ArrayLength - 1;
                else
                    Return++;
            }
        }

        return Return;
    }
    public static void EnableCanvas(CanvasGroup C, bool isTrue)
    {
        float value = 0;
        if (isTrue)
            value = 1;
        C.alpha = value;
        C.interactable = isTrue;
        C.blocksRaycasts = isTrue;
    }
}
