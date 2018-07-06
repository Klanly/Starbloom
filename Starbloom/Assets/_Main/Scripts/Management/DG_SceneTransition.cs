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
        CreateNetworkObjects(false);

        QuickFind.PathfindingGeneration.NavMeshIsGenerated = false;

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

        //Set Player, and Cam.
        DG_SceneEntryObject Portal = QuickFind.SceneEntryPoints.GetItemFromID(LoadingPortalID);
        QuickFind.PlayerTrans.position = Portal.transform.position;
        QuickFind.PlayerTrans.eulerAngles = Portal.transform.eulerAngles;
        QuickFind.PlayerCam.InstantSetCameraAngle(Portal.CameraFacing);

        QuickFind.PathfindingGeneration.SceneChange(SceneUnloading);


        QuickFind.FadeScreen.FadeIn(DG_GUI_FadeScreen.FadeInSpeeds.QuickFade, this.gameObject, "FadeInEnded");
    }

    public void FadeInEnded()
    {
        Transitioning = false;
    }





    public void NavMeshLoaded()
    {
        CreateNetworkObjects(true);
    }
    void CreateNetworkObjects(bool isEnable)
    {
        NetworkScene NS = QuickFind.NetworkObjectManager.GetSceneByID(QuickFind.NetworkSync.CurrentScene);
        for (int i = 0; i < NS.NetworkObjectList.Count; i++)
        {
            NetworkObject NO = NS.NetworkObjectList[i];
            if (!isEnable)
            {
                if (NO.ObjectType == NetworkObjectManager.NetworkObjectTypes.Enemy)
                    Destroy(NO.transform.GetChild(0).gameObject);
                else
                    NO.gameObject.SetActive(false);
            }
            else
            {
                if (NO.ObjectType == NetworkObjectManager.NetworkObjectTypes.Enemy)
                    NO.SpawnNetworkObject(NS);
                else
                    NO.gameObject.SetActive(true);
            }           
        }
    }
}
