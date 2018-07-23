using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_CharacterControllers : Photon.MonoBehaviour
{

    public GameObject MalePrefabRef;
    public GameObject FemalePrefabRef;


    public GameObject[] DebugCharacters = null;
    [System.NonSerialized] public bool GameStart = true;


    private void Awake()
    {
        QuickFind.CharacterManager = this;
    }


    public void SpawnCharController(DG_PlayerCharacters.GenderValue Gender, DG_NetworkSync.Users NewUser)
    {
        GameObject newPlayerObject;
        if (Gender == DG_PlayerCharacters.GenderValue.Male) newPlayerObject = Instantiate(MalePrefabRef, Vector3.zero, Quaternion.identity);
        else newPlayerObject = Instantiate(FemalePrefabRef, Vector3.zero, Quaternion.identity);

        NewUser.CharacterLink = newPlayerObject.GetComponent<DG_CharacterLink>();
        NewUser.CharacterLink.PlayerID = NewUser.PlayerCharacterID;
    }
}
