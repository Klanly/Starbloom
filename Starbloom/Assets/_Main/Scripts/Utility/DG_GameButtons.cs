using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;




[System.Serializable]
public class DG_GameButtons
{
    public enum Controller
    {
        Keyboard,
        Player1Controller,
        Player2Controller
    }
    public enum ButtonState
    {
        None,
        Down,
        Held,
        Up
    }
    public enum AxisState
    {
        None,
        PosDown,
        PosHeld,
        PosUp,
        NegDown,
        NegHeld,
        NegUp
    }


    [System.Serializable]
    public class Button
    {
        [Header("Keys")]
        public KeyCode MainKey;
        public KeyCode AltKey;

        [System.NonSerialized] public bool Held;
        [System.NonSerialized] public bool Up;
        [System.NonSerialized] public ButtonState CurrentState;


        public void Check()
        {
            if (Input.GetKey(MainKey) || Input.GetKey(AltKey)) Held = true; else Held = false;
            if (Input.GetKeyUp(MainKey) || Input.GetKeyUp(AltKey)) Up = true; else Up = false;
        }
    }

    [System.Serializable]
    public class JoyAxis
    {



        public bool Inverted = false;
        public string InputString;
        public bool InvertedAlt = false;
        public string AltInputString;

        [System.NonSerialized] public AxisState CurrentAxisState;

        //[System.NonSerialized]
        public float Value;
        //[System.NonSerialized]
        public bool Held;
        //[System.NonSerialized]
        public bool Up;
        //[System.NonSerialized]
        public bool Positive;

        public void Check(JoyAxis Joy)
        {
            //You fucked up, and removed shit homie.
            if (InputString == string.Empty) return;

            //Joy Stick Check
            bool Detected = false;
            float Axis = Input.GetAxis(InputString);
            if (Inverted) Axis = -Axis;
            Detected = CheckDetection(Axis, Joy);
            if (Detected)
                Value = Axis;
            else if (AltInputString != string.Empty)
            {
                float AltAxis = Input.GetAxis(AltInputString);
                if (InvertedAlt) AltAxis = -AltAxis;
                Detected = CheckDetection(AltAxis, Joy);
                if (Detected)
                    Value = AltAxis;
            }

            if (!Detected)
            {
                if (Held) { Held = false; Up = true; }
                    else { Held = false; Up = false; }
            }
        }
        bool CheckDetection(float AxisValue, JoyAxis Joy)
        {
            if (AxisValue > 0.1f) { Joy.Positive = true; Joy.Held = true;
                return true; }
            else if (AxisValue < -0.1f) { Joy.Positive = false; Joy.Held = true;
                return true; }

            return false;
        }
    }



    public Controller ControllerType;
    ////////////////////////////////////
    [Header("------ Movement --------------------------------------------")]
    public Button UpDir;
    public Button DownDir;
    public Button LeftDir;
    public Button RightDir;
    ////////////////////////////////////
    [Header("------ Action Buttons --------------------------------------------")]
    public Button Action;
    public Button SecondaryAction;
    public Button Special;
    public Button Jump;
    ////////////////////////////////////
    [Header("------ Utility Buttons --------------------------------------------")]
    public Button StartBut;
    public Button CameraAllowPan;
    public Button CameraBut;
    public Button ScrollRight;
    public Button ScrollLeft;
    ////////////////////////////////////
    [Header("------ Joy Axis --------------------------------------------")]
    public JoyAxis JoyVert;
    public JoyAxis JoyHor;
    [Header("Right Stick")]
    public JoyAxis RJoyVert;
    public JoyAxis RJoyHor;
    [Header("MouseAxis")]
    public JoyAxis MouseAxis;
    ////////////////////////////////////



    public void CheckButtons()
    {
        //Directionals ////////////////////////////////////////////////////////
        UpDir.Check();
        DownDir.Check();
        LeftDir.Check();
        RightDir.Check();
        //Buttons ////////////////////////////////////////////////////////
        Action.Check();
        SecondaryAction.Check();
        Special.Check();
        Jump.Check();
        //Utility ////////////////////////////////////////////////////////
        StartBut.Check();
        CameraAllowPan.Check();
        CameraBut.Check();
        ScrollRight.Check();
        ScrollLeft.Check();
        //Joy Axis ////////////////////////////////////////////////////////
        JoyVert.Check(JoyVert);
        JoyHor.Check(JoyHor);
        RJoyVert.Check(RJoyVert);
        RJoyHor.Check(RJoyHor);
        MouseAxis.Check(MouseAxis);
    }



    public string[] FetchData()
    {
        List<string> ButtonList = new List<string>();

        Add(ButtonList, UpDir);
        Add(ButtonList, DownDir);
        Add(ButtonList, LeftDir);
        Add(ButtonList, RightDir);

        Add(ButtonList, Action);
        Add(ButtonList, SecondaryAction);
        Add(ButtonList, Special);
        Add(ButtonList, Jump);

        Add(ButtonList, StartBut);
        Add(ButtonList, CameraAllowPan);
        Add(ButtonList, CameraBut);
        Add(ButtonList, ScrollRight);
        Add(ButtonList, ScrollLeft);

        return ButtonList.ToArray();
    }
    void Add(List<string> ButtonList, Button Button)
    {
        ButtonList.Add(Button.MainKey.ToString());
        ButtonList.Add(Button.AltKey.ToString());
    }


    int Loadindex;
    public void LoadData(string[] IncomingData)
    {
        Loadindex = 0;

        Set(IncomingData, UpDir);
        Set(IncomingData, DownDir);
        Set(IncomingData, LeftDir);
        Set(IncomingData, RightDir);

        Set(IncomingData, Action);
        Set(IncomingData, SecondaryAction);
        Set(IncomingData, Special);
        Set(IncomingData, Jump);

        Set(IncomingData, StartBut);
        Set(IncomingData, CameraAllowPan);
        Set(IncomingData, CameraBut);
        Set(IncomingData, ScrollRight);
        Set(IncomingData, ScrollLeft);
    }
    void Set(string[] IncomingData, Button Button)
    {
        Button.MainKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), IncomingData[Loadindex]);
        Loadindex++;
        Button.AltKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), IncomingData[Loadindex]);
        Loadindex++;
    }



    public List<Button> GetButtonList()
    {
        List<Button> ButtonList = new List<Button>();
        ButtonList.Add(UpDir);
        ButtonList.Add(DownDir);
        ButtonList.Add(LeftDir);
        ButtonList.Add(RightDir);

        ButtonList.Add(Action);
        ButtonList.Add(SecondaryAction);
        ButtonList.Add(Special);
        ButtonList.Add(Jump);

        ButtonList.Add(StartBut);
        ButtonList.Add(CameraAllowPan);
        ButtonList.Add(CameraBut);
        ButtonList.Add(ScrollRight);
        ButtonList.Add(ScrollLeft);

        return ButtonList;
    }
}
