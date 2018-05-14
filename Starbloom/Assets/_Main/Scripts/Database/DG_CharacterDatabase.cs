using System.Collections;
using System.Collections.Generic;
using UnityEngine;



#if UNITY_EDITOR
using UnityEditor;
/////////////////////////////////////////////////////////////////////////////////Editor Extension Buttons
[CustomEditor(typeof(DG_CharacterDatabase))]
class DG_CharacterDatabaseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        //Buttons

        DG_CharacterDatabase myScript = (DG_CharacterDatabase)target;

        if (GUILayout.Button("FetchPlayableCharacters"))
        {


            DG_CharacterDatabase CharacterDatabase = QuickFindInEditor.GetEditorCharacterDatabase();
            Transform PlayableCharsTransform = CharacterDatabase.transform.GetChild(0);

            DG_CharacterDatabase.UnlockableCharacter[] NewPlayabledCharactersList = new DG_CharacterDatabase.UnlockableCharacter[PlayableCharsTransform.childCount];
            for (int i = 0; i < PlayableCharsTransform.childCount; i++)
            {
                if (myScript.PlayabledCharactersList != null && myScript.PlayabledCharactersList[i] != null)
                {
                    if (myScript.PlayabledCharactersList[i].UnlockedBoolSaveLocation != 0)
                    {
                        Debug.Log("Save Location Already Exists.");
                        return;
                    }
                }

                DG_CharacterObject CO = PlayableCharsTransform.GetChild(i).GetComponent<DG_CharacterObject>();

                NewPlayabledCharactersList[i] = new DG_CharacterDatabase.UnlockableCharacter();
                NewPlayabledCharactersList[i].CharacterDatabaseID = CO.DatabaseID;

                DG_DataBoolManager Manager = QuickFindInEditor.GetEditorDataBools();
                string CharacterName = QuickFindInEditor.GetEditorDataStrings().GetStringFromIDInEditor(CO.SaveStringID).StringValue;
                NewPlayabledCharactersList[i].UnlockedBoolSaveLocation = Manager.GenerateNewDatabaseItem("Unlockable Chars", CharacterName);
                NewPlayabledCharactersList[i].EditorNote = CharacterName;
            }
            myScript.PlayabledCharactersList = NewPlayabledCharactersList;
        }



        DrawDefaultInspector();
    }
}
//////////////////////////////////////////////////////////////////////////////////
#endif





public class DG_CharacterDatabase : MonoBehaviour {

    [System.Serializable]
    public class UnlockableCharacter
    {
        public string EditorNote;
        public int CharacterDatabaseID;
        public int UnlockedBoolSaveLocation;
    }
    public UnlockableCharacter[] PlayabledCharactersList;



    [System.NonSerialized]
    public DG_CharacterObject[] CharacterList;

    private void Awake()
    {
        QuickFind.CharacterDatabase = this;

        int Counter = 0;
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform Child = transform.GetChild(i);
            for (int iN = 0; iN < Child.childCount; iN++)
                Counter++;
        }


        CharacterList = new DG_CharacterObject[Counter];
        int index = 0;
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform Child = transform.GetChild(i);
            for (int iN = 0; iN < Child.childCount; iN++)
            {
                CharacterList[index] = Child.GetChild(iN).GetComponent<DG_CharacterObject>();
                index++;
            }
        }
    }

    public DG_CharacterObject GetItemFromID(int ID)
    {
        DG_CharacterObject ReturnItem;
        for (int i = 0; i < CharacterList.Length; i++)
        {
            ReturnItem = CharacterList[i];
            if (ReturnItem.DatabaseID == ID)
                return ReturnItem;
        }
        Debug.Log("Get By ID Failed");
        return CharacterList[0];
    }
    public DG_CharacterObject GetItemFromIDInEditor(int ID)
    {
        DG_CharacterObject ReturnItem;

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform Child = transform.GetChild(i);

            for (int iN = 0; iN < Child.childCount; iN++)
            {
                ReturnItem = Child.GetChild(iN).GetComponent<DG_CharacterObject>();
                if (ReturnItem.DatabaseID == ID)
                    return ReturnItem;
            }
        }
        Debug.Log("Get By ID Failed");
        return null;
    }








    public DG_CharacterObject GenerateNewDatabaseItem(string CatagoryName, string ItemName, string Identifier)
    {
        string CatagorySearch = "Catagory - " + CatagoryName;
        int value = -1;
        for (int i = 0; i < transform.childCount; i++)
        {
            string childname = transform.GetChild(i).name;
            if (childname == CatagorySearch)
            {
                value = i;
                break;
            }
        }
        Transform Catagory;
        if (value == -1) //Generate New Catagory
        {
            GameObject newObject = new GameObject();
            newObject.transform.SetParent(transform);
            newObject.name = CatagorySearch;
            Catagory = newObject.transform;
        }
        else
            Catagory = transform.GetChild(value);


        GameObject NewDatabaseObject = new GameObject();
        NewDatabaseObject.transform.SetParent(Catagory);
        NewDatabaseObject.AddComponent<DG_CharacterObject>();

        int AvailableDataID = FindNextAvailableDatabaseID(NewDatabaseObject.transform);
        NewDatabaseObject.name = AvailableDataID.ToString() + " - " + Identifier + " - " + ItemName;

        DG_CharacterObject DataItem = NewDatabaseObject.GetComponent<DG_CharacterObject>();
        DataItem.DatabaseID = AvailableDataID;

        return DataItem;
    }
    int FindNextAvailableDatabaseID(Transform NewItem)
    {
        Transform Cat = NewItem.parent;
        Transform Tracker = Cat.parent;

        int HighestNumber = 0;

        for (int i = 0; i < Tracker.childCount; i++)
        {
            Transform Child = Tracker.GetChild(i);
            for (int iN = 0; iN < Child.childCount; iN++)
            {
                DG_CharacterObject BoolItem = Child.GetChild(iN).GetComponent<DG_CharacterObject>();
                if (BoolItem.DatabaseID > HighestNumber)
                    HighestNumber = BoolItem.DatabaseID;
            }
        }
        return HighestNumber + 1;
    }
}
