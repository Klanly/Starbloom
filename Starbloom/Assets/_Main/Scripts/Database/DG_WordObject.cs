using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;




public class DG_WordObject : MonoBehaviour {

    [System.Serializable]
    public class TextEntry
    {
        public DG_TextLanguageFonts.Languages Language;
        [TextArea(4, 10)]
        public string stringEntry;
    }

    [HideInInspector] public int DatabaseID;
    [HideInInspector] public bool LockItem;
    public string Name;

    public string DevNotes;
    [Header("Values")]
    [ListDrawerSettings(ListElementLabelName = "Language")]
    public TextEntry[] TextValues;


    [Button(ButtonSizes.Small)]
    public void UpdateLanguages()
    {
        PopulateLanguageOptions(this);
    }

    public void PopulateLanguageOptions(DG_WordObject CloneScript)
    {
        int languageCount = System.Enum.GetValues(typeof(DG_TextLanguageFonts.Languages)).Length;
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

                NewArray[i].Language = (DG_TextLanguageFonts.Languages)i;
            }
            TextValues = NewArray;
        }
    }

    public TextEntry GetTextEntryByLanguage(DG_TextLanguageFonts.Languages Language)
    {
        for(int i = 0; i < TextValues.Length; i++)
        {
            if (TextValues[i].Language == Language)
                return TextValues[i];
        }
        return null;
    }




    [Button(ButtonSizes.Small)]
    public void SyncNameToText()
    {
        Name = GetFirst10Letters(TextValues[0].stringEntry);
    }
    string GetFirst10Letters(string Base)
    {
        char[] c = Base.ToCharArray();
        System.Text.StringBuilder SB = new System.Text.StringBuilder();
        int count = 10;
        if (c.Length < 10) count = c.Length;
        for (int i = 0; i < count; i++)
            SB.Append(c[i]);
        return SB.ToString();
    }

}
