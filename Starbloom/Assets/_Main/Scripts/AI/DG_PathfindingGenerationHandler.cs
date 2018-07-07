using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;

public class DG_PathfindingGenerationHandler : MonoBehaviour
{
    

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
            if (SC == UnloadedScene) { GenerateMesh = false; break; }
        }
        if (GenerateMesh) { GenerateNavMesh(); this.enabled = false; }
    }



    public void SceneChange(Scene UnloadingScene)
    {
        UnloadedScene = UnloadingScene;
        this.enabled = true;
    }
    [Button(ButtonSizes.Small)]
    public void GenerateNavMesh()
    {
        NavMeshIsGenerated = true;
        myNavMeshSurface.BuildNavMesh();
        QuickFind.SceneTransitionHandler.NavMeshLoaded();
    }
}
