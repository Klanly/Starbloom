using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NA_DialogueGUIController : MonoBehaviour
{
    public enum GUIStates
    {
        NotInConversatation,
        TextTyping,
        TextComplete
    }
    public enum DialogueStates
    {
        AwaitingButton,
        PopulatingCoice,
        AwaitingChoice
    }

    [System.NonSerialized] public GUIStates GUIState;
    [System.NonSerialized] public DialogueStates DialogueState;
    [System.NonSerialized] public int PlayerID = -2;

    [Header("GUI")]
    public CanvasGroup UICanvas = null;
    public GameObject TopPanel;
    public UnityEngine.UI.Image NextButton = null;
    [Header("Descisions")]
    public CanvasGroup DescisionCanvas = null;
    public TMPro.TextMeshProUGUI MainTextDisplay;
    public TMPro.TextMeshProUGUI[] DescisionArray;
    public Image[] ControllerSelectionDisplays;


    [System.NonSerialized] public NodeLink ActiveDialogue;
    [System.NonSerialized] public NodeLink.Window[] PathChoices;

    private void Awake()
    {
        QuickFind.DialogueGUIController = this;
    }
    private void Start()
    {
        QuickFind.EnableCanvas(UICanvas, false);
        QuickFind.EnableCanvas(DescisionCanvas, false);



        transform.localPosition = Vector3.zero;
    }

    public void ActionKeyPressed()
    {
        if (QuickFind.NameChangeUI.NameChangeIsOpen) return;

        DG_PlayerInput.Player Char = QuickFind.InputController.GetPlayerByPlayerID(PlayerID);

        if (GUIState == GUIStates.NotInConversatation && QuickFind.ContextDetectionHandler.ContextHit)
        {
            DG_ContextObject CO = QuickFind.ContextDetectionHandler.LastEncounteredContext.GetComponent<DG_ContextObject>();
            NetworkObject NO = QuickFind.NetworkObjectManager.ScanUpTree(CO.transform);
            TriggerEvent(NO.ItemRefID);
        }
        else if (GUIState == GUIStates.TextTyping)
            QuickFind.TextPrintout.RushAllActiveTextEffects();
        else if (GUIState == GUIStates.TextComplete)
        {
            if (DialogueState == DialogueStates.AwaitingButton)
                NextTrigger();
            else if (DialogueState == DialogueStates.AwaitingChoice)
                QuickFind.GUIContextHandler.ContextButtonViaKey();
        }
    }
    public void TriggerEvent(int ContextID)
    {
        if (ContextID == 0)
        {
            Debug.LogError("Context ID is still set to zero.  Please set a value for this object.");
            return;
        }
        Debug.Log("New Context Event Started");

        ActiveDialogue = QuickFind.DialogueManager.GetItemFromID(ContextID);
        ActiveDialogue.Reset();
        NextTrigger();
    }
    public void PlayerChoice(int Choice) //Triggered from Inpector in Unity
    {
        ActiveDialogue.NextChoice(Choice);
        TriggerEventInternal();
    }
    public void SetGuiState(int State, int PlayerID)
    {
        //This is triggered from an inpector OnEnd event from Text Over Time script in "Text" Object in GUI Canvas
        switch (State)
        {
            case 0: GUIState = GUIStates.NotInConversatation; break;
            case 1:
                {
                    NextButton.enabled = false;
                    GUIState = GUIStates.TextTyping; break;
                }
            case 2:
                {
                    GUIState = GUIStates.TextComplete;
                    if (DialogueState == DialogueStates.PopulatingCoice)
                    {
                        DialogueState = DialogueStates.AwaitingChoice;
                        BuildAllOptions(PlayerID);
                    }
                    else if (DialogueState == DialogueStates.AwaitingButton)
                        NextButton.enabled = true;
                }
                break;
        }
    }
    public void NextTrigger()
    {
        ActiveDialogue.Next();
        TriggerEventInternal();
    }
    public void ClearAllOptions()
    {
        QuickFind.EnableCanvas(DescisionCanvas, false);
        QuickFind.InputController.GetPlayerByPlayerID(PlayerID).InputMode = DG_PlayerInput.Player.InputStateModes.Normal;
        QuickFind.GUIContextHandler.isDialogueOption = false;
    }

    void TriggerEventInternal()
    {
        if(ActiveDialogue.EndTree)
        {
            EndConversation();
            return;
        }

        if (ActiveDialogue.Current.Type == NodeLink.WindowTypes.Text)
            DisplayText();
        else if (ActiveDialogue.Current.Type == NodeLink.WindowTypes.Choice)
        {
            if (ActiveDialogue.Current.ContextBool2) //Quest Check
            {
                int QuestID = ActiveDialogue.Current.ContextInt;
                DG_QuestObject QuestObject = QuickFind.QuestDatabase.GetItemFromID(QuestID);

                bool QuestIsComplete = QuestObject.QuestIsComplete();
                PathChoices = ActiveDialogue.GetWindowsByWindow(ActiveDialogue.Current);

                for (int i = 0; i < PathChoices.Length; i++)
                {
                    NodeLink.Window WindowOption = PathChoices[i];
                    if (QuestIsComplete)
                    {
                        if (WindowOption.ContextBool3)
                            continue;
                        else
                            ActiveDialogue.Current = WindowOption;
                    }
                    else
                    {
                        if (!WindowOption.ContextBool3)
                            continue;
                        else
                            ActiveDialogue.Current = WindowOption;
                    }
                }
                TriggerEventInternal();
            }
            else if (ActiveDialogue.Current.ContextBool3)
            {
                int QuestID = ActiveDialogue.Current.ContextInt;
                DG_QuestObject QuestObject = QuickFind.QuestDatabase.GetItemFromID(QuestID);
                QuestObject.CompleteQuest(PlayerID);
                PathChoices = ActiveDialogue.GetWindowsByWindow(ActiveDialogue.Current);
                if (PathChoices != null && PathChoices.Length > 0)
                    ActiveDialogue.Current = PathChoices[0];
                else
                    ActiveDialogue.EndTree = true;
                TriggerEventInternal();
            }
        }
        else if (ActiveDialogue.Current.Type == NodeLink.WindowTypes.ChoiceAnswer)
            DisplayText();
    }
    void DisplayText()
    {
        QuickFind.InputController.GetPlayerByPlayerID(PlayerID).Moveable = false;
        QuickFind.EnableCanvas(UICanvas, true);

        QuickFind.TextPrintout.AddNewDisplayText(BuildDialogueDisplay(ActiveDialogue.Current), MainTextDisplay);

        NodeLink.Window NextWindow = ActiveDialogue.GetNextChoice();
        if (NextWindow == null)
            DialogueState = DialogueStates.AwaitingButton;
        else
        {
            if (NextWindow.Type == NodeLink.WindowTypes.Choice && NextWindow.ContextBool)
                DialogueState = DialogueStates.PopulatingCoice;
            else
                DialogueState = DialogueStates.AwaitingButton;
        }
    }
    void BuildAllOptions(int PlayerID)
    {
        QuickFind.EnableCanvas(DescisionCanvas, true);

        for (int i = 0; i < DescisionArray.Length; i++)
            DescisionArray[i].transform.parent.gameObject.SetActive(false);

        NodeLink.Window NextWindow = ActiveDialogue.GetNextChoice();
        PathChoices = ActiveDialogue.GetWindowsByWindow(NextWindow);
        List<Image> DescisionOptions = new List<Image>();
        for (int i = 0; i < PathChoices.Length; i++)
        {
            NodeLink.Window WindowOption = PathChoices[i];
            bool DisplayTrue = true;
            if (WindowOption.ContextBool)  //Quest Requirement
            {
                int QuestID = WindowOption.ContextInt;
                DG_QuestObject QuestObject = QuickFind.QuestDatabase.GetItemFromID(QuestID);

                if (!QuestObject.QuestRequirementsAreMet(PlayerID))
                    DisplayTrue = false;
            }

            if (DisplayTrue)
            {
                DescisionArray[i].transform.parent.gameObject.SetActive(true);
                string Display = QuickFind.WordDatabase.GetItemFromID(WindowOption.ContextInt2).TextValues[(int)QuickFind.UserSettings.CurrentLanguage].stringEntry;
                string ScanedString = ScanForContext(Display, false);
                QuickFind.TextPrintout.AddNewDisplayText(ScanedString, DescisionArray[i]);

                ControllerSelectionDisplays[i].enabled = false;
                DescisionOptions.Add(ControllerSelectionDisplays[i]);
            }
            else
                DescisionOptions.Add(ControllerSelectionDisplays[i]);
        }

        QuickFind.TextPrintout.RushAllActiveTextEffects();

        //Set up Context for Controller.
        QuickFind.InputController.GetPlayerByPlayerID(PlayerID).InputMode = DG_PlayerInput.Player.InputStateModes.DialogueMenu;
        QuickFind.GUIContextHandler.isDialogueOption = true;
        QuickFind.GUIContextHandler.ControllerMenuPosition = 0;
        QuickFind.GUIContextHandler.OpenNewContextMenuSelectionState(DescisionOptions.ToArray());
    }
    void EndConversation()
    {
        GUIState = GUIStates.NotInConversatation;
        QuickFind.InputController.GetPlayerByPlayerID(PlayerID).Moveable = true;

        QuickFind.EnableCanvas(UICanvas, false);
        QuickFind.EnableCanvas(DescisionCanvas, false);

        QuickFind.InputController.GetPlayerByPlayerID(PlayerID).InputMode = DG_PlayerInput.Player.InputStateModes.Normal;

        MainTextDisplay.text = string.Empty;
        for (int i = 0; i < DescisionArray.Length; i++)
            DescisionArray[i].text = string.Empty;
    }

    string BuildDialogueDisplay(NodeLink.Window DisplayWindow)
    {
        string FinalDisplay = string.Empty;
        string Name = string.Empty;
        string TextDisplay = string.Empty;

        if (ActiveDialogue.Current.ContextInt != 0)
        {
            DG_CharacterObject Char = QuickFind.CharacterDatabase.GetItemFromID(ActiveDialogue.Current.ContextInt);
            //
            Name = QuickFind.WordDatabase.GetItemFromID(Char.NameWordID).TextValues[(int)QuickFind.UserSettings.CurrentLanguage].stringEntry;
        }
        //
        TextDisplay = QuickFind.WordDatabase.GetItemFromID(DisplayWindow.ContextInt2).TextValues[(int)QuickFind.UserSettings.CurrentLanguage].stringEntry;
        string ScanedString = ScanForContext(TextDisplay, false);

        FinalDisplay = string.Format("{0}{1}{2}", Name, "\n", ScanedString);

        return FinalDisplay;
    }

    public static string ScanForContext(string DatabaseString, bool Editor)
    {
        char[] s = DatabaseString.ToCharArray();

        if (s[0].ToString() != "*")
            return DatabaseString;
        else
        {
            System.Text.StringBuilder builder = new System.Text.StringBuilder("");
            for (int i = 1; i < s.Length; i++)
            {
                string value = s[i].ToString();
                if (value != "*")
                    builder.Append(value);
                else
                {
                    //Hit a star to Trade out the context.
                    System.Text.StringBuilder ReplaceString = new System.Text.StringBuilder("");
                    int CurrentPos = i + 1;
                    for (int iN = CurrentPos; iN < s.Length; iN++)
                    {
                        i++;
                        string InnerValue = s[iN].ToString();
                        if (InnerValue != "*")
                            ReplaceString.Append(InnerValue);
                        else
                            break;
                    }
                    //Once Code is found, Find it in Database
                    string code = ReplaceString.ToString();
                    string DatabaseColumn = string.Empty;
                    char[] CodeS = code.ToCharArray();

                    if (!Editor)
                    {
                        switch (CodeS[0].ToString())
                        {
                            //Color Code 
                            case "D": DatabaseColumn = QuickFind.ColorDatabase.GetTextColorByID(CodeValue(CodeS)); break;
                            //Character
                            case "C": DatabaseColumn = QuickFind.WordDatabase.GetNameFromID(1, CodeValue(CodeS), false); break;
                            //PartyMember
                            case "P": 
                                {
                                    //DG_CharacterObject CharacterObject = QuickFind.PlayerParty.GetCharacterByPartyID(CodeValue(CodeS));
                                    //if (!CharacterObject.NameEditableByUser)
                                    //    DatabaseColumn = QuickFind.WordDatabase.GetItemFromID(CharacterObject.DefaultNameWordID, CharacterObject.DefaultNameCatagoryID).TextValues[(int)QuickFind.UserSettings.CurrentLanguage].stringEntry;
                                    //else
                                    //    DatabaseColumn = QuickFind.DataStrings.GetStringFromID(CharacterObject.SaveStringID).StringValue;
                                }
                                break;
                            //Items
                            case "I": 
                                {
                                    DG_ItemObject ItemObject = QuickFind.ItemDatabase.GetItemFromID(CodeValue(CodeS));
                                    DatabaseColumn = QuickFind.WordDatabase.GetItemFromID(ItemObject.ToolTipType.MainLocalizationID).TextValues[(int)QuickFind.UserSettings.CurrentLanguage].stringEntry;
                                }
                                break;
                        }

                    }
                    else
                    {
                        #if UNITY_EDITOR
                        switch (CodeS[0].ToString())
                        {
                            //Color Code
                            case "D": DatabaseColumn = QuickFindInEditor.GetEditorColorCodes().GetTextColorByID(CodeValue(CodeS)); break;
                            //Character
                            case "C": DatabaseColumn = QuickFind.WordDatabase.GetNameFromID(1, CodeValue(CodeS), false); break;
                            //PartyMember
                            case "P":
                                {
                                    //DG_CharacterObject CharacterObject = QuickFindInEditor.GetEditorPlayerPartyStats().GetCharacterByPartyID(CodeValue(CodeS));
                                    //
                                    //if (!CharacterObject.NameEditableByUser)
                                    //    DatabaseColumn = QuickFind.WordDatabase.GetItemFromID(CharacterObject.DefaultNameWordID, CharacterObject.DefaultNameCatagoryID).TextValues[(int)QuickFind.UserSettings.CurrentLanguage].stringEntry;
                                    //else
                                    //    DatabaseColumn = QuickFindInEditor.GetEditorDataStrings().GetStringFromIDInEditor(CharacterObject.SaveStringID).StringValue;

                                }
                                break;
                            //Items
                            case "I": 
                                {
                                    DG_ItemObject ItemObject = QuickFindInEditor.GetEditorItemDatabase().GetItemFromID(CodeValue(CodeS));

                                    if (!Editor)
                                        DatabaseColumn = QuickFind.WordDatabase.GetItemFromID(ItemObject.ToolTipType.MainLocalizationID).TextValues[(int)QuickFind.UserSettings.CurrentLanguage].stringEntry;
                                    else
                                        DatabaseColumn = QuickFindInEditor.GetEditorWordDatabase().GetItemFromID(ItemObject.ToolTipType.MainLocalizationID).TextValues[(int)QuickFindInEditor.GetEditorUserSettings().CurrentLanguage].stringEntry;
                                }
                                break;
                        }
                        #endif
                    }
                    builder.Append(DatabaseColumn);
                }
            }
            return builder.ToString();
        }
    }
    static int CodeValue(char[] CodeS)
    {
        System.Text.StringBuilder NewString = new System.Text.StringBuilder("");
        for (int iN = 1; iN < CodeS.Length; iN++)
            NewString.Append(CodeS[iN].ToString());

        int OutVal;
        int.TryParse(NewString.ToString(), out OutVal);
        return OutVal;
    }
    public static string GetStaticText(int TextWordID)
    {
        DG_WordObject WO = QuickFind.WordDatabase.GetItemFromID(TextWordID);
        string DatabaseString = QuickFind.WordDatabase.GetWordByLanguage(WO.TextValues);
        return NA_DialogueGUIController.ScanForContext(DatabaseString, false);
    }
}
