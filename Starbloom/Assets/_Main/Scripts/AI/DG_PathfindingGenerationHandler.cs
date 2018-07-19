using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;

public class DG_PathfindingGenerationHandler : MonoBehaviour
{

    int LoadingScene;
    int LoadingPlayer;
    bool IgnoreTheUnloadScene;
    Scene UnloadedScene;
    [System.NonSerialized] public bool NavMeshIsGenerated = false;
    [System.NonSerialized] public NavMeshSurface myNavMeshSurface;



    private void Awake()
    {
        QuickFind.PathfindingGeneration = this;
        myNavMeshSurface = transform.GetComponent<NavMeshSurface>();
        this.enabled = false;
    }

    private void Update()
    {
        int scenecount = SceneManager.sceneCount;
        bool GenerateMesh = true;

        for (int i = 0; i < scenecount; i++)
        {
            Scene SC = SceneManager.GetSceneAt(i);
            if (!IgnoreTheUnloadScene && SC == UnloadedScene) { GenerateMesh = false; break; }
        }
        if (GenerateMesh) { this.enabled = false; GenerateNavMesh(LoadingScene, LoadingPlayer); }
    }



    public void SceneChange(Scene UnloadingScene, int Scene, int PlayerID, bool BypassWait, bool UnloadScene)
    {
        LoadingScene = Scene;
        LoadingPlayer = PlayerID;
        IgnoreTheUnloadScene = !UnloadScene;
        UnloadedScene = UnloadingScene;
        if(BypassWait)
            GenerateNavMesh(LoadingScene, LoadingPlayer);
        else
            this.enabled = true;
    }

    [Button(ButtonSizes.Small)]
    public void GenerateMeshDebug()
    {
        myNavMeshSurface.BuildNavMesh();
    }

    public void GenerateNavMesh(int SceneID, int PlayerID)
    {
        NavMeshIsGenerated = true;
        QuickFind.SceneTransitionHandler.SetStageBeforeNavMeshLoad(SceneID);
        myNavMeshSurface.BuildNavMesh();
        QuickFind.SceneTransitionHandler.NavMeshLoaded(SceneID, PlayerID);   
    }
}
