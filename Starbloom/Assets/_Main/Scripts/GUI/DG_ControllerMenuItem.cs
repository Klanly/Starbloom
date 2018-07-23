using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_ControllerMenuItem : MonoBehaviour {

    public enum ButtonSwitch
    {
        Action,
        Cancel,
        Skill,
        Menu,
        UpDir,
        DownDir,
        LeftDir,
        RightDir,
        Close
    }

    public UnityEngine.UI.Image SelectionDisplay = null;
    public ButtonSwitch ButtonValue;
    public TMPro.TextMeshProUGUI DisplayText = null;
    public TMPro.TextMeshProUGUI KeyboardText = null;
    public TMPro.TextMeshProUGUI ControllerText = null;

    [System.NonSerialized] public bool isActive = false;


    private void Awake()
    {
        SelectionDisplay.enabled = false;
    }


    public void P1ContextThroughMouse()
    {
        ContextButtonViaMouse(0);
    }
    public void P2ContextThroughMouse()
    {
        ContextButtonViaMouse(1);
    }

    void ContextButtonViaMouse(int Index)
    {
        QuickFind.ControllerChange.ContextButtonViaMouse(this, Index);
    }



    public DG_GameButtons.Button GetButton(DG_PlayerInput.Player Player)
    {
        Debug.Log("Fix this later");

        return null;
    }
}
