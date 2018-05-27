using System.Collections;
using System.Collections.Generic;
using UnityEngine;



#if UNITY_EDITOR
using UnityEditor;
/////////////////////////////////////////////////////////////////////////////////Editor Extension Buttons
[CustomEditor(typeof(DG_TextStatic))]
class DG_TextStaticEditor : Editor
{
    public override void OnInspectorGUI()
    {
        //Buttons
        DG_TextStatic myScript = (DG_TextStatic)target;
        if (GUILayout.Button("GotoTextInEditor"))
        {
            Selection.activeGameObject = QuickFindInEditor.GetEditorWordDatabase().GetItemFromID(myScript.TextWordID).gameObject;
        }

        DrawDefaultInspector();
    }
}
//////////////////////////////////////////////////////////////////////////////////
#endif




public class DG_TextStatic : MonoBehaviour {

    public int TextWordID;



    public void OnEnable()
    {
        Load();
    }
    public void ManualLoad()
    {
        Load();
    }
    void Load()
    {
        if (QuickFind.WordDatabase == null)
            return;

        transform.GetComponent<TMPro.TextMeshProUGUI>().text = NA_DialogueGUIController.GetStaticText(TextWordID);
    }
}
