using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_MenuContextItem : MonoBehaviour {


    public UnityEngine.UI.Image SelectionDisplay = null;
    

    public void ContextButtonViaMouse(int PlayerValue)
    {
        QuickFind.GUIContextHandler.ContextButtonViaMouse(this, PlayerValue);
    }
}
