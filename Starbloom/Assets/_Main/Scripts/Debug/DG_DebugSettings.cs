using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_DebugSettings : MonoBehaviour {

    [Header("Network")]
    public bool PlayOnline = true;
    [Header("Debug Tools")]
    public bool DisableAudio = false;
    public bool BypassMainMenu = false;


    private void Awake()
    {
        QuickFind.GameSettings = this;

        if (DisableAudio)
            AudioListener.volume = 0;
    }
}
