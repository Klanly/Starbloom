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
    public bool ShowAttackHitboxes;






    private void Awake()
    {
        QuickFind.GameSettings = this;
    }
    private void Start()
    {
        if (DisableAudio)
        {
            QuickFind.AudioManager.MasterMixerGroup.audioMixer.SetFloat("MasterVolume", -80f);
            AudioListener.volume = 0;
        }
    }


    private void Update()
    {
        if (!EnableDebugKeycodes) return;

        if (Input.GetKey(KeyCode.Keypad9)) ShowDebugGameObject();
        if (Input.GetKey(KeyCode.Keypad0)) JumptoScenePoint(0);
        if (Input.GetKey(KeyCode.Keypad1)) JumptoScenePoint(1);
        if (Input.GetKey(KeyCode.Keypad2)) JumptoScenePoint(2);
    }

    void ShowDebugGameObject()
    {
#if UNITY_EDITOR
        Selection.activeGameObject = this.gameObject;
#endif
    }
    void JumptoScenePoint(int ScenePointValue)
    {
        DG_SceneEntryObject Portal = QuickFind.SceneEntryPoints.GetItemFromID(ScenePointValue);
        QuickFind.PlayerTrans.position = Portal.transform.position;
        QuickFind.PlayerTrans.eulerAngles = Portal.transform.eulerAngles;
        QuickFind.PlayerCam.InstantSetCameraAngle(Portal.CameraFacing);
    }
}
