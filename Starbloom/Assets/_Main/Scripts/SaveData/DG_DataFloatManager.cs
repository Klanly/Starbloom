using System.Collections;
using System.Collections.Generic;
using UnityEngine;



#if UNITY_EDITOR
using UnityEditor;
/////////////////////////////////////////////////////////////////////////////////Editor Extension Buttons
[CustomEditor(typeof(DG_DataFloatManager))]
class DG_DataFloatManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        //Buttons

        DG_DataFloatManager myScript = (DG_DataFloatManager)target;
        if (GUILayout.Button("SaveFloatTracker"))
            myScript.SaveFloatTracker();
        if (GUILayout.Button("LoadFloatTracker"))
            myScript.LoadFloatTracker();
    }
}
//////////////////////////////////////////////////////////////////////////////////
#endif





public class DG_DataFloatManager : MonoBehaviour {

    [System.NonSerialized]
    public DG_DataFloatItem[] FloatList;

    private void Awake()
    {
        QuickFind.DataFloats = this;

        int Counter = 0;
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform Child = transform.GetChild(i);
            for (int iN = 0; iN < Child.childCount; iN++)
                Counter++;
        }

        FloatList = new DG_DataFloatItem[Counter];
        int index = 0;
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform Child = transform.GetChild(i);
            for (int iN = 0; iN < Child.childCount; iN++)
            {
                FloatList[index] = Child.GetChild(iN).GetComponent<DG_DataFloatItem>();
                index++;
            }
        }
    }

    public DG_DataFloatItem GetFloatFromID(int ID)
    {
        DG_DataFloatItem ReturnConversation;
        for (int i = 0; i < FloatList.Length; i++)
        {
            ReturnConversation = FloatList[i];
            if (ReturnConversation.DatabaseID == ID)
                return ReturnConversation;
        }
        Debug.Log("Get By ID Failed");
        return FloatList[0];
    }


    public void SaveFloatTracker()
    {
        float[] FloatArray = new float[FloatList.Length];
        for (int i = 0; i < FloatArray.Length; i++)
            FloatArray[i] = FloatList[i].FloatValue;

        QuickFind.SaveHandler.SaveFloatTracker(FloatArray);
    }
    public void LoadFloatTracker()
    {
        float[] FloatArray = QuickFind.SaveHandler.LoadFloatTracker();
        for (int i = 0; i < FloatArray.Length; i++)
            FloatList[i].FloatValue = FloatArray[i];
    }
}
