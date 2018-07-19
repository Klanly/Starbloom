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
    [System.NonSerialized] public DG_AIAnimationSync AnimationSync;
    [ReadOnly] public bool YouAreOwner;


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
        if (Position == Vector3.zero) return; //Appropriate Nav point was not found, don't do anything right now.
        Vector3 LocalPos = Position + RelayNetworkObject.Scene.transform.position;

        if (OutMovementData == null) OutMovementData = new int[6];
        OutMovementData[0] = RelayNetworkObject.Scene.SceneID;
        OutMovementData[1] = RelayNetworkObject.NetworkObjectID;
        OutMovementData[2] = QuickFind.ConvertFloatToInt(LocalPos.x);
        OutMovementData[3] = QuickFind.ConvertFloatToInt(LocalPos.y);
        OutMovementData[4] = QuickFind.ConvertFloatToInt(LocalPos.z);
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
        Vector3 GlobalPos = Position - RelayNetworkObject.Scene.transform.position;
        Movement.ReceiveMoveOrder(GlobalPos, (DG_AIEntityMovement.MovementBehaviour)Behaviour);
    }

    #endregion

    #region UTIL
    //Network
    public bool CheckIfYouAreOwner()
    {
        if (QuickFind.NetworkSync.ThisPlayerBelongsToMe(RelayNetworkObject.Scene.ScenePlayerOwnerID)) { YouAreOwner = true; return true; }
        else { YouAreOwner = false; return false; }
    }
    public Vector3 FindRandomNavMeshPoint(Vector3 CenterPoint, float Radius)
    {
        Vector3 point;
        for (int i = 0; i < 100; i++)
        {
            if (RandomPoint(CenterPoint, Radius, out point))
            {
                NavMeshPath path = new NavMeshPath();
                Movement.agent.CalculatePath(point, path);
                if (path.status == NavMeshPathStatus.PathPartial || path.status == NavMeshPathStatus.PathInvalid) continue;
                else return point;
            }
        }
        //Debug.Log("Valid Nav Mesh spot could not be found.");
        return Vector3.zero;
    }
    public bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * range;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }
        //Debug.Log("Valid Random Point Could Not be found.");
        result = Vector3.zero;
        return false;
    }
    public Vector3 AcceptableNavMeshPoint(Vector3 Position)
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(Position, out hit, 1.0f, NavMesh.AllAreas))
            return hit.position;
        else
            return Vector3.zero;
    }

    #endregion
}
