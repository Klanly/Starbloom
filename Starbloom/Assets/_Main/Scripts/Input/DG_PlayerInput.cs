using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;






public class DG_PlayerInput : MonoBehaviour {

    public enum ContextDetection
    {
        InfrontPlayer,
        MousePosition,
        MiddleScreenPosition
    }
    public enum CurrentInputState
    {
        Default,
        InMenu,
        InCinema,
        PerformingAction
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

        [System.NonSerialized] public DG_CharacterLink CharLink;
        [System.NonSerialized] public InputStateModes InputState = InputStateModes.Normal;
        [Header("Context Detection")]
        public ContextDetection Context;
        public ContextDetection Targeting;


        [System.NonSerialized] public bool NoActionThisFrame; //Added to Enable Double clicking with Unity's built in button system.
        [System.NonSerialized] public bool Moveable = true;
        [System.NonSerialized] public bool InVehicle = false;
        [System.NonSerialized] public bool ShowCursor = true;

        [System.NonSerialized] public float VerticalAxis;
        [System.NonSerialized] public float HorizontalAxis;
        [System.NonSerialized] public float CamVerticalAxis;
        [System.NonSerialized] public float CamHorizontalAxis;
        [System.NonSerialized] public float CamZoomAxis;

        [Header("Button Set")] public DG_GameButtons ButtonSet;
    }



    [Header("Debug")]
    public bool PrintButtonPressed;
    public bool EnableMoveTowardsMouse;
    LazyWalking LazyWalker() { if (LW == null) LW = new LazyWalking(); return LW; }
    LazyWalking LW;



    [Header("Players")]
    public Player MainPlayer;

    [System.NonSerialized] public CurrentInputState InputState;


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

        if (InputState == CurrentInputState.Default)
        {

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

            if (EnableMoveTowardsMouse) { LazyWalker().InternalUpdate(); if (LazyWalker().isFound) { MainPlayer.VerticalAxis = LazyWalker().Z; MainPlayer.HorizontalAxis = LazyWalker().X; } }

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



        if (MainPlayer.ButtonSet.StartBut.Up) //Menu Button
        {
            if (QuickFind.PlayerCam.MainCam.isActiveAndEnabled && InputState != CurrentInputState.InCinema)
            {
                if (QuickFind.StorageUI.StorageUIOpen)
                    QuickFind.StorageUI.CloseStorageUI();
                else
                    QuickFind.GUI_OverviewTabs.OpenUI();
            }
        }

        if (InputState == CurrentInputState.Default || InputState == CurrentInputState.InMenu)
        {
            //Fix Later for Controller Input
            MainPlayer.CamZoomAxis = 0;
            float ScrollAxis = Input.GetAxis("Mouse ScrollWheel");
            if (ScrollAxis > 0) MainPlayer.CamZoomAxis = 1;
            if (ScrollAxis < 0) MainPlayer.CamZoomAxis = -1;
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
