using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class DG_ScenePortalTrigger : MonoBehaviour {

    
    public string SceneString;
    public int PortalValue;


    public void TriggerSceneChange()
    {
        QuickFind.SceneTransitionHandler.TriggerSceneChange(SceneString, PortalValue);

    }
}
