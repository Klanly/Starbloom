using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserSettings : MonoBehaviour {

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



    public float TextSpeed = .03f;
    public Languages CurrentLanguage = Languages.English;
    public TextMeshProLanguageFiles[] LanguageFonts;








    private void Awake()
    {
        QuickFind.UserSettings = this;
    }



    public void SetTMProLanguage(TMPro.TextMeshProUGUI TextMeshProObject)
    {
        TextMeshProObject.font = GetFont();
    }
    TMPro.TMP_FontAsset GetFont()
    {
        for(int i = 0; i < LanguageFonts.Length;i++)
        {
            TextMeshProLanguageFiles LF = LanguageFonts[i];
            if (LF.LanguageSetting == CurrentLanguage)
                return LF.FontAsset;
        }
        return null;
    }
}
