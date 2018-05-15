#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;


public class SafePlayButton : EditorWindow
{
    [MenuItem("Window/SafePlayButton")]
    public static void Init() { GetWindow(typeof(SafePlayButton)); }

    GameObject LastKnown;
    bool Set = false;

    private void Awake()
    {
        this.minSize = new Vector2(3, 3);
    }
    private void OnGUI()
    {
        GUI.backgroundColor = Color.green;
        if(GUI.Button(new Rect(3, 3, position.width - 6, position.height - 6), "Safe Play Button"))
            Detection();

        if (Application.isPlaying && Set)
        {
            Selection.activeGameObject = LastKnown;
            Set = false;
        }
    }
    public void Detection()
    {
        if (!EditorApplication.isPlaying)
        {
            Set = true;
            LastKnown = Selection.activeGameObject;
            Selection.activeGameObject = null;
            EditorApplication.isPlaying = true;
        }
        else
            EditorApplication.isPlaying = false;
    }
}

#endif