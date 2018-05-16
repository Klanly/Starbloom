using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class DG_PlayerCharacters : MonoBehaviour {

    public List<PlayerCharacter> PlayerCharacters;





    [System.Serializable]
    public class PlayerCharacter
    {
        public string Name;
        public CharacterEquipment Equipment;
    }

    [System.Serializable]
    public class CharacterEquipment
    {
        [Header("Equipment Slot -------------------------------------------------------------------------------")]
        public int WeaponDatabaseId;
        public int HatId;
        public int Ring1;
        public int Ring2;
        public int Boots;

        [Header("Cosmetic")]
        public int HairID;
        public int ShirtId;
        public int PantsID;

        [ListDrawerSettings(NumberOfItemsPerPage = 12, Expanded = false)]
        public RucksackSlot[] RucksackSlots;
    }

    [System.Serializable]
    public class RucksackSlot
    {
        public bool Unlocked;
        public int ContainedItem;
        public int StackValue;
    }


}
