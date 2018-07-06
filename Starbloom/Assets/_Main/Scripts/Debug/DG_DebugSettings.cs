using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


public class DG_DebugSettings : MonoBehaviour {

    [Header("Network")]
    public bool PlayOnline = true;
    [Header("Build Convenience")]
    public bool DisableAudio = false;
    [Header("Character")]
    public DG_PlayerCharacters.GenderValue GeneratedGender;
    public bool DontHideGear;
    [Header("Time Saving")]
    public bool BypassMainMenu = false;
    public bool DisableAnimations;
    public bool AllowActionsOnHold;
    public bool AllowUIOnHold;
    public bool ForceInstantFade;
    [Header("Debug Tools")]
    public bool EnableDebugKeycodes = false;
    public bool DisableAllPoolSpawningAtStart;






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
        if (!EnableDebugKeycodes) return;

        if (Input.GetKey(KeyCode.Keypad0)) ShowDebugGameObject();
    }

    void ShowDebugGameObject()
    {
#if UNITY_EDITOR
        Selection.activeGameObject = this.gameObject;
#endif
    }
}
