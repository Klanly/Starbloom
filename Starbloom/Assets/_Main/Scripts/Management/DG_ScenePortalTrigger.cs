using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class DG_ScenePortalTrigger : MonoBehaviour {

    
    public string SceneString;
    public int PortalValue;
    public int Owner;

    [Button(ButtonSizes.Large)]
    public void TriggerSceneChange()
    {
        if (!Application.isPlaying) return;
        QuickFind.SceneTransitionHandler.TriggerSceneChange(SceneString, PortalValue);

    }
}
