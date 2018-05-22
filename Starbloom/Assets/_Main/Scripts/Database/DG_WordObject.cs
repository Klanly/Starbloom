using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
/////////////////////////////////////////////////////////////////////////////////Editor Extension Buttons
[CustomEditor(typeof(DG_WordObject))]
class DG_WordObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        //Buttons

        DG_WordObject myScript = (DG_WordObject)target;
        if (GUILayout.Button("UpdateLanguages"))
            myScript.UpdateLanguages();
    }
}
//////////////////////////////////////////////////////////////////////////////////
#endif




public class DG_WordObject : MonoBehaviour {

    [System.Serializable]
    public class TextEntry
    {
        public UserSettings.Languages Language;
        [TextArea(4, 10)]
        public string stringEntry;
    }

    [HideInInspector] public int DatabaseID;
    [HideInInspector] public bool LockItem;
    public string ObjectName;

    public TextEntry[] TextValues;



    public void UpdateLanguages()
    {
        PopulateLanguageOptions(this);
    }

    public void PopulateLanguageOptions(DG_WordObject CloneScript)
    {
        int languageCount = System.Enum.GetValues(typeof(UserSettings.Languages)).Length;
        if(TextValues == null || TextValues.Length < languageCount)
        {
            TextEntry[] NewArray = new TextEntry[languageCount];
            for(int i = 0; i < languageCount; i++)
            {
                NewArray[i] = new TextEntry();

                if (TextValues != null)
                {
                    if (i < TextValues.Length && TextValues[i] != null)
                        NewArray[i].stringEntry = TextValues[i].stringEntry;
                }

                NewArray[i].Language = (UserSettings.Languages)i;
            }
            TextValues = NewArray;
        }
    }

    public TextEntry GetTextEntryByLanguage(UserSettings.Languages Language)
    {
        for(int i = 0; i < TextValues.Length; i++)
        {
            if (TextValues[i].Language == Language)
                return TextValues[i];
        }
        return null;
    }
}
