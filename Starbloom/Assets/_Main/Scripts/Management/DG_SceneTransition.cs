using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DG_SceneTransition : MonoBehaviour {


    public class AIObjectsWaitingForNetObjects
    {
        public int NetworkID;
        public int PositionX;
        public int PositionY;
        public int PositionZ;
        public DG_AIEntity.AIDestinationTransfer AICharData;
    }

    Scene SceneUnloading;
    int NetworkSceneIndexUnloading;

    string SceneLoading;
    int NetworkSceneIndexLoading;
    int LoadingPortalID;
    DG_SceneEntryPoints LoadingSceneEntryPoints;
    bool Transitioning = false;
    Material Skybox;

    int[] RequestInts;
    List<int> AITransferData;
    [System.NonSerialized] public List<AIObjectsWaitingForNetObjects> AwaitingAIObjects;


    private void Awake()
    {
        AITransferData = new List<int>();
        AwaitingAIObjects = new List<AIObjectsWaitingForNetObjects>();
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
        CreateNetworkAIObjects(false);

        QuickFind.PathfindingGeneration.NavMeshIsGenerated = false;

        Skybox = RenderSettings.skybox;

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

        RenderSettings.skybox = Skybox;

        //Load Network Objects;
        SetSelfInScene(NetworkSceneIndexLoading);

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
        GetSceneAILocations();
    }

    void GetSceneAILocations()
    {
        NetworkScene NS = QuickFind.NetworkObjectManager.GetSceneByID(QuickFind.NetworkSync.CurrentScene);
        if (NS.SomeoneElseIsInThisScene())
        {
            if (RequestInts == null) RequestInts = new int[2];
            RequestInts[0] = NS.SceneID;
            RequestInts[1] = QuickFind.NetworkSync.UserID;
            QuickFind.NetworkSync.RequestAIPositionsFromOwner(RequestInts);
        }
        else
            CreateNetworkAIObjects(true);
    }
    public void AIPositionsRequested(int[] InData)
    {
        NetworkScene NS = QuickFind.NetworkObjectManager.GetSceneByID(InData[0]);
        if (NS.SceneOwnerID != QuickFind.NetworkSync.UserID) return;

        int ObjectCount = 0;

        for (int i = 0; i < NS.NetworkObjectList.Count; i++)
        {
            NetworkObject NO = NS.NetworkObjectList[i];
            if (NO.ObjectType != NetworkObjectManager.NetworkObjectTypes.Enemy) continue;
            if (NO.AICharData == null) { Debug.Log("Character Without AI data left in scene."); continue; }

            ObjectCount++;

            AITransferData.Add(NO.NetworkObjectID);
            AITransferData.Add(NO.PositionX);
            AITransferData.Add(NO.PositionY);
            AITransferData.Add(NO.PositionZ);

            DG_AIEntity.AIDestinationTransfer DT = NO.AICharData[0];
            AITransferData.Add(DT.BehaviourType);
            int Reached = 0; if (DT.DestinationReached) Reached = 1;
            AITransferData.Add(Reached);

            if(!DT.DestinationReached)
            {
                AITransferData.Add(DT.DestinationX);
                AITransferData.Add(DT.DestinationY);
                AITransferData.Add(DT.DestinationZ);
            }
        }
        AITransferData.Add(ObjectCount);

        QuickFind.NetworkSync.ReturnAIPositionsToReqester(InData[1], AITransferData.ToArray());
        AITransferData.Clear();
    }
    public void AIPositionsReceived(int[] InData)
    {
        AIObjectsWaitingForNetObjects AwaitObject;
        NetworkScene NS = QuickFind.NetworkObjectManager.GetSceneByID(QuickFind.NetworkSync.CurrentScene);
        int index = 0;
        int Count = InData[(InData.Length - 1)];
        for (int i = 0; i < Count; i++)
        {
            int ID = InData[index]; index++;
            NetworkObject NO = QuickFind.NetworkObjectManager.GetItemByID(NS.SceneID, ID);
            if (NO == null)
            {
                AwaitObject = new AIObjectsWaitingForNetObjects();
                index = PopulateAwaitingObject(AwaitObject, InData, index, ID);
                AwaitingAIObjects.Add(AwaitObject);
                continue;
            }

            NO.PositionX = InData[index]; index++;
            NO.PositionY = InData[index]; index++;
            NO.PositionZ = InData[index]; index++;
            NO.transform.position = QuickFind.ConvertIntsToPosition(NO.PositionX, NO.PositionY, NO.PositionZ);

            if (NO.AICharData == null) NO.AICharData = new DG_AIEntity.AIDestinationTransfer[1];
            NO.AICharData[0] = new DG_AIEntity.AIDestinationTransfer();

            DG_AIEntity.AIDestinationTransfer DT = NO.AICharData[0];
            DT.BehaviourType = InData[index]; index++;
            int Reached = InData[index]; index++;
            if (Reached == 0) DT.DestinationReached = false; else DT.DestinationReached = true;
            if (!DT.DestinationReached)
            {
                DT.DestinationX = InData[index]; index++;
                DT.DestinationY = InData[index]; index++;
                DT.DestinationZ = InData[index]; index++;
            }
        }

        CreateNetworkAIObjects(true);
    }


    void CreateNetworkAIObjects(bool isEnable)
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
                    NO.SpawnNetworkObject(NS, true);
                else
                    NO.gameObject.SetActive(true);
            }           
        }
    }






    int PopulateAwaitingObject(AIObjectsWaitingForNetObjects WaitingObject, int[] InData, int index, int ID)
    {
        WaitingObject.NetworkID = ID;
        WaitingObject.PositionX = InData[index]; index++;
        WaitingObject.PositionY = InData[index]; index++;
        WaitingObject.PositionZ = InData[index]; index++;

        WaitingObject.AICharData = new DG_AIEntity.AIDestinationTransfer();

        WaitingObject.AICharData.BehaviourType = InData[index]; index++;
        int Reached = InData[index]; index++;
        if (Reached == 0) WaitingObject.AICharData.DestinationReached = false; else WaitingObject.AICharData.DestinationReached = true;
        if (!WaitingObject.AICharData.DestinationReached)
        {
            WaitingObject.AICharData.DestinationX = InData[index]; index++;
            WaitingObject.AICharData.DestinationY = InData[index]; index++;
            WaitingObject.AICharData.DestinationZ = InData[index]; index++;
        }
        return index;
    }
    public void CheckIfObjectIsTheOneWaiting(NetworkObject NO)
    {
        for(int i = 0; i < AwaitingAIObjects.Count; i++)
        {
            AIObjectsWaitingForNetObjects AWO = AwaitingAIObjects[i];
            if(AWO.NetworkID == NO.NetworkObjectID)
            {
                NO.PositionX = AWO.PositionX;
                NO.PositionY = AWO.PositionY;
                NO.PositionZ = AWO.PositionZ;
                NO.transform.position = QuickFind.ConvertIntsToPosition(NO.PositionX, NO.PositionY, NO.PositionZ);

                if (NO.AICharData == null) NO.AICharData = new DG_AIEntity.AIDestinationTransfer[1];
                NO.AICharData[0] = new DG_AIEntity.AIDestinationTransfer();
                DG_AIEntity.AIDestinationTransfer DT = NO.AICharData[0];

                DT.BehaviourType = AWO.AICharData.BehaviourType;
                DT.DestinationReached = AWO.AICharData.DestinationReached;
                if (!DT.DestinationReached)
                {
                    Debug.Log("Destination Not Reached");
                    DT.DestinationX = AWO.AICharData.DestinationX;
                    DT.DestinationY = AWO.AICharData.DestinationY;
                    DT.DestinationZ = AWO.AICharData.DestinationZ;
                }
                else
                    Debug.Log("Destination Reached");

                AwaitingAIObjects.Remove(AWO);
            }
        }
    }







    public void SetSelfInScene(int NewScene)
    {
        int SceneLeaving = QuickFind.NetworkSync.CurrentScene;
        QuickFind.NetworkSync.CurrentScene = NewScene;
        LocalSetSelfInScene(SceneLeaving, NewScene);

        int[] IntGroup = new int[3];
        IntGroup[0] = QuickFind.NetworkSync.UserID;
        IntGroup[1] = NewScene;
        IntGroup[2] = SceneLeaving;
        QuickFind.NetworkSync.NetSetUserInScene(IntGroup);
    }
    public void LocalSetSelfInScene(int SceneLeaving, int NewScene)
    {
        if (SceneLeaving != -1) QuickFind.NetworkObjectManager.GetSceneByID(SceneLeaving).SceneOwnerID = 0;
        DG_NetworkSync.Users U = QuickFind.NetworkSync.GetUserByID(QuickFind.NetworkSync.UserID);
        U.SceneID = NewScene;
        QuickFind.NetworkObjectManager.GetSceneByID(NewScene).SelfEnteredScene();
    }
    public void RemoteSetUserInScene(int[] IntGroup)
    {
        int ID = IntGroup[0];
        DG_NetworkSync.Users U = QuickFind.NetworkSync.GetUserByID(ID);
        U.SceneID = IntGroup[1];
        int SceneLeaving = IntGroup[2];
        if (SceneLeaving != -1) QuickFind.NetworkObjectManager.GetSceneByID(SceneLeaving).UserLeftScene(U);
    }
}
