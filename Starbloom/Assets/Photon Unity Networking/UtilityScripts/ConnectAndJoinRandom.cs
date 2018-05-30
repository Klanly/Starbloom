using UnityEngine;



public class ConnectAndJoinRandom : Photon.MonoBehaviour
{
    public string GameID = "DefaultRoomName";
    [HideInInspector] public int RequestedCharacterNum = -1;
    [HideInInspector] public bool CreateNewRoom;

    [Header("Variables")]
    public bool AutoConnect = true;
    public int Version = 1;
    private bool ConnectInUpdate = true;

    bool GameStart = true;



    private void Awake()
    {
        QuickFind.NetworkMaster = this;
    }


    public void StartGame()
    {
        PhotonNetwork.autoJoinLobby = false;
        PhotonNetwork.offlineMode = !QuickFind.GameSettings.PlayOnline;
        GameStart = false;
    }


    public virtual void Update()
    {
        if (GameStart)
            return;

        if (ConnectInUpdate && AutoConnect && !PhotonNetwork.connected)
        {
            ConnectInUpdate = false;
            PhotonNetwork.ConnectUsingSettings(Version + "." + SceneManagerHelper.ActiveSceneBuildIndex);
        }
    }





    //////////////////////////////////////////////////////////

    public virtual void OnConnectedToMaster()
    {
        if (CreateNewRoom)
            PhotonNetwork.CreateRoom(GameID, new RoomOptions() { MaxPlayers = 4 }, null);
        else
        {
            if (QuickFind.GameSettings.BypassMainMenu)
                PhotonNetwork.JoinRandomRoom();
            else
                PhotonNetwork.JoinRoom(GameID);
        }
    }
    public virtual void OnJoinedLobby()
    {
        //We do not have a lobby atm.
    }



    public void OnJoinedRoom()
    {
        if (PhotonNetwork.isMasterClient)
            PhotonNetwork.InstantiateSceneObject("NetworkSync", Vector3.zero, Quaternion.identity, 0, null);
    }



    public void OnPhotonPlayerDisconnected(PhotonPlayer player)
    {
        for(int i = 0; i < QuickFind.NetworkSync.UserList.Count; i++)
        {
            DG_NetworkSync.Users U = QuickFind.NetworkSync.UserList[i];
            PhotonView PV = U.PhotonClone.GetComponent<PhotonView>();
            if (PV.ownerId == player.ID)
                Debug.Log("player " + U.PlayerCharacterID.ToString() + " Disconnected");
            QuickFind.NetworkSync.UserList.RemoveAt(i);
        }
    }








    public virtual void OnPhotonRandomJoinFailed()
    { PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = 4 }, null); }
    public virtual void OnFailedToConnectToPhoton(DisconnectCause cause)
    { Debug.LogError("Cause: " + cause); }
}
