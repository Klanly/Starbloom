using Photon;
using UnityEngine;

namespace AnySync.Examples
{
    public class PhotonNetworkManager : PunBehaviour
    {
        [SerializeField]
        private GameObject _playerPrefab;
        [SerializeField]
        private Transform _startPosition;

        private void Start()
        {
            ConnectToPhoton();
        }

        private void ConnectToPhoton()
        {
            PhotonNetwork.ConnectUsingSettings("0");
        }

        public override void OnConnectedToMaster()
        {
            base.OnConnectedToMaster();
            PhotonNetwork.JoinOrCreateRoom("TestRoom", new RoomOptions(), TypedLobby.Default);
        }

        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();
            PhotonNetwork.Instantiate(_playerPrefab.name, _startPosition.position, _startPosition.rotation, 0);
        }

        // aggressive reconnect
        public override void OnDisconnectedFromPhoton()
        {
            base.OnDisconnectedFromPhoton();
            ConnectToPhoton();
        }
    }
}