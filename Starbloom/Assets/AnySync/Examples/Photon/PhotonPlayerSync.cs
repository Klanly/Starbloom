using Photon;
using UnityEngine;

namespace AnySync.Examples
{
    public class PhotonPlayerSync : PunBehaviour, IPunObservable
    {
        private const float MovementAcceleration = 13f;

        private bool _teleportOnNextSync;
        private Vector2 _input;

        private Rigidbody _rigidbody;
        private SyncBuffer _syncBuffer;
        private Transform _transform;
        PhotonView PV;
        private void Awake()
        {
            PV = transform.parent.GetComponent<PhotonView>();
            _rigidbody = GetComponent<Rigidbody>();
            _syncBuffer = GetComponent<SyncBuffer>();
            _transform = transform;
        }

        private void Update()
        {
            if (!PV.isMine)
            {
                if (_syncBuffer.HasKeyframes)
                {
                    _syncBuffer.UpdatePlayback(Time.deltaTime);
                    _transform.position = _syncBuffer.Position;
                    _transform.rotation = _syncBuffer.Rotation;
                }
            }
            else
            {
                //// movement
                //_input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
                //
                //// teleportation
                //if (Input.GetKeyDown(KeyCode.Space))
                //{
                //    var newPosition = new Vector3(Random.Range(-6f, 6f), 2f, Random.Range(-6f, 6f));
                //    _rigidbody.position = newPosition;
                //    _rigidbody.rotation = Quaternion.identity;
                //    _rigidbody.velocity = Vector3.zero;
                //    _rigidbody.angularVelocity = Vector3.zero;
                //    _teleportOnNextSync = true;
                //}
            }
        }

        private float _timeSinceLastSync;
        private void FixedUpdate()
        {
            if (PV.isMine)
            {
                // control the rigidbody locally
                _rigidbody.AddForce(new Vector3(-_input.x, 0f, -_input.y) * MovementAcceleration * Time.deltaTime, ForceMode.VelocityChange);
                _timeSinceLastSync += Time.deltaTime;
            }
        }

        private bool _forceSync;
        public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
        {
            base.OnPhotonPlayerConnected(newPlayer);
            
            _forceSync = true;
        }

        private Vector3 _lastSentVelocity;
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.isWriting)
            {
                if (!_forceSync && _rigidbody.velocity == _lastSentVelocity)
                {
                    stream.SendNext(false);
                    return;
                }
                else
                    stream.SendNext(true);

                _forceSync = false;

                stream.SendNext(_timeSinceLastSync);
                stream.SendNext(_rigidbody.position);
                stream.SendNext(_rigidbody.rotation);
                stream.SendNext(_rigidbody.velocity);
                stream.SendNext(_teleportOnNextSync);

                _lastSentVelocity = _rigidbody.velocity;
                _teleportOnNextSync = false;
                _timeSinceLastSync = 0f;
            }
            else
            {
                bool Continue = (bool)stream.ReceiveNext();
                if (Continue)
                {
                    var interpolationTime = (float)stream.ReceiveNext();
                    var position = (Vector3)stream.ReceiveNext();
                    var rotation = (Quaternion)stream.ReceiveNext();
                    var velocity = (Vector3)stream.ReceiveNext();
                    var isTeleport = (bool)stream.ReceiveNext();

                    if (isTeleport)
                    {
                        if (_syncBuffer.HasKeyframes)
                            _syncBuffer.AddKeyframe(interpolationTime, _syncBuffer.LastReceivedKeyframe.Position, _syncBuffer.LastReceivedKeyframe.Rotation, _syncBuffer.LastReceivedKeyframe.Velocity, _syncBuffer.LastReceivedKeyframe.AngularVelocity);

                        _syncBuffer.AddKeyframe(0f, position, rotation, velocity);
                    }
                    else
                    {
                        _syncBuffer.AddKeyframe(interpolationTime, position, rotation, velocity);
                    }
                }
            }
        }
    }
}