using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

public class NodeViewer_BattleAI
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

            }
            else if (WindowList.Type == NodeLink.WindowTypes.Choice)
            {

            }
        }

        return FinalStyle;
    }


    public static void DrawPreview(NodeLink CurrentNode)
    {
        for (int j = 0; j < CurrentNode.Windows.Count; j++)
        {
            NodeLink.Window WindowList = CurrentNode.Windows[j];

            if (WindowList.Type == NodeLink.WindowTypes.Choice)
            {
                if (WindowList.ContextBool2)
                    GUI.Label(new Rect(WindowList.Size.x + 40, WindowList.Size.y, 100, 55), "State Check");
            }
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
            {
                if (CurrentViewedWindow.NodeType == NodeLink.NodeType.Default)
                {
                    GUI.Label(new Rect(35, 25, 120, 15), "Action Node");

                    GUI.Label(new Rect(10, 50, 120, 15), "Action ID");
                    CurrentViewedWindow.ContextInt4 = EditorGUI.IntField(new Rect(5, 65, 132, 18), CurrentViewedWindow.ContextInt);
                }
                else
                {
                    if (CurrentViewedWindow.NodeType == NodeLink.NodeType.Start)
                        GUI.Label(new Rect(40, 20, 120, 15), "Start Node");
                    return;
                }


            }
            //Path Type
            else if (CurrentViewedWindow.Type == NodeLink.WindowTypes.ChoiceAnswer)
            {
                GUI.Label(new Rect(35, 25, 120, 15), "Path Node");
            }
            //Split Type
            else if (CurrentViewedWindow.Type == NodeLink.WindowTypes.Choice)
            {
                if (!CurrentViewedWindow.ContextBool && !CurrentViewedWindow.ContextBool2)
                {
                    CurrentViewedWindow.ContextBool = GUI.Toggle(new Rect(1, 50, 150, 15), CurrentViewedWindow.ContextBool, "Roll Check");
                    CurrentViewedWindow.ContextBool2 = GUI.Toggle(new Rect(1, 65, 150, 15), CurrentViewedWindow.ContextBool2, "State Check");
                }
                if (CurrentViewedWindow.ContextBool)
                    CurrentViewedWindow.ContextBool = GUI.Toggle(new Rect(1, 50, 150, 15), CurrentViewedWindow.ContextBool, "Roll Check");
                if (CurrentViewedWindow.ContextBool2)
                {
                    CurrentViewedWindow.ContextBool2 = GUI.Toggle(new Rect(1, 50, 150, 15), CurrentViewedWindow.ContextBool2, "State Check");
                    GUI.Label(new Rect(1, 70, 77, 15), "Quest ID");
                    CurrentViewedWindow.ContextInt = EditorGUI.IntField(new Rect(3, 85, 130, 15), CurrentViewedWindow.ContextInt);

                }
            }

        }
        else
            NodeViewOptions.DisplayOptions(CurrentNode);
    }
}

#endif
