//************************************COPYRIGHT 2017 CURTIS MARTINDALE***********************************
//***********************************************Version 1.0*********************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class node_AIMovement : MonoBehaviour
{

    public enum AIType { wander, nodeWander, wayPoints, guard };
    public AIType aiType;
    public enum ChaseType { direct, surround };
    [Tooltip("Direct is recommend for melee characters.")]
    public ChaseType chaseType;
    [Tooltip("Main Interaction layers. Select all enemy, player and level geometry layers. Typically only 'default' and 'LocalPlayer' need to be selected.")]
    public LayerMask interactionLayers;
    [Tooltip("this layer is used for agents to communicate. Typical use is to alert other agents to chase the player, for instance. Typically just the 'Enemy layer")]
    public LayerMask enemyLayers;
    [Tooltip("Drag in your character model here.")]
    public Transform myBody;

    [SerializeField] private WanderOptions wanderSettings = new WanderOptions();
    [SerializeField] public NodeWanderOptions nodeWanderSettings = new NodeWanderOptions();
    [SerializeField] public WayPointOptions wayPointSettings = new WayPointOptions();
    [SerializeField] public SpeedOptions speedSettings = new SpeedOptions();
    [SerializeField] private DistanceOptions distanceSettings = new DistanceOptions();
    [SerializeField] private MarkerOptions markerSettings = new MarkerOptions();
    [SerializeField] public DetectionOptions detectionSettings = new DetectionOptions();
    [SerializeField] public AdvancedOptions advancedSettings = new AdvancedOptions();

    [System.Serializable]
    public class WanderOptions
    {
        [Tooltip("minimum time agent will pause before picking a new wander destination")]
        public float wanderPauseTimeLow = 3.0f;
        [Tooltip("max time agent will pause before picking a new wander destination")]
        public float wanderPauseTimeHigh = 6.0f;
        [Tooltip("max distance from current position agent will wander")]
        public float wanderRadius = 25.0f;
    }
    [System.Serializable]
    public class NodeWanderOptions
    {
        [Tooltip("If true, the agent will only wander to nodes with the group name defined in the 'Node Group Name' setting")]
        public bool useNodeGroup = false;
        [Tooltip("If 'Use Node Group' is true, the name of the node group the agent can wander through")]
        public string nodeGroupName;
        [Tooltip("If true, the agent will wander through its list of nodes in order, good for patrol type behaviour. If false, the agent will pick a random node each time, good for search behavior")]
        public bool followSequence;
        [Tooltip("minimum time agent will pause before picking a new node destination")]
        public float nodeWanderPauseTimeLow = 6.0f;
        [Tooltip("max time agent will pause before picking a new node destination")]
        public float nodeWanderPauseTimeHigh = 6.0f;
    }
    [System.Serializable]
    public class WayPointOptions
    {
        [Tooltip("minimum amount of time AI will pause when it reaches a wayPoint")]
        public float patrolStopTimeLow = 4.0f;
        [Tooltip("max amount of time AI will pause when it reaches a wayPoint")]
        public float patrolStopTimeHigh = 8.0f;
        [Tooltip("this allows you to specify unique stop times for each wayPoint. set to false if not sure.")]
        public bool useCustomWPTime;
        [Tooltip("if useCustomWPTime is true, use this to set the stop times.")]
        public int[] wayPointStopTime;
        [Tooltip("this allows AI to trigger scripts at custom wayPoints")]
        public bool useWayPointScript;
        [Tooltip("the list of wayPoints this AI will patrol")]
        public Transform[] wayPoints;
    }

    [System.Serializable]
    public class SpeedOptions
    {
        [Tooltip("speed to chase target")]
        public float chaseSpeed = 1.0f;
        [Tooltip("normal speed for agent, used during its non chase behaviors")]
        public float walkSpeed = 0.5f;
        [Tooltip("Helps character turn faster")]
        public float m_MovingTurnSpeed = 360;
        [Tooltip("Helps character turn faster")]
        public float m_StationaryTurnSpeed = 180;
        [Tooltip("Multiplies the agents move speed. Best left at 1")]
        public float m_MoveSpeedMultiplier = 1f;
        [Tooltip("Multiplies the agents animation speed. Best left at 1")]
        public float m_AnimSpeedMultiplier = 1f;
        [Tooltip("Adjust the speed the agent looks at the player")]
        public float rotateSpeed = 7.0f;
        [Tooltip("Used to smooth out movement transistions. Higher values are smoother but less responsive")]
        public float smoothMove = 0.2f;
    }

    [System.Serializable]
    public class DistanceOptions
    {
        [Tooltip("Max distance agent will stop chasing player. Set a higher distance for ranged enemies, and a short distance for melee enemies, for example.")]
        public float stoppingDistanceHigh = 7.0f;
        [Tooltip("Minimum distance agent will stop chasing player. Set both stopping distance settings if you want the enemy to stop chasing the player at a set distance. With a melee character for instance, you would probably want both settings to be 0.5 or 1.")]
        public float stoppingDistanceLow = 2.5f;
        [Tooltip("the maximum distance at which the enemy will attempt to attack player")]
        public float attackingDistance = 100.0f;
        [Tooltip("the distance at which a enemy will detect a player, regardless of position.")]
        public float personalAwarenessDistance = 3.0f;
    }

    [System.Serializable]
    public class MarkerOptions
    {
        [Tooltip("Optional object used to ID AI on players HUD. typically a pointer or some other object above enemy head.")]
        public GameObject myMark;
        [Tooltip("activates AI marker when it notices player. ")]
        public bool markActiveOnAlert = true;
        [Tooltip("marker color when AI is completely unaware of player")]
        public Color markerUnawareColor = Color.green;
        [Tooltip("color when player is in range of AI. if AI makes visual contact with Player, it could notice player")]
        public Color markerCautionColor = Color.yellow;
        [Tooltip("color when AI is chasing/attacking player")]
        public Color markerAttackColor = Color.red;
    }

    [System.Serializable]
    public class DetectionOptions
    {
        [Tooltip("toggles rather the AI can detect enemies all around him. If false, AI can only detect player in front.")]
        public bool omniVision = false;
        [Tooltip("default distance the AI can see")]
        public float normalVisionDistance = 40.0f;
        [Tooltip("how far the AI can see when pursuing player")]
        public float pursueVisionDistance = 100.0f;
        [HideInInspector]
        public float visionDistance = 40.0f; // AIs current vision distance
        [Tooltip("Angle which AI can see")]
        public float viewAngle = 110.0f;
        [Tooltip("the fastest the AI can react to seeing the player")]
        public float reactionTimeLow = 0.5f;
        [Tooltip("the slowest the AI can react to seeing the player")]
        public float reactionTimeHigh = 5.0f;
        [Tooltip("Affects how distance from player changes reaction time")]
        public float reactionTimeSmooth = 0.15f;
        [Tooltip("how long an AI will search/chase a player after loosing sight ")]
        public float searchTime = 10.0f;
        [Tooltip("how long an AI will campout at players last known location ")]
        public float campTime = 2.0f;
        [Tooltip("if true, the agent will alert other agents while chasing the player.")]
        public bool alertOtherAgents = true;
        [Tooltip("Adjust where the agent looks at the player. By default, agents will look at the center of the player, but the center of the player may be behind cover. Raising this value would have the agent look for the players head instead.")]
        public float heightOffset;
        [Tooltip("transform used to search for player. Typically set around AI models eye height")]
        public Transform myEyes;
    }

    [System.Serializable]
    public class AdvancedOptions
    {
        [Tooltip("If true, instead of chasing an enemy this agent will maintain its position and attack")]
        public bool holdGround;
        [Tooltip("How long it takes in between instances the agent can be woken up/alerted by other enemies ")]
        public float wakeUpTimeOut = 60.0f;
        [Tooltip("If true, a line will be drawn in the editor between the agent and its current target ")]
        public bool debugTarget;
        [Tooltip("If debugTarget is true, the color of the line between the agent and its current target ")]
        public Color debugColor;
        [Tooltip("How frequent the players position is updated. Lower value is better quality but higher performance cost")]
        public float playerUpdateInterval = 0.1f;
        [Tooltip("Adjust how close an agent needs to get to its target to trigger the script that it has arrived at its destination")]
        [Range(0.0f, 1.0f)]
        public float stopDistanceAdjust = 0.5f;
        [Tooltip("The minimum amount of time an agent will stay at one position in surround mode")]
        public float positionChangeLow = 5.0f;
        [Tooltip("The mmaximum amount of time an agent will stay at one position in surround mode")]
        public float positionChangeHigh = 10.0f;
    }

    [HideInInspector]
    public bool canSeePlayer; //true if AI can "see" player, if raycast from AI eye position hits player
    [HideInInspector]
    public bool chase; //chase target toggle
    [HideInInspector]
    public bool checkingForPlayer; //true if AI is checking for player
    [HideInInspector]
    public bool attackOk; //used to activate attacking behavior for animation and shooting scripts. Generally speaking, if true this means "agent is in range of and ready to interact with target"
    [HideInInspector]
    public bool pauseMovement; //coming soon
    [HideInInspector]
    public Transform target; // refrence to players Transform
    [HideInInspector]
    public Transform targetWayPoint; //guess? :) The, er, target waypoint
    [HideInInspector]
    public float distance; //distance 2 target
    [HideInInspector]
    public float playerDistance; //distance 2 player
    [HideInInspector]
    public float stoppingDistance; //distance to stop chasing player, hold ground and attack
    [HideInInspector]
    public NavMeshAgent agent;

    bool adjustRotation; // if true, the agent is adjusting its rotation to match its guard location
    bool cautious; //if true, player is within vision range of AI
    bool campLastKnownSpot; //true while AI is camping players last known position
    bool lostTrailCheck; //while true, if campLostKnownSpot is enabled, the player stops movement and investigates players last known spot
    bool moveToGoalNode; // if true, agent is moving to its goal node
    bool moveToWanderSpot; //if true, agent is moving to new wander destination
    bool moveToWayPoint; //used to determine if AI should move to next wayPoint
    bool postedUp; //true if this agent has found a spot to attack player in surround mode.
    bool returnToGuardSpot; // if true, agent is returing to guard spot after chasing player
    bool timerReset; //if true, resets reaction timer
    bool useWander; //if true, AI will wander
    bool useWayPoints; //if true, AI will follow wayPoints
    bool useNodeWander; //if true, AI will wander from node to node. 
    bool useGuard; //if true, AI will guard its start location. 
    bool wokenUp; //used to prevent wakeup function being called repeatedly. True if ai has been alerted by other ai/player.
    bool wanderSpotFound; //used for while loop to find new wander spot

    float isInFront; //float used to calculate if player is in front of AI
    float guardSpotDistance; //how far ai is from initial guard spot
    float m_TurnAmount;
    float m_ForwardAmount;
    float nodeGoalDistance; //how far ai is from goal node
    float wayPointDistance; //distance to next wayPoint
    float wanderSpotDistance; //distance to wanderSpot
    float step; //frequency of movement calculations
    float reactionTime; //how quick the AI reacts to player in range. changes depending on distance, and min/max reactions times, and reaction time smoothing
    float startTime; //used for reaction time calculations
    float tempReactionTime; //how long the AI will look for a player it "noticed". a snapshot taken from the current reaction time (above)
    float tempFloater; //used to lerp marker color when detecting player.
    float xOffset; //actual xoffset derived from xOffsetFactor
    float zOffset; //actual zoffset derived from xOffsetFactor

    node_AIAnimation myAnim; //refrence to animation script
    public node_NodeManager nManager; // refrence to the node manager gameObject
    int findWanderSpotTimeOutMax = 1000;//how many times attempts to find a new wander spot before escaping to guard mode. You should never hit this point, but its here just in case.
    int timeOutClock = 0; //used to keep track of wander spot attempts
    int nodeWanderSequenceID; //used to keep track of node targets when wandering through a sequence of nodes
    int wayPointIndex; //used for picking wayPoints
    List<Transform> myNodes = new List<Transform>(); // a list of nodes
    RaycastHit hit; //Generic raycast Hit
    Transform goalNode;  //agents current goal node, if using nodes
    Transform guardSpot; // when using guardMode, the transform that is spawned at start and the agent will guard
    Transform wanderSpot; //transform used to move agent towards new wander destination
    Vector3 rotationTarget; //used for rotation calculations
    Vector3 targetPosition; //the position the agent is moving towards. This could be the player, or a spot around the player if you are using surround mode.

    Coroutine trailCheck;
    Coroutine playerGone;
    Coroutine locatePlayer;



    //***********************START************************************	
    void OnEnable()
    {
        if (this.GetComponent<node_AIAnimation>() != null)
        {
            myAnim = this.GetComponent<node_AIAnimation>();
        }
        if (GameObject.FindGameObjectWithTag("GameController") != null)
        {
            nManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<node_NodeManager>();
        }
        if (GameObject.FindGameObjectWithTag("Player"))
        {
            target = GameObject.FindGameObjectWithTag("Player").transform; //finds player in scene
        }

        SetupAgent();
    }



    //***********************UPDATE************************************	
    void Update()
    {
        if (!QuickFind.PathfindingGeneration.NavMeshIsGenerated) return;

        if (target != null)
        {
            playerDistance = Vector3.Distance(transform.position, target.position); //Gets Distance to target/player

            if (!chase)
            {
                distance = Vector3.Distance(transform.position, target.position); //Gets Distance to target/player
            }
            if (chase)
            {
                if (chaseType == ChaseType.direct)
                {
                    targetPosition = target.position;
                    distance = Vector3.Distance(transform.position, targetPosition); //Gets Distance to target/player
                }
                if (chaseType == ChaseType.surround)
                {
                    distance = Vector3.Distance(transform.position, targetPosition); //Gets Distance to target/player
                }
            }
        }
        reactionTime = Mathf.Clamp(distance * detectionSettings.reactionTimeSmooth, detectionSettings.reactionTimeLow, detectionSettings.reactionTimeHigh);

        //******ASIGNED BEHAVIOR*************************
        //NODE WANDER ROUTINE
        if (useNodeWander && goalNode && moveToGoalNode)
        {
            nodeGoalDistance = Vector3.Distance(transform.position, goalNode.position);

            if (nodeGoalDistance <= agent.stoppingDistance + advancedSettings.stopDistanceAdjust && agent.speed != 0)
            {
                StartCoroutine(nodeWanderHit(Random.Range(nodeWanderSettings.nodeWanderPauseTimeLow, nodeWanderSettings.nodeWanderPauseTimeHigh), false));
            }
            if (advancedSettings.debugTarget)
            {
                Debug.DrawLine(detectionSettings.myEyes.position, goalNode.position, advancedSettings.debugColor);
            }
        }
        //GUARD ROUTINE
        if (useGuard && returnToGuardSpot)
        {
            guardSpotDistance = Vector3.Distance(transform.position, guardSpot.position);

            if (guardSpotDistance <= agent.stoppingDistance + advancedSettings.stopDistanceAdjust && agent.speed != 0)
            {
                guardSpotHit();
            }
            if (advancedSettings.debugTarget)
            {
                Debug.DrawLine(detectionSettings.myEyes.position, guardSpot.position, advancedSettings.debugColor);
            }
        }
        //WAYPOINT ROUTINE
        if (moveToWayPoint)
        {
            if (!wayPointSettings.useWayPointScript)
            {
                wayPointDistance = Vector3.Distance(transform.position, targetWayPoint.position);
            }
            if (!wayPointSettings.useWayPointScript && wayPointDistance <= agent.stoppingDistance + advancedSettings.stopDistanceAdjust && agent.speed != 0)
            {
                StartCoroutine(wayPointHit(Random.Range(wayPointSettings.patrolStopTimeLow, wayPointSettings.patrolStopTimeHigh), false));
            }
            if (advancedSettings.debugTarget)
            {
                Debug.DrawLine(detectionSettings.myEyes.position, targetWayPoint.position, advancedSettings.debugColor);
            }
        }
        //WANDER ROUTINE
        if (moveToWanderSpot)
        {
            if(wanderSpot == null) StartCoroutine(wanderSpotHit(Random.Range(wanderSettings.wanderPauseTimeLow, wanderSettings.wanderPauseTimeHigh), false));

            wanderSpotDistance = Vector3.Distance(transform.position, new Vector3(wanderSpot.position.x, transform.position.y, wanderSpot.position.z));
            if (wanderSpotDistance <= agent.stoppingDistance + advancedSettings.stopDistanceAdjust && agent.speed != 0)
            {
                StartCoroutine(wanderSpotHit(Random.Range(wanderSettings.wanderPauseTimeLow, wanderSettings.wanderPauseTimeHigh), false));
            }
            if (advancedSettings.debugTarget)
            {
                Debug.DrawLine(detectionSettings.myEyes.position, new Vector3(wanderSpot.position.x, transform.position.y, wanderSpot.position.z), advancedSettings.debugColor);
            }
        }
        //************PLAYER DETECTION ROUTINES*****************
        //Detects player within a certain range, regardless if player is visible or not
        if (target != null && distance <= distanceSettings.personalAwarenessDistance && !chase)
        {
            if (playerVisible(detectionSettings.myEyes.position) == true)
            {
                pursuePlayer(true);
            }
        }
        //if target is in front of AI, in vision range, and AI can see (raycast hit) player, start chasing
        if (distance < detectionSettings.visionDistance && !chase)
        {
            if (!cautious)
            {
                if (markerSettings.myMark)
                {
                    markerSettings.myMark.GetComponent<Renderer>().material.color = markerSettings.markerCautionColor;
                }
                cautious = true;
            }
            if (!detectionSettings.omniVision && target != null)
            {
                isInFront = Vector3.Dot(target.position - transform.position, transform.forward);
                if (isInFront > 0 && !chase)
                {
                    Vector3 direction = target.position - transform.position;
                    float angle = Vector3.Angle(direction, transform.forward);
                    if (angle < detectionSettings.viewAngle * 0.5f && Physics.Raycast(detectionSettings.myEyes.position, new Vector3(target.position.x, target.position.y + detectionSettings.heightOffset, target.position.z) - detectionSettings.myEyes.position, out hit, detectionSettings.visionDistance, interactionLayers))
                    {
                        if (hit.collider.gameObject.layer == target.gameObject.layer)
                        {
                            if (!checkingForPlayer)
                            {
                                activateMark(true);
                                if (locatePlayer != null)
                                {
                                    StopCoroutine(locatePlayer);
                                }
                                locatePlayer = StartCoroutine(detectPlayer());
                            }
                        }
                    }
                }
            }
            //this is an option for the AI to pursue player simply if player is in range. no raycasting involved
            if (detectionSettings.omniVision)
            {
                if (playerVisible(detectionSettings.myEyes.position) == true)
                {
                    pursuePlayer(true);
                }
                else
                {
                    pursuePlayer(false);
                }
            }
        }
        //stops being cautious if player is out of range
        if (distance > detectionSettings.visionDistance && cautious && !checkingForPlayer)
        {
            cautious = false;
            if (markerSettings.myMark)
            {
                markerSettings.myMark.GetComponent<Renderer>().material.color = markerSettings.markerUnawareColor;
            }
        }
        //Reaction to player sighting
        if (checkingForPlayer)
        {
            if (!timerReset)
            {
                startTime = Time.time;
                timerReset = true;
            }
            if (cautious && isInFront > 0 && playerVisible(detectionSettings.myEyes.position) == true)
            {
                if (myBody)
                {
                    lookAtPlayer();
                }
                tempFloater = Mathf.Lerp(0, 1, (Time.time - startTime) / tempReactionTime);
                if (markerSettings.myMark)
                {
                    markerSettings.myMark.GetComponent<Renderer>().material.color = Color.Lerp(markerSettings.markerCautionColor, markerSettings.markerAttackColor, tempFloater);
                }
            }
        }

        //************CHASING AND ATTACKING PLAYER*****************
        //stop chasing player if in attack range 
        if (distance <= stoppingDistance && chase && playerVisible(detectionSettings.myEyes.position) == true && agent.speed != 0)
        {
            agent.speed = 0;
        }
        //start chasing player again if they moves out of stopping distance
        if (distance > stoppingDistance && chase && agent.speed != speedSettings.chaseSpeed)
        {
            agent.speed = speedSettings.chaseSpeed;
        }
        //stops firing at player if out of range
        if (distance > distanceSettings.attackingDistance && attackOk || lostTrailCheck && attackOk)
        {
            attackOk = false;
        }
        //fire at player if in range
        if (distance <= distanceSettings.attackingDistance && !attackOk && chase)
        {
            attackOk = true;
        }

        //*******INVESTIGATING PLAYERS TRAIL/LAST KNOWN POSITION ROUTINE************
        if (chase)
        {// && canSeePlayer){
            if (advancedSettings.debugTarget)
            {
                Debug.DrawLine(detectionSettings.myEyes.position, targetPosition, advancedSettings.debugColor);
            }
            if (playerVisible(detectionSettings.myEyes.position) == false)
            {
                canSeePlayer = false;
                if (!lostTrailCheck)
                {
                    lostTrailCheck = true;
                    if (trailCheck != null)
                    {
                        StopCoroutine(trailCheck);
                    }
                    trailCheck = StartCoroutine(lostTrailTimeOut());
                }
            }
            else
            {
                if (myBody)
                {
                    lookAtPlayer();
                }
                canSeePlayer = true;
                if (trailCheck != null)
                {
                    StopCoroutine(trailCheck);
                }
                lostTrailCheck = false;
            }
        }
        else if (myBody && myBody.localEulerAngles.y != 0)
        {
            myBody.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f); //used to reset rotation from lookAtPlayer
        }

        //*******DRIVE AGENT************
        if (agent.remainingDistance > agent.stoppingDistance)
        {
            Move(agent.desiredVelocity, false, false);
        }
        else
            Move(Vector3.zero, false, false);
        if (adjustRotation)
        {
            Vector3 newDir = Vector3.RotateTowards(transform.forward, rotationTarget, step, 00.0F);
            transform.rotation = Quaternion.LookRotation(newDir);
            if (transform.rotation == guardSpot.rotation)
            {
                adjustRotation = false;
            }
        }
        step = speedSettings.rotateSpeed * Time.deltaTime;
    }

    //	***********
    void SetupAgent()
    {
        agent = gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();

        if (markerSettings.myMark)
        {
            markerSettings.myMark.GetComponent<Renderer>().material.color = markerSettings.markerUnawareColor; //sets marker color to default
        }
        //gets random position for enemy to stop chasing player and hold ground. set stoppingDistanceLow and High to the same float if you want a set distance
        stoppingDistance = Random.Range(distanceSettings.stoppingDistanceLow, distanceSettings.stoppingDistanceHigh);
        detectionSettings.visionDistance = detectionSettings.normalVisionDistance; // sets vision distance to default
        startTime = Time.time;

        //Assigning ai behavior
        switch (aiType)
        {
            case AIType.wander: useWander = true; break;
            case AIType.wayPoints: useWayPoints = true; break;
            case AIType.nodeWander: useNodeWander = true; break;
            case AIType.guard: useGuard = true; break;
        }
        if (useNodeWander && nManager)
        {
            if (nodeWanderSettings.useNodeGroup)
            { //Adds nodes with the specified name. 
                foreach (Transform x in nManager.nodes)
                {
                    if (x.GetComponent<node_Node>().nodeGroup == nodeWanderSettings.nodeGroupName)
                    {
                        myNodes.Add(x);
                    }
                }
            }
            else
            {
                foreach (Transform x in nManager.nodes)
                {
                    myNodes.Add(x);
                }
            }
            if (myNodes.Count == 0)
            {
                Debug.LogWarning("No suitable Nodes");
                useNodeWander = false;
                useGuard = true;
            }
            //assign a goal node
            if (useNodeWander)
            {
                if (!nodeWanderSettings.followSequence)
                {
                    goalNode = myNodes[Random.Range(0, myNodes.Count - 1)];
                }
                if (nodeWanderSettings.followSequence)
                {
                    goalNode = myNodes[0];
                    nodeWanderSequenceID = 0;
                }
                agent.speed = speedSettings.walkSpeed;
                moveToGoalNode = true;
                agent.SetDestination(goalNode.position);
            }
        }
        if (useNodeWander && !nManager)
        {
            useNodeWander = false;
            useGuard = true;
            Debug.Log("no node manager. using guard mode for " + this.gameObject.name);
        }
        if (useWayPoints)
        {
            if (!wayPointSettings.useWayPointScript && wayPointSettings.wayPoints.Length != 0)
            {
                targetWayPoint = wayPointSettings.wayPoints[0];
                wayPointIndex = 0;
                agent.speed = speedSettings.walkSpeed;
                if (wayPointSettings.wayPointStopTime.Length != wayPointSettings.wayPoints.Length)
                {
                    wayPointSettings.useCustomWPTime = false;
                }
                moveToWayPoint = true;
                agent.SetDestination(targetWayPoint.position);
            }
            if (wayPointSettings.useWayPointScript && this.GetComponent<node_ConditionalWayPointHandler>().myWayPoints.Length != 0)
            {
                targetWayPoint = this.GetComponent<node_ConditionalWayPointHandler>().myTargetWayPoint;
                agent.speed = speedSettings.walkSpeed;
                moveToWayPoint = true;
                agent.SetDestination(targetWayPoint.position);
            }
            if (wayPointSettings.useWayPointScript && this.GetComponent<node_ConditionalWayPointHandler>().myWayPoints.Length == 0 || wayPointSettings.wayPoints.Length == 0 && !wayPointSettings.useWayPointScript)
            { //if there are no wayPoints to follow, defaults to wander mode
                Debug.LogWarning("no way points found. Agent set to wander mode.");
                useWander = true;
                moveToWayPoint = false;
                useWayPoints = false;
                wayPointSettings.useCustomWPTime = false;
            }
            if (wayPointSettings.useWayPointScript)
            {
                wayPointSettings.useCustomWPTime = false;
            }
        }
        if (useWander)
        {
            moveToWayPoint = false;
            useWayPoints = false;
            useGuard = false;
            useNodeWander = false;
            agent.speed = speedSettings.walkSpeed;
            GameObject wanderObject = new GameObject();
            wanderObject.name = this.name + " wander target";
            wanderSpot = wanderObject.transform;
            Vector3 point;
            while (!wanderSpotFound && timeOutClock < findWanderSpotTimeOutMax)
            {
                if (RandomPoint(transform.position, wanderSettings.wanderRadius, out point))
                {
                    NavMeshPath path = new NavMeshPath();
                    agent.CalculatePath(point, path);
                    if (path.status == NavMeshPathStatus.PathPartial || path.status == NavMeshPathStatus.PathInvalid)
                    {
                        wanderSpotFound = false;
                        Debug.Log("partial or invalid path. fix navmesh");
                        timeOutClock += 1;
                    }
                    else
                    {
                        wanderSpot.position = point;
                        agent.SetDestination(wanderSpot.position);// = targetWayPoint;
                        moveToWanderSpot = true;
                        wanderSpotFound = true;
                    }
                }
                else
                {
                    wanderSpotFound = false;
                    timeOutClock += 1;
                }
            }
            if (timeOutClock >= findWanderSpotTimeOutMax)
            {
                Debug.Log("valid wander spot could not be found.");
                //useWander = false;
                //useGuard = true;
            }
        }
        if (useGuard)
        {
            GameObject startLocation = new GameObject();
            startLocation.name = this.name + " guard Spot";
            startLocation.transform.position = this.transform.position;
            startLocation.transform.rotation = this.transform.rotation;
            guardSpot = startLocation.transform;
            agent.speed = 0;
        }
        setKinematic(true);
        agent.updateRotation = false;
        agent.updatePosition = true;
        //	m_Rigidbody = GetComponent<Rigidbody>();
        //	m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        speedSettings.m_MoveSpeedMultiplier = 0.5f;
    }



    //***********************ACTIVATE MARK************************************	
    public void activateMark(bool turnOn)
    {
        if (markerSettings.myMark != null)
            markerSettings.myMark.GetComponent<Renderer>().enabled = turnOn;
    }

    //***********************ALERT OTHERS************************************
    void alertOthers()
    {
        var hitColliders1 = Physics.OverlapSphere(transform.position, 7.0f, enemyLayers);
        for (var i = 0; i < hitColliders1.Length; i++)
        {
            if (hitColliders1[i].tag == "Enemy" && hitColliders1[i].gameObject != this.gameObject)
            {
                hitColliders1[i].SendMessageUpwards("wakeUp", SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    //***********************APPLY EXTRA TURN ROTATION************************************
    void ApplyExtraTurnRotation()
    {
        // help the character turn faster (this is in addition to root rotation in the animation)
        float turnSpeed = Mathf.Lerp(speedSettings.m_StationaryTurnSpeed, speedSettings.m_MovingTurnSpeed, m_ForwardAmount);
        transform.Rotate(0, m_TurnAmount * turnSpeed * Time.deltaTime, 0);
    }

    //***********************DETECT PLAYER************************************	
    IEnumerator detectPlayer()
    {
        agent.speed = 0;
        tempReactionTime = reactionTime;
        checkingForPlayer = true;
        yield return new WaitForSeconds(tempReactionTime);
        if (playerVisible(detectionSettings.myEyes.position) == true)
        {
            pursuePlayer(true);
        }
        if (!chase && cautious && !campLastKnownSpot)
        {
            if (markerSettings.myMark)
            {
                markerSettings.myMark.GetComponent<Renderer>().material.color = markerSettings.markerCautionColor;
            }
            agent.speed = speedSettings.walkSpeed;
        }
        checkingForPlayer = false;
        timerReset = false;
    }

    //***********************GUARD SPOT HIT************************************
    public void guardSpotHit()
    {
        agent.speed = 0;
        returnToGuardSpot = false;
        rotationTarget = guardSpot.forward;
        adjustRotation = true;
    }

    //***********************LOST TRAIL TIMEOUT************************************	
    IEnumerator lostTrailTimeOut()
    {
        yield return new WaitForSeconds(detectionSettings.searchTime);
        if (!canSeePlayer)
        {
            if (playerGone != null)
            {
                StopCoroutine(playerGone);
            }
            playerGone = StartCoroutine(playerEscaped());
        }
        lostTrailCheck = false;
    }

    //***********************LOOK AT PLAYER************************************
    public void lookAtPlayer()
    {
        Debug.DrawLine(detectionSettings.myEyes.position, hit.point, Color.red);
        Vector3 point = target.position;
        point.y = myBody.transform.position.y;
        Vector3 targetDir = point - myBody.transform.position;
        Vector3 newDir = Vector3.RotateTowards(myBody.transform.forward, targetDir, step, 00.0F);
        myBody.transform.rotation = Quaternion.LookRotation(newDir);
    }

    //***********************MOVE************************************
    public void Move(Vector3 move, bool crouch, bool jump)
    {
        // convert the world relative moveInput vector into a local-relative
        // turn amount and forward amount required to head in the desired
        // direction.
        if (move.magnitude > 1f) move.Normalize();
        move = transform.InverseTransformDirection(move);
        m_TurnAmount = Mathf.Atan2(move.x, move.z);
        m_ForwardAmount = move.z * speedSettings.m_MoveSpeedMultiplier;
        ApplyExtraTurnRotation();
        // send input and other state parameters to the animator
        if (myAnim != null)
        {
            myAnim.UpdateAnimator(m_ForwardAmount, m_TurnAmount, speedSettings.smoothMove, speedSettings.m_AnimSpeedMultiplier);
        }
    }

    //***********************NODE WANDER HIT************************************
    IEnumerator nodeWanderHit(float pointWaitTime, bool resumePatrol)
    {
        agent.speed = 0;
        yield return new WaitForSeconds(pointWaitTime);
        if (!nodeWanderSettings.followSequence)
        {
            goalNode = myNodes[Random.Range(0, myNodes.Count - 1)];
        }
        if (nodeWanderSettings.followSequence)
        {
            goalNode = myNodes[nodeWanderSequenceID];
            nodeWanderSequenceID += 1;
            if (nodeWanderSequenceID >= myNodes.Count)
            {
                nodeWanderSequenceID = 0;
            }
        }
        moveToGoalNode = true;
        agent.SetDestination(goalNode.position);
        agent.speed = speedSettings.walkSpeed;
    }

    //***********************PLAYER VISIBLE************************************
    public bool playerVisible(Vector3 checkSpot)
    {
        bool pv = false;
        if (Physics.Raycast(checkSpot, new Vector3(target.position.x, target.position.y + detectionSettings.heightOffset, target.position.z) - checkSpot,
            out hit, detectionSettings.visionDistance, interactionLayers) && hit.collider.gameObject.layer == target.gameObject.layer)
        {
            pv = true;
        }
        return pv;
    }

    //***********************PURSUE PLAYER************************************
    public void pursuePlayer(bool playerInSight)
    {
        if (playerGone != null)
        {
            StopCoroutine(playerGone);
        }
        if (markerSettings.myMark)
        {
            markerSettings.myMark.GetComponent<Renderer>().material.color = markerSettings.markerAttackColor;
            activateMark(true);
        }
        if (advancedSettings.holdGround)
        {
            stoppingDistance = distance;
        }
        if (detectionSettings.alertOtherAgents)
        {
            InvokeRepeating("alertOthers", 0.1f, Random.Range(1.0f, 1.5f));
            moveToWayPoint = false;
            moveToWanderSpot = false;
            returnToGuardSpot = false;
            moveToGoalNode = false;
            cautious = false;
            adjustRotation = false;
            canSeePlayer = playerInSight;
            agent.speed = speedSettings.chaseSpeed;
            chase = true;
            speedSettings.m_MoveSpeedMultiplier = 1.0f;
            StartCoroutine("updatePlayerPos");
        }
    }

    //***********************PLAYER ESCAPED************************************
    IEnumerator playerEscaped()
    {
        CancelInvoke();
        agent.speed = 0;
        chase = false;
        speedSettings.m_MoveSpeedMultiplier = 0.5f;

        attackOk = false;
        campLastKnownSpot = true;
        if (markerSettings.myMark != null)
        {
            markerSettings.myMark.GetComponent<Renderer>().material.color = markerSettings.markerUnawareColor;
        }
        yield return new WaitForSeconds(detectionSettings.campTime); //CAMP OUT FOR CAMP TIME. SET TO 0 TO AVOID CAMPING
        campLastKnownSpot = false; //LOST PLAYER AND RESUME DUTY ROUTINE
        cautious = false;
        if (useNodeWander)
        {
            if (!nodeWanderSettings.followSequence)
            {
                goalNode = myNodes[Random.Range(0, myNodes.Count - 1)];
            }
            if (nodeWanderSettings.followSequence)
            {
                goalNode = myNodes[nodeWanderSequenceID];
            }
            moveToGoalNode = true;
            agent.SetDestination(goalNode.position);
        }
        if (useWayPoints && !wayPointSettings.useWayPointScript)
        {
            moveToWayPoint = true;
            StartCoroutine(wayPointHit(Random.Range(wayPointSettings.patrolStopTimeLow, wayPointSettings.patrolStopTimeHigh), true));
        }
        if (useWander)
        {
            moveToWanderSpot = true;
        }
        if (useGuard)
        {
            returnToGuardSpot = true;
            agent.SetDestination(guardSpot.position);

        }
        if (useWander)
        {
            StartCoroutine(wanderSpotHit(Random.Range(wanderSettings.wanderPauseTimeLow, wanderSettings.wanderPauseTimeHigh), false));
        }
        agent.speed = speedSettings.walkSpeed;
    }

    //***********************RANDOM POINT************************************	
    bool RandomPoint(Vector3 center, float range, out Vector3 result)
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

    //***********************SET KINEMATIC************************************	
    void setKinematic(bool newValue)
    {
        Component[] components = GetComponentsInChildren(typeof(Rigidbody));
        foreach (Component c in components)
        {
            (c as Rigidbody).isKinematic = newValue;
        }
    }

    //***********************UPDATE PLAYER POS************************************
    IEnumerator updatePlayerPos()
    {
        if (chaseType == ChaseType.direct)
        {
            targetPosition = target.position;
            agent.SetDestination(targetPosition);
        }
        if (chaseType == ChaseType.surround)
        {
            targetPosition = new Vector3(targetPosition.x, targetPosition.y, targetPosition.z);
            if (playerVisible(targetPosition) && postedUp)
            {
                agent.SetDestination(targetPosition);
            }
            else
            {
                Vector3 attackPoint;
                for (int i = 0; i < 500; i++)
                {
                    if (RandomPoint(target.position, 10.0f, out attackPoint))
                    {
                        targetPosition = attackPoint;
                        targetPosition = new Vector3(targetPosition.x, targetPosition.y + 1.5f, targetPosition.z);
                        if (playerVisible(targetPosition))
                        {
                            agent.SetDestination(targetPosition);
                            i = 2000;
                            StartCoroutine("positionTimer");
                        }
                    }
                }
                if (playerVisible(targetPosition) == false)
                {
                    chaseType = ChaseType.direct;
                    targetPosition = target.position;
                    agent.SetDestination(targetPosition);
                }
            }
        }
        yield return new WaitForSeconds(advancedSettings.playerUpdateInterval);

        if (chase == true)
        {
            StartCoroutine("updatePlayerPos");
        }
    }
    //***********************POSITION TIMER************************************	
    IEnumerator positionTimer()
    {
        postedUp = true;
        yield return new WaitForSeconds(Random.Range(advancedSettings.positionChangeLow, advancedSettings.positionChangeHigh));
        postedUp = false;
    }

    //***********************WAKE UP************************************	
    public void wakeUp()
    {
        if (!chase && !lostTrailCheck && !wokenUp)
        {
            if (playerVisible(detectionSettings.myEyes.position) == true)
            {
                pursuePlayer(true);
            }
            else
            {
                pursuePlayer(false);
            }
            StartCoroutine("wakeUpTimeOut");
        }

    }

    //***********************WAKE UP TIMEOUT************************************
    IEnumerator wakeUpTimeOut()
    {
        wokenUp = true;
        yield return new WaitForSeconds(advancedSettings.wakeUpTimeOut);
        wokenUp = false;
    }

    //***********************WANDER SPOT HIT************************************
    IEnumerator wanderSpotHit(float pointWaitTime, bool resumePatrol)
    {
        agent.speed = 0;
        yield return new WaitForSeconds(pointWaitTime);
        Vector3 point;
        wanderSpotFound = false;
        timeOutClock = 0;
        while (!wanderSpotFound && timeOutClock < findWanderSpotTimeOutMax)
        {
            if (RandomPoint(transform.position, wanderSettings.wanderRadius, out point))
            {
                NavMeshPath path = new NavMeshPath();
                agent.CalculatePath(point, path);
                if (path.status == NavMeshPathStatus.PathPartial || path.status == NavMeshPathStatus.PathInvalid)
                {
                    wanderSpotFound = false;
                    //Debug.Log("partial or invalid path. fix navmesh");
                    timeOutClock += 1;
                }
                else
                {
                    wanderSpot.position = point;
                    agent.SetDestination(wanderSpot.position);// = targetWayPoint;
                    moveToWanderSpot = true;
                    wanderSpotFound = true;
                }
            }
            else
            {
                wanderSpotFound = false;
                timeOutClock += 1;
            }
        }
        if (timeOutClock >= findWanderSpotTimeOutMax)
        {
            Debug.Log("valid wander spot could not be found. Agent using guard mode.");
            useWander = false;
            useGuard = true;
            GameObject startLocation = new GameObject();
            startLocation.name = this.name + " guard Spot";
            startLocation.transform.position = this.transform.position;
            startLocation.transform.rotation = this.transform.rotation;
            guardSpot = startLocation.transform;
            agent.speed = 0;
        }
        agent.speed = speedSettings.walkSpeed;
    }


    //***********************WAYPOINT HIT************************************
    public IEnumerator wayPointHit(float wayPointWaitTime, bool resumePatrol)
    {
        agent.speed = 0;
        if (wayPointSettings.useCustomWPTime)
        {
            yield return new WaitForSeconds(wayPointSettings.wayPointStopTime[wayPointIndex]);
        }
        if (!resumePatrol)
        {
            wayPointIndex += 1;
        }
        if (wayPointIndex >= wayPointSettings.wayPoints.Length)
        {
            wayPointIndex = 0;
        }
        if (!wayPointSettings.useCustomWPTime && !wayPointSettings.useWayPointScript)
        {
            yield return new WaitForSeconds(wayPointWaitTime);
        }
        if (!wayPointSettings.useWayPointScript)
        {
            targetWayPoint = wayPointSettings.wayPoints[wayPointIndex];
            agent.SetDestination(targetWayPoint.position);
        }
        if (wayPointSettings.useWayPointScript)
        {
            targetWayPoint = this.GetComponent<node_ConditionalWayPointHandler>().myTargetWayPoint;
            agent.SetDestination(targetWayPoint.position);

        }
        agent.speed = speedSettings.walkSpeed;
    }
}
