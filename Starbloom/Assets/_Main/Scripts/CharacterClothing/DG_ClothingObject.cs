using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class DG_ClothingObject : MonoBehaviour {


    [System.Serializable]
    public class CharacterOffsetPoints
    {
        public DG_PlayerCharacters.GenderValue Gender;
        public Vector9 OffsetData;
    }

    [HideInInspector] public int DatabaseID;
    [HideInInspector] public bool LockItem;

    public string Name;



    public DG_PlayerCharacters.GenderValue Gender;
    public DG_ClothingHairManager.ClothHairType Type;
    public GameObject[] Prefabs;
    public CharacterOffsetPoints[] CharOffsetPoint;
    public int AnimationDatabaseNumber;
    public DG_CombatHandler.Resistances[] ClothingResistances;




    public CharacterOffsetPoints GetCharOffsetRefByGender(DG_PlayerCharacters.GenderValue Gender)
    {
        for (int i = 0; i < CharOffsetPoint.Length; i++)
        {
            if (CharOffsetPoint[i].Gender == Gender)
                return CharOffsetPoint[i];
        }
        return null;
    }
    public Vector9 GetPositionByGender(DG_PlayerCharacters.GenderValue Gender)
    {
        for (int i = 0; i < CharOffsetPoint.Length; i++)
        {
            if (CharOffsetPoint[i].Gender == Gender)
                return CharOffsetPoint[i].OffsetData;
        }
        return Vector9.Zero();
    }





    [Button(ButtonSizes.Medium)]
    public void RuntimeAddClothingItem()
    {
        if (!Application.isPlaying) return;    
        QuickFind.ClothingHairManager.ClothingAdd(QuickFind.NetworkSync.UserID, DatabaseID);
    }

    [Button(ButtonSizes.Small)]
    public void SyncNameToPrefab0()
    {
        Name = Prefabs[0].gameObject.name;
    }

}
