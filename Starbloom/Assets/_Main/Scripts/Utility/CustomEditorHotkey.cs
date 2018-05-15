#if UNITY_EDITOR
using UnityEditor;

public static class CustomEditorHotkey
{
    [MenuItem("Window/CustomEditorHotkey/InputDetection _F5")]
    static void InputDetection()
    {
        Detection();
    }
    [MenuItem("Window/CustomEditorHotkey/InputDetection1 _`")]
    static void InputDetection1()
    {
        if(!EditorGUIUtility.editingTextField)
            Detection();
    }


    static void Detection()
    {
        if (!EditorApplication.isPlaying)
        {
            Selection.activeGameObject = null;
            EditorApplication.isPlaying = true;
        }
        else
            EditorApplication.isPlaying = false;
    }
}

#endif