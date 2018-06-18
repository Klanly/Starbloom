using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class DG_ScenePortalTrigger : MonoBehaviour {

    
    public string SceneString;
    public int PortalValue;
<<<<<<< .merge_file_a14060
=======
    public int Owner;
>>>>>>> .merge_file_a14000


    public void TriggerSceneChange()
    {
        QuickFind.SceneTransitionHandler.TriggerSceneChange(SceneString, PortalValue);

    }
}
