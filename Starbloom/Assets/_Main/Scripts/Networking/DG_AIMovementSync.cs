using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_AIMovementSync : MonoBehaviour {

    //public DG_NetworkSyncRates.SyncRateTypes NetSyncType;
    //
    //DG_NetworkSyncRates.SyncRateModule Sync;
    //[System.NonSerialized] public float Distance;
    //[System.NonSerialized] public bool isController = false;
    //[System.NonSerialized] public DG_AIEntity MovementScript;
    //float Timer;
    //Transform _T;
    //NetworkObject RelayNetworkObject;
    //
    //
    //Vector3 KnownPosition;
    //Vector3 KnownHeading;
    //int[] OutData;
    //
    //
    //
    //private void Start()
    //{
    //    if (!QuickFind.GameStartHandler.GameHasStarted) { Debug.Log("AI Object Left In Scene, Destroying"); Destroy(gameObject); return; }
    //
    //    _T = transform;
    //    MovementScript = transform.GetComponent<DG_AIEntity>();
    //    if (RelayNetworkObject == null) RelayNetworkObject = transform.parent.GetComponent<NetworkObject>();
    //    KnownPosition = QuickFind.ConvertIntsToPosition(RelayNetworkObject.PositionX, RelayNetworkObject.PositionY, RelayNetworkObject.PositionZ);
    //    KnownHeading.y = QuickFind.ConvertIntToFloat(RelayNetworkObject.YFacing);
    //}
    //
    //
    //private void Update()
    //{
    //    if (Sync == null) Sync = QuickFind.NetworkSyncRates.GetSyncModuleByType(NetSyncType);
    //    CheckIfYouAreOwner();
    //
    //
    //    if (isController)
    //    {
    //        Timer = Timer - Time.deltaTime;
    //        if (Timer < 0)
    //        {
    //            Timer = Sync.SendRate;
    //            SendOutAIPosition();
    //        }
    //    }
    //
    //
    //    if (!QuickFind.WithinDistanceVec(transform.position, KnownPosition, Sync.MaxDistance))
    //    {
    //        _T.position = KnownPosition;
    //        Distance = 0;
    //    }
    //    else
    //    {
    //        Distance = Vector3.Distance(_T.position, KnownPosition);
    //
    //        if (!isController)
    //        {
    //            int AdditiveSpeedMultiplier = 1;
    //            if (Distance > 2) AdditiveSpeedMultiplier = (int)Distance;
    //            float SlerpRate = Sync.SlerpMoveRate * AdditiveSpeedMultiplier;
    //            _T.position = Vector3.MoveTowards(_T.position, KnownPosition, SlerpRate);
    //        }
    //    }
    //    if (!isController)
    //        _T.eulerAngles = QuickFind.AngleLerp(_T.eulerAngles, KnownHeading, Sync.SlerpTurnRate);
    //}
    //
    //
    //void SendOutAIPosition()
    //{
    //    if (OutData == null) OutData = new int[6];
    //
    //    OutData[0] = QuickFind.NetworkSync.CurrentScene;
    //    OutData[1] = RelayNetworkObject.NetworkObjectID;
    //
    //    Vector3 Pos = _T.position;
    //    float Dir = _T.eulerAngles.y;
    //
    //    RelayNetworkObject.PositionX = QuickFind.ConvertFloatToInt(Pos.x);
    //    RelayNetworkObject.PositionY = QuickFind.ConvertFloatToInt(Pos.y);
    //    RelayNetworkObject.PositionZ = QuickFind.ConvertFloatToInt(Pos.z);
    //    RelayNetworkObject.YFacing = QuickFind.ConvertFloatToInt(Dir);
    //
    //    KnownPosition = QuickFind.ConvertIntsToPosition(RelayNetworkObject.PositionX, RelayNetworkObject.PositionY, RelayNetworkObject.PositionZ);
    //
    //    OutData[2] = RelayNetworkObject.PositionX;
    //    OutData[3] = RelayNetworkObject.PositionY;
    //    OutData[4] = RelayNetworkObject.PositionZ;
    //    OutData[5] = RelayNetworkObject.YFacing;
    //
    //    QuickFind.NetworkSync.SendAILocationSync(OutData);
    //}
    //
    //public void UpdatePlayerPos(int[] InData)
    //{
    //    RelayNetworkObject.PositionX = InData[2];
    //    RelayNetworkObject.PositionY = InData[3];
    //    RelayNetworkObject.PositionZ = InData[4];
    //    RelayNetworkObject.YFacing  = InData[5];
    //
    //    KnownPosition = QuickFind.ConvertIntsToPosition(RelayNetworkObject.PositionX, RelayNetworkObject.PositionY, RelayNetworkObject.PositionZ);
    //    KnownHeading.y = QuickFind.ConvertIntToFloat(RelayNetworkObject.YFacing);
    //}
    //
    //
    //
    //
    //
}
