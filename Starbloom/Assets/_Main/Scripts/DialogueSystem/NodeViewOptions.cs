using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

public class NodeViewOptions : MonoBehaviour {

    [System.Serializable]
    public class ColorOptions
    {
        public Color32 BackGroundColor;
        public Color32 PanelColor;

        public Color StartColor;
        public Color TextColor;
        public Color CheckQuestColor;
        public Color YesPathColor;
        public Color NoPathColor;
        public Color DialogueOptionsColor;
        public Color DialogueNoQuestPath;
        public Color DialogueQuestPath;
        public Color QuestCompleteColor;

        public Color DefaultLine;
        public Color SucessLine;
        public Color FailLine;
    }


    [HideInInspector] public bool OptionsOpen = false;

    public ColorOptions DialogueOptions;
    public ColorOptions BattleOptions;
    public DG_TextLanguageFonts.Languages ExtraLanguage;







    public static void DisplayOptions(NodeLink CurrentNode)
    {
        GUI.Label(new Rect(10, 20, 120, 15), "Background Colors");

        GetColorOption(CurrentNode).BackGroundColor = EditorGUI.ColorField(new Rect(10, 40, 130, 15), GetColorOption(CurrentNode).BackGroundColor);
        GetColorOption(CurrentNode).PanelColor = EditorGUI.ColorField(new Rect(10, 55, 130, 15), GetColorOption(CurrentNode).PanelColor);

        if (GUI.Button(new Rect(10, 70, 110, 15), "Refresh BG"))
            NodeViewer.MakeTextures(CurrentNode);

        GUI.Label(new Rect(10, 100, 120, 15), "Node Colors");

        GetColorOption(CurrentNode).StartColor = EditorGUI.ColorField(new Rect(10, 120, 130, 15), GetColorOption(CurrentNode).StartColor);
        GetColorOption(CurrentNode).TextColor = EditorGUI.ColorField(new Rect(10, 135, 130, 15), GetColorOption(CurrentNode).TextColor);
        GetColorOption(CurrentNode).CheckQuestColor = EditorGUI.ColorField(new Rect(10, 150, 130, 15), GetColorOption(CurrentNode).CheckQuestColor);
        GetColorOption(CurrentNode).YesPathColor = EditorGUI.ColorField(new Rect(10, 165, 130, 15), GetColorOption(CurrentNode).YesPathColor);
        GetColorOption(CurrentNode).NoPathColor = EditorGUI.ColorField(new Rect(10, 180, 130, 15), GetColorOption(CurrentNode).NoPathColor);
        GetColorOption(CurrentNode).DialogueOptionsColor = EditorGUI.ColorField(new Rect(10, 195, 130, 15), GetColorOption(CurrentNode).DialogueOptionsColor);
        GetColorOption(CurrentNode).DialogueNoQuestPath = EditorGUI.ColorField(new Rect(10, 210, 130, 15), GetColorOption(CurrentNode).DialogueNoQuestPath);
        GetColorOption(CurrentNode).DialogueQuestPath = EditorGUI.ColorField(new Rect(10, 225, 130, 15), GetColorOption(CurrentNode).DialogueQuestPath);
        GetColorOption(CurrentNode).QuestCompleteColor = EditorGUI.ColorField(new Rect(10, 240, 130, 15), GetColorOption(CurrentNode).QuestCompleteColor);
    }

    public static ColorOptions GetColorOption(NodeLink CurrentNode)
    {
        switch (CurrentNode.NodeWindowType)
        {
            case NodeLink.NodeWindowtype.BattleBehaviour: return GetOptions().BattleOptions;
            case NodeLink.NodeWindowtype.Dialogue: return GetOptions().DialogueOptions;
        }

        return QuickFindInEditor.GetEditorNodeViewOptions().DialogueOptions;
    }
    static NodeViewOptions NVO = null;
    public static NodeViewOptions GetOptions()
    {
        if (NVO == null)
            NVO = QuickFindInEditor.GetEditorNodeViewOptions();
        return NVO;
    }
}

#endif
