using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;

public class NodeViewer : EditorWindow
{

#region Variables
    //Saved Textures
    static Texture2D BackgroundTexture;
    static Texture2D PanelTexture;

    Dictionary<int, object[]> Ids = new Dictionary<int, object[]>();
    float Timer = 0;
    //Used for scrolling in the main view
    Vector2 ScrollPosition = Vector2.zero;
    Vector2 PreviousPosition = Vector2.zero;
    Rect NewTree = new Rect(10, 10, 500, 200);
    float ScrollviewOffset = 150f;
    bool Pressed = false;
    bool Drag = false;
    bool OverSomething = false;
    bool Connecting = false;
    string BoxName = "";
    //Sizes
    Vector2 WindowSize = new Vector2(100, 55);
    Vector2 WorkSize = new Vector2(3000, 2000);

    NodeLink CurrentNode;
    NodeLink.Window ConnectingCurrent = null;
    NodeLink.Window CurrentViewedWindow = null;
#endregion

    [MenuItem("Window/Node Viewer")]
    public static void Init()
    {
        EditorWindow.GetWindow(typeof(NodeViewer));
    }
    private void OnGUI()
    {
        if (Selection.activeTransform != null)
        {
            NodeLink.NodeWindowtype CurType = NodeLink.NodeWindowtype.Dialogue;
            if (CurrentNode != null)
                CurType = CurrentNode.NodeWindowType;

            if (Selection.activeTransform.GetComponent<NodeLink>() != null)
                CurrentNode = Selection.activeTransform.GetComponent<NodeLink>();

            if (CurrentNode == null)
                return;

            if (CurrentNode.NodeWindowType != CurType)
                MakeTextures(CurrentNode);
        }
        if (CurrentNode == null)
            return;


        if (Event.current.type == EventType.MouseDown && Event.current.button != 2)
            Pressed = true;
        if(Event.current.type == EventType.MouseDrag)
        {
            Pressed = false;
            Drag = true;
        }
        if (Event.current.type == EventType.MouseUp)
        {
            Pressed = false;
            Drag = false;
            OverSomething = false;
        }




        CheckConnections();

        if (PanelTexture == null)
            MakeTextures(CurrentNode);
        GUI.DrawTexture(new Rect(0, 0, 145, WorkSize.y), PanelTexture, ScaleMode.StretchToFill);

        //Left Window
        if(CurrentNode.NodeWindowType == NodeLink.NodeWindowtype.Dialogue)
            NodeViewer_Dialogue.DrawLeftWindow(CurrentNode, CurrentViewedWindow);
        if (CurrentNode.NodeWindowType == NodeLink.NodeWindowtype.BattleBehaviour)
            NodeViewer_BattleAI.DrawLeftWindow(CurrentNode, CurrentViewedWindow);


        //Creates large scroll view for the work area
        ScrollPosition = GUI.BeginScrollView(new Rect(ScrollviewOffset, 0, position.width, position.height), ScrollPosition, new Rect(Vector2.zero, WorkSize), GUIStyle.none, GUIStyle.none);
        //Makes the background a dark gray
        GUI.DrawTexture(new Rect(0, 0, WorkSize.x, WorkSize.y), BackgroundTexture, ScaleMode.StretchToFill);


        Handles.BeginGUI();
        DrawConnections(Color.white);
        Handles.EndGUI();

        BeginWindows();
        ClearIds();
        BuildWindows();
        EndWindows();

        //Node Specifics
        if(CurrentNode.NodeWindowType == NodeLink.NodeWindowtype.Dialogue)
            NodeViewer_Dialogue.DrawPreview(CurrentNode);
        if (CurrentNode.NodeWindowType == NodeLink.NodeWindowtype.BattleBehaviour)
            NodeViewer_BattleAI.DrawPreview(CurrentNode);

        GUI.EndScrollView();

        if (CurrentNode.NodeWindowType == NodeLink.NodeWindowtype.Dialogue)
            GUI.Label(new Rect(160, 0, 220, 15), "Dialogue Node Editor");
        if (CurrentNode.NodeWindowType == NodeLink.NodeWindowtype.BattleBehaviour)
            GUI.Label(new Rect(160, 0, 220, 15), "Battle AI Node Editor");


        if (new Rect(0, 0, position.width, position.height).Contains(Event.current.mousePosition))
        {
            if (Event.current.button == 1)
            {
                GenericMenu Menu = new GenericMenu();

                RightClickMenus(Menu);
                if (CheckDialogueExists())
                    Menu.AddItem(new GUIContent("Save"), false, Save);
                Menu.ShowAsContext();
            }

            if (Event.current.button == 2 && Event.current.type == EventType.MouseDrag)
            {
                Vector2 CurrentPos = Event.current.mousePosition;  

                if (Vector2.Distance(CurrentPos, PreviousPosition) < 50)
                {
                    ScrollPosition += NewDragPos(CurrentPos, PreviousPosition);
                    Event.current.Use();
                }
                PreviousPosition = CurrentPos;
            }
        }
        if (Drag && OverSomething)
            DragMain(CurrentViewedWindow, !Event.current.control);

    }
    void Update()
    {
        //If making a connection, constantly repaint so the line draws to the mouse pointer
        if (Connecting || Drag)
            Repaint();
        //Calls repaint more frequently for updating when a Dialogues component is added, etc.
        Timer += 0.01f;
        if (Timer > 1)
        {
            Timer = 0;
            Repaint();
        }
    }


    #region Helper Functions

    public static void MakeTextures(NodeLink CurrentNode)
    {
        BackgroundTexture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        BackgroundTexture.SetPixel(0, 0, NodeViewOptions.GetColorOption(CurrentNode).BackGroundColor);
        BackgroundTexture.Apply();

        PanelTexture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        PanelTexture.SetPixel(0, 0, NodeViewOptions.GetColorOption(CurrentNode).PanelColor);
        PanelTexture.Apply();
    }
    NodeLink.Window CreateNewWindow(Vector2 Position, int ParentId, NodeLink.WindowTypes Type = NodeLink.WindowTypes.Text)
    {
        NodeLink.Window NewWindow = new NodeLink.Window(CurrentNode.CurrentId, ParentId, new Rect(Position, WindowSize), Type);
        CurrentNode.CurrentId++;
        return NewWindow;
    }
    public static NodeLink.Window FindPreviousWindow(NodeLink.Window winToFind, NodeLink CurrentNode)
    {
        //If this is the first window, there is no previous
        if (CurrentNode.FirstWindow == winToFind.ID)
            return null;
        //Checks all the connections
        for (int i = 0; i < CurrentNode.Windows.Count; i++)
        {
            //If any of the connections is equal to the one we're trying to find, we return this Window
            for (int j = 0; j < CurrentNode.Windows[i].Connections.Count; j++)
            {
                NodeLink.Window Curr = CurrentNode.GetWindow(CurrentNode.Windows[i].Connections[j]);
                if (Curr == winToFind)
                    return CurrentNode.Windows[i];
            }
        }
        return null;
    }
    public static List<NodeLink.Window> FindPreviousWindows(NodeLink.Window winToFind, NodeLink CurrentNode)
    {
        if (CurrentNode.FirstWindow == winToFind.ID)
            return null;
        List<NodeLink.Window> TheList = new List<NodeLink.Window>();

        //Checks all the connections
        for (int i = 0; i < CurrentNode.Windows.Count; i++)
        {
            //If any of the connections is equal to the one we're trying to find, we return this Window
            for (int j = 0; j < CurrentNode.Windows[i].Connections.Count; j++)
            {
                NodeLink.Window Curr = CurrentNode.GetWindow(CurrentNode.Windows[i].Connections[j]);
                if (Curr == winToFind)
                    TheList.Add(CurrentNode.Windows[i]);
            }
        }
        return TheList;
    }
    bool Find(NodeLink.Window win, NodeLink.Window winToFind)
    {
        for (int i = 0; i < CurrentNode.Windows.Count; i++)
        {
            if (CurrentNode.Windows[i] == winToFind)
                return true;
        }
        return false;

    }
    bool CheckDialogueExists()
    {
        if (CurrentNode.FirstWindow == -562 || CurrentNode.Windows[CurrentNode.FirstWindow].Connections == null)
            return false;
        else
            return true;
    }
    void ClearIds()
    {
        Ids.Clear();
    }
    void Save()
    {
        for (int i = 0; i < UnityEditor.SceneManagement.EditorSceneManager.sceneCount; i++)
            UnityEditor.SceneManagement.EditorSceneManager.SaveScene(UnityEditor.SceneManagement.EditorSceneManager.GetSceneAt(i));
    }
    void DragMain(NodeLink.Window Window, bool Recursive)
    {
        Vector2 OldPos;
        OldPos.x = Window.Size.x;
        OldPos.y = Window.Size.y;

        Vector2 CurPos = Event.current.mousePosition;
        CurPos.x -= ScrollviewOffset;
        CurPos.x += ScrollPosition.x;
        CurPos.y += ScrollPosition.y;
        Window.Size.x = CurPos.x;
        Window.Size.y = CurPos.y;

        Vector2 Diff = NewDragPos(CurPos, OldPos);
        if (Recursive)
            DragLoop(Window, Diff);
    }
    void DragLoop(NodeLink.Window Window, Vector2 Diff)
    {
        NodeLink.Window[] WindowsArray = CurrentNode.GetAllWindowConnections(Window);

        for (int i = 0; i < WindowsArray.Length; i++)
        {
            NodeLink.Window Wind = WindowsArray[i];
            Wind.Size.x -= Diff.x;
            Wind.Size.y -= Diff.y;
            DragLoop(Wind, Diff);
        }
    }
    private Vector2 RotateVector2(Vector2 aPoint, Vector2 aPivot, float aDegree)
    {
        Vector3 pivot = aPivot;
        return pivot + Quaternion.Euler(0, 0, aDegree) * (aPoint - aPivot);
    }
    private float AngleBetweenVector2(Vector2 a, Vector2 b)
    {
        Vector2 difference = b - a;
        float sign = (b.y < a.y) ? -1f : 1f;
        return Vector2.Angle(Vector2.right, difference) * sign;
    }
    Vector2 NewDragPos(Vector2 Cur, Vector2 Old)
    {
        Vector2 ReturnVec;
        ReturnVec.x = Old.x - Cur.x;
        ReturnVec.y = Old.y - Cur.y;

        return ReturnVec;
    }

    #endregion

    #region Window Functions

    void AddWindowWrapper(object data)
    {
        AddWindow(data);
    }
    NodeLink.Window AddWindow(object data)
    {
        Undo.RecordObject(CurrentNode, "Dialogue");

        object[] Data = (object[])data;
        //Retrieving Data
        Vector2 Position = (Vector2)Data[0];
        NodeLink.Window WindowClickedOn = (NodeLink.Window)Data[1];
        NodeLink.WindowTypes Type = (NodeLink.WindowTypes)Data[2];

        int ParentId = -1;
        if (WindowClickedOn != null)
            ParentId = WindowClickedOn.ID;
        //Creates the new window
        NodeLink.Window NewlyCreatedWindow = CreateNewWindow(Position, ParentId, Type);

        //Checks if this is the first node
        if (WindowClickedOn == null)
            CurrentNode.FirstWindow = NewlyCreatedWindow.ID;
        else
            WindowClickedOn.Connections.Add(NewlyCreatedWindow.ID);

        CurrentNode.Windows.Add(NewlyCreatedWindow);

        return NewlyCreatedWindow;
    }
    void AddWindowAfter(object win)
    {
        Undo.RecordObject(CurrentNode, "Dialogue");

        NodeLink.Window Curr = (NodeLink.Window)win;

        if (Curr.Connections.Count > 1)
        {
            if (Curr.ContextBool2)
                return;
        }

        NodeLink.Window NewlyCreatedWindow = CreateNewWindow(Curr.Size.position + new Vector2(8, 0), Curr.ID);

        if (!Curr.ContextBool2 && !Curr.ContextBool || Curr.ContextBool && Curr.Type == NodeLink.WindowTypes.ChoiceAnswer)
        {
            for (int i = 0; i < Curr.Connections.Count; i++)
            {
                NewlyCreatedWindow.Connections.Add(Curr.Connections[i]);
            }
            Curr.Connections.Clear();
        }

        Curr.Connections.Add(NewlyCreatedWindow.ID);
        CurrentNode.Windows.Add(NewlyCreatedWindow);
    }
    void RemoveWindow(object win)
    {
        NodeLink.Window Curr = (NodeLink.Window)win;
        ClearIds();

        //If the window we're removing is the start window, we have a custom check
        if (Curr.ID == CurrentNode.FirstWindow)
        {
            //We don't allow the user to remove the first node
            if (Curr.Connections.Count == 0)
                CurrentNode.FirstWindow = -562;
            return;
        }

        NodeLink.Window PrevWindow = FindPreviousWindow(Curr, CurrentNode);
        //If the window to remove has connections
        if (Curr.Connections.Count != 0 && PrevWindow != null)
        {
            //We go through it's connections, and add them to the previous window
            for (int i = 0; i < Curr.Connections.Count; i++)
            {
                PrevWindow.Connections.Add(Curr.Connections[i]);
            }
        }
        if (PrevWindow != null)
        {
            if (Curr.Connections.Count > 1)
            {
                if(Curr.ContextBool)
                    PrevWindow.Type = NodeLink.WindowTypes.Choice;
                if(Curr.ContextBool2)
                    PrevWindow.Type = NodeLink.WindowTypes.ChoiceAnswer;
            }
            //Removes the window from existence
            PrevWindow.Connections.Remove(Curr.ID);
            Curr.Parent = -2;
            for (int i = 0; i < Curr.Connections.Count; i++)
            {
                CurrentNode.GetWindow(Curr.Connections[i]).Parent = -2;
            }
        }
        CurrentNode.Windows.Remove(Curr);
        ClearIds();
    }
    void RemoveWindowTree(object win)
    {
        NodeLink.Window Curr = (NodeLink.Window)win;
        //If this is the first node, removes everything
        if (Curr.ID == CurrentNode.FirstWindow)
        {
            if (Curr.Connections.Count == 0)
                CurrentNode.FirstWindow = -562;
            return;
        }
        //Simply removes the node, and lets everything connected die
        NodeLink.Window PrevWindow = FindPreviousWindow(Curr, CurrentNode);
        Curr.Parent = -2;
        PrevWindow.Connections.Remove(Curr.ID);
    }

    void DrawConnections(Color LineColor)
    {
        if (!CheckDialogueExists()) return;
        //Goes through the window's connections
        for (int j = 0; j < CurrentNode.Windows.Count; j++)
        {
            for (int i = 0; i < CurrentNode.Windows[j].Connections.Count; i++)
            {
                NodeLink.Window WindowList = CurrentNode.Windows[j];

                Color Use = LineColor;

                //Draws a line with the correct color between the current window and connection
                Handles.DrawBezier(WindowList.Size.center, CurrentNode.GetWindow(WindowList.Connections[i]).Size.center, new Vector2(WindowList.Size.center.x, WindowList.Size.center.y), new Vector2(CurrentNode.GetWindow(WindowList.Connections[i]).Size.center.x, CurrentNode.GetWindow(WindowList.Connections[i]).Size.center.y), Use, null, 2f);

                Use = LineColor;

                //Finds the center between the points
                float xPos = (WindowList.Size.center.x) + ((CurrentNode.GetWindow(WindowList.Connections[i]).Size.center.x - WindowList.Size.center.x) / 2);
                float yPos = (WindowList.Size.center.y) + ((CurrentNode.GetWindow(WindowList.Connections[i]).Size.center.y - WindowList.Size.center.y) / 2);
                Vector2 Middle = new Vector2(xPos, yPos);
            }
        }

        if (Connecting)
        {
            Color ManualColor = Color.magenta;
            Handles.DrawBezier(ConnectingCurrent.Size.center, Event.current.mousePosition, new Vector2(ConnectingCurrent.Size.xMax + 50f, ConnectingCurrent.Size.center.y), new Vector2(Event.current.mousePosition.x, Event.current.mousePosition.y), ManualColor, null, 5f);
        }
    }
    void CheckConnections()
    {
        for (int i = 0; i < CurrentNode.Windows.Count; i++)
        {
            List<NodeLink.Window> WindowList = FindPreviousWindows(CurrentNode.Windows[i], CurrentNode);
            if ((WindowList == null || WindowList.Count == 0) && CurrentNode.Windows[i].NodeType != NodeLink.NodeType.Start && CurrentNode.Windows.Count > 1)
                RemoveWindow(CurrentNode.Windows[i]);
        }
    }
    void StartConnection(object win)
    {
        Connecting = true;
        ConnectingCurrent = (NodeLink.Window)win;
    }
    void CreateConnection(object win)
    {
        NodeLink.Window Curr = (NodeLink.Window)win;

        if (Find(Curr, ConnectingCurrent) || ConnectingCurrent.Connections.Contains(Curr.ID))
        {
            if (!ConnectingCurrent.Connections.Contains(Curr.ID))// && ConnectingCurrent.Type == WindowTypes.Choice)
            {
                ConnectingCurrent.Connections.Add(Curr.ID);
            }
            Connecting = false;

            if (Curr.Connections.Count > 1)
                Curr.Type = NodeLink.WindowTypes.Choice;

            return;
        }

        for (int i = 0; i < ConnectingCurrent.Connections.Count; i++)
        {
            if (ConnectingCurrent.Connections[i] == Curr.ID)
            {
                Connecting = false;
                return;
            }
        }


        ConnectingCurrent.Connections.Add(Curr.ID);
        Connecting = false;
    }
    void EstablishNewWindowConnection(object data)
    {
        object[] Data = (object[])data;
        Vector2 Position = (Vector2)Data[0];
        NodeLink.WindowTypes Type = (NodeLink.WindowTypes)Data[1];

        object[] Vals = { Position, ConnectingCurrent, Type };
        CreateConnection(AddWindow(Vals));
    }
    void CancelConnection()
    {
        Connecting = false;
    }
    void Convert(object win)
    {
        NodeLink.Window Curr = (NodeLink.Window)win;
        if (Curr.Type == NodeLink.WindowTypes.Choice)
            Curr.Type = NodeLink.WindowTypes.Text;
        else
            Curr.Type = NodeLink.WindowTypes.Choice;
    }
    void CheckPosition(NodeLink.Window win)
    {
        Vector2 newPos = win.Size.position;
        if (win.Size.center.x < 0)
            win.Size.position = new Vector2(0, newPos.y);
        if (win.Size.center.x > WorkSize.x)
            win.Size.position = new Vector2(WorkSize.x - (win.Size.width), newPos.y);
        if (win.Size.center.y < 0)
            win.Size.position = new Vector2(newPos.x, 0);
        if (win.Size.center.y > WorkSize.y)
            win.Size.position = new Vector2(newPos.x, WorkSize.y - (win.Size.height));
    }

    void RightClickMenus(GenericMenu Menu)
    {
        Vector2 AdjustedMousePosition = Event.current.mousePosition + ScrollPosition - new Vector2(50, 50);

        if (!CheckDialogueExists())
        {
            object[] Vals = { AdjustedMousePosition, null, NodeLink.WindowTypes.Text };
            Menu.AddItem(new GUIContent("Create First Window"), false, AddWindowWrapper, Vals);

            return;
        }

        for (int j = 0; j < CurrentNode.Windows.Count; j++)
        {
            NodeLink.Window WindowList = CurrentNode.Windows[j];

            //Accounts for the area when the user has scrolled
            Rect AdjustedArea = new Rect(new Vector2(WindowList.Size.position.x + ScrollviewOffset, WindowList.Size.position.y) - ScrollPosition, WindowList.Size.size);

            if (Connecting)
            {
                object[] CreateInfoDialogue = { AdjustedMousePosition, NodeLink.WindowTypes.Text };
                object[] CreateInfoChoice = { AdjustedMousePosition, NodeLink.WindowTypes.Choice };
                if (ConnectingCurrent.Type == NodeLink.WindowTypes.Text && ConnectingCurrent.Connections.Count == 0 || ConnectingCurrent.Type == NodeLink.WindowTypes.ChoiceAnswer && ConnectingCurrent.Connections.Count == 0 || ConnectingCurrent.Type == NodeLink.WindowTypes.Choice)
                {
                    Menu.AddItem(new GUIContent("Create Dialogue Window"), false, EstablishNewWindowConnection, CreateInfoDialogue);
                    Menu.AddItem(new GUIContent("Create Decision Window"), false, EstablishNewWindowConnection, CreateInfoChoice);
                }

            }

            //Checks if the mouse is on the current box
            if (AdjustedArea.Contains(Event.current.mousePosition))
            {
                if (Connecting)
                {
                    if (AdjustedArea.Contains(Event.current.mousePosition))
                    {
                        if (ConnectingCurrent.Type == NodeLink.WindowTypes.Text && ConnectingCurrent.Connections.Count == 0 || ConnectingCurrent.Type == NodeLink.WindowTypes.ChoiceAnswer && ConnectingCurrent.Connections.Count == 0 || ConnectingCurrent.Type == NodeLink.WindowTypes.Choice)
                            Menu.AddItem(new GUIContent("Establish Connection"), false, CreateConnection, WindowList);
                    }
                }
                else
                {
                    if (WindowList.Type == NodeLink.WindowTypes.Text && WindowList.Connections.Count == 0 || WindowList.Type == NodeLink.WindowTypes.ChoiceAnswer && WindowList.Connections.Count == 0 || WindowList.Type == NodeLink.WindowTypes.Choice)
                        Menu.AddItem(new GUIContent("Create Connection"), false, StartConnection, WindowList);

                    NodeLink.Window Previous = FindPreviousWindow(WindowList, CurrentNode);
                    if (Previous != null && FindPreviousWindow(Previous, CurrentNode) != null)
                        Menu.AddItem(new GUIContent("Remove Window"), false, RemoveWindow, WindowList);
                    //Menu.AddItem(new GUIContent("Remove Window Tree"), false, RemoveWindowTree, WindowList);
                }

            }
        }

        if (Connecting)
            Menu.AddItem(new GUIContent("Cancel"), false, CancelConnection);
    }
    void NodeButtons(int windowID)
    {
        if (!Ids.ContainsKey(windowID)) return;
        NodeLink.Window Win = (NodeLink.Window)Ids[windowID][0];

        

        if (Win.NodeType == NodeLink.NodeType.Start)
        {
            if (GUI.Button(new Rect(2, 16, 30, 15), "+"))
                AddWindowAfter(Win);
        }
        else
        {
            GUI.backgroundColor = Color.magenta;
            if (GUI.Button(new Rect(0, 0,  13, 13), "*"))
                Convert(Win);
            GUI.backgroundColor = Color.white;

            if (GUI.Button(new Rect(2, 35, 30, 14), "+"))
                AddWindowAfter(Win);
        }

        //GUI.DragWindow();
    }

    #endregion


    void BuildWindows()
    {
        if (!CheckDialogueExists()) return;

        for (int j = 0; j < CurrentNode.Windows.Count; j++)
        {
            NodeLink.Window WindowList = CurrentNode.Windows[j];

            List<NodeLink.Window> PrevWindow = FindPreviousWindows(WindowList, CurrentNode);

            if (PrevWindow != null)
            {
                for (int i = 0; i < PrevWindow.Count; i++)
                {
                    if (PrevWindow[i].Type == NodeLink.WindowTypes.Choice)
                    {
                        WindowList.Type = NodeLink.WindowTypes.ChoiceAnswer;
                        break;
                    }
                    if (PrevWindow[i].Type == NodeLink.WindowTypes.ChoiceAnswer && WindowList.Type == NodeLink.WindowTypes.ChoiceAnswer)
                        WindowList.Type = NodeLink.WindowTypes.Text;
                    if (PrevWindow[i].Type == NodeLink.WindowTypes.Text && WindowList.Type == NodeLink.WindowTypes.ChoiceAnswer)
                        WindowList.Type = NodeLink.WindowTypes.Text;
                }
            }

            //Default naming
            BoxName = "";

            //Determines what type of node it is
            if (WindowList.ID == CurrentNode.FirstWindow)
                WindowList.NodeType = NodeLink.NodeType.Start;
            else
                WindowList.NodeType = NodeLink.NodeType.Default;


            SetNodeText(WindowList);

            //Sets up the ID for the window
            object[] Vals = { WindowList };
            if (!Ids.ContainsKey(WindowList.ID))
                Ids.Add(WindowList.ID, Vals);

            //Set Colors
            GUIStyle FinalStyle = new GUIStyle();
            if (CurrentNode.NodeWindowType == NodeLink.NodeWindowtype.Dialogue)
                FinalStyle = NodeViewer_Dialogue.SetColors(WindowList, CurrentNode);
            if (CurrentNode.NodeWindowType == NodeLink.NodeWindowtype.BattleBehaviour)
                FinalStyle = NodeViewer_BattleAI.SetColors(WindowList, CurrentNode);


            CheckPosition(WindowList);
            WindowList.Size = GUI.Window(WindowList.ID, WindowList.Size, NodeButtons, BoxName, FinalStyle);
            

            //Size

            WindowList.Size.width = 35;
            if (WindowList.NodeType != NodeLink.NodeType.Default)
                WindowList.Size.height = 30;
            else
                WindowList.Size.height = 50;


            if(WindowList.Size.Contains(Event.current.mousePosition))
            {
                if (Pressed)
                {
                    NodeLink.Window Prev = CurrentViewedWindow;
                    CurrentViewedWindow = WindowList;
                    OverSomething = true;
                    if (Prev != CurrentViewedWindow)
                        GUI.FocusControl("Null");
                }
            }      
        }
    }

    void SetNodeText(NodeLink.Window WindowList)
    {
        if (WindowList.NodeType == NodeLink.NodeType.Default)
        {
            if (CurrentNode.NodeWindowType == NodeLink.NodeWindowtype.Dialogue)
            {
                switch (WindowList.Type)
                {
                    case NodeLink.WindowTypes.Text: BoxName = "Text"; break;
                    case NodeLink.WindowTypes.Choice: BoxName = ""; break;
                    case NodeLink.WindowTypes.ChoiceAnswer: BoxName = "Path"; break;
                }
            }
            if (CurrentNode.NodeWindowType == NodeLink.NodeWindowtype.BattleBehaviour)
            {
                switch (WindowList.Type)
                {
                    case NodeLink.WindowTypes.Text: BoxName = "Act"; break;
                    case NodeLink.WindowTypes.Choice: BoxName = "Check"; break;
                    case NodeLink.WindowTypes.ChoiceAnswer: BoxName = "Path"; break;
                }
            }
        }
        else
            BoxName = "Start";
    }
}
#endif
