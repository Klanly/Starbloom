using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DG_SceneTransition : MonoBehaviour {


    Scene SceneUnloading;
    int NetworkSceneIndexUnloading;

    string SceneLoading;
    int NetworkSceneIndexLoading;
    int LoadingPortalID;

    DG_SceneEntryPoints LoadingSceneEntryPoints;


    bool Transitioning = false;

    private void Awake()
    {
        QuickFind.SceneTransitionHandler = this;
    }


    public void TriggerSceneChange(string SceneString, int PortalValue)
    {
        if (Transitioning) return;

        Transitioning = true;
        SceneUnloading = SceneManager.GetActiveScene();
        NetworkSceneIndexUnloading = QuickFind.SceneList.GetSceneIndexByString(SceneUnloading.name);

        SceneLoading = SceneString;
        NetworkSceneIndexLoading = QuickFind.SceneList.GetSceneIndexByString(SceneString);
        LoadingPortalID = PortalValue;

        //Unload Network Objects
        NetworkScene NS = QuickFind.NetworkObjectManager.GetSceneByID(QuickFind.NetworkSync.CurrentScene);
        for (int i = 0; i < NS.NetworkObjectList.Count; i++)
            NS.NetworkObjectList[i].gameObject.SetActive(false);

        QuickFind.FadeScreen.FadeOut(DG_GUI_FadeScreen.FadeInSpeeds.QuickFade, this.gameObject, "FadeEnded");
    }

    public void FadeEnded()
    {
        SceneManager.LoadScene(SceneLoading, LoadSceneMode.Additive);
        SceneManager.UnloadSceneAsync(SceneUnloading);
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
        Scene LoadScene = SceneManager.GetSceneByName(SceneLoading);
        SceneManager.SetActiveScene(LoadScene);

        //Load Network Objects;
        QuickFind.NetworkSync.SetSelfInScene(NetworkSceneIndexLoading);
        NetworkScene NS = QuickFind.NetworkObjectManager.GetSceneByID(QuickFind.NetworkSync.CurrentScene);
        for (int i = 0; i < NS.NetworkObjectList.Count; i++)
            NS.NetworkObjectList[i].gameObject.SetActive(true);


        //Set Player, and Cam.
        GameObject[] goArray = LoadScene.GetRootGameObjects();
        for (int i = 0; i < goArray.Length; i++)
        { if (goArray[i].name == "Scene Transition Entry Points") { LoadingSceneEntryPoints = goArray[i].GetComponent<DG_SceneEntryPoints>(); break; } }

        DG_SceneEntryPoints.PortalPoint PP = LoadingSceneEntryPoints.GetStartingPositionByID(LoadingPortalID);
        QuickFind.PlayerTrans.position = PP.PortalTransformReference.position;
        QuickFind.PlayerTrans.eulerAngles = PP.PortalTransformReference.eulerAngles;
        QuickFind.PlayerCam.InstantSetCameraAngle(PP.PortalTransformReference.GetChild(0).eulerAngles);


        QuickFind.FadeScreen.FadeIn(DG_GUI_FadeScreen.FadeInSpeeds.QuickFade, this.gameObject, "FadeInEnded");
    }

    public void FadeInEnded()
    {
        Transitioning = false;
    }
}
