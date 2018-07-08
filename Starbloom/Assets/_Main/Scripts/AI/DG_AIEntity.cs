using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Sirenix.OdinInspector;

public class DG_AIEntity : MonoBehaviour {


    public class AIDestinationTransfer
    {
        public int BehaviourType;
        public bool DestinationReached;
        public int DestinationX;
        public int DestinationY;
        public int DestinationZ;
    }


    #region Variables

    //Network
    [System.NonSerialized] public Transform _T;
    [System.NonSerialized] public NetworkObject RelayNetworkObject;
    [System.NonSerialized] public DG_AIEntityMovement Movement;
    [System.NonSerialized] public DG_AIEntityCombat Combat;
    [System.NonSerialized] public DG_AIEntityDetection Detection;


    Vector3 KnownDestination;
    float NodeID;
    int[] OutMovementData;


    #endregion

    private void Awake()
    {
        _T = transform;
    }

    public void Load()
    {
        DG_AIEntityMovement Mov = GetComponent<DG_AIEntityMovement>();
        if (Mov != null) { Movement = Mov; Movement.Entity = this; Movement.enabled = true; }
        DG_AIEntityCombat Com = GetComponent<DG_AIEntityCombat>();
        if (Com != null) {Combat = Com; Combat.Entity = this; Combat.enabled = true; }
        DG_AIEntityDetection Nav = GetComponent<DG_AIEntityDetection>();
        if (Nav != null) { Detection = Nav; Detection.Entity = this; Detection.enabled = true; }
    }

    private void Update()
    {
        SetNetworkObjectPosition();
    }


    #region Net_Movement

    public void SetNetworkObjectPosition()
    {
        Vector3 Pos = _T.position;
        RelayNetworkObject.PositionX = QuickFind.ConvertFloatToInt(Pos.x);
        RelayNetworkObject.PositionY = QuickFind.ConvertFloatToInt(Pos.y);
        RelayNetworkObject.PositionZ = QuickFind.ConvertFloatToInt(Pos.z);
    }



    public void SetAgentDestination(Vector3 Position, int CurrentMovementBehaviour)
    {
        if (OutMovementData == null) OutMovementData = new int[6];
        OutMovementData[0] = QuickFind.NetworkSync.CurrentScene;
        OutMovementData[1] = RelayNetworkObject.NetworkObjectID;
        OutMovementData[2] = QuickFind.ConvertFloatToInt(Position.x);
        OutMovementData[3] = QuickFind.ConvertFloatToInt(Position.y);
        OutMovementData[4] = QuickFind.ConvertFloatToInt(Position.z);
        OutMovementData[5] = CurrentMovementBehaviour;
        QuickFind.NetworkSync.SendAIDestination(OutMovementData);
    }

    public void ReceiveAgentDestination(int[] InData) { DestinationMoveOrder(InData[2], InData[3], InData[4], InData[5]); }


    public void DestinationMoveOrder(int X, int Y, int Z, int Behaviour)
    {
        AIDestinationTransfer DataTransfer = RelayNetworkObject.AICharData[0];
        DataTransfer.DestinationX = X;
        DataTransfer.DestinationY = Y;
        DataTransfer.DestinationZ = Z;
        DataTransfer.BehaviourType = Behaviour;

        Vector3 Position = QuickFind.ConvertIntsToPosition(X, Y, Z);
        Movement.ReceiveMoveOrder(Position, (DG_AIEntityMovement.MovementBehaviour)Behaviour);
    }

    #endregion

    #region UTIL
    //Network
    public bool CheckIfYouAreOwner()
    {
        if (QuickFind.NetworkSync.UserID == RelayNetworkObject.Scene.SceneOwnerID) return true;
        else return false;
    }
    #endregion
}
