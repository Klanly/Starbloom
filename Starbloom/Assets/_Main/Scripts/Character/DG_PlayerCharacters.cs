using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class DG_PlayerCharacters : MonoBehaviour {


    public string FarmName;
    [ListDrawerSettings(NumberOfItemsPerPage = 1, Expanded = false)]
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
        public int ActiveRucksackSlotId;

        [Header("Equipment Slots -------------------------------------------------------------------------------")]
        public int HatId;
        public int Ring1;
        public int Ring2;
        public int Boots;

        [Header("Cosmetic")]
        public int HairID;
        public int ShirtId;
        public int PantsID;

        [Header("Rucksack")]
        public int RuckSackUnlockedSize = 12;
        [ListDrawerSettings(NumberOfItemsPerPage = 12, Expanded = false)]
        public RucksackSlot[] RucksackSlots;
    }

    [System.Serializable]
    public class RucksackSlot
    {
        public int ContainedItem;
        public int StackValue;
    }





    private void Awake()
    {
        QuickFind.Farm = this;
    }


}
