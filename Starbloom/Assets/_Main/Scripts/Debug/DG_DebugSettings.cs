using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DG_DebugSettings : MonoBehaviour {

    [Header("Network")]
    public bool PlayOnline = true;
    public bool BypassMainMenu = false;
    [Header("Debug Tools")]
    public bool DisableAudio = false;
    public bool EnableDebugKeycodes = false;


    [HideInInspector] public GameObject LastSelected;

    private void Awake()
    {
        QuickFind.GameSettings = this;

        if (DisableAudio)
            AudioListener.volume = 0;
    }


    private void Update()
    {
        if (!EnableDebugKeycodes)
            return;

        if (Input.GetKeyUp(KeyCode.Alpha1))
            SetCharacterDifferentScene();
        if (Input.GetKeyUp(KeyCode.Alpha2))
            SetCharacterMainScene();

        if (Input.GetKeyUp(KeyCode.Alpha3))
            SetRandomItemStackInRucksack();
        if (Input.GetKeyUp(KeyCode.Alpha3))
            GiveRandomItemToRucksack();


    }

    void SetCharacterDifferentScene()
    { QuickFind.NetworkSync.SetSelfInScene(1); }
    void SetCharacterMainScene()
    { QuickFind.NetworkSync.SetSelfInScene(0); }

    void SetRandomItemStackInRucksack()
    {
        DG_PlayerCharacters.CharacterEquipment CE = QuickFind.Farm.PlayerCharacters[QuickFind.NetworkSync.PlayerCharacterID].Equipment;

        int slot = Random.Range(0, CE.RuckSackUnlockedSize);
        int ItemID = Random.Range(0, 10);
        int StackValue = Random.Range(0, 50);

        QuickFind.NetworkSync.SetRucksackValue(slot, ItemID, StackValue);
    }
    void GiveRandomItemToRucksack()
    {

    }
}
