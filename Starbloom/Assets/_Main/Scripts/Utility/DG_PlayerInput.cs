﻿using System.Collections;
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
        [Header("Context Detection")] public ContextDetection Context;


        [HideInInspector] public bool NoActionThisFrame; //Added to Enable Double clicking with Unity's built in button system.
        [HideInInspector] public bool Moveable = true;
        [HideInInspector] public bool InVehicle = false;
        [HideInInspector] public bool ShowCursor = true;

        [HideInInspector] public float VerticalAxis;
        [HideInInspector] public float HorizontalAxis;
        [HideInInspector] public float CamVerticalAxis;
        [HideInInspector] public float CamHorizontalAxis;
        [Header("Button Set")] public DG_GameButtons ButtonSet;
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


        //Example of fetching Input buttons.
        //QuickFind.InputController.MainPlayer.ButtonSet.Action.Up; 


        float JoystickVertical = 0;
        float JoystickHorizontal = 0;
        //Wasd Check
        if (MainPlayer.ButtonSet.UpDir.Held) JoystickVertical = 1;
        if (MainPlayer.ButtonSet.DownDir.Held) JoystickVertical = -1;
        if (MainPlayer.ButtonSet.RightDir.Held) JoystickHorizontal = 1;
        if (MainPlayer.ButtonSet.LeftDir.Held) JoystickHorizontal = -1;
        //Controller Left Stick Check
        if (MainPlayer.ButtonSet.JoyVert.Held) JoystickVertical = MainPlayer.ButtonSet.JoyVert.Value;
        if (MainPlayer.ButtonSet.JoyHor.Held) JoystickHorizontal = MainPlayer.ButtonSet.JoyHor.Value;
        //Set Values
        MainPlayer.VerticalAxis = JoystickVertical;
        MainPlayer.HorizontalAxis = JoystickHorizontal;
        

        JoystickVertical = 0;
        JoystickHorizontal = 0;
        //Camera Axis Check
        bool MiddleMouseHeld = Input.GetMouseButton(2);
        if (MainPlayer.ButtonSet.RJoyVert.Held) JoystickVertical = MainPlayer.ButtonSet.RJoyVert.Value;
            else if (MiddleMouseHeld) JoystickVertical = Input.GetAxis("Vertical");
        if (MainPlayer.ButtonSet.RJoyHor.Held) JoystickHorizontal = MainPlayer.ButtonSet.RJoyHor.Value;
            else if (MiddleMouseHeld) JoystickHorizontal = Input.GetAxis("Horizontal");
        //Set Values
        MainPlayer.CamVerticalAxis = JoystickVertical;
        MainPlayer.CamHorizontalAxis = JoystickHorizontal;
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
