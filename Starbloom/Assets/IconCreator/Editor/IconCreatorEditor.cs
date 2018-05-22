using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(IconCreator))]
public class IconCreatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Repaint();

        var iconCreator = (IconCreator) target;

        GUILayout.Space(10);

        EditorGUILayout.LabelField("Model preview index", EditorStyles.boldLabel);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("<", GUILayout.Width(100)))
        {
            iconCreator.CurrentIndex--;
            iconCreator.ChangePreviewModel();
        }
        GUILayout.FlexibleSpace();
        GUILayout.Label(iconCreator.IconValues.ModelPivots[iconCreator.CurrentIndex]._models.name);
        GUILayout.FlexibleSpace();
        if (GUILayout.Button(">", GUILayout.Width(100)))
        {
            iconCreator.CurrentIndex++;
            iconCreator.ChangePreviewModel();
        }
        GUILayout.EndHorizontal();

        DrawDefaultInspector();

        GUILayout.Space(20);
        GUI.color = Color.green;

        bool AllowAll = false;
        bool AllowOne = false;

        if (GUILayout.Button("Create icons"))
            AllowAll = true;
        else if (GUILayout.Button("Create Index icon"))
            AllowOne = true;
        else
            return;

        var assembly = typeof(EditorWindow).Assembly;
        var type = assembly.GetType("UnityEditor.GameView");
        var gameView = EditorWindow.GetWindow(type);
        gameView.Focus();

        if (AllowAll)
            CreateAllIcons();
        if (AllowOne)
            CreateSingleIcon();
    }

    void CreateSingleIcon()
    {
        var iconCreator = (IconCreator)target;
        iconCreator.CreateIcons(true);
    }
    void CreateAllIcons()
    {
        var iconCreator = (IconCreator)target;
        iconCreator.CreateIcons(false);
    }
}