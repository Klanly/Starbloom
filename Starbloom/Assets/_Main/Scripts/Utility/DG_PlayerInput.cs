using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;






public class DG_PlayerInput : MonoBehaviour {

    public enum ContextDetection
    {
        InfrontPlayer,
        MousePosition
    }

    [System.Serializable]
    public class Player
    {
        public enum InputStateModes
        {
            Normal,
            DialogueMenu,
            NameChangeMenu,
            ControllerChangeMenu
        }

        [HideInInspector] public DG_CharacterLink CharLink;
        [HideInInspector] public InputStateModes InputState = InputStateModes.Normal;

        [HideInInspector] public bool NoActionThisFrame; //Added to Enable Double clicking with Unity's built in button system.
        [HideInInspector] public bool Moveable = true;
        [HideInInspector] public bool InVehicle = false;
        [HideInInspector] public bool ShowCursor = true;


        [HideInInspector] public float VerticalAxis;
        [HideInInspector] public float HorizontalAxis;


        [Header("Context Detection")]
        public ContextDetection Context;

        [Header("Button Set")]
        public DG_GameButtons ButtonSet;
    }

    [Header("Debug")]
    public bool PrintButtonPressed;
    [Header("Players")]
    public Player MainPlayer;


    private void Awake()
    {
        QuickFind.InputController = this;
        MainPlayer.ButtonSet.GetButtonList();
    }


    private void Update()
    {
        CheckCharInput();
        if (PrintButtonPressed)
            DetectPressedKeyOrButton();
    }


    void CheckCharInput()
    {
        MainPlayer.ButtonSet.CheckButtons();


        float JoystickVertical = 0;
        float JoystickHorizontal = 0;

        //Direction Check
        if (MainPlayer.ButtonSet.UpDir.Held) JoystickVertical = 1;
        if (MainPlayer.ButtonSet.DownDir.Held) JoystickVertical = -1;
        if (MainPlayer.ButtonSet.RightDir.Held) JoystickHorizontal = 1;
        if (MainPlayer.ButtonSet.LeftDir.Held) JoystickHorizontal = -1;

        if (MainPlayer.ButtonSet.JoyVert.Held)
            JoystickVertical = MainPlayer.ButtonSet.JoyVert.Value;
        if (MainPlayer.ButtonSet.JoyHor.Held)
            JoystickHorizontal = MainPlayer.ButtonSet.JoyHor.Value;



        MainPlayer.VerticalAxis = JoystickVertical;
        MainPlayer.HorizontalAxis = JoystickHorizontal;


        if (MainPlayer.ButtonSet.Interact.Up) //Primary Action Key
        {

        }



    }





    public void DetectPressedKeyOrButton()
    {
        foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(kcode))
            {
                string KeyCodeString = kcode.ToString();
                KeyCode ConvertedKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), KeyCodeString);
                Debug.Log("KeyCode down: " + ConvertedKey);
                break;
            }
        }
    }
}
