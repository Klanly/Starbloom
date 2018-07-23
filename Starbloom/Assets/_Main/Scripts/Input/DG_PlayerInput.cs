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
        public DG_GameButtons.Controller CurrentController;
        [ReadOnly] public CurrentInputState CurrentInputState;
        [ReadOnly] public DG_CharacterLink CharLink;
        [System.NonSerialized] public DG_GameButtons ActiveButtonSet;


        //
        [System.NonSerialized] public DG_GameButtons.ButtonState InteractButton;
        [System.NonSerialized] public DG_GameButtons.ButtonState ToolButton;
        [System.NonSerialized] public DG_GameButtons.ButtonState SpecialButton;
        [System.NonSerialized] public DG_GameButtons.ButtonState JumpButton;
        //
        [System.NonSerialized] public DG_GameButtons.ButtonState StartButton;
        [System.NonSerialized] public DG_GameButtons.ButtonState CameraAllowPanButton;
        [System.NonSerialized] public DG_GameButtons.ButtonState CameraTransitionButton;
        [System.NonSerialized] public DG_GameButtons.ButtonState ScrollRightButton;
        [System.NonSerialized] public DG_GameButtons.ButtonState ScrollLeftButton;

        //Movement
        [System.NonSerialized] public float VerticalAxis;
        [System.NonSerialized] public DG_GameButtons.AxisState VerticalAxisState;
        [System.NonSerialized] public float HorizontalAxis;
        [System.NonSerialized] public DG_GameButtons.AxisState HorizontalAxisState;
        //Camera
        [System.NonSerialized] public float CamVerticalAxis;
        [System.NonSerialized] public DG_GameButtons.AxisState CamVerticalAxisState;
        [System.NonSerialized] public float CamHorizontalAxis;
        [System.NonSerialized] public DG_GameButtons.AxisState CamHorizontalAxisState;
        //Scroll
        [System.NonSerialized] public DG_GameButtons.AxisState CamScrollAxisState;
    }



    [Header("Debug")]
    public bool PrintButtonPressed;



    [Header("Players")]
    [ListDrawerSettings(NumberOfItemsPerPage = 1)]
    public Player[] Players;
    [Header("Button Sets")]
    [ListDrawerSettings(NumberOfItemsPerPage = 1)]
    public DG_GameButtons[] ButtonSets;


    private void Awake()
    {
        QuickFind.InputController = this;
        for(int i = 0; i < ButtonSets.Length; i++) ButtonSets[i].GetButtonList();
        Players[0].ActiveButtonSet = EquipButtonSet(Players[0].CurrentController);
        Players[1].ActiveButtonSet = EquipButtonSet(Players[1].CurrentController);
    }
    public DG_GameButtons EquipButtonSet(DG_GameButtons.Controller Controller)
    {
        for (int i = 0; i < ButtonSets.Length; i++)
        { if (ButtonSets[i].ControllerType == Controller) return ButtonSets[i]; } return ButtonSets[0];
    }

    private void Update()
    {
        for (int i = 0; i < Players.Length; i++)
        {
            bool CoopMode = false;
            if (QuickFind.NetworkSync.Player2PlayerCharacter != -1) CoopMode = true;
            else if (i == 1) return;

            Player P = Players[i];
            P.ActiveButtonSet.CheckButtons();

            UserSettings.PlayerSettings PS = QuickFind.UserSettings.SingleSettings;
            if (CoopMode) PS = QuickFind.UserSettings.CoopSettings[i];

            switch (P.CurrentInputState)
            {
                case CurrentInputState.Default: HandleDefaultControlState(P); break;
            }
        }
        if (PrintButtonPressed) DetectPressedKeyOrButton();
    }

    void HandleDefaultControlState(Player P)
    {
        //State Stuff
        ///////////////////////////////////////////////////////////////

        P.InteractButton = P.ActiveButtonSet.Action.CurrentState;
        P.ToolButton = P.ActiveButtonSet.Action.CurrentState;
        P.SpecialButton = P.ActiveButtonSet.Action.CurrentState;
        P.JumpButton = P.ActiveButtonSet.Action.CurrentState;

        P.StartButton = P.ActiveButtonSet.Action.CurrentState;
        P.CameraAllowPanButton = P.ActiveButtonSet.Action.CurrentState;
        P.CameraTransitionButton = P.ActiveButtonSet.Action.CurrentState;
        P.ScrollRightButton = P.ActiveButtonSet.Action.CurrentState;
        P.ScrollLeftButton = P.ActiveButtonSet.Action.CurrentState;

        P.VerticalAxisState = P.ActiveButtonSet.JoyVert.CurrentAxisState;
        P.HorizontalAxisState = P.ActiveButtonSet.JoyHor.CurrentAxisState;
        P.CamVerticalAxisState = P.ActiveButtonSet.RJoyVert.CurrentAxisState;
        P.CamHorizontalAxisState = P.ActiveButtonSet.RJoyHor.CurrentAxisState;
        P.CamScrollAxisState = P.ActiveButtonSet.MouseAxis.CurrentAxisState;

        //Axis Stuff
        ///////////////////////////////////////////////////////////////

        //Wasd Check
        float AxisVertical = 0; if (P.ActiveButtonSet.UpDir.Held) AxisVertical = 1; if (P.ActiveButtonSet.DownDir.Held) AxisVertical = -1;
        float AxisHorizontal = 0; if (P.ActiveButtonSet.RightDir.Held) AxisHorizontal = 1; if (P.ActiveButtonSet.LeftDir.Held) AxisHorizontal = -1;
        //Controller Left Stick Check
        if (P.ActiveButtonSet.JoyVert.Held) AxisVertical = P.ActiveButtonSet.JoyVert.Value;
        if (P.ActiveButtonSet.JoyHor.Held) AxisHorizontal = P.ActiveButtonSet.JoyHor.Value;
        //Set Values
        P.VerticalAxis = AxisVertical;
        P.HorizontalAxis = AxisHorizontal;
        //Camera Axis Check
        P.CamVerticalAxis = 0; if (P.ActiveButtonSet.RJoyVert.Held) P.CamVerticalAxis = P.ActiveButtonSet.RJoyVert.Value;
        P.CamHorizontalAxis = 0; if (P.ActiveButtonSet.RJoyHor.Held) P.CamHorizontalAxis = P.ActiveButtonSet.RJoyHor.Value;

        




        //Should be handled By Camera

        //if (P.ButtonSet.CameraBut.Up)
        //{
        //    if (P.CharLink.PlayerCam.CurrentCameraState == CameraLogic.CameraState.Disabled || P.CharLink.PlayerCam.CurrentCameraState == CameraLogic.CameraState.DisabledHideMouse) return;
        //
        //    if (P.CharLink.PlayerCam.CurrentCameraAngle == CameraLogic.UserCameraMode.Isometric) P.CharLink.PlayerCam.CurrentCameraAngle = CameraLogic.UserCameraMode.Thirdperson;
        //    else if (P.CharLink.PlayerCam.CurrentCameraAngle == CameraLogic.UserCameraMode.Thirdperson) P.CharLink.PlayerCam.CurrentCameraAngle = CameraLogic.UserCameraMode.Isometric;
        //    QuickFind.PlayerCam.EnableCamera(P.CharLink.PlayerCam, true);
        //}


        //Should be handled by Inventory GUI

        //if (P.ButtonSet.StartBut.Up) //Menu Button
        //{
        //    if (P.CurrentInputState != CurrentInputState.InCinema)
        //    {
        //        if (QuickFind.StorageUI.StorageGuis[i].StorageUIOpen)
        //            QuickFind.StorageUI.CloseStorageUI(P.CharLink.PlayerID);
        //        else
        //            QuickFind.GUI_OverviewTabs.OpenUI(i);
        //    }
        //}
    }
    void HandleInMenuState()
    {

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
