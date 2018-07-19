using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Sirenix.OdinInspector;






public class DG_PlayerInput : MonoBehaviour {

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


        [ReadOnly] public DG_CharacterLink CharLink;
        [ReadOnly] public CurrentInputState CurrentInputState;
        [ReadOnly] public InputStateModes InputMode = InputStateModes.Normal;

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



    [Header("Players")]
    [ListDrawerSettings(NumberOfItemsPerPage = 1)]
    public Player[] Players;



    private void Awake()
    {
        QuickFind.InputController = this;
        Players[0].ButtonSet.GetButtonList();
        Players[1].ButtonSet.GetButtonList();
    }


    private void Update()
    {
        CheckCharInput();
        if (PrintButtonPressed)
            DetectPressedKeyOrButton();
    }


    void CheckCharInput()
    {
        for (int i = 0; i < Players.Length; i++)
        {
            Player P = Players[i];
            P.ButtonSet.CheckButtons();

            if (P.CurrentInputState == CurrentInputState.Default)
            {

                float JoystickVertical = 0;
                float JoystickHorizontal = 0;
                //Wasd Check
                if (P.ButtonSet.UpDir.Held) JoystickVertical = 1;
                if (P.ButtonSet.DownDir.Held) JoystickVertical = -1;
                if (P.ButtonSet.RightDir.Held) JoystickHorizontal = 1;
                if (P.ButtonSet.LeftDir.Held) JoystickHorizontal = -1;
                //Controller Left Stick Check
                if (P.ButtonSet.JoyVert.Held) JoystickVertical = P.ButtonSet.JoyVert.Value;
                if (P.ButtonSet.JoyHor.Held) JoystickHorizontal = P.ButtonSet.JoyHor.Value;
                //Set Values
                P.VerticalAxis = JoystickVertical;
                P.HorizontalAxis = JoystickHorizontal;

                JoystickVertical = 0;
                JoystickHorizontal = 0;
                //Camera Axis Check

                if (P.ButtonSet.RJoyVert.Value != 0) JoystickVertical = P.ButtonSet.RJoyVert.Value;
                else if (Input.GetAxis("Vertical") != 0) JoystickVertical = Input.GetAxis("Vertical");

                if (P.ButtonSet.RJoyHor.Value != 0) JoystickHorizontal = P.ButtonSet.RJoyHor.Value;
                else if (Input.GetAxis("Horizontal") != 0) JoystickHorizontal = Input.GetAxis("Horizontal");

                //Set Values
                P.CamVerticalAxis = JoystickVertical;
                P.CamHorizontalAxis = JoystickHorizontal;

                if (P.ButtonSet.CameraBut.Up)
                {
                    if (P.CharLink.PlayerCam.CurrentCameraState == CameraLogic.CameraState.Disabled || P.CharLink.PlayerCam.CurrentCameraState == CameraLogic.CameraState.DisabledHideMouse) return;

                    if (P.CharLink.PlayerCam.CurrentCameraAngle == CameraLogic.UserCameraMode.Isometric) P.CharLink.PlayerCam.CurrentCameraAngle = CameraLogic.UserCameraMode.Thirdperson;
                    else if (P.CharLink.PlayerCam.CurrentCameraAngle == CameraLogic.UserCameraMode.Thirdperson) P.CharLink.PlayerCam.CurrentCameraAngle = CameraLogic.UserCameraMode.Isometric;
                    QuickFind.PlayerCam.EnableCamera(P.CharLink.PlayerCam, true);
                }

            }



            if (P.ButtonSet.StartBut.Up) //Menu Button
            {
                if (P.CurrentInputState != CurrentInputState.InCinema)
                {
                    if (QuickFind.StorageUI.StorageUIOpen)
                        QuickFind.StorageUI.CloseStorageUI();
                    else
                        QuickFind.GUI_OverviewTabs.OpenUI();
                }
            }

            if (P.CurrentInputState == CurrentInputState.Default || P.CurrentInputState == CurrentInputState.InMenu)
            {
                //Fix Later for Controller Input
                P.CamZoomAxis = 0;
                float ScrollAxis = Input.GetAxis("Mouse ScrollWheel");
                if (ScrollAxis > 0) P.CamZoomAxis = 1;
                if (ScrollAxis < 0) P.CamZoomAxis = -1;
            }
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

    public Player GetPlayerByPlayerID(int PlayerID)
    {
        if (QuickFind.NetworkSync == null) return Players[0];
        if (PlayerID == QuickFind.NetworkSync.Player1PlayerCharacter) return Players[0];
        else return Players[1];
    }
}
