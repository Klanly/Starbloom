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

    [HideInInspector] public bool isActive = false;


    private void Awake()
    {
        SelectionDisplay.enabled = false;
    }


    public void ContextButtonViaMouse()
    {
        QuickFind.ControllerChange.ContextButtonViaMouse(this);
    }



    public DG_GameButtons.Button GetButton()
    {
        switch(ButtonValue)
        {
            case ButtonSwitch.Action: return QuickFind.InputController.MainPlayer.ButtonSet.Action;
            case ButtonSwitch.Cancel: return QuickFind.InputController.MainPlayer.ButtonSet.Interact;
            case ButtonSwitch.Skill: return QuickFind.InputController.MainPlayer.ButtonSet.Special;
            case ButtonSwitch.Menu: return QuickFind.InputController.MainPlayer.ButtonSet.StartBut;

            case ButtonSwitch.UpDir: return QuickFind.InputController.MainPlayer.ButtonSet.UpDir;
            case ButtonSwitch.DownDir: return QuickFind.InputController.MainPlayer.ButtonSet.DownDir;
            case ButtonSwitch.LeftDir: return QuickFind.InputController.MainPlayer.ButtonSet.LeftDir;
            case ButtonSwitch.RightDir: return QuickFind.InputController.MainPlayer.ButtonSet.RightDir;
            case ButtonSwitch.Close: return null;
        }

        return null;
    }
}
