using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class DG_ClothingObject : MonoBehaviour {


    [HideInInspector] public int DatabaseID;
    [HideInInspector] public bool LockItem;

    public string Name;



    public DG_PlayerCharacters.GenderValue Gender;
    public DG_ClothingHairManager.ClothHairType Type;
    public GameObject[] Prefabs;


    [Button(ButtonSizes.Medium)]
    public void DebugSceneAddClothingItem()
    {
        if (!Application.isPlaying) return;

        DG_CharacterLink CharLink = null;

        if (GameObject.Find("MainPlayer_Male") != null)
            CharLink = GameObject.Find("MainPlayer_Male").GetComponent<DG_CharacterLink>();
        else
            CharLink = GameObject.Find("MainPlayer_Female").GetComponent<DG_CharacterLink>();
       
        QuickFind.ClothingHairManager.AddClothingItem(CharLink, DatabaseID);
    }
}
