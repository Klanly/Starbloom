using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



#if UNITY_EDITOR
using UnityEditor;
/////////////////////////////////////////////////////////////////////////////////Editor Extension Buttons
[CustomEditor(typeof(DG_GUIControllerGhange))]
class DG_GUIControllerGhangeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        //Buttons
        DG_GUIControllerGhange myScript = (DG_GUIControllerGhange)target;
        if (GUILayout.Button("OpenControllerChangeUI"))
            myScript.OpenControllerChangeUI();

        DrawDefaultInspector();
    }
}
//////////////////////////////////////////////////////////////////////////////////
#endif








public class DG_GUIControllerGhange : MonoBehaviour
{

    public CanvasGroup UICanvas = null;
    public Transform Grid1 = null;
    public Transform Grid2 = null;

    [Header("Set Button UI")]
    public GameObject ButtonScreen = null;
    public DG_ControllerMenuItem ChangeScreenItem = null;
    [Header("Static Text")]
    public DG_TextStatic[] StaticTextArray;

    int index = 0;
    DG_ControllerMenuItem[] MenuItems;
    bool SelectingNewInput = false;

    int TimeDifference = 20;
    int Timer;



    private void Awake()
    {
        QuickFind.ControllerChange = this;
    }
    private void Start()
    {
        QuickFind.EnableCanvas(UICanvas, false);

        transform.localPosition = Vector3.zero;
        this.enabled = false;
    }

    private void Update()
    {
        if (Timer > 0)
        {
            Timer--;
            return;
        }

        if (SelectingNewInput)
        {
            if (Input.anyKey)
            {
                CheckInput();
                SelectingNewInput = false;
                Timer = TimeDifference;
                OpenControllerChangeUI();
            }
        }
    }


    public void ActionButtonSent()
    {
        if (Timer > 0)
            return;

        if (MenuItems[index].ButtonValue != DG_ControllerMenuItem.ButtonSwitch.Close)
        {
            Timer = TimeDifference;
            MenuItems[index].SelectionDisplay.enabled = false;
            MenuItems[index].isActive = false;
            OpenChangeButtonScreen();
        }
        else
            CloseControllerChangeUI();
    }


    public void OpenControllerChangeUI()
    {
        QuickFind.EnableCanvas(UICanvas, true);

        DG_PlayerInput.Player MP = QuickFind.InputController.MainPlayer;
        MP.InputState = DG_PlayerInput.Player.InputStateModes.ControllerChangeMenu;

        if (MenuItems == null)
            FillArray();

        FillCurrentValueText();

        MenuItems[index].SelectionDisplay.enabled = false;
        MenuItems[index].isActive = false;

        index = 0;
        MenuItems[0].SelectionDisplay.enabled = true;
        ButtonScreen.SetActive(false);

        for (int i = 0; i < StaticTextArray.Length; i++)
            StaticTextArray[i].ManualLoad();

        this.enabled = true;
    }

    public void CloseControllerChangeUI()
    {
        if (SelectingNewInput || Timer > 0)
            return;

        QuickFind.EnableCanvas(UICanvas, false);

        DG_PlayerInput.Player MP = QuickFind.InputController.MainPlayer;
        MP.InputState = DG_PlayerInput.Player.InputStateModes.Normal;

        this.enabled = false;
    }

    public void AdjustButtonViaController(bool isUp)
    {
        if (SelectingNewInput)
            return;

        MenuItems[index].SelectionDisplay.enabled = false;
        MenuItems[index].isActive = false;

        bool isController = false;
        if (QuickFind.InputController.MainPlayer.ButtonSet.JoyVert.Up)
            isController = true;
        if (isController)
            index = QuickFind.GetNextValueInArray(index, Grid1.childCount, !isUp, true);
        else
            index = QuickFind.GetNextValueInArray(index, MenuItems.Length, !isUp, true);
        MenuItems[index].SelectionDisplay.enabled = true;
        MenuItems[index].isActive = true;
    }

    public void ContextButtonViaMouse(DG_ControllerMenuItem ItemScript)
    {
        MenuItems[index].SelectionDisplay.enabled = false;
        MenuItems[index].isActive = false;

        for (int i = 0; i < MenuItems.Length; i++)
        {
            if (ItemScript == MenuItems[i])
                index = i;
        }

        MenuItems[index].SelectionDisplay.enabled = true;
        MenuItems[index].isActive = true;
    }
    void OpenChangeButtonScreen()
    {
        SelectingNewInput = true;
        DG_ControllerMenuItem Item = MenuItems[index];


        string display = Item.DisplayText.text;
        ChangeScreenItem.DisplayText.text = display;
        display = Item.KeyboardText.text;
        ChangeScreenItem.KeyboardText.text = display;
        display = Item.ControllerText.text;
        ChangeScreenItem.ControllerText.text = display;

        MenuItems[index].SelectionDisplay.enabled = false;
        MenuItems[index].isActive = false;

        ButtonScreen.SetActive(true);

    }






    void FillArray()
    {
        int index = 0;
        int count = Grid1.childCount;
        count = count + Grid2.childCount;
        MenuItems = new DG_ControllerMenuItem[count];
        for (int i = 0; i < Grid1.childCount; i++)
        {
            MenuItems[index] = Grid1.GetChild(i).GetComponent<DG_ControllerMenuItem>();
            index++;
        }
        for (int i = 0; i < Grid2.childCount; i++)
        {
            MenuItems[index] = Grid2.GetChild(i).GetComponent<DG_ControllerMenuItem>();
            index++;
        }
    }
    void FillCurrentValueText()
    {
        for (int i = 0; i < MenuItems.Length; i++)
        {
            if (MenuItems[i].GetButton() != null)
            {
                MenuItems[i].KeyboardText.text = MenuItems[i].GetButton().MainKey.ToString();
                string NewVal = MenuItems[i].GetButton().AltKey.ToString();
                if (NewVal == "None")
                    NewVal = string.Empty;
                MenuItems[i].ControllerText.text = NewVal;
            }
        }
    }





    void CheckInput()
    {
        foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(kcode))
            {
                string KeyCodeString = kcode.ToString();
                KeyCode ConvertedKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), KeyCodeString);

                char[] StringArray = KeyCodeString.ToCharArray();
                if (StringArray[0].ToString() == "J" && StringArray[1].ToString() == "o")
                {
                    SwapIfNeeded(ConvertedKey, false);
                    MenuItems[index].GetButton().AltKey = ConvertedKey;
                    break;
                }
                else
                {
                    SwapIfNeeded(ConvertedKey, true);
                    MenuItems[index].GetButton().MainKey = ConvertedKey;
                    break;
                }
            }
        }
    }

    void SwapIfNeeded(KeyCode ConvertedKey, bool isMain)
    {
        //Swap Out if Already Using Button;
        List<DG_GameButtons.Button> ButtonList = QuickFind.InputController.MainPlayer.ButtonSet.GetButtonList();
        for (int i = 0; i < ButtonList.Count; i++)
        {
            if (isMain)
            {
                if (ButtonList[i].MainKey == ConvertedKey)
                {
                    ButtonList[i].MainKey = MenuItems[index].GetButton().MainKey;
                    return;
                }
            }
            else
            {
                if (ButtonList[i].AltKey == ConvertedKey)
                {
                    ButtonList[i].AltKey = MenuItems[index].GetButton().AltKey;
                    return;
                }
            }
        }
    }
}
