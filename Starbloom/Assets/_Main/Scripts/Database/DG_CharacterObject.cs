using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
/////////////////////////////////////////////////////////////////////////////////Editor Extension Buttons
[CustomEditor(typeof(DG_CharacterObject))]
class DG_CharacterObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        //Buttons

        DG_CharacterObject myScript = (DG_CharacterObject)target;
        if (GUILayout.Button("FindNextAvailableDatabaseID"))
            myScript.FindNextAvailableDatabaseID();
        if (GUILayout.Button("GenerateNewSaveLocation"))
        {
            if (!myScript.NameEditableByUser)
            {
                Debug.Log("Name Not Set To Be Editable By User! Set it to be editable to save a character name location");
                return;
            }
            if (myScript.SaveStringID != 0)
            {
                Debug.Log("Save String ID already set for this Character");
                return;
            }

            if (myScript.DatabaseID == 0)
                myScript.FindNextAvailableDatabaseID();
            string CharName = "CharacterName - " + myScript.DatabaseID.ToString();
            myScript.EquipmentDatabaseID = myScript.SaveStringID;

            CharName = myScript.DatabaseID.ToString() + " - CharacterName - ";
        }
    }
}
//////////////////////////////////////////////////////////////////////////////////
#endif



public class DG_CharacterObject : MonoBehaviour {

    public int DatabaseID;
    public string DevNotes;

    //public bool HasName;
    public int DefaultNameCatagoryID;
    public int DefaultNameWordID;
    public bool NameEditableByUser;
    public int SaveStringID;

    [Header("Equipment Slot")]
    public int EquipmentDatabaseID;

    [Header("Character Stats")]
    public bool CharacterHasStats;
    public CharacterStats[] CharacterStat;

    [System.Serializable]
    public class CharacterStats
    {

    }




    public void FindNextAvailableDatabaseID()
    {
        Transform Cat = transform.parent;
        Transform Tracker = Cat.parent;

        int HighestNumber = 0;

        for (int i = 0; i < Tracker.childCount; i++)
        {
            Transform Child = Tracker.GetChild(i);
            for (int iN = 0; iN < Child.childCount; iN++)
            {
                DG_CharacterObject Item = Child.GetChild(iN).GetComponent<DG_CharacterObject>();
                if(Item.DatabaseID != 0)
                {
                    Debug.Log("This Object Already Has a Database ID");
                    return;
                }

                if (Item.DatabaseID > HighestNumber)
                    HighestNumber = Item.DatabaseID;
            }
        }
        DatabaseID = HighestNumber + 1;
        transform.gameObject.name = DatabaseID.ToString() + " - ";
    }
}
