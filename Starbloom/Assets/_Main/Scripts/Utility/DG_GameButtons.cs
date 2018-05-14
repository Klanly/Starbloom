using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;




[System.Serializable]
public class DG_GameButtons
{
    [System.Serializable]
    public class Button
    {
        [Header("Keys")]
        public KeyCode MainKey;
        public KeyCode AltKey;

        [HideInInspector] public bool Held;
        [HideInInspector] public bool Up;

        public void Check()
        {
            if (Input.GetKey(MainKey) || Input.GetKey(AltKey)) Held = true; else Held = false;
            if (Input.GetKeyUp(MainKey) || Input.GetKeyUp(AltKey)) Up = true; else Up = false;
        }
    }

    [System.Serializable]
    public class JoyAxis
    {
        public float Value;
        public bool Inverted = false;
        public string InputString;
        public bool InvertedAlt = false;
        public string AltInputString;  
        [HideInInspector] public bool Held;
        [HideInInspector] public bool Up;
        [HideInInspector] public bool Positive;

        public void Check()
        {
            bool AxisIsGreater = true;
            //Joy Stick Check
            float Axis = Input.GetAxis(InputString);
            if (Inverted) Axis = -Axis;
            float AltAxis = Input.GetAxis(AltInputString);
            if (InvertedAlt) AltAxis = -AltAxis;
            if (Axis > 0.1f || AltAxis > 0.1f)
                { Positive = true; Held = true; if (Axis > AltAxis) AxisIsGreater = true; }
            else if (Axis < -0.1f || AltAxis < -0.1f)
                { Positive = false; Held = true; if (Axis < AltAxis) AxisIsGreater = true; }
            else if (Held)
                { Held = false; Up = true; }
            else
                { Held = false; Up = false; }

            if (AxisIsGreater)
                Value = Axis;
            else
                Value = AltAxis;
        }
    }



    //This could be a list, but might start getting a little harder to figure it out later.



    ////////////////////////////////////
    [Header("------ Movement --------------------------------------------")]
    public Button UpDir;
    public Button DownDir;
    public Button LeftDir;
    public Button RightDir;
    ////////////////////////////////////
    [Header("------ Action Buttons --------------------------------------------")]
    public Button Action;
    public Button Interact;
    public Button Special;
    ////////////////////////////////////
    [Header("------ Menu Buttons --------------------------------------------")]
    public Button StartBut;
    ////////////////////////////////////
    [Header("------ Joy Axis --------------------------------------------")]
    public JoyAxis JoyVert;
    public JoyAxis JoyHor;
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
        Interact.Check();
        Special.Check();
        //Start / Select ////////////////////////////////////////////////////////
        StartBut.Check();
        //Joy Axis ////////////////////////////////////////////////////////
        JoyVert.Check();
        JoyHor.Check();
}



    public string[] FetchData()
    {
        List<string> ButtonList = new List<string>();

        Add(ButtonList, UpDir);
        Add(ButtonList, DownDir);
        Add(ButtonList, LeftDir);
        Add(ButtonList, RightDir);

        Add(ButtonList, Action);
        Add(ButtonList, Interact);
        Add(ButtonList, Special);

        Add(ButtonList, StartBut);

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
        Set(IncomingData, Interact);
        Set(IncomingData, Special);

        Set(IncomingData, StartBut);
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
        ButtonList.Add(Interact);
        ButtonList.Add(Special);
        ButtonList.Add(StartBut);

        return ButtonList;
    }
}
