using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class ConnectAndJoinRandom : Photon.MonoBehaviour
{
    [Header("Variables")]
    public bool AutoConnect = true;
    public int Version = 1;
    private bool ConnectInUpdate = true;





    private void Awake()
    {
        QuickFind.NetworkMaster = this;
    }


    public void StartGame()
    {
        PhotonNetwork.autoJoinLobby = false;
    }


    public virtual void Update()
    {
        if (ConnectInUpdate && AutoConnect && !PhotonNetwork.connected)
        {
            ConnectInUpdate = false;
            PhotonNetwork.ConnectUsingSettings(Version + "." + SceneManagerHelper.ActiveSceneBuildIndex);
        }
    }





    //////////////////////////////////////////////////////////

    public virtual void OnConnectedToMaster()
    {
        PhotonNetwork.JoinRandomRoom();
    }
    public virtual void OnJoinedLobby()
    {
        PhotonNetwork.JoinRandomRoom();
    }
    public virtual void OnPhotonRandomJoinFailed()
    {
        Debug.Log("Creating New Room");
        PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = 4 }, null);
    }
    public virtual void OnFailedToConnectToPhoton(DisconnectCause cause)
    {
        Debug.LogError("Cause: " + cause);
    }




    public void OnJoinedRoom()
    {
        if (PhotonNetwork.isMasterClient)
            PhotonNetwork.InstantiateSceneObject("NetworkSync", Vector3.zero, Quaternion.identity, 0, null);
    }
}
