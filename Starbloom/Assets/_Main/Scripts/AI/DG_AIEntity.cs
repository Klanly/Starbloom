using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Sirenix.OdinInspector;

public class DG_AIEntity : MonoBehaviour {

    #region CustomClasses

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

        [Header("MovementTypes")]
        public bool UseWander;
        [ShowIf("UseWander")]
        public WanderOptions[] Wander;

        public bool UseNodeWander;
        [ShowIf("UseNodeWander")]
        public NodeWanderOptions[] NodeWander;




        [System.Serializable]
        public class WanderOptions
        {
            public float wanderPauseTimeLow = 3.0f;
            public float wanderPauseTimeHigh = 6.0f;
            public float wanderRadius = 25.0f;
        }
        [System.Serializable]
        public class NodeWanderOptions
        {
            public bool followSequence;
            public float nodeWanderPauseTimeLow = 6.0f;
            public float nodeWanderPauseTimeHigh = 6.0f;
        }
    }
    #endregion

    #region Enums

    public enum AIBehaviourType
    {
        Move_Wander,
        Move_NodeWander,
        Move_Guard
    }
    public enum MovementStates
    {
        DontMove,
        WalkToTarget,
        RunToTarget,
        WaitBeforeNextMove
    }
    public enum AIDetectionType
    {
        SightAndRange,
        SightOnly,
        RangeOnly,
        GodAwareness
    }

    #endregion

    #region Variables

    public MovementOptions MovementSettings;

    [Header("Debug")]  
    [ReadOnly] public AIBehaviourType CurrentState;
    [ReadOnly] public MovementStates CurrentMovementState;
    [ReadOnly] public Transform TargetPosition;
    [ReadOnly] public float DistanceToTarget;
    public AIBehaviourType DebugSetBehaviourType;


    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public int findWanderSpotTimeOutMax = 1000;//how many times attempts to find a new wander spot before escaping to guard mode. You should never hit this point, but its here just in case.
    [HideInInspector] public int timeOutClock = 0; //used to keep track of wander spot attempts
    [HideInInspector] public int nodeWanderSequenceID = -1; //used to keep track of node targets when wandering through a sequence of nodes

    RaycastHit hit;
    bool WaitingForTimer;
    float Timer;

    #endregion


    private void OnEnable()
    {     
        agent = gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();
        GetNextPosition();
        agent.updateRotation = false;
        agent.updatePosition = true;
    }

    void Update()
    {
        HandleMovementUpdate();
    }



    #region Movement

    void HandleMovementUpdate()
    {
        if (!QuickFind.PathfindingGeneration.NavMeshIsGenerated) return;

        if (WaitingForTimer) { Timer -= Time.deltaTime; if (Timer < 0) { WaitingForTimer = false; GetNextPosition(); } }

        if (CurrentMovementState != MovementStates.DontMove)
        {
            bool DestinationReached = false;
            float RandTime = 0;
            if (TargetPosition == null) DestinationReached = true;
            else
            {
                DistanceToTarget = Vector3.Distance(transform.position, new Vector3(TargetPosition.position.x, transform.position.y, TargetPosition.position.z));
                if (DistanceToTarget <= agent.stoppingDistance + MovementSettings.stopDistanceAdjust && agent.speed != 0)
                {
                    DestinationReached = true;
                    if (CurrentState == AIBehaviourType.Move_NodeWander)
                        RandTime = Random.Range(MovementSettings.NodeWander[0].nodeWanderPauseTimeLow, MovementSettings.NodeWander[0].nodeWanderPauseTimeHigh);
                    if (CurrentState == AIBehaviourType.Move_Wander)
                        RandTime = Random.Range(MovementSettings.Wander[0].wanderPauseTimeLow, MovementSettings.Wander[0].wanderPauseTimeHigh);
                }
            }
            if (DestinationReached)
            {
                WaitingForTimer = true;
                agent.speed = 0;
                Timer = RandTime;
            }
        }

        //Move Entity
        if (agent.remainingDistance > agent.stoppingDistance) Move(agent.desiredVelocity, false, false);
        else Move(Vector3.zero, false, false);
    }

    public void GetNextPosition()
    {
        if (CurrentState == DG_AIEntity.AIBehaviourType.Move_NodeWander)
        {
            if (!MovementSettings.NodeWander[0].followSequence)
                TargetPosition = QuickFind.ScenePathNodes.ItemCatagoryList[Random.Range(0, QuickFind.ScenePathNodes.ItemCatagoryList.Length)].transform;
            if (MovementSettings.NodeWander[0].followSequence)
            {
                TargetPosition = QuickFind.ScenePathNodes.GetItemFromID(nodeWanderSequenceID).transform;
                nodeWanderSequenceID += 1;
                if (nodeWanderSequenceID >= QuickFind.ScenePathNodes.ItemCatagoryList.Length) nodeWanderSequenceID = 0;
            }
            CurrentMovementState = DG_AIEntity.MovementStates.WalkToTarget;
            agent.SetDestination(TargetPosition.position);
            agent.speed = MovementSettings.walkSpeed;
        }


        if (CurrentState == DG_AIEntity.AIBehaviourType.Move_Wander)
        {
            if (TargetPosition == null) NewTarget();
            Vector3 point;
            CurrentMovementState = DG_AIEntity.MovementStates.WaitBeforeNextMove;
            timeOutClock = 0;
            while (CurrentMovementState == DG_AIEntity.MovementStates.WaitBeforeNextMove && timeOutClock < findWanderSpotTimeOutMax)
            {
                if (RandomPoint(transform.position, MovementSettings.Wander[0].wanderRadius, out point))
                {
                    NavMeshPath path = new NavMeshPath();
                    agent.CalculatePath(point, path);
                    if (path.status == NavMeshPathStatus.PathPartial || path.status == NavMeshPathStatus.PathInvalid) timeOutClock += 1;
                    else
                    {
                        TargetPosition.position = point;
                        agent.SetDestination(TargetPosition.position);// = targetWayPoint;
                        CurrentMovementState = DG_AIEntity.MovementStates.WalkToTarget;
                    }
                }
                else timeOutClock += 1;
            }
            if (timeOutClock >= findWanderSpotTimeOutMax)
            {
                Debug.Log("valid wander spot could not be found. Agent using guard mode.");
                CurrentState = DG_AIEntity.AIBehaviourType.Move_Guard;
            }

            if (CurrentMovementState == DG_AIEntity.MovementStates.WalkToTarget) agent.speed = MovementSettings.walkSpeed;
        }
        if (CurrentState == DG_AIEntity.AIBehaviourType.Move_Guard)
        {
            if (TargetPosition == null) NewTarget();
            TargetPosition.position = transform.position;
            TargetPosition.rotation = transform.rotation;
            CurrentMovementState = DG_AIEntity.MovementStates.DontMove;
            agent.speed = 0;
        }
    }

    public void Move(Vector3 move, bool crouch, bool jump)
    {
        if (move.magnitude > 1f) move.Normalize();
        move = transform.InverseTransformDirection(move);

        float m_TurnAmount = Mathf.Atan2(move.x, move.z);
        float m_ForwardAmount = move.z;
        float turnSpeed = Mathf.Lerp(MovementSettings.m_StationaryTurnSpeed, MovementSettings.m_MovingTurnSpeed, m_ForwardAmount);
        transform.Rotate(0, m_TurnAmount * turnSpeed * Time.deltaTime, 0);
    }

    #endregion







    #region UTIL
    //***********************RANDOM POINT************************************	
    void NewTarget()
    {
        TargetPosition = new GameObject().transform;
        TargetPosition.SetParent(QuickFind.AIMasterRef.transform);
    }

    void ChangeBehaviourState(AIBehaviourType BehaviourState)
    {
        if (CurrentState == AIBehaviourType.Move_NodeWander) TargetPosition = null; 
        else Destroy(TargetPosition.gameObject);
        CurrentState = BehaviourState;
        CurrentMovementState = MovementStates.WaitBeforeNextMove;
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
        result = Vector3.zero;
        return false;
    }

    [Button(ButtonSizes.Small)]
    public void ChangeToDebugBehaviourState()
    {
        ChangeBehaviourState(DebugSetBehaviourType);
    }
    #endregion
}
