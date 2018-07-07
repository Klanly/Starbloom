using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Sirenix.OdinInspector;

public class DG_AIEntity : MonoBehaviour {

    #region Variables

    [System.Serializable]
    public class MovementOptions
    {
        [Range(0.0f, 1.0f)]
        public float stopDistanceAdjust = 0.5f;
        public float walkSpeed = 0.5f;
        public float RunSpeed = 1.0f;
        public float m_StationaryTurnSpeed = 180;
        public float m_MovingTurnSpeed = 360;
        public float rotateSpeed = 7.0f;
        public float smoothMove = 0.2f;

        [Header("Node Logic")]
        public bool FollowSequence;
        public int SpecificNode = -1;
        [Header("Position Scan")]
        public float NewPositionRadius = 25.0f;
        public float NodeLingerRange = 3.0f;
    }

    //Movement
    public enum MovementBehaviour
    {
        Guard,

        Wander,
        WanderNearNode,

        WalkToNode,
        RunToNode
    }


    [Header("Network")]
    [System.NonSerialized] public NetworkObject RelayNetworkObject;
    Vector3 KnownDestination;
    float NodeID;
    int[] OutMovementData;


    [Header("Movement")]
    public MovementBehaviour StartMovementBehaviour;

    [ReadOnly] public MovementBehaviour CurrentMovementBehaviour;
    [ReadOnly] public int nodeWanderSequenceID = -1;

    [System.NonSerialized] public MovementOptions MovementSettings;
    [System.NonSerialized] public NavMeshAgent agent;
    [System.NonSerialized] public bool DestinationReached = false;
    bool WaitingForTimer;
    float Timer;
    Transform _T;


    #endregion


    void Update()
    {
        if (_T == null) InitialLoad();
        HandleMovementUpdate();
    }


    private void InitialLoad()
    {
        _T = transform;
        CurrentMovementBehaviour = StartMovementBehaviour;
        GetNextPosition();
        agent.updateRotation = false;
        agent.updatePosition = true;
    }


    #region Movement

    void HandleMovementUpdate()
    {
        if (!QuickFind.PathfindingGeneration.NavMeshIsGenerated) return;

        if (WaitingForTimer) { Timer -= Time.deltaTime; if (Timer < 0) { WaitingForTimer = false; GetNextPosition(); } }

        if (agent.speed != 0)
        {
            DestinationReached = false;
            float RandTime = 0;
            Vector3 Destination = agent.destination;

            float DistanceToTarget = Vector3.Distance(transform.position, new Vector3(Destination.x, transform.position.y, Destination.z));
            if (DistanceToTarget <= agent.stoppingDistance + MovementSettings.stopDistanceAdjust)
            { DestinationReached = true; RandTime = Random.Range(0, MovementSettings.NodeLingerRange); }
            if (DestinationReached) { WaitingForTimer = true; agent.speed = 0; Timer = RandTime; }
            //Move Entity
            if (agent.remainingDistance > agent.stoppingDistance) Move(agent.desiredVelocity);
            else Move(Vector3.zero);
        }
    }

    public void GetNextPosition()
    {
        if (!CheckIfYouAreOwner()) return;
        if (CurrentMovementBehaviour == MovementBehaviour.WalkToNode || CurrentMovementBehaviour == MovementBehaviour.WanderNearNode || CurrentMovementBehaviour == MovementBehaviour.RunToNode)
        {
            Vector3 NodePosition;
            if (MovementSettings.SpecificNode != -1) NodePosition = QuickFind.ScenePathNodes.ItemCatagoryList[MovementSettings.SpecificNode].transform.position;
            if (!MovementSettings.FollowSequence) NodePosition = QuickFind.ScenePathNodes.ItemCatagoryList[Random.Range(0, QuickFind.ScenePathNodes.ItemCatagoryList.Length)].transform.position;
            else
            {
                NodePosition = QuickFind.ScenePathNodes.GetItemFromID(nodeWanderSequenceID).transform.position;
                nodeWanderSequenceID += 1;
                if (nodeWanderSequenceID >= QuickFind.ScenePathNodes.ItemCatagoryList.Length) nodeWanderSequenceID = 0;
            }
            if (CurrentMovementBehaviour == MovementBehaviour.WanderNearNode) FindWanderPoint(NodePosition);
            else SetAgentDestination(NodePosition);
        }
        if (CurrentMovementBehaviour == MovementBehaviour.Wander) FindWanderPoint(transform.position);
        if (CurrentMovementBehaviour == MovementBehaviour.Guard) SetAgentDestination(_T.position);
    }

    void FindWanderPoint(Vector3 CenterPoint)
    {
        Vector3 point;
        for (int i = 0; i < 100; i++)
        {
            if (RandomPoint(CenterPoint, MovementSettings.NewPositionRadius, out point))
            {
                NavMeshPath path = new NavMeshPath();
                agent.CalculatePath(point, path);
                if (path.status == NavMeshPathStatus.PathPartial || path.status == NavMeshPathStatus.PathInvalid) continue;
                else { SetAgentDestination(point); return; }
            }
        }
        Debug.Log("valid wander spot could not be found. Agent using guard mode.");
        ChangeMovementBehaviourState(MovementBehaviour.Guard);
    }
    public void Move(Vector3 move)
    {
        if (move.magnitude > 1f) move.Normalize();
        move = transform.InverseTransformDirection(move);

        float m_TurnAmount = Mathf.Atan2(move.x, move.z);
        float m_ForwardAmount = move.z;
        float turnSpeed = Mathf.Lerp(MovementSettings.m_StationaryTurnSpeed, MovementSettings.m_MovingTurnSpeed, m_ForwardAmount);
        transform.Rotate(0, m_TurnAmount * turnSpeed * Time.deltaTime, 0);
    }

    //Net
    void SetAgentDestination(Vector3 Position)
    {
        if (OutMovementData == null) OutMovementData = new int[6];
        OutMovementData[0] = QuickFind.NetworkSync.CurrentScene;
        OutMovementData[1] = RelayNetworkObject.NetworkObjectID;
        OutMovementData[2] = QuickFind.ConvertFloatToInt(Position.x);
        OutMovementData[3] = QuickFind.ConvertFloatToInt(Position.y);
        OutMovementData[4] = QuickFind.ConvertFloatToInt(Position.z);
        OutMovementData[5] = (int)CurrentMovementBehaviour;
        QuickFind.NetworkSync.SendAIDestination(OutMovementData);
    }
    public void ReceiveAgentDestination(int[] InData)
    {
        Vector3 Position = QuickFind.ConvertIntsToPosition(InData[2], InData[3], InData[4]);
        agent.SetDestination(Position);
        CurrentMovementBehaviour = (MovementBehaviour)InData[5];
        switch(CurrentMovementBehaviour)
        {
            case MovementBehaviour.WalkToNode: agent.speed = MovementSettings.walkSpeed; break;
            case MovementBehaviour.Wander: agent.speed = MovementSettings.walkSpeed; break;
            case MovementBehaviour.WanderNearNode: agent.speed = MovementSettings.walkSpeed; break;
            case MovementBehaviour.Guard: agent.speed = MovementSettings.walkSpeed; break;

            case MovementBehaviour.RunToNode: agent.speed = MovementSettings.RunSpeed; break;
        }
    }

    #endregion


    #region Set Values
    public void SetMovementSettings(MovementOptions Settings) {MovementSettings = Settings;}
    void ChangeMovementBehaviourState(MovementBehaviour Behaviour) { CurrentMovementBehaviour = Behaviour; GetNextPosition(); }

    [Button(ButtonSizes.Small)] public void DebugShiftMovementState() { ChangeMovementBehaviourState(StartMovementBehaviour); }

    #endregion

    #region UTIL
    public void GetNavAgent() { agent = gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>(); agent.enabled = true; }
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
        result = Vector3.zero;
        return false;
    }

    //Network
    public bool CheckIfYouAreOwner()
    {
        if (QuickFind.NetworkSync.UserID == RelayNetworkObject.Scene.SceneOwnerID) return true;
        else return false;
    }
    #endregion
}
