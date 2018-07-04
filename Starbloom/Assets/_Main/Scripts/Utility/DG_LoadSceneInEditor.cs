using UnityEngine;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif











[ExecuteInEditMode]
public class DG_LoadSceneInEditor : MonoBehaviour
{
    [System.Serializable]
    public class SceneLink
    {
        [Header("-----------------------------------------------")]
        public bool NoEditorEditLoad = false;
        public bool NoEditorPlayLoad = false;
        public bool NoBuildLoad = false;
        public string SceneName;
        [Multiline(5)]
        public string EditorSceneFolderPath;
    }

    [ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "SceneName")]
    public SceneLink[] AdditiveSceneLinks;




    void Start()
    {
        int scenecount;
        bool loadScene;



        //Editor

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            scenecount = EditorSceneManager.sceneCount;
            loadScene = true;
            for (int ind = 0; ind < AdditiveSceneLinks.Length; ind++)
            {
                SceneLink AdditiveScene = AdditiveSceneLinks[ind];

                if (AdditiveScene.NoEditorEditLoad) continue;

                for (int i = 0; i < scenecount; i++)
                {
                    Scene SC = EditorSceneManager.GetSceneAt(i);
                    if (SC.name == AdditiveScene.SceneName)
                    {
                        loadScene = false;
                        break;
                    }
                }
                if (loadScene)
                    EditorSceneManager.OpenScene(AdditiveScene.EditorSceneFolderPath + AdditiveScene.SceneName + ".unity", OpenSceneMode.Additive);
            }
        }
#endif


        //Build

        if (Application.isPlaying)
        {
            scenecount = SceneManager.sceneCount;
            loadScene = true;
            for (int ind = 0; ind < AdditiveSceneLinks.Length; ind++)
            {
                SceneLink AdditiveScene = AdditiveSceneLinks[ind];

                bool IsEditor = false;

#if UNITY_EDITOR
                if (AdditiveScene.NoEditorPlayLoad && Application.isEditor) { IsEditor = true; continue; }
#endif

                if(!IsEditor && AdditiveScene.NoBuildLoad) continue;

                for (int i = 0; i < scenecount; i++)
                {
                    Scene SC = SceneManager.GetSceneAt(i);
                    if (SC.name == AdditiveScene.SceneName)
                    {
                        loadScene = false;
                        break;
                    }
                }
                if (loadScene)
                    SceneManager.LoadScene(AdditiveScene.SceneName, LoadSceneMode.Additive);
            }
        }
    }
}
