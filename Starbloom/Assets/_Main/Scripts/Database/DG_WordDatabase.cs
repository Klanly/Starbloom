using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
/////////////////////////////////////////////////////////////////////////////////Editor Extension Buttons
[CustomEditor(typeof(DG_WordDatabase))]
class DG_WordDatabaseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        //Buttons

        DG_WordDatabase myScript = (DG_WordDatabase)target;
        if (GUILayout.Button("MakeNewCatagory"))
            myScript.MakeNewCatagory();
    }
}
//////////////////////////////////////////////////////////////////////////////////
#endif



public class DG_WordDatabase : MonoBehaviour {

    public class WordObjectCatagory
    {
        public DG_WordObject[] WordList;
    }


    [System.NonSerialized]
    public WordObjectCatagory[] CatagoryList;


    private void Awake()
    {
        QuickFind.WordDatabase = this;

        int CatagoryCounter = 0;
        for (int i = 0; i < transform.childCount; i++)
            CatagoryCounter++;


        CatagoryList = new WordObjectCatagory[CatagoryCounter];
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform Child = transform.GetChild(i);
            CatagoryList[i] = new WordObjectCatagory();
            CatagoryList[i].WordList = new DG_WordObject[Child.childCount];
            for (int iN = 0; iN < Child.childCount; iN++)
                CatagoryList[i].WordList[iN] = Child.GetChild(iN).GetComponent<DG_WordObject>();
        }
    }



    public string GetWordFromID(int ID = 0, int CatID = 0)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
            return GetWordByLanguage((QuickFindInEditor.GetEditorWordDatabase().GetItemFromID(ID, CatID).TextValues));
        else
#endif
            return GetWordByLanguage((QuickFind.WordDatabase.GetItemFromID(ID, CatID).TextValues));
    }

    public string GetNameFromID(int ID = 0, bool FirstTimeNameShown = false)
    {
        string ReturnString = string.Empty;
        DG_CharacterObject CharacterObject;

#if UNITY_EDITOR
        if (!Application.isPlaying)
            CharacterObject = QuickFindInEditor.GetEditorCharacterDatabase().GetItemFromIDInEditor(ID);
        else
#endif
            CharacterObject = QuickFind.CharacterDatabase.GetItemFromID(ID);

        DG_WordObject WO;
        if (!CharacterObject.NameEditableByUser || FirstTimeNameShown)
        {
            WO = QuickFind.WordDatabase.GetItemFromID(CharacterObject.DefaultNameWordID, CharacterObject.DefaultNameCatagoryID);
            ReturnString = GetWordByLanguage(WO.TextValues);
        }
        else
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                ReturnString = QuickFindInEditor.GetEditorDataStrings().GetStringFromIDInEditor(CharacterObject.SaveStringID).StringValue;
            else
#endif
                ReturnString = QuickFind.DataStrings.GetStringFromID(CharacterObject.SaveStringID).StringValue;
        }

        return ReturnString;
    }

    public DG_WordObject GetItemFromID(int ID, int CatagoryID)
    {
        DG_WordObject ReturnItem;

        for (int i = 0; i < CatagoryList.Length; i++)
        {
            WordObjectCatagory Cat = CatagoryList[i];
            if (i == CatagoryID)
            {
                ReturnItem = ReturnObj(Cat, ID);
                return ReturnItem;
            }
        }
        Debug.Log("Get By ID Failed");
        return null;
    }


    public DG_WordObject GetItemFromIDInEditor(int ID, int CatagoryID)
    {
        DG_WordObject ReturnItem;

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform Child = null;
            Child = transform.GetChild(CatagoryID);

            for (int iN = 0; iN < Child.childCount; iN++)
            {
                ReturnItem = Child.GetChild(iN).GetComponent<DG_WordObject>();
                if (ReturnItem.DatabaseID == ID)
                    return ReturnItem;
            }
        }
        if(ID != 0)
            Debug.Log("Get By ID Failed");
        return null;
    }


    public void MakeNewCatagory()
    {
        int childcount = transform.childCount;

        GameObject NewClone = new GameObject();
        NewClone.transform.SetParent(transform);

        NewClone.name =  "Catagory - " + childcount.ToString() + " - ";

        GameObject NewCloneChild = new GameObject();
        NewCloneChild.transform.SetParent(NewClone.transform);

        DG_WordObject WO = NewCloneChild.AddComponent<DG_WordObject>();
        WO.DatabaseID = 0;
        WO.FindNextAvailableDatabaseID();

        WO.DevNotes = string.Empty;
        WO.PopulateLanguageOptions(WO);
    }




















    DG_WordObject ReturnObj(WordObjectCatagory Cat, int ID)
    {
        DG_WordObject ReturnItem;
        for (int iN = 0; iN < Cat.WordList.Length; iN++)
        {
            ReturnItem = Cat.WordList[iN];
            if (ReturnItem.DatabaseID == ID)
                return ReturnItem;
        }
        return null;
    }
    public string GetWordByLanguage(DG_WordObject.TextEntry[] TextArray)
    {
        UserSettings.Languages CurrentLanguage;

#if UNITY_EDITOR
        if (!Application.isPlaying)
            CurrentLanguage = QuickFindInEditor.GetEditorUserSettings().CurrentLanguage;
        else
#endif
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
