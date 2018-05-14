using System.Collections;
using System.Collections.Generic;
using UnityEngine;



#if UNITY_EDITOR
using UnityEditor;
/////////////////////////////////////////////////////////////////////////////////Editor Extension Buttons
[CustomEditor(typeof(NodeLink))]
class NodeLinkEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        //Buttons

        NodeLink myScript = (NodeLink)target;
        if (GUILayout.Button("FindNextAvailableDatabaseID"))
            myScript.FindNextAvailableDatabaseID();
    }
}
//////////////////////////////////////////////////////////////////////////////////
#endif







public class NodeLink : MonoBehaviour
{

    public enum WindowTypes { Text, Choice, ChoiceAnswer, Reward }
    public enum NodeType { Start, Default}
    public enum NodeWindowtype { Dialogue, BattleBehaviour }

    //Window class, holds all the info a Window needs
    [System.Serializable]
    public class Window
    {


#if UNITY_EDITOR
        public Rect Size;
        public Window(int id, int parent, Rect newSize, WindowTypes type = WindowTypes.Text, NodeType nodeType = NodeType.Default) { ID = id; Parent = parent; Type = type; NodeType = nodeType; Size = newSize; }
        public int Parent;
        public bool IsChoice() { if (Type == WindowTypes.Choice) return true; else return false; }
#endif


        //Window Data
        public int ID;
        public List<int> Connections = new List<int>();
        public WindowTypes Type;
        public NodeType NodeType;

        //GUI Data
        public bool ContextBool = false;
        public bool ContextBool2 = false;
        public bool ContextBool3 = false;
        public int ContextInt = 0;
        public int ContextInt2 = 0;
        public int ContextInt3 = 0;
        public int ContextInt4 = 0;
        //
    }



    public int DatabaseID;
    public NodeWindowtype NodeWindowType;

    [HideInInspector] public int CurrentId;
    [HideInInspector] public int FirstWindow = -562;
    [HideInInspector] public List<Window> Windows = new List<Window>();
    [HideInInspector] public Window Current;
    [HideInInspector] public bool EndTree;








    public Window GetWindow(int ID)
    {
        for (int i = 0; i < Windows.Count; i++)
        {
            if (Windows[i].ID == ID)
                return Windows[i];
        }
        return null;
    }

    public Window[] GetWindowsByWindow(Window NextWindow)
    {
        if (NextWindow.Type != WindowTypes.Choice)
            return new Window[] { };
        else
        {
            List<Window> Choices = new List<Window>();
            for (int i = 0; i < NextWindow.Connections.Count; i++)
                Choices.Add(GetWindow(NextWindow.Connections[i]));
            return Choices.ToArray();
        }
    }
    public Window[] GetAllWindowConnections(Window NextWindow)
    {
        List<Window> Choices = new List<Window>();
        for (int i = 0; i < NextWindow.Connections.Count; i++)
            Choices.Add(GetWindow(NextWindow.Connections[i]));
        return Choices.ToArray();
    }
    public int Next()
    {
        EndTree = false;

        if (Current.Type == WindowTypes.Choice)
            return Current.Connections.Count;
        else if (Current.Connections.Count == 0)
        {
            EndTree = true;
            return -1;
        }
        else
        {
            Current = GetWindow(Current.Connections[0]);
            return 0;
        }
    }
    public void SetChoiceWindow(Window ChoiceWindow)
    {
        Current = ChoiceWindow;
    }
    public bool NextChoice(int choice)
    {
        for (int i = 0; i < Current.Connections.Count; i++)
        {
            if (GetWindow(Current.Connections[i]).ContextInt == choice)
            {
                Current = GetWindow(GetWindow(Current.Connections[i]).Connections[0]);
                return true;
            }
        }
        return false;
    }
    public Window GetNextChoice()
    {
        if (Current.Connections.Count < 1)
            return null;

        return GetWindow(Current.Connections[0]);
    }
    /// Set the current node back to the beginning
    public int Reset()
    {
        if (Windows.Count == 0)
            return -1;

        Current = Windows[FirstWindow];
        if (Current == null)
            return -1;
        return Current.ContextInt;
    }

    /// Returns if you're at the end of the dialogue tree
    public bool End()
    {
        if (Current.Connections.Count == 0)
            return true;
        else
            return false;
    }






    public void FindNextAvailableDatabaseID()
    {
        Transform Cat = transform.parent;
        Transform Tracker = Cat.parent;

        int HighestNumber = 0;

        for (int i = 0; i < Tracker.childCount; i++)
        {
            Transform Child = Tracker.GetChild(i);
            for (int iN = 0; iN < Child.childCount; iN++)
            {
                NodeLink DialogueItem = Child.GetChild(iN).GetComponent<NodeLink>();
                if (DialogueItem.DatabaseID != 0)
                {
                    Debug.Log("This Object Already Has a Database ID");
                    return;
                }
                if (DialogueItem.DatabaseID > HighestNumber)
                    HighestNumber = DialogueItem.DatabaseID;
            }
        }
        DatabaseID = HighestNumber + 1;
        transform.gameObject.name = DatabaseID.ToString() + " - ";
    }
}


