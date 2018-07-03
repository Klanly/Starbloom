using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


public class DG_DebugSettings : MonoBehaviour {

    [Header("Network")]
    public bool PlayOnline = true;
    public bool BypassMainMenu = false;
    
    [Header("Debug Tools")]
    public bool DisableAudio = false;
    public bool EnableDebugKeycodes = false;
    public bool ShowGrowthDebug = false;
    public bool DisableAllPoolSpawningAtStart;
    public bool ForceGender;
    public DG_PlayerCharacters.GenderValue ForcedGender;
    public bool ShowToolOnEquip;


    [HideInInspector] public GameObject LastSelected;



    private void Awake()
    {
        QuickFind.GameSettings = this;
    }
    private void Start()
    {
        if (DisableAudio)
            QuickFind.AudioManager.MasterMixerGroup.audioMixer.SetFloat("MasterVolume", -80f);
    }


    private void Update()
    {
        if (!EnableDebugKeycodes)
            return;


        if (Input.GetKey(KeyCode.Keypad0)) ShowDebugGameObject(true);
        if (Input.GetKey(KeyCode.Keypad1)) ShowDebugGameObject(false);
    }

    void ShowDebugGameObject(bool ToDebug)
    {
#if UNITY_EDITOR
        if (ToDebug)
        {
            LastSelected = Selection.activeGameObject;
            Selection.activeGameObject = this.gameObject;
        }
        else
            Selection.activeGameObject = LastSelected;
#endif
    }
}
