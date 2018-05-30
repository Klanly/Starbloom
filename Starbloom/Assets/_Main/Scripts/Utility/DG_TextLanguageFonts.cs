using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_TextLanguageFonts : MonoBehaviour {

    public enum Languages
    {
        English,
        Russian,
        Spanish,
        Chinese,
        Javanese,
        Korean,
        Hindu
    }

    [System.Serializable]
    public class TextMeshProLanguageFiles
    {
        public Languages LanguageSetting;
        public TMPro.TMP_FontAsset FontAsset = null;
    }

    public TextMeshProLanguageFiles[] LanguageFonts;

    public void SetTMProLanguage(TMPro.TextMeshProUGUI TextMeshProObject)
    {
        TextMeshProObject.font = GetFont();
    }
    TMPro.TMP_FontAsset GetFont()
    {
        for (int i = 0; i < LanguageFonts.Length; i++)
        {
            TextMeshProLanguageFiles LF = LanguageFonts[i];
            if (LF.LanguageSetting == (DG_TextLanguageFonts.Languages)QuickFind.UserSettings.CurrentLanguage)
                return LF.FontAsset;
        }
        return null;
    }
}
