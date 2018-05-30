using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UserSettings : MonoBehaviour {

    //Gameplay
    public float TextSpeed = .04f;
    public int CurrentLanguage = 0;





    bool Save;
    string SettingsDirectory;


    private void Awake()
    {
        QuickFind.UserSettings = this;
        SettingsDirectory = DG_LocalDataHandler.FindOrCreateSaveDirectory(Environment.CurrentDirectory, "UserSettings") + "/";
        LoadAllSettings();
    }



    public void LoadAllSettings()
    {
        Save = false;
        SetText(TextSpeed);
        SetLang(CurrentLanguage);
        Save = true;
    }



    public void SetText(float Value){ if(!Save) Value = (float)SaveOrLoad("TextSpeed",       "Float", TextSpeed);       TextSpeed       = Value; }
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
