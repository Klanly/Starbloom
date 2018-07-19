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


    public void ContextButtonViaMouse()
    {
        QuickFind.ControllerChange.ContextButtonViaMouse(this);
    }



    public DG_GameButtons.Button GetButton(DG_PlayerInput.Player Player)
    {
        switch(ButtonValue)
        {
            case ButtonSwitch.Action: return Player.ButtonSet.Action;
            case ButtonSwitch.Cancel: return Player.ButtonSet.SecondaryAction;
            case ButtonSwitch.Skill: return Player.ButtonSet.Special;
            case ButtonSwitch.Menu: return Player.ButtonSet.StartBut;

            case ButtonSwitch.UpDir: return Player.ButtonSet.UpDir;
            case ButtonSwitch.DownDir: return Player.ButtonSet.DownDir;
            case ButtonSwitch.LeftDir: return Player.ButtonSet.LeftDir;
            case ButtonSwitch.RightDir: return Player.ButtonSet.RightDir;
            case ButtonSwitch.Close: return null;
        }

        return null;
    }
}
