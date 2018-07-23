using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
using UnityEditor;
#endif


public class UserSettings : MonoBehaviour {

    [System.Serializable]
    public class PlayerSettings
    {
        [Header("Camera")]
        public float CameraHorizontalPanSpeed;
        public float CameraVerticalPanSpeed;

        [Header("Third Person Settings")]
        public CameraLogic.ControlMode ThirdPersonCameraControlMode;
        public CameraLogic.ContextDetection ThirdPersonInteractionDetection;
        public CameraLogic.ContextDetection ThirdPersonEnemyDetectionMode;
        public CameraLogic.ContextDetection ThirdPersonBreakableDetectionMode;
        public CameraLogic.ContextDetection ThirdPersonObjectPlacementDetectionMode;


        [Header("Isometric Settings")]
        public CameraLogic.ControlMode IsometricCameraControlMode;
        public CameraLogic.ContextDetection IsometricInteractMode;
        public CameraLogic.ContextDetection IsometricBreakableDetectionMode;
        public CameraLogic.ContextDetection IsometricEnemyDetectionMode;
        public CameraLogic.ContextDetection IsometricObjectPlacementDetectionMode;

        [Header("Gameplay - Text")]
        public float TextSpeed = .04f;
    }

    public PlayerSettings SingleSettings;
    public PlayerSettings[] CoopSettings;


    [Header("Gameplay - Text")]
    public int CurrentLanguage = 0;
    [Header("Graphics")]
    public bool GlobalDisableWaterReflection = false;





    bool Save;
    string SettingsDirectory;


    private void Awake()
    {
        QuickFind.UserSettings = this;
        SettingsDirectory = DG_LocalDataHandler.FindOrCreateSaveDirectory(Environment.CurrentDirectory, "UserSettings") + "/";


#if UNITY_EDITOR
        ArrayList SceneViews = SceneView.sceneViews;
        foreach (SceneView SV in SceneViews)
        {
            if (SV.in2DMode)
            {
                Debug.Log("Disabling Water Reflections while in 2D mode.");
                GlobalDisableWaterReflection = true;
            }
        }
#endif

    }

    private void Start()
    {
        LoadAllSettings();
    }


    public void LoadAllSettings()
    {
        UserSettings.PlayerSettings PS = QuickFind.UserSettings.SingleSettings;

        Save = false;
        SetText(PS.TextSpeed, PS);
        SetLang(CurrentLanguage);
        Save = true;
    }



    public void SetText(float Value, UserSettings.PlayerSettings PS) { if(!Save) Value = (float)SaveOrLoad("TextSpeed",       "Float", PS.TextSpeed); PS.TextSpeed       = Value; }
    public void SetLang(int   Value){ if(!Save) Value = (int)  SaveOrLoad("CurrentLanguage", "Int",   CurrentLanguage); CurrentLanguage = Value; }















    //ShortCut
    object SaveOrLoad(string s, string t, object o)
    {
        switch (t)
        {
            case "Bool": if (Save) SaveShortCut(t, o, s); else o = LoadOrCreate(t, o, s); break;
            case "Int": if (Save) SaveShortCut(t, o, s); else o = LoadOrCreate(t, o, s); break;
            case "Float": if (Save) SaveShortCut(t, o, s); else o = LoadOrCreate(t, o, s); break;
        }
        return o;
    }
    object LoadOrCreate(string Value, object Current, string Name){if (!Check(Name)){SaveShortCut(Value, Current, Name);return Current;}else return LoadShortCut(Name, Value);}
    //Check
    bool Check(string Value) { if (DG_LocalDataHandler.CheckIfOverwritingPreviousFile(SettingsDirectory, Value)) return true; else return false; }



    //Load
    object LoadShortCut(string ValueWanted, string SaveValue)
    {
        switch (SaveValue)
        {
            case "Bool": return LoadBool(ValueWanted);
            case "Int": return LoadInt(ValueWanted);
            case "Float": return LoadFloat(ValueWanted);
        }
        return null;
    }
    bool LoadBool(string Value) { return DG_LocalDataHandler.LoadBool(SettingsDirectory + Value); }
    int LoadInt(string Value) { return DG_LocalDataHandler.LoadInt(SettingsDirectory + Value); }
    float LoadFloat(string Value) { return DG_LocalDataHandler.LoadFloat(SettingsDirectory + Value); }



    //Save
    void SaveShortCut(string SaveValue, object Object, string Name)
    {
        switch(SaveValue)
        {
            case "Bool": BoolAdjusted(Name, (bool)Object); break;
            case "Int": IntAdjusted(Name, (int)Object); break;
            case "Float": FloatAdjusted(Name, (float)Object); break;
        }
    }
    void BoolAdjusted(string Object, bool Value) { DG_LocalDataHandler.SaveBool(Value, SettingsDirectory + Object); }
    void IntAdjusted(string Object, int Value) { DG_LocalDataHandler.SaveInt(Value, SettingsDirectory + Object); }
    void FloatAdjusted(string Object, float Value) { DG_LocalDataHandler.SaveFloat(Value, SettingsDirectory + Object); }
}
