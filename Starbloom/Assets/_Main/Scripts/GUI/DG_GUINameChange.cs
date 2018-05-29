using System.Collections;
using System.Collections.Generic;
using UnityEngine;



#if UNITY_EDITOR
using UnityEditor;
/////////////////////////////////////////////////////////////////////////////////Editor Extension Buttons
[CustomEditor(typeof(DG_GUINameChange))]
class DG_GUINameChangeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        //Buttons
        DG_GUINameChange myScript = (DG_GUINameChange)target;
        if (GUILayout.Button("OpenNameChangeUIFirstTime"))
            myScript.OpenNameChangeUI(myScript.DebugCharIDVal, true);
        if (GUILayout.Button("OpenNameChangeUINotFirstTime"))
            myScript.OpenNameChangeUI(myScript.DebugCharIDVal, false);

        DrawDefaultInspector();
    }
}
//////////////////////////////////////////////////////////////////////////////////
#endif







public class DG_GUINameChange : MonoBehaviour {

    public enum NameChangeGuiOptions
    {
        Space,
        Delete,
        Select,
        Default
    }




    public CanvasGroup UICanvas = null;
    public DG_TextLetterGroup NameRegion;
    public DG_TextLetterGroup LetterSelectRegion;
    public Transform TextButtonGrid;


    [Header("Letter Regions")]
    public int LetterGridCatVal;
    public int LetterGridIDVal;

    [Header("Top Text")]
    public DG_TextStatic TopText = null;

    [Header("Debug")]
    public int DebugCharIDVal;


    [HideInInspector] public bool NameChangeIsOpen = false;
    bool InLetterRegion = true;
    int CurrentRow = 0;
    int CurrentColumn = 0;
    int DefaultCharID;

    DG_TextButton[] ButtonColumn;



    private void Awake()
    {
        QuickFind.NameChangeUI = this;

        ButtonColumn = new DG_TextButton[TextButtonGrid.childCount];
        for (int i = 0; i < TextButtonGrid.childCount; i++)
            ButtonColumn[i] = TextButtonGrid.GetChild(i).GetComponent<DG_TextButton>();
    }
    private void Start()
    {
        QuickFind.EnableCanvas(UICanvas, false);
        transform.localPosition = Vector3.zero;
    }

    //////////////////////////////////////////////////////////////////////////////////


    public void OpenNameChangeUI(int CharIDVal, bool FirstTimeDisplay)
    {
        QuickFind.EnableCanvas(UICanvas, true);

        DG_PlayerInput.Player MP = QuickFind.InputController.MainPlayer;
        MP.InputState = DG_PlayerInput.Player.InputStateModes.NameChangeMenu;

        DefaultCharID = CharIDVal;
        //Populate Single Letter Regions
        NameRegion.OpenUI(true, DefaultCharID, 0, FirstTimeDisplay);
        LetterSelectRegion.OpenUI(false, LetterGridIDVal, LetterGridCatVal);

        //Populate Static Text Regions
        TopText.ManualLoad();
        ActivateTextButtons();
        LetterSelectRegion.Row[0].Column[0].QueueActivate(false);
        ActivateBlinkLetters();

        InLetterRegion = true;
        CurrentRow = 0;
        CurrentColumn = 0;

        NameChangeIsOpen = true;
    }
    public void CloseNameChangeUI()
    {
        QuickFind.EnableCanvas(UICanvas, false);

        DG_PlayerInput.Player MP = QuickFind.InputController.MainPlayer;
        MP.InputState = DG_PlayerInput.Player.InputStateModes.Normal;

        NameChangeIsOpen = false;

        ActivateLetter(false);
        ActivateButton(false);
    }


    //////////////////////////////////////////////////////////////////////////////////

    void ActivateTextButtons()
    {
        for (int i = 0; i < ButtonColumn.Length; i++)
            ButtonColumn[i].TextScript.ManualLoad();
    }
    void ActivateBlinkLetters()
    {
        bool Break = false;
        int Count = NameRegion.Row[0].Column.Length;
        for (int i = 0; i < Count; i++)
        {
            if (NameRegion.Row[0].Column[i].HasLetter && i != (Count - 1))
                continue;
            else if (i == (Count - 1))
                Break = true;
            else
                Break = true;

            if (Break)
            {
                NameRegion.Row[0].Column[i].QueueActivate(true);
                break;
            }
        }
    }

    //////////////////////////////////////////////////////////////////////////////////

    public void MovePosition(int Value)
    {
        switch (Value)
        {
            case 1: //Up
                {
                    if (InLetterRegion)
                        ShiftLetterValue(false, -1);
                    else
                        ShiftButtonValue(-1);
                }
                break;
            case 2: //Down
                {
                    if (InLetterRegion)
                        ShiftLetterValue(false, 1);
                    else
                        ShiftButtonValue(1);
                }
                break;
            case 3: //Right
                {
                    if (InLetterRegion)
                        ShiftLetterValue(true, 1);
                }
                break;
            case 4: //Left
                {
                    if (InLetterRegion)
                        ShiftLetterValue(true, -1);
                    else
                        ShiftRegion(true);
                }
                break;
        }
    }

    //Letter Region
    void ShiftLetterValue(bool IsHor, int Value)
    {
        ActivateLetter(false);
        if (IsHor)
        {
            CurrentColumn = CurrentColumn + Value;
            if (CurrentColumn < 0)
                CurrentColumn = 0;
            if (CurrentColumn == LetterSelectRegion.Row[CurrentRow].Column.Length)
                ShiftRegion(false);
            else
                ActivateLetter(true);
        }
        else
        {
            CurrentRow = CurrentRow + Value;
            if (CurrentRow < 0)
                CurrentRow = 0;
            if (CurrentRow == LetterSelectRegion.Row.Length)
                CurrentRow = LetterSelectRegion.Row.Length - 1;

            ActivateLetter(true);
        }
    }
    void ActivateLetter(bool isTrue)
    {
        LetterSelectRegion.Row[CurrentRow].Column[CurrentColumn].CurrentlyActive = isTrue;
        if (isTrue)
            LetterSelectRegion.Row[CurrentRow].Column[CurrentColumn].QueueActivate(false);
        else
            LetterSelectRegion.Row[CurrentRow].Column[CurrentColumn].QueueDeactivate(false);
    }

    //Button Region
    void ShiftButtonValue(int Value)
    {
        ActivateButton(false);
        CurrentColumn = CurrentColumn + Value;
        if (CurrentColumn < 0)
            CurrentColumn = 0;
        if (CurrentColumn == ButtonColumn.Length)
            CurrentColumn = ButtonColumn.Length - 1;

        ActivateButton(true);
    }
    void ActivateButton(bool isTrue)
    {
        ButtonColumn[CurrentColumn].CurrentlyActive = isTrue;
        if (isTrue)
            ButtonColumn[CurrentColumn].QueueActivate(true);
        else
            ButtonColumn[CurrentColumn].QueueDeactivate(false);
    }

    //Shift
    void ShiftRegion(bool ToLetters)
    {
        InLetterRegion = ToLetters;
        if (ToLetters)
        {
            ActivateButton(false);
            CurrentRow = 0;
            CurrentColumn = LetterSelectRegion.Row[0].Column.Length - 1;
            ActivateLetter(true);
        }
        else
        {
            CurrentColumn = 0;
            ActivateButton(true);
        }
    }


    //////////////////////////////////////////////////////////////////////////////////

    public void ActionKeyPressed()
    {
        if (!NameChangeIsOpen)
            return;

        if (InLetterRegion) //Add letter to name.
        {
            int Active = CurrentActiveLetter();
            NameRegion.Row[0].Column[Active].TextLetter.text = LetterSelectRegion.Row[CurrentRow].Column[CurrentColumn].TextLetter.text;
            int Next = Active + 1;
            if(Next != NameRegion.Row[0].Column.Length)
                ChangeActive(Active, Next);
        }
        else //Press Button
        {
            DG_TextButton TB = ButtonColumn[CurrentColumn];
            switch (TB.GuiOption)
            {
                case NameChangeGuiOptions.Default: Default(); break;
                case NameChangeGuiOptions.Delete: Delete(); break;
                case NameChangeGuiOptions.Select: Select(); break;
                case NameChangeGuiOptions.Space: Space(); break;
            }
        }
    }
    void Default()
    {
        NameRegion.OpenUI(true, DefaultCharID, 0, true);
        ActivateBlinkLetters();
    }
    void Delete()
    {
        int Active = CurrentActiveLetter();
        if (Active == NameRegion.Row[0].Column.Length - 1 && NameRegion.Row[0].Column[Active].TextLetter.text != string.Empty) //Last Letter
        {
            NameRegion.Row[0].Column[Active].IsSpace = false;
            NameRegion.Row[0].Column[Active].TextLetter.text = string.Empty;
        }
        else
        {
            int Previous = Active - 1;
            if (Previous != -1)
            {
                NameRegion.Row[0].Column[Previous].IsSpace = false;
                NameRegion.Row[0].Column[Previous].TextLetter.text = string.Empty;
                ChangeActive(Active, Previous);
            }
        }
    }
    void Select()
    {
        DG_CharacterObject CharacterObject = QuickFind.CharacterDatabase.GetItemFromID(DefaultCharID);
        //QuickFind.DataStrings.GetStringFromID(CharacterObject.SaveStringID).StringValue = BuildNameString();
        CloseNameChangeUI();
    }
    void Space()
    {
        int Active = CurrentActiveLetter();
        if (Active != NameRegion.Row[0].Column.Length - 1)
        {
            NameRegion.Row[0].Column[Active].IsSpace = true;
            ChangeActive(Active, Active + 1);
        }
    }


    string BuildNameString()
    {
        System.Text.StringBuilder SB = new System.Text.StringBuilder();
        for(int i = 0; i < NameRegion.Row[0].Column.Length; i++)
        {
            if (NameRegion.Row[0].Column[i].TextLetter.text != string.Empty)
                SB.Append(NameRegion.Row[0].Column[i].TextLetter.text);
            else if(NameRegion.Row[0].Column[i].IsSpace)
                SB.Append(" ");
        }
        return SB.ToString();
    }




    int CurrentActiveLetter()
    {
        int count = NameRegion.Row[0].Column.Length;
        for (int i = 0; i < count; i++)
        {
            if (NameRegion.Row[0].Column[i].CurrentlyActive)
                return i;
        }
        return 0;
    }
    void ChangeActive(int Active, int NextVal)
    {
        NameRegion.Row[0].Column[Active].CurrentlyActive = false;
        NameRegion.Row[0].Column[Active].QueueDeactivate(true);
        NameRegion.Row[0].Column[NextVal].CurrentlyActive = true;
        NameRegion.Row[0].Column[NextVal].QueueActivate(true);
    }
}
