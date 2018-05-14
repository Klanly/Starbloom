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
        if (GUILayout.Button("FindNextAvailableDatabaseID"))
            myScript.FindNextAvailableDatabaseID();
        if (GUILayout.Button("MakeNewObject"))
            myScript.MakeNewObject();
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

    public int DatabaseID;
    public string DevNotes;
    public TextEntry[] TextValues;




    public void FindNextAvailableDatabaseID()
    {
        int HighestNumber = 0;

        Transform Child = transform.parent;
        for (int iN = 0; iN < Child.childCount; iN++)
        {
            DG_WordObject BoolItem = Child.GetChild(iN).GetComponent<DG_WordObject>();
            if (BoolItem == this && BoolItem.DatabaseID != 0)
            {
                Debug.Log("This Object Already Has a Database ID");
                return;
            }
            if (BoolItem.DatabaseID > HighestNumber)
                HighestNumber = BoolItem.DatabaseID;
        }
        DatabaseID = HighestNumber + 1;
        transform.gameObject.name = DatabaseID.ToString();
    }
    public void MakeNewObject()
    {
        GameObject NewClone = GameObject.Instantiate(transform.gameObject);
        NewClone.transform.SetParent(transform.parent);

        DG_WordObject CloneScript = NewClone.GetComponent<DG_WordObject>();
        CloneScript.DatabaseID = 0;
        CloneScript.FindNextAvailableDatabaseID();

        CloneScript.DevNotes = string.Empty;
        PopulateLanguageOptions(CloneScript);
    }
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
