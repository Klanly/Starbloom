using System.Collections;
using System.Collections.Generic;
using UnityEngine;



#if UNITY_EDITOR
using UnityEditor;
/////////////////////////////////////////////////////////////////////////////////Editor Extension Buttons
[CustomEditor(typeof(DG_PlayerEquipment))]
class DG_PlayerEquipmentEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DG_PlayerEquipment myScript = (DG_PlayerEquipment)target;

        if (GUILayout.Button("GenerateDatabaseIDs"))
        {
            DG_DataIntManager IntManager = QuickFindInEditor.GetEditorDataInts();

            for (int i = 0; i < myScript.EquipmentSlots.Length; i++)
            {
                myScript.EquipmentSlots[i].DatabaseID = i + 1;

                if (myScript.EquipmentSlots[i].WeaponDatabaseId == 0)
                {
                    int NewDatabasePosition = IntManager.GenerateNewDatabaseItem("Equipment", "Weapon Slot " + i.ToString() + " ID");
                    myScript.EquipmentSlots[i].WeaponDatabaseId = NewDatabasePosition;
                }
                if (myScript.EquipmentSlots[i].ArmDatabaseId == 0)
                {
                    int NewDatabasePosition = IntManager.GenerateNewDatabaseItem("Equipment", "Armor Slot " + i.ToString() + " ID");
                    myScript.EquipmentSlots[i].ArmDatabaseId = NewDatabasePosition;
                }
                if (myScript.EquipmentSlots[i].AccDatabaseId == 0)
                {
                    int NewDatabasePosition = IntManager.GenerateNewDatabaseItem("Equipment", "Accessory Slot " + i.ToString() + " ID");
                    myScript.EquipmentSlots[i].AccDatabaseId = NewDatabasePosition;
                }

                for (int iN = 0; iN < myScript.EquipmentSlots[i].WeaponMateriaDatabaseIDs.Length; iN++)
                {
                    if (myScript.EquipmentSlots[i].WeaponMateriaDatabaseIDs[iN] == 0)
                    {
                        int NewDatabasePosition = IntManager.GenerateNewDatabaseItem("Equipment", "Weapon Materia Slot " + i.ToString() + " ID");
                        myScript.EquipmentSlots[i].WeaponMateriaDatabaseIDs[iN] = NewDatabasePosition;
                    }
                }
                for (int iN = 0; iN < myScript.EquipmentSlots[i].ArmMateriaDatabaseIDs.Length; iN++)
                {
                    if (myScript.EquipmentSlots[i].ArmMateriaDatabaseIDs[iN] == 0)
                    {
                        int NewDatabasePosition = IntManager.GenerateNewDatabaseItem("Equipment", "Arm Materia Slot " + i.ToString() + " ID");
                        myScript.EquipmentSlots[i].ArmMateriaDatabaseIDs[iN] = NewDatabasePosition;
                    }
                }
            }
        }

        DrawDefaultInspector();
    }
}
//////////////////////////////////////////////////////////////////////////////////
#endif





public class DG_PlayerEquipment : MonoBehaviour {

    [System.Serializable]
    public class CharacterEquipment
    {
        public int DatabaseID;

        [Header("Equipment Slot -------------------------------------------------------------------------------")]
        public int WeaponDatabaseId;
        public int ArmDatabaseId;
        public int AccDatabaseId;

        //Materia Slots
        public int[] WeaponMateriaDatabaseIDs = new int[8];
        public int[] ArmMateriaDatabaseIDs = new int[8];
    }

    public CharacterEquipment[] EquipmentSlots;








    public CharacterEquipment GetItemFromID(int ID)
    {
        CharacterEquipment ReturnItem;
        for (int i = 0; i < EquipmentSlots.Length; i++)
        {
            ReturnItem = EquipmentSlots[i];
            if (ReturnItem.DatabaseID == ID)
                return ReturnItem;
        }
        Debug.Log("Get By ID Failed");
        return null;
    }
}
