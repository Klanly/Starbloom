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
    public bool ShowGrowthDebug = false;
    public bool DisableAllPoolSpawningAtStart;


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
    }

    void SetCharacterDifferentScene()
    { QuickFind.NetworkSync.SetSelfInScene(1); }
    void SetCharacterMainScene()
    { QuickFind.NetworkSync.SetSelfInScene(0); }
}
