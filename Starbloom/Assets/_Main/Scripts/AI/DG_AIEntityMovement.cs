using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Sirenix.OdinInspector;

public class DG_AIEntityMovement : MonoBehaviour {

    [System.Serializable]
    public class MovementOptions
    {
        public MovementBehaviour DefaultMovementBehaviour;
        public MovementBehaviour DebugMovementBehaviour;
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

    #region Variables

    //Movement
    public enum MovementBehaviour
    {
        InitialLoad,
        //Stopped States
        Stopped,
        GuardNearNode,
        //Wander States
        Wander,
        WanderNearNode,
        WanderNearGeneratedPosition,
        //Following A Node Path
        WalkToNode,
        RunToNode,
        //Go To a new Location
        WalkToTargetLocation,
        RunToTargetLocation,
        WalkToDetectedTarget,
        RunToDetectedTarget
    }

    //Movement
    [ReadOnly] public MovementBehaviour CurrentMovementBehaviour;
    [ReadOnly] public int nodeWanderSequenceID = -1;

    [System.NonSerialized] public DG_AIEntity Entity;
    [System.NonSerialized] public MovementOptions MovementSettings;
    [System.NonSerialized] public NavMeshAgent agent;
    [System.NonSerialized] public bool ResumeMoveOrderOnInitialize = false;

    [System.NonSerialized] public Vector3 RequestedMoveLocation;
    bool WaitingForTimer;
    float Timer;

    #endregion

    private void Awake()
    {
        this.enabled = false;
    }


    void Update()
    {
        if (CurrentMovementBehaviour == MovementBehaviour.InitialLoad) InitialLoad();
        HandleMovementUpdate();
    }


    private void InitialLoad()
    {
        CurrentMovementBehaviour = MovementSettings.DefaultMovementBehaviour;
        agent.updateRotation = false;
        agent.updatePosition = true;
        if (ResumeMoveOrderOnInitialize)
        {
            DG_AIEntity.AIDestinationTransfer DataTransfer = Entity.RelayNetworkObject.AICharData[0];
            Entity.DestinationMoveOrder(DataTransfer.DestinationX, DataTransfer.DestinationY, DataTransfer.DestinationZ, DataTransfer.BehaviourType);
        }
        else
            GetNextPosition();
    }


    #region Movement

    void HandleMovementUpdate()
    {
        if (!QuickFind.PathfindingGeneration.NavMeshIsGenerated) return;

        if (WaitingForTimer) { Timer -= Time.deltaTime; if (Timer < 0) { WaitingForTimer = false; GetNextPosition(); } }

        if (agent.speed != 0)
        {
            Entity.RelayNetworkObject.AICharData[0].DestinationReached = false;
            float RandTime = 0;
            Vector3 Destination = agent.destination;

            float DistanceToTarget = Vector3.Distance(transform.position, new Vector3(Destination.x, transform.position.y, Destination.z));
            if (DistanceToTarget <= agent.stoppingDistance + MovementSettings.stopDistanceAdjust)
            {
                //Reached Destination
                Entity.RelayNetworkObject.AICharData[0].DestinationReached = true;
                RandTime = Random.Range(0, MovementSettings.NodeLingerRange);
            }
            if (Entity.RelayNetworkObject.AICharData[0].DestinationReached) { WaitingForTimer = true; agent.speed = 0; Timer = RandTime; }
            //Move Entity
            if (agent.remainingDistance > agent.stoppingDistance) Move(agent.desiredVelocity);
            else Move(Vector3.zero);
        }
    }

    public void GetNextPosition()
    {
        if (!Entity.CheckIfYouAreOwner()) return;
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
            if (CurrentMovementBehaviour == MovementBehaviour.WanderNearNode) Entity.SetAgentDestination(Entity.FindRandomNavMeshPoint(NodePosition, MovementSettings.NewPositionRadius), (int)CurrentMovementBehaviour);
            else Entity.SetAgentDestination(NodePosition, (int)CurrentMovementBehaviour);
        }
        //General Point Assigned From External Script.
        if (CurrentMovementBehaviour == MovementBehaviour.WalkToTargetLocation || CurrentMovementBehaviour == MovementBehaviour.RunToTargetLocation)
            Entity.SetAgentDestination(RequestedMoveLocation, (int)CurrentMovementBehaviour);
        //Target Transform Assigned From External Script.
        if (CurrentMovementBehaviour == MovementBehaviour.WalkToDetectedTarget || CurrentMovementBehaviour == MovementBehaviour.RunToDetectedTarget)
            Entity.SetAgentDestination(Entity.Detection.DetectedTarget.position, (int)CurrentMovementBehaviour);

        if (CurrentMovementBehaviour == MovementBehaviour.Wander) Entity.SetAgentDestination(Entity.FindRandomNavMeshPoint(transform.position, MovementSettings.NewPositionRadius), (int)CurrentMovementBehaviour);
        if (CurrentMovementBehaviour == MovementBehaviour.Stopped) Entity.SetAgentDestination(Entity._T.position, (int)CurrentMovementBehaviour);
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
    public void ReceiveMoveOrder(Vector3 Position, MovementBehaviour Behaviour)
    {
        agent.SetDestination(Position);

        CurrentMovementBehaviour = Behaviour;
        switch (CurrentMovementBehaviour)
        {
            case MovementBehaviour.WalkToNode: agent.speed = MovementSettings.walkSpeed; break;
            case MovementBehaviour.Wander: agent.speed = MovementSettings.walkSpeed; break;
            case MovementBehaviour.WanderNearNode: agent.speed = MovementSettings.walkSpeed; break;
            case MovementBehaviour.Stopped: agent.speed = MovementSettings.walkSpeed; break;
            case MovementBehaviour.WalkToDetectedTarget: agent.speed = MovementSettings.walkSpeed; break;
            case MovementBehaviour.WalkToTargetLocation: agent.speed = MovementSettings.walkSpeed; break;

            case MovementBehaviour.RunToNode: agent.speed = MovementSettings.RunSpeed; break;
            case MovementBehaviour.RunToDetectedTarget: agent.speed = MovementSettings.RunSpeed; break;
            case MovementBehaviour.RunToTargetLocation: agent.speed = MovementSettings.RunSpeed; break;
        }
    }

    #endregion


    #region Set Values
    public void SetMovementSettings(MovementOptions Settings) { MovementSettings = Settings; }
    public void ChangeMovementBehaviourState(MovementBehaviour Behaviour) { CurrentMovementBehaviour = Behaviour; GetNextPosition(); }

    [Button(ButtonSizes.Small)] [HideInEditorMode] public void SetToDebugMovementState() { ChangeMovementBehaviourState(MovementSettings.DebugMovementBehaviour); }

    #endregion

    #region UTIL
    public void GetNavAgent() { agent = gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>(); agent.enabled = true; }
    #endregion
}
