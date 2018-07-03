﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
using UnityEditor;

public class DG_SceneJumpTool : MonoBehaviour {

    [System.Serializable]
    public class ScenePortals
    {
        public string Name;
        public DG_ScenePortalTrigger Portal;
        [Button(ButtonSizes.Small)] public void TransistionToScene() { Portal.TriggerSceneChange(); }
    }



    [HideInEditorMode]
    public DG_SceneEntryPoints.PortalPoint[] PortalPoints;
    [HideInEditorMode]
    public List<ScenePortals> SceneTransistionList;

    [HideInEditorMode]
    [Button(ButtonSizes.Medium)]
    public void JumpToCompleteSceneList()
    {
        Selection.activeGameObject = QuickFind.SceneList.gameObject;
    }


    private void Awake()
    {
        SceneTransistionList = new List<ScenePortals>();
    }

    public void LoadScenePortals(DG_SceneEntryPoints EntryPointsScript)
    {
        PortalPoints = EntryPointsScript.PortalPoints;

        SceneTransistionList.Clear();

        DG_ScenePortalTrigger[] allObjects = UnityEngine.Object.FindObjectsOfType<DG_ScenePortalTrigger>();
        foreach (DG_ScenePortalTrigger go in allObjects)
        {
            ScenePortals SP = new ScenePortals();
            SP.Portal = go;
            SP.Name = go.SceneString;
            SceneTransistionList.Add(SP);
        }
    }
}

#endif
