using UnityEngine;
using System.Collections;
using System.Collections.Generic;





#if UNITY_EDITOR
using UnityEditor.SceneManagement;
public static class QuickFindInEditor
{
    public static NodeViewOptions NodeViewOptions = null;
    public static NodeViewOptions GetEditorNodeViewOptions()
    {
        if (GameObject.Find("NodeViewerOptions") != null)
            return GameObject.Find("NodeViewerOptions").GetComponent<NodeViewOptions>();
        else
        {
            EditorSceneManager.OpenScene("Assets/_Main/Scenes/Additive/EditorOnly/EditorOnlyScene.unity", OpenSceneMode.Additive);
            return GameObject.Find("NodeViewerOptions").GetComponent<NodeViewOptions>();
        }
    }


    //Database
    public static DG_DialogueManager GetEditorDialogueManager() { return GameObject.Find("Dialogue Database").GetComponent<DG_DialogueManager>(); }
    public static DG_WordDatabase GetEditorWordDatabase() { return GameObject.Find("Word Database").GetComponent<DG_WordDatabase>(); }
    public static DG_QuestDatabase GetEditorQuestDatabase() { return GameObject.Find("Quest Database").GetComponent<DG_QuestDatabase>(); }
    public static DG_ItemsDatabase GetEditorItemDatabase() { return GameObject.Find("Item Database").GetComponent<DG_ItemsDatabase>(); }
    public static DG_CharacterDatabase GetEditorCharacterDatabase() { return GameObject.Find("Character Database").GetComponent<DG_CharacterDatabase>(); }
    public static DG_ColorCodes GetEditorColorCodes() { return GameObject.Find("Color Database").GetComponent<DG_ColorCodes>(); }

    //Save Data
    public static DG_DataBoolManager GetEditorDataBools() { return GameObject.Find("Bool Tracker").GetComponent<DG_DataBoolManager>(); }
    public static DG_DataIntManager GetEditorDataInts() { return GameObject.Find("Int Tracker").GetComponent<DG_DataIntManager>(); }
    public static DG_DataFloatManager GetEditorDataFloats() { return GameObject.Find("Float Tracker").GetComponent<DG_DataFloatManager>(); }
    public static DG_DataStringManager GetEditorDataStrings() { return GameObject.Find("String Tracker").GetComponent<DG_DataStringManager>(); }
    public static UserSettings GetEditorUserSettings() { return GameObject.Find("Player Settings").GetComponent<UserSettings>(); }

}
#endif





public static class QuickFind
{
    //InputController
    public static DG_PlayerInput InputController = null;
    public static DG_CharacterControllers CharacterManager = null;


    //GUI
    public static DG_GUIMainMenu MainMenuUI = null;
    public static DG_GUI_FadeScreen FadeScreen = null;
    public static NA_DialogueGUIController DialogueGUIController = null;
    public static DG_TextPrintout TextPrintout = null;
    public static DG_GUINameChange NameChangeUI = null;
    public static DG_GUIControllerGhange ControllerChange = null;
    public static DG_GUIContextHandler GUIContextHandler = null;


    //Cameras
    public static CameraLogic PlayerCam;
    public static EZCameraShake.CameraShaker CameraShake = null;


    //Network
    public static ConnectAndJoinRandom NetworkMaster = null;
    public static DG_NetworkSync IDMaster = null;


    //Database
    public static DG_DialogueManager DialogueManager = null;
    public static DG_WordDatabase WordDatabase = null;
    public static DG_QuestDatabase QuestDatabase = null;
    public static DG_ItemsDatabase ItemDatabase = null;
    public static DG_CharacterDatabase CharacterDatabase = null;
    public static DG_ColorCodes ColorDatabase = null;

    //To Be removed
    public static DG_PlayerInventory PlayerInventory = null;


    //Save Data
    public static DG_DataBoolManager DataBools = null;
    public static DG_DataIntManager DataInts = null;
    public static DG_DataFloatManager DataFloats = null;
    public static DG_DataStringManager DataStrings = null;
    public static UserSettings UserSettings = null;


    //Environment
    public static WeatherHandler WeatherHandler = null;
    public static Tenkoku.Core.TenkokuModule WeatherModule = null;
    public static TimeHandler TimeHandler = null;

    //Serialization
    public static DG_LocalDataHandler SaveHandler;


    //Debug
    public static DG_DebugSettings GameSettings = null;














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
