using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DG_SceneTransition : MonoBehaviour {

    
    public class PlayerTransitioning
    {
        public bool Transitioning = false;
        public int PlayerID;
        public Scene SceneUnloading;
        public string SceneLoading;
        public int NetworkSceneIndexUnloading;
        public int NetworkSceneIndexLoading;
        public int LoadingPortalID;
    }


    public class AIObjectsWaitingForNetObjects
    {
        public int NetworkID;
        public int PositionX;
        public int PositionY;
        public int PositionZ;
        public DG_AIEntity.AIDestinationTransfer AICharData;
    }

    public float CenterStage;
    public float StageA;
    public float StageB;

    PlayerTransitioning Player1Transition;
    PlayerTransitioning Player2Transition;

    DG_SceneEntryPoints LoadingSceneEntryPoints;


    int[] RequestInts;
    List<int> AITransferData;

    public bool PrintDebug;

    [System.NonSerialized] public List<AIObjectsWaitingForNetObjects> AwaitingAIObjects;


    private void Awake()
    {
        AITransferData = new List<int>();
        AwaitingAIObjects = new List<AIObjectsWaitingForNetObjects>();
        QuickFind.SceneTransitionHandler = this;
        Player1Transition = new PlayerTransitioning();
        Player2Transition = new PlayerTransitioning();
    }



    public void TriggerSceneChange(string SceneString, int PortalValue, int PlayerID)
    {
        bool isPlayer1 = (QuickFind.NetworkSync.Player1PlayerCharacter == PlayerID);
        if (isPlayer1 && Player1Transition.Transitioning) return;
        if (!isPlayer1 && Player2Transition.Transitioning) return;

        PlayerTransitioning PT = Player1Transition;
        if (isPlayer1) PT = Player1Transition;
        else PT = Player2Transition;

        PT.Transitioning = true;
        PT.PlayerID = PlayerID;
        PT.SceneUnloading = SceneManager.GetActiveScene();
        PT.NetworkSceneIndexUnloading = QuickFind.SceneList.GetSceneIndexByString(PT.SceneUnloading.name);
        PT.SceneLoading = SceneString;
        PT.NetworkSceneIndexLoading = QuickFind.SceneList.GetSceneIndexByString(SceneString);
        PT.LoadingPortalID = PortalValue;

        //Unload Network Objects

        bool UnloadLeavingScene = true;
        if (QuickFind.NetworkSync.AnyPlayersIControlAreInScene(PT.NetworkSceneIndexUnloading, PT.PlayerID)) UnloadLeavingScene = false;
        if (UnloadLeavingScene)
            CreateNetworkAIObjects(false, PT.NetworkSceneIndexUnloading);

        QuickFind.PathfindingGeneration.NavMeshIsGenerated = false;

        if (PT.NetworkSceneIndexLoading == PT.NetworkSceneIndexUnloading)
            FinalizeInScene(PT);
        else
        {
            if (isPlayer1)
                QuickFind.FadeScreen.FadeOut(DG_GUI_FadeScreen.FadeInSpeeds.QuickFade, this.gameObject, "FadeEndedPlayer1");
            else
                QuickFind.FadeScreen.FadeOut(DG_GUI_FadeScreen.FadeInSpeeds.QuickFade, this.gameObject, "FadeEndedPlayer2");
        }
    }

    public void FadeEndedPlayer1()
    {
        SceneLoad(Player1Transition);
    }
    public void FadeEndedPlayer2()
    {
        SceneLoad(Player2Transition);
    }

    void SceneLoad(PlayerTransitioning TransistionData)
    {
        SceneManager.LoadScene(TransistionData.SceneLoading, LoadSceneMode.Additive);

        bool UnloadLeavingScene = true;
        if (QuickFind.NetworkSync.AnyPlayersIControlAreInScene(TransistionData.NetworkSceneIndexUnloading, TransistionData.PlayerID)) UnloadLeavingScene = false;
        if(UnloadLeavingScene) SceneManager.UnloadSceneAsync(TransistionData.SceneUnloading);

        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        PlayerTransitioning PT;
        if (scene.name == Player1Transition.SceneLoading) PT = Player1Transition;
        else PT = Player2Transition;

        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
        Scene LoadScene = SceneManager.GetSceneByName(PT.SceneLoading);
        SceneManager.SetActiveScene(LoadScene);

        FinalizeInScene(PT);
    }
    void FinalizeInScene(PlayerTransitioning PT)
    {
        PT.Transitioning = false;
        //Load Network Objects;
        SetSelfInScene(PT.NetworkSceneIndexLoading, PT.PlayerID);

        bool BypassWait = false;
        if (PT.NetworkSceneIndexLoading == PT.NetworkSceneIndexUnloading) BypassWait = true;

        bool UnloadLeavingScene = true;
        if (QuickFind.NetworkSync.AnyPlayersIControlAreInScene(PT.NetworkSceneIndexUnloading, PT.PlayerID)) UnloadLeavingScene = false;

        QuickFind.PathfindingGeneration.SceneChange(PT.SceneUnloading, PT.NetworkSceneIndexLoading, PT.PlayerID, BypassWait, UnloadLeavingScene);
    }


    public void SetStageBeforeNavMeshLoad(int SceneID)
    {
        PlayerTransitioning PT;
        if (SceneID == Player1Transition.NetworkSceneIndexLoading) PT = Player1Transition;
        else PT = Player2Transition;

        //Set Stage
        GameObject Stage = SceneManager.GetSceneByName(PT.SceneLoading).GetRootGameObjects()[0];
        SceneIDList.SceneIdentity SI = QuickFind.SceneList.GetSceneById(PT.NetworkSceneIndexLoading);
        if (SI.CenterStageScene) Stage.transform.position = new Vector3(CenterStage, 0, 0);
        else
        {
            if (PT.PlayerID == QuickFind.NetworkSync.Player1PlayerCharacter) Stage.transform.position = new Vector3(StageA,0 , 0);
            else Stage.transform.position = new Vector3(StageB, 0 , 0);
        }
        SI.SceneLink.transform.position = Stage.transform.position;
        for(int i = 0; i < QuickFind.NetworkSync.UserList.Count; i++)
        {
            DG_NetworkSync.Users U = QuickFind.NetworkSync.UserList[i];
            if(U.SceneID == SceneID) U.CharacterLink.transform.position = Stage.transform.position;
        }       
    }


    public void NavMeshLoaded(int SceneID, int PlayerID)
    {
        if(PrintDebug) Debug.Log("Nav Mesh Loaded");

        PlayerTransitioning PT;
        if (PlayerID == QuickFind.NetworkSync.Player1PlayerCharacter) PT = Player1Transition;
        else PT = Player2Transition;
        DG_CharacterLink CharLink = QuickFind.NetworkSync.GetCharacterLinkByPlayerID(PT.PlayerID);

        //Set Player, and Cam.
        DG_SceneEntryObject Portal = QuickFind.SceneEntryPoints.GetItemFromID(PT.LoadingPortalID);
        CharLink.PlayerTrans.position = Portal.transform.position;
        CharLink.PlayerTrans.eulerAngles = Portal.transform.eulerAngles;

        int index = 0; if (PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) index = 1;
        QuickFind.PlayerCam.InstantSetCameraAngle(Portal.CameraFacing, CharLink.PlayerCam, index);

        //Environment
        QuickFind.WeatherHandler.CheckEnvironmentFX();


        //Load AI
        NetworkScene NS = QuickFind.NetworkObjectManager.GetSceneByID(SceneID);
        int PlayerInScene = NS.SomeoneElseIsInThisScene(PlayerID);
        if (QuickFind.NetworkSync.ThisPlayerBelongsToMe(PlayerInScene)) return;
        if (PlayerInScene != -1 && QuickFind.NetworkSync.UserList.Count != 1)
        {
            if (RequestInts == null) RequestInts = new int[2];
            RequestInts[0] = NS.SceneID;
            RequestInts[1] = QuickFind.NetworkSync.NetID;
            QuickFind.NetworkSync.RequestAIPositionsFromOwner(RequestInts);
        }
        else
            CreateNetworkAIObjects(true, NS.SceneID);

        //FadeIn
        QuickFind.FadeScreen.FadeIn(DG_GUI_FadeScreen.FadeInSpeeds.QuickFade);
    }
    public void AIPositionsRequested(int[] InData)
    {
        NetworkScene NS = QuickFind.NetworkObjectManager.GetSceneByID(InData[0]);
        if (!QuickFind.NetworkSync.ThisPlayerBelongsToMe(NS.ScenePlayerOwnerID)) return;

        int ObjectCount = 0;

        AITransferData.Add(NS.SceneID);

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
        int index = 0;
        NetworkScene NS = QuickFind.NetworkObjectManager.GetSceneByID(InData[index]); index++;
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

        CreateNetworkAIObjects(true, NS.SceneID);
    }


    public void CreateNetworkAIObjects(bool isEnable, int SceneID)
    {
        NetworkScene NS = QuickFind.NetworkObjectManager.GetSceneByID(SceneID);
        for (int i = 0; i < NS.NetworkObjectList.Count; i++)
        {
            NetworkObject NO = NS.NetworkObjectList[i];
            if (!isEnable)
            {
                if (NO.ObjectType == NetworkObjectManager.NetworkObjectTypes.Enemy)
                {
                    if(NO.transform.childCount > 0)
                        Destroy(NO.transform.GetChild(0).gameObject);
                }
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







    public void SetSelfInScene(int NewScene, int PlayerID)
    {
        int SceneLeaving = QuickFind.NetworkSync.GetUserByPlayerID(PlayerID).SceneID;
        if (SceneLeaving == -1) SceneLeaving = 0;
        QuickFind.NetworkSync.GetUserByPlayerID(PlayerID).SceneID = NewScene;
        LocalSetSelfInScene(SceneLeaving, NewScene, PlayerID);

        int[] IntGroup = new int[3];
        IntGroup[0] = PlayerID;
        IntGroup[1] = NewScene;
        IntGroup[2] = SceneLeaving;
        QuickFind.NetworkSync.NetSetUserInScene(IntGroup);
    }
    public void LocalSetSelfInScene(int SceneLeaving, int NewScene, int PlayerID)
    {
        if(SceneLeaving != NewScene) QuickFind.NetworkObjectManager.GetSceneByID(SceneLeaving).ScenePlayerOwnerID = -3;
        DG_NetworkSync.Users U = QuickFind.NetworkSync.GetUserByPlayerID(PlayerID);
        U.SceneID = NewScene;
        QuickFind.NetworkObjectManager.GetSceneByID(NewScene).SelfEnteredScene(PlayerID);
    }
    public void RemoteSetUserInScene(int[] IntGroup)
    {
        int ID = IntGroup[0];
        DG_NetworkSync.Users U = QuickFind.NetworkSync.GetUserByPlayerID(ID);
        U.SceneID = IntGroup[1];
        int SceneLeaving = IntGroup[2];
        if (SceneLeaving != -1) QuickFind.NetworkObjectManager.GetSceneByID(SceneLeaving).UserLeftScene(U);

        U.CharacterLink.transform.position = QuickFind.NetworkObjectManager.GetSceneByID(U.SceneID).transform.position;
    }
}
