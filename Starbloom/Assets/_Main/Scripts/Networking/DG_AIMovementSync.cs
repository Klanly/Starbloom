using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_AIMovementSync : MonoBehaviour {

    public DG_NetworkSyncRates.SyncRateTypes NetSyncType;
    public DG_AIAnimationSync AnimSync;

    [HideInInspector] public float Distance;
    DG_NetworkSyncRates.SyncRateModule Sync;
    bool isController = false;  
    float Timer;
    Transform _T;
    NetworkObject RelayNetworkObject;
    node_AIMovement MovementScript;

    Vector3 KnownPosition;
    Vector3 KnownHeading;
    int[] OutData;


    private void Start()
    {
        _T = transform;
        MovementScript = transform.GetComponent<node_AIMovement>();
        if (RelayNetworkObject == null) RelayNetworkObject = transform.parent.GetComponent<NetworkObject>();
        KnownPosition = QuickFind.ConvertIntsToPosition(RelayNetworkObject.PositionX, RelayNetworkObject.PositionY, RelayNetworkObject.PositionZ);
        KnownHeading.y = QuickFind.ConvertIntToFloat(RelayNetworkObject.YFacing);
    }


    private void Update()
    {
        if (Sync == null) Sync = QuickFind.NetworkSyncRates.GetSyncModuleByType(NetSyncType);
        CheckIfYouAreOwner();

        if (isController)
        {
            Timer = Timer - Time.deltaTime;
            if (Timer < 0)
            {
                Timer = Sync.SendRate;
                SendOutAIPosition();
            }
        }
        else
        {
            if (!QuickFind.WithinDistanceVec(transform.position, KnownPosition, Sync.MaxDistance))
            {
                _T.position = KnownPosition;
                Distance = 0;
            }
            else
            {
                int AdditiveSpeedMultiplier = 1;
                Distance = Vector3.Distance(_T.position, KnownPosition);
                if (Distance > 2) AdditiveSpeedMultiplier = (int)Distance;
                float SlerpRate = Sync.SlerpMoveRate * AdditiveSpeedMultiplier;
                _T.position = Vector3.MoveTowards(_T.position, KnownPosition, SlerpRate);
            }
            _T.eulerAngles = QuickFind.AngleLerp(_T.eulerAngles, KnownHeading, Sync.SlerpTurnRate);
        }
    }


    void SendOutAIPosition()
    {
        if (OutData == null) OutData = new int[6];

        OutData[0] = QuickFind.NetworkSync.CurrentScene;
        OutData[1] = RelayNetworkObject.NetworkObjectID;

        Vector3 Pos = _T.position;
        float Dir = _T.eulerAngles.y;

        RelayNetworkObject.PositionX = QuickFind.ConvertFloatToInt(Pos.x);
        RelayNetworkObject.PositionY = QuickFind.ConvertFloatToInt(Pos.y);
        RelayNetworkObject.PositionZ = QuickFind.ConvertFloatToInt(Pos.z);
        RelayNetworkObject.YFacing = QuickFind.ConvertFloatToInt(Dir);

        OutData[2] = RelayNetworkObject.PositionX;
        OutData[3] = RelayNetworkObject.PositionY;
        OutData[4] = RelayNetworkObject.PositionZ;
        OutData[5] = RelayNetworkObject.YFacing;

        QuickFind.NetworkSync.SendAILocationSync(OutData);
    }

    public void UpdatePlayerPos(int[] InData)
    {
        RelayNetworkObject.PositionX = InData[2];
        RelayNetworkObject.PositionY = InData[3];
        RelayNetworkObject.PositionZ = InData[4];
        RelayNetworkObject.YFacing  = InData[5];

        KnownPosition = QuickFind.ConvertIntsToPosition(RelayNetworkObject.PositionX, RelayNetworkObject.PositionY, RelayNetworkObject.PositionZ);
        KnownHeading.y = QuickFind.ConvertIntToFloat(RelayNetworkObject.YFacing);
    }




    void CheckIfYouAreOwner()
    {
        if(QuickFind.NetworkSync.UserID == RelayNetworkObject.Scene.SceneOwnerID)
        {
            if (MovementScript.isActiveAndEnabled)
                return;
            else
            {
                isController = true;
                transform.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = true;
                MovementScript.enabled = true;
                transform.GetComponent<node_AIAnimation>().enabled = true;

            }
        }
    }
}
