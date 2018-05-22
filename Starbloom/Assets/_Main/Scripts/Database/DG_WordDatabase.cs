using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DG_WordDatabase : MonoBehaviour {

    [HideInInspector] public DG_WordObject[] ItemCatagoryList;
    [HideInInspector] public int ListCount;

    private void Awake()
    {
        QuickFind.WordDatabase = this;
    }



    public string GetWordFromID(int ID = 0)
    {
        return GetWordByLanguage((GetItemFromID(ID).TextValues));
    }

    public string GetNameFromID(int CatID = 0, int ID = 0, bool FirstTimeNameShown = false)
    {
        string ReturnString = string.Empty;
        DG_CharacterObject CharacterObject;

        CharacterObject = QuickFind.CharacterDatabase.GetItemFromID(ID);

        DG_WordObject WO = QuickFind.WordDatabase.GetItemFromID(CharacterObject.NameWordID);
        ReturnString = GetWordByLanguage(WO.TextValues);
        return ReturnString;
    }

    public DG_WordObject GetItemFromID(int ID)
    {
        if (ID < 0) { Debug.Log("Get By ID Failed"); return null; }
        if (ID >= ItemCatagoryList.Length) { Debug.Log("Get By ID Failed"); return null; }

        return ItemCatagoryList[ID];
    }
    DG_WordObject ReturnObj(int ID)
    {
        return GetItemFromID(ID);
    }


    public string GetWordByLanguage(DG_WordObject.TextEntry[] TextArray)
    {
        UserSettings.Languages CurrentLanguage;
        CurrentLanguage = QuickFind.UserSettings.CurrentLanguage;

        for (int i = 0; i < TextArray.Length; i++)
        {
            DG_WordObject.TextEntry TE = TextArray[i];
            if (TE.Language == CurrentLanguage)
            {
                if(CurrentLanguage != UserSettings.Languages.English)
                {
                    char[] StringChar = TextArray[i].stringEntry.ToCharArray();
                    System.Text.StringBuilder SB = new System.Text.StringBuilder();
                    for(int iN = 1; iN < StringChar.Length; iN++)
                        SB.Append(StringChar[iN]);

                    return SB.ToString();
                }
                else
                    return TE.stringEntry;
            }
        }
        return string.Empty;
    }
}
