using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_TextButton : MonoBehaviour {

    public DG_TextStatic TextScript = null;
    public UnityEngine.UI.Image Arrow = null;
    public DG_GUINameChange.NameChangeGuiOptions GuiOption;

    [System.NonSerialized] public bool CurrentlyActive = false;


    float Timer;


    private void Awake()
    {
        Arrow.enabled = false;
    }

    public void QueueActivate(bool Blink)
    {
        CurrentlyActive = true;
        Arrow.enabled = true;
        Arrow.color = Color.yellow;
    }
    public void QueueDeactivate(bool DisplayUnderline)
    {
        if (DisplayUnderline)
            Arrow.color = Color.white;
        else
            Arrow.enabled = false;
    }
}
