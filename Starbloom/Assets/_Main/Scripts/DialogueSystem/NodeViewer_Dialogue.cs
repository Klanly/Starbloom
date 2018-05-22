using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#if UNITY_EDITOR

using UnityEditor;

public class NodeViewer_Dialogue
{

    public static GUIStyle SetColors(NodeLink.Window WindowList, NodeLink CurrentNode)
    {
        NodeLink.Window PreviousWindow = NodeViewer.FindPreviousWindow(WindowList, CurrentNode);

        //Creates the actual window
        GUIStyle FinalStyle = new GUIStyle("flow node 0");
        FinalStyle.fontSize = 12;

        if (WindowList.NodeType == NodeLink.NodeType.Start)
        {
            FinalStyle.contentOffset = new Vector2(0, -28);
            GUI.color = NodeViewOptions.GetColorOption(CurrentNode).StartColor;
            FinalStyle.normal.textColor = Color.white;
        }
        else
        {
            FinalStyle.contentOffset = new Vector2(0, -12);
            if (WindowList.Type == NodeLink.WindowTypes.Text)
                GUI.color = NodeViewOptions.GetColorOption(CurrentNode).TextColor;
            else if (WindowList.Type == NodeLink.WindowTypes.ChoiceAnswer)
            {
                if (PreviousWindow.ContextBool2)
                {
                    if (WindowList.ContextBool3)
                        GUI.color = NodeViewOptions.GetColorOption(CurrentNode).NoPathColor;
                    else
                        GUI.color = NodeViewOptions.GetColorOption(CurrentNode).YesPathColor;
                }
                if (PreviousWindow.ContextBool)
                {
                    if (WindowList.ContextBool)
                        GUI.color = NodeViewOptions.GetColorOption(CurrentNode).DialogueQuestPath;
                    else
                        GUI.color = NodeViewOptions.GetColorOption(CurrentNode).DialogueNoQuestPath;
                }
                if (PreviousWindow.ContextBool3)
                    GUI.color = NodeViewOptions.GetColorOption(CurrentNode).TextColor;
            }
            else if (WindowList.Type == NodeLink.WindowTypes.Choice)
            {
                if (WindowList.ContextBool2)
                    GUI.color = NodeViewOptions.GetColorOption(CurrentNode).CheckQuestColor;
                if (WindowList.ContextBool)
                    GUI.color = NodeViewOptions.GetColorOption(CurrentNode).DialogueOptionsColor;
                if (WindowList.ContextBool3)
                    GUI.color = NodeViewOptions.GetColorOption(CurrentNode).QuestCompleteColor;
            }
        }

        return FinalStyle;
    }

    public static void DrawPreview(NodeLink CurrentNode)
    {
        DG_WordDatabase WDB = QuickFindInEditor.GetEditorWordDatabase();
        UserSettings US = QuickFindInEditor.GetEditorUserSettings();
        int LanguageValue = (int)US.CurrentLanguage;

        GUIStyle TextStyle = new GUIStyle();
        TextStyle.wordWrap = true;
        TextStyle.clipping = TextClipping.Clip;
        TextStyle.richText = true;

        for (int j = 0; j < CurrentNode.Windows.Count; j++)
        {
            NodeLink.Window WindowList = CurrentNode.Windows[j];

            DG_WordObject WO = null;
            string Preview = string.Empty;
            if (WindowList.Type == NodeLink.WindowTypes.Text && WindowList.NodeType == NodeLink.NodeType.Default)
                WO = WDB.GetItemFromID(WindowList.ContextInt2);
            else if (WindowList.Type == NodeLink.WindowTypes.ChoiceAnswer)
            {
                NodeLink.Window PrevWindow = NodeViewer.FindPreviousWindow(WindowList, CurrentNode);
                if (PrevWindow.ContextBool)
                {
                    if (WindowList.ContextBool)
                        Preview += "<b>Need Qst " + WindowList.ContextInt.ToString() + " Requiremets</b>       ";
                }
                WO = WDB.GetItemFromID(WindowList.ContextInt2);
            }
            else if (WindowList.Type == NodeLink.WindowTypes.Choice)
            {
                if (WindowList.ContextBool2)//Quest Check
                    Preview = "<b>Check Qst " + WindowList.ContextInt.ToString() + "</b>";
                else if (WindowList.ContextBool)
                    Preview = "<b>Dialogue Options</b>";
                else if (WindowList.ContextBool3)
                    Preview = "<b>Quest " + WindowList.ContextInt.ToString() + " Complete</b>";
            }
            else
                continue;

            if (WO != null)
                Preview += WO.TextValues[LanguageValue].stringEntry;
            GUI.Label(new Rect(WindowList.Size.x + 40, WindowList.Size.y, 100, 55), Preview, TextStyle);
        }
    }
    public static void DrawLeftWindow(NodeLink CurrentNode, NodeLink.Window CurrentViewedWindow)
    {
        NodeViewOptions.GetOptions().OptionsOpen = GUI.Toggle(new Rect(0, 0, 130, 18), NodeViewOptions.GetOptions().OptionsOpen, "  Options");

        if (!NodeViewOptions.GetOptions().OptionsOpen)
        {
            if (CurrentViewedWindow == null)
                return;


            //Event Type
            if (CurrentViewedWindow.Type == NodeLink.WindowTypes.Text)
                ShowText(false, CurrentViewedWindow, CurrentNode);
            //Path Type
            else if (CurrentViewedWindow.Type == NodeLink.WindowTypes.ChoiceAnswer)
            {
                NodeLink.Window PrevWindow = NodeViewer.FindPreviousWindow(CurrentViewedWindow, CurrentNode);
                if (PrevWindow == null)
                    return;

                if (PrevWindow.ContextBool)
                {
                    //QUEST Requirement
                    ShowText(true, CurrentViewedWindow, CurrentNode);

                    CurrentViewedWindow.ContextBool = GUI.Toggle(new Rect(5, 42, 130, 18), CurrentViewedWindow.ContextBool, "Quest Req");

                    GUI.Label(new Rect(5, 60, 120, 15), "Quest ID");
                    if (GUI.Button(new Rect(95, 60, 40, 15), "Goto"))
                        Selection.activeGameObject = QuickFindInEditor.GetEditorQuestDatabase().GetItemFromID(CurrentViewedWindow.ContextInt).gameObject;
                    CurrentViewedWindow.ContextInt = EditorGUI.IntField(new Rect(5, 75, 130, 18), CurrentViewedWindow.ContextInt);
                }
                else if (PrevWindow.ContextBool2)
                {
                    //QUEST Requirement
                    ShowText(false, CurrentViewedWindow, CurrentNode);
                    CurrentViewedWindow.ContextBool3 = GUI.Toggle(new Rect(5, 40, 130, 15), CurrentViewedWindow.ContextBool3, "Fail Case");
                }
                else if (PrevWindow.ContextBool3)
                    ShowText(false, CurrentViewedWindow, CurrentNode);
                else
                    GUI.Label(new Rect(1, 20, 77, 15), "No Type");
            }
            //Split Type
            else if (CurrentViewedWindow.Type == NodeLink.WindowTypes.Choice)
            {
                GUI.Label(new Rect(53, 25, 120, 15), "Action");

                if (!CurrentViewedWindow.ContextBool && !CurrentViewedWindow.ContextBool2 && !CurrentViewedWindow.ContextBool3)
                {
                    CurrentViewedWindow.ContextBool = GUI.Toggle(new Rect(1, 50, 150, 15), CurrentViewedWindow.ContextBool, "User Input");
                    CurrentViewedWindow.ContextBool2 = GUI.Toggle(new Rect(1, 65, 150, 15), CurrentViewedWindow.ContextBool2, "Quest Check");
                    CurrentViewedWindow.ContextBool3 = GUI.Toggle(new Rect(1, 80, 150, 15), CurrentViewedWindow.ContextBool3, "Complete Quest");

                }
                if (CurrentViewedWindow.ContextBool)
                    CurrentViewedWindow.ContextBool = GUI.Toggle(new Rect(1, 50, 150, 15), CurrentViewedWindow.ContextBool, "User Input");
                if (CurrentViewedWindow.ContextBool2)
                {
                    CurrentViewedWindow.ContextBool2 = GUI.Toggle(new Rect(1, 50, 150, 15), CurrentViewedWindow.ContextBool2, "Quest Check");
                    GUI.Label(new Rect(1, 70, 77, 15), "Quest ID");
                    if (GUI.Button(new Rect(95, 70, 40, 15), "Goto"))
                        Selection.activeGameObject = QuickFindInEditor.GetEditorQuestDatabase().GetItemFromID(CurrentViewedWindow.ContextInt).gameObject;
                    CurrentViewedWindow.ContextInt = EditorGUI.IntField(new Rect(3, 85, 130, 15), CurrentViewedWindow.ContextInt);

                }
                if (CurrentViewedWindow.ContextBool3)
                {
                    CurrentViewedWindow.ContextBool3 = GUI.Toggle(new Rect(1, 55, 150, 15), CurrentViewedWindow.ContextBool3, "Complete Quest");
                    GUI.Label(new Rect(1, 75, 77, 15), "Quest ID");
                    if (GUI.Button(new Rect(95, 75, 40, 15), "Goto"))
                        Selection.activeGameObject = QuickFindInEditor.GetEditorQuestDatabase().GetItemFromID(CurrentViewedWindow.ContextInt).gameObject;
                    CurrentViewedWindow.ContextInt = EditorGUI.IntField(new Rect(3, 95, 130, 15), CurrentViewedWindow.ContextInt);
                }
            }


        }
        else
            NodeViewOptions.DisplayOptions(CurrentNode);
    }
    static void ShowText(bool Hide, NodeLink.Window CurrentViewedWindow, NodeLink CurrentNode)
    {
        if (CurrentViewedWindow.Type == NodeLink.WindowTypes.Text)
        {
            if (CurrentViewedWindow.NodeType == NodeLink.NodeType.Default)
            {
                GUI.Label(new Rect(55, 25, 120, 15), "Text");
            }
            else
            {
                if (CurrentViewedWindow.NodeType == NodeLink.NodeType.Start)
                    GUI.Label(new Rect(40, 20, 120, 15), "Start Node");
                else
                    GUI.Label(new Rect(45, 20, 120, 15), "End Node");
                return;
            }
        }
        else
            GUI.Label(new Rect(55, 20, 120, 15), "Path");

        //Dialogue

        GUI.Label(new Rect(10, 110, 120, 15), "Cat ID");
        if (GUI.Button(new Rect(12, 145, 40, 15), "Goto"))
            Selection.activeGameObject = QuickFindInEditor.GetEditorWordDatabase().transform.GetChild(CurrentViewedWindow.ContextInt4).gameObject;
        CurrentViewedWindow.ContextInt4 = EditorGUI.IntField(new Rect(5, 125, 55, 18), CurrentViewedWindow.ContextInt4);

        GUI.Label(new Rect(80, 110, 75, 15), "Text ID");
        if (GUI.Button(new Rect(80, 145, 40, 15), "Goto"))
            Selection.activeGameObject = QuickFindInEditor.GetEditorWordDatabase().GetItemFromID(CurrentViewedWindow.ContextInt2).gameObject;
        CurrentViewedWindow.ContextInt2 = EditorGUI.IntField(new Rect(65, 125, 75, 18), CurrentViewedWindow.ContextInt2);


        if (!Hide)
        {
            GUI.Label(new Rect(5, 60, 120, 15), "Char ID");
            if (GUI.Button(new Rect(12, 95, 40, 15), "Goto"))
                Selection.activeGameObject = QuickFindInEditor.GetEditorCharacterDatabase().GetItemFromID(CurrentViewedWindow.ContextInt).gameObject;
            CurrentViewedWindow.ContextInt = EditorGUI.IntField(new Rect(5, 75, 55, 18), CurrentViewedWindow.ContextInt);

            GUI.Label(new Rect(75, 60, 75, 15), "Cam ID");
            if (GUI.Button(new Rect(80, 95, 40, 15), "Goto"))
                Debug.Log("Camera Not Implemented Yet");
            CurrentViewedWindow.ContextInt3 = EditorGUI.IntField(new Rect(65, 75, 75, 18), CurrentViewedWindow.ContextInt3);
        }


        //DisplayText
        DG_WordDatabase WDB = QuickFindInEditor.GetEditorWordDatabase();
        DG_WordObject WO = WDB.GetItemFromID(CurrentViewedWindow.ContextInt2);
        UserSettings US = QuickFindInEditor.GetEditorUserSettings();
        GUIStyle DisplayStyle = new GUIStyle();
        DisplayStyle.wordWrap = true;
        DisplayStyle.clipping = TextClipping.Clip;
        if (WO != null)
        {
            GUI.Label(new Rect(5, 190, 138, 100), NA_DialogueGUIController.ScanForContext(WO.TextValues[(int)US.CurrentLanguage].stringEntry, true), DisplayStyle);
            WO.TextValues[0].stringEntry = GUI.TextArea(new Rect(5, 240, 138, 60), WO.TextValues[0].stringEntry);
            NodeViewOptions.GetOptions().ExtraLanguage = (UserSettings.Languages)EditorGUI.EnumPopup(new Rect(5, 305, 138, 60), NodeViewOptions.GetOptions().ExtraLanguage);
            WO.GetTextEntryByLanguage(NodeViewOptions.GetOptions().ExtraLanguage).stringEntry = GUI.TextArea(new Rect(5, 325, 138, 60), WO.GetTextEntryByLanguage(NodeViewOptions.GetOptions().ExtraLanguage).stringEntry);
        }
    }
}


#endif
