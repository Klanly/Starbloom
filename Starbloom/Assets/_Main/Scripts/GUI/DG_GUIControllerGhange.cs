using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;








public class DG_GUIControllerGhange : MonoBehaviour
{
    [System.Serializable]
    public class PlayerGUIControllerChange
    {
        public CanvasGroup UICanvas = null;
        public UnityEngine.UI.GraphicRaycaster Raycaster;
        public Transform Grid1 = null;
        public Transform Grid2 = null;

        [Header("Set Button UI")]
        public GameObject ButtonScreen = null;
        public DG_ControllerMenuItem ChangeScreenItem = null;
        [Header("Static Text")]
        public DG_TextStatic[] StaticTextArray;

        [System.NonSerialized] public int index = 0;
        [System.NonSerialized] public DG_ControllerMenuItem[] MenuItems;
        [System.NonSerialized] public bool SelectingNewInput = false;

        [System.NonSerialized] public int TimeDifference = 20;
        [System.NonSerialized] public int Timer;
    }

    public PlayerGUIControllerChange[] PlayerGUIChanges;



    private void Awake()
    {
        QuickFind.ControllerChange = this;
    }
    private void Start()
    {
        QuickFind.EnableCanvas(PlayerGUIChanges[0].UICanvas, false, PlayerGUIChanges[0].Raycaster);
        QuickFind.EnableCanvas(PlayerGUIChanges[1].UICanvas, false, PlayerGUIChanges[1].Raycaster);

        transform.localPosition = Vector3.zero;
        this.enabled = false;
    }

    private void Update()
    {
        for (int i = 0; i < PlayerGUIChanges.Length; i++)
        {
            if (QuickFind.InputController.Players[i].CharLink == null) continue;

            PlayerGUIControllerChange PGC = PlayerGUIChanges[i];

            if (PGC.Timer > 0)
            {
                PGC.Timer--;
                return;
            }

            if (PGC.SelectingNewInput)
            {
                if (Input.anyKey)
                {
                    CheckInput(i);
                    PGC.SelectingNewInput = false;
                    PGC.Timer = PGC.TimeDifference;
                    OpenControllerChangeUI(i);
                }
            }
        }
    }


    public void P1ActionButtonSent()
    {
        ActionButtonSent(0);
    }
    public void P2ActionButtonSent()
    {
        ActionButtonSent(1);
    }
    void ActionButtonSent(int Index)
    {
        PlayerGUIControllerChange PGC = PlayerGUIChanges[Index];

        if (PGC.Timer > 0)
            return;

        if (PGC.MenuItems[PGC.index].ButtonValue != DG_ControllerMenuItem.ButtonSwitch.Close)
        {
            PGC.Timer = PGC.TimeDifference;
            PGC.MenuItems[PGC.index].SelectionDisplay.enabled = false;
            PGC.MenuItems[PGC.index].isActive = false;
            OpenChangeButtonScreen(Index);
        }
        else
            CloseControllerChangeUI(Index);
    }

    [Button(ButtonSizes.Small)]
    public void DebugOpenControllerChangeUI()
    {
        OpenControllerChangeUI(0);
    }

    
    public void OpenControllerChangeUI(int Index)
    {
        PlayerGUIControllerChange PGC = PlayerGUIChanges[Index];

        QuickFind.EnableCanvas(PGC.UICanvas, true, PGC.Raycaster);

        int PlayerID = QuickFind.NetworkSync.Player1PlayerCharacter;
        if (Index == 1) PlayerID = QuickFind.NetworkSync.Player2PlayerCharacter;

        DG_PlayerInput.Player MP = QuickFind.InputController.GetPlayerByPlayerID(PlayerID);
        MP.CurrentInputState = DG_PlayerInput.CurrentInputState.InMenu;

        if (PGC.MenuItems == null)
            FillArray(Index);

        FillCurrentValueText(Index);

        PGC.MenuItems[PGC.index].SelectionDisplay.enabled = false;
        PGC.MenuItems[PGC.index].isActive = false;

        PGC.index = 0;
        PGC.MenuItems[0].SelectionDisplay.enabled = true;
        PGC.ButtonScreen.SetActive(false);

        for (int i = 0; i < PGC.StaticTextArray.Length; i++)
            PGC.StaticTextArray[i].ManualLoad();

        this.enabled = true;
    }


    public void CloseP1ControllerChange()
    {
        CloseControllerChangeUI(0);
    }
    public void CloseP2ControllerChange()
    {
        CloseControllerChangeUI(1);
    }


    void CloseControllerChangeUI(int Index)
    {
        PlayerGUIControllerChange PGC = PlayerGUIChanges[Index];

        if (PGC.SelectingNewInput || PGC.Timer > 0)
            return;

        QuickFind.EnableCanvas(PGC.UICanvas, false, PGC.Raycaster);

        int PlayerID = QuickFind.NetworkSync.Player1PlayerCharacter;
        if (Index == 1) PlayerID = QuickFind.NetworkSync.Player2PlayerCharacter;

        DG_PlayerInput.Player MP = QuickFind.InputController.GetPlayerByPlayerID(PlayerID);
        MP.CurrentInputState = DG_PlayerInput.CurrentInputState.Default;

        this.enabled = false;
    }

    public void P1AdjustByController(bool isUp)
    {
        AdjustButtonViaController(isUp, 0);
    }
    public void P2AdjustByController(bool isUp)
    {
        AdjustButtonViaController(isUp, 1);
    }

    void AdjustButtonViaController(bool isUp, int Index)
    {
        PlayerGUIControllerChange PGC = PlayerGUIChanges[Index];

        if (PGC.SelectingNewInput)
            return;

        PGC.MenuItems[PGC.index].SelectionDisplay.enabled = false;
        PGC.MenuItems[PGC.index].isActive = false;

        int PlayerID = QuickFind.NetworkSync.Player1PlayerCharacter;
        if (Index == 1) PlayerID = QuickFind.NetworkSync.Player2PlayerCharacter;

        bool isController = false;
        if (QuickFind.InputController.Players[Index].VerticalAxisState == DG_GameButtons.AxisState.PosUp)
            isController = true;
        if (isController)
            PGC.index = QuickFind.GetValueInArrayLoop(PGC.index, PGC.Grid1.childCount, !isUp, true);
        else
            PGC.index = QuickFind.GetValueInArrayLoop(PGC.index, PGC.MenuItems.Length, !isUp, true);
        PGC.MenuItems[PGC.index].SelectionDisplay.enabled = true;
        PGC.MenuItems[PGC.index].isActive = true;
    }

    public void ContextButtonViaMouse(DG_ControllerMenuItem ItemScript, int Index)
    {
        PlayerGUIControllerChange PGC = PlayerGUIChanges[Index];

        PGC.MenuItems[PGC.index].SelectionDisplay.enabled = false;
        PGC.MenuItems[PGC.index].isActive = false;

        for (int i = 0; i < PGC.MenuItems.Length; i++)
        {
            if (ItemScript == PGC.MenuItems[i])
                PGC.index = i;
        }

        PGC.MenuItems[PGC.index].SelectionDisplay.enabled = true;
        PGC.MenuItems[PGC.index].isActive = true;
    }
    void OpenChangeButtonScreen(int Index)
    {
        PlayerGUIControllerChange PGC = PlayerGUIChanges[Index];

        PGC.SelectingNewInput = true;
        DG_ControllerMenuItem Item = PGC.MenuItems[PGC.index];


        string display = Item.DisplayText.text;
        PGC.ChangeScreenItem.DisplayText.text = display;
        display = Item.KeyboardText.text;
        PGC.ChangeScreenItem.KeyboardText.text = display;
        display = Item.ControllerText.text;
        PGC.ChangeScreenItem.ControllerText.text = display;

        PGC.MenuItems[PGC.index].SelectionDisplay.enabled = false;
        PGC.MenuItems[PGC.index].isActive = false;

        PGC.ButtonScreen.SetActive(true);

    }






    void FillArray(int Index)
    {
        PlayerGUIControllerChange PGC = PlayerGUIChanges[Index];

        int index = 0;
        int count = PGC.Grid1.childCount;
        count = count + PGC.Grid2.childCount;
        PGC.MenuItems = new DG_ControllerMenuItem[count];
        for (int i = 0; i < PGC.Grid1.childCount; i++)
        {
            PGC.MenuItems[index] = PGC.Grid1.GetChild(i).GetComponent<DG_ControllerMenuItem>();
            index++;
        }
        for (int i = 0; i < PGC.Grid2.childCount; i++)
        {
            PGC.MenuItems[index] = PGC.Grid2.GetChild(i).GetComponent<DG_ControllerMenuItem>();
            index++;
        }
    }
    void FillCurrentValueText(int Index)
    {
        PlayerGUIControllerChange PGC = PlayerGUIChanges[Index];

        int PlayerID = QuickFind.NetworkSync.Player1PlayerCharacter;
        if (Index == 1) PlayerID = QuickFind.NetworkSync.Player2PlayerCharacter;

        for (int i = 0; i < PGC.MenuItems.Length; i++)
        {
            if (PGC.MenuItems[i].GetButton(QuickFind.InputController.GetPlayerByPlayerID(PlayerID)) != null)
            {
                PGC.MenuItems[i].KeyboardText.text = PGC.MenuItems[i].GetButton(QuickFind.InputController.GetPlayerByPlayerID(PlayerID)).MainKey.ToString();
                string NewVal = PGC.MenuItems[i].GetButton(QuickFind.InputController.GetPlayerByPlayerID(PlayerID)).AltKey.ToString();
                if (NewVal == "None")
                    NewVal = string.Empty;
                PGC.MenuItems[i].ControllerText.text = NewVal;
            }
        }
    }





    void CheckInput(int Index)
    {
        PlayerGUIControllerChange PGC = PlayerGUIChanges[Index];

        int PlayerID = QuickFind.NetworkSync.Player1PlayerCharacter;
        if (Index == 1) PlayerID = QuickFind.NetworkSync.Player2PlayerCharacter;

        foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(kcode))
            {
                string KeyCodeString = kcode.ToString();
                KeyCode ConvertedKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), KeyCodeString);

                char[] StringArray = KeyCodeString.ToCharArray();
                if (StringArray[0].ToString() == "J" && StringArray[1].ToString() == "o")
                {
                    SwapIfNeeded(ConvertedKey, false, Index);
                    PGC.MenuItems[PGC.index].GetButton(QuickFind.InputController.GetPlayerByPlayerID(PlayerID)).AltKey = ConvertedKey;
                    break;
                }
                else
                {
                    SwapIfNeeded(ConvertedKey, true, Index);
                    PGC.MenuItems[PGC.index].GetButton(QuickFind.InputController.GetPlayerByPlayerID(PlayerID)).MainKey = ConvertedKey;
                    break;
                }
            }
        }
    }

    void SwapIfNeeded(KeyCode ConvertedKey, bool isMain, int Index)
    {
        PlayerGUIControllerChange PGC = PlayerGUIChanges[Index];

        int PlayerID = QuickFind.NetworkSync.Player1PlayerCharacter;
        if (Index == 0) PlayerID = QuickFind.NetworkSync.Player2PlayerCharacter;

        Debug.Log("Needs to be updated");

        //Swap Out if Already Using Button;
        //List<DG_GameButtons.Button> ButtonList = QuickFind.InputController.Players[Index].ActiveButtonSet;
        //for (int i = 0; i < ButtonList.Count; i++)
        //{
        //    if (isMain)
        //    {
        //        if (ButtonList[i].MainKey == ConvertedKey)
        //        {
        //            ButtonList[i].MainKey = PGC.MenuItems[PGC.index].GetButton(QuickFind.InputController.GetPlayerByPlayerID(PlayerID)).MainKey;
        //            return;
        //        }
        //    }
        //    else
        //    {
        //        if (ButtonList[i].AltKey == ConvertedKey)
        //        {
        //            ButtonList[i].AltKey = PGC.MenuItems[PGC.index].GetButton(QuickFind.InputController.GetPlayerByPlayerID(PlayerID)).AltKey;
        //            return;
        //        }
        //    }
        //}
    }
}
