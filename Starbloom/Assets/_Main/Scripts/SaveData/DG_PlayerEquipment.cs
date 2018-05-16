using System.Collections;
using System.Collections.Generic;
using UnityEngine;





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
