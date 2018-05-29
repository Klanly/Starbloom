using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;


public class Fishing_MasterHandler : MonoBehaviour
{
    public enum FishingStates
    {
        Idle,
        SearchingForCast,
        CastCharging,
        Casting,
        AwaitingFish,
        FishEventTriggered,
        HitDisplay,
        Reeling,
        PullingInBobber,
        BobberPulledIn,
        WaitForUpTrigger
    }

    public enum WaterTypes
    {
        River,
        Lake,
        Ocean
    }

    FishingStates CurrentState = FishingStates.Idle;
    public LayerMask WaterDetection;
    public WaterTypes DetectedWater;

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    [Header("Cast Detection----------------------------------------------")]
    public float DetectionInterval;
    public int DetectionCount;
    Transform CastDetectionHelper;
    Vector3[] DebugCastDetectionPoints() { if (V == null) V = new Vector3[DetectionCount]; return V; }
    Vector3[] V;

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    [Header("Casting")]
    public float MaxCastMinimum;
    public float CastAdjustSpeed;
    float CastPower;
    bool isUP = true;

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    [Header("Cast")]
    public float MaxCastDistance;
    public float CastTime;
    float CastTimer;
    Vector3 EndPoint;
    float EndDistance;

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //Awaiting Fish Bite.
    float AwaitingFishTimer;

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    [Header("Hooked Event")]
    public float HookedEventTime;
    float HookedEventTimer;

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    [Header("Hook Successful")]
    public float HookedSuccessfulDisplayTime;
    float HookedSuccessfulDisplayTimer;

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    [Header("Reeling")]
    public float ReelMoveSpeed;
    public float FishCaughtSpeed;
    float CurrentReelValue;
    float CurrentFishCaughtValue;
    bool ReelingHeld;

    [Header("Reeling Minigame Values")]
    public float RegionMoveSpeed;
    float MaxFishHeight;
    float MaxRegionHeight;

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    [Header("Bobber Pull In")]
    public float PullInTime;
    float PullInTimer;

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    [Header("References")]
    public GameObject FishingLine;
    public GameObject BobberModel;

    [Header("Camera")]
    public float CameraHeight;
    public float CameraSpeed;


    ////////////////////////////////////////////////////////////////////////////////////////////////////
    [Header("Debug Print-----------------------------------------------------")]
    public bool PrintDebug;
    [Header("Debug Gizmos-----------------------------------------------------")]
    public bool GizmoFishingDetection;
    public bool GizmoCasterRange;
    [Header("Debug Values")]
    public bool QuickCatchFish;
    public float DebugFishMoveUpSpeed;



    [Button(ButtonSizes.Small)] public void ResetFishingState() { CurrentState = FishingStates.Idle; }
    float CameraInitialPosition;




    private void Awake()
    {
        QuickFind.FishingHandler = this;
        CastDetectionHelper = new GameObject().transform;
        CastDetectionHelper.SetParent(transform);
    }
    private void Start()
    {
        CameraInitialPosition = QuickFind.PlayerCam.transform.localEulerAngles.x;
    }
    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;
        if (GizmoFishingDetection) { for (int i = 0; i < DebugCastDetectionPoints().Length; i++) Gizmos.DrawSphere(DebugCastDetectionPoints()[i], .4f); }
        if (GizmoCasterRange && CurrentState != FishingStates.Idle) { Gizmos.DrawSphere(CalcEndPoint(), .3f); }
    }

    void SetAnimationState(string Animation)
    { 
        switch(Animation)
        {
            case "Idle": break;
            case "Casting" : break;
            case "AwaitingFish": break;
            case "PullingInBobber": break;
            case "Reeling": break;
        }
    }


    //Input Reactions
    public void ExternalUpdate(bool isUp)
    {
        if (CurrentState == FishingStates.Idle) { SetAnimationState("Casting"); CurrentState = FishingStates.SearchingForCast; QuickFind.InputController.InputState = DG_PlayerInput.CurrentInputState.PerformingAction; }
        if (CurrentState == FishingStates.SearchingForCast) { if (FishableAreaIsPossible()) { AdjustCamera(true); CastPower = 0; CurrentState = FishingStates.CastCharging; } else if (isUp) { SetAnimationState("Idle"); CurrentState = FishingStates.Idle; QuickFind.InputController.InputState = DG_PlayerInput.CurrentInputState.Default; } }
        if (CurrentState == FishingStates.CastCharging) { CastPower = CalculateCastPower(); if (isUp) { PlayCastEffect(); EndPoint = CalcEndPoint();  CastTimer = CastTime; CurrentState = FishingStates.Casting; } }
        if (CurrentState == FishingStates.AwaitingFish) { SetAnimationState("PullingInBobber"); CurrentState = FishingStates.PullingInBobber; AdjustCamera(false); }
        if (CurrentState == FishingStates.FishEventTriggered) { if (PrintDebug) Debug.Log("Fish Hook Successful"); SetAnimationState("Reeling"); HookedSuccessfulDisplayTimer = HookedSuccessfulDisplayTime; QuickFind.FishingGUI.DisplayHookedText(BobberModel.transform); CurrentState = FishingStates.HitDisplay; }
        if (CurrentState == FishingStates.Reeling) ReelingHeld = true;
        if(CurrentState == FishingStates.WaitForUpTrigger) { if (isUp) CurrentState = FishingStates.Idle; }
    }

    //Internal Responses
    private void Update()
    {
        if (CurrentState == FishingStates.Casting) { if (isCastComplete()) { SetAnimationState("AwaitingFish"); QuickFind.FishingGUI.DisableEnergyBar(); CalculateNextFishEvent(); CurrentState = FishingStates.AwaitingFish; } }
        if (CurrentState == FishingStates.AwaitingFish) { if (DoWeTriggerFishEvent()) { QuickFind.FishingGUI.DisplayFishEventAlert(); CurrentState = FishingStates.FishEventTriggered; } }
        if (CurrentState == FishingStates.FishEventTriggered) { if (FishEventTimerRanTooLong()) CurrentState = FishingStates.Casting; }
        if (CurrentState == FishingStates.HitDisplay) { if (HitDisplayFinished()) { OpenFishingGUI(); CurrentState = FishingStates.Reeling; } }
        if (CurrentState == FishingStates.Reeling) { float Progress = ReelProgressBar(); if (Progress < 0 || Progress > 1) { PullInTimer = PullInTime; SetAnimationState("PullingInBobber"); CurrentState = FishingStates.PullingInBobber; AdjustCamera(false); } }
        if (CurrentState == FishingStates.PullingInBobber) { if (BobberPulledIn()) { SetAnimationState("Idle"); CurrentState = FishingStates.WaitForUpTrigger; QuickFind.InputController.InputState = DG_PlayerInput.CurrentInputState.Default; } }
    }




    bool FishableAreaIsPossible()
    {
        CastDetectionHelper.position = QuickFind.PlayerTrans.position;
        CastDetectionHelper.rotation = QuickFind.PlayerTrans.rotation;
        Vector3 LiftedPos = CastDetectionHelper.position; LiftedPos.y = LiftedPos.y + 1;
        CastDetectionHelper.position = LiftedPos;
        for (int i = 0; i < DetectionCount; i++)
        {
            CastDetectionHelper.position += CastDetectionHelper.forward * DetectionInterval;

            Ray Ray = new Ray(CastDetectionHelper.position, Vector3.down);
            RaycastHit NewRayInfo;

            if (Physics.Raycast(Ray, out NewRayInfo, 10, WaterDetection))
            {
                if (GizmoFishingDetection) DebugCastDetectionPoints()[i] = NewRayInfo.point;
                if (NewRayInfo.collider.CompareTag("River")) { DetectedWater = WaterTypes.River; if (PrintDebug) Debug.Log("Cast On River"); return true; }
                else if (NewRayInfo.collider.CompareTag("Lake")) { DetectedWater = WaterTypes.Lake; if (PrintDebug) Debug.Log("Cast On Lake"); return true; }
                else if (NewRayInfo.collider.CompareTag("Ocean")) { DetectedWater = WaterTypes.Ocean; if (PrintDebug) Debug.Log("Cast On Ocean"); return true; }
                else Debug.Log("Hit - " + NewRayInfo.collider.name);
            }
        }
        return false;
    }
    float CalculateCastPower()
    {
        float Adjust = CastAdjustSpeed;
        if (!isUP) Adjust = -Adjust;
        CastPower = CastPower + Adjust;

        if(CastPower > 1) { CastPower = 1; isUP = false; }
        if (CastPower < 0) { CastPower = 0; isUP = true; }

        if (PrintDebug) Debug.Log("Detecting Cast Power, Send Message to GUI here.");
        QuickFind.FishingGUI.SetEnergyBar(CastPower);
        return CastPower;
    }
    void AdjustCamera(bool isUP)
    {
        if(isUP) QuickFind.PlayerCam.SetNewVerticalPanPosition(CameraHeight, CameraSpeed);
        else QuickFind.PlayerCam.SetNewVerticalPanPosition(CameraInitialPosition, CameraSpeed);
    }

    void PlayCastEffect()
    {
        if (CastPower > MaxCastMinimum)
        {
            if (PrintDebug) Debug.Log("Max Cast Effect");
            QuickFind.FishingGUI.DisplayMaxText();
        }
    }

    Vector3 CalcEndPoint()
    {
        float CastDistance = MaxCastDistance * CastPower;
        CastDetectionHelper.position = QuickFind.PlayerTrans.position;
        CastDetectionHelper.rotation = QuickFind.PlayerTrans.rotation;
        Vector3 LiftedPos = CastDetectionHelper.position; LiftedPos.y = LiftedPos.y + 1;
        CastDetectionHelper.position = LiftedPos;
        CastDetectionHelper.position += CastDetectionHelper.forward * CastDistance;
        Ray Ray = new Ray(CastDetectionHelper.position, Vector3.down);
        RaycastHit NewRayInfo;

        if (Physics.Raycast(Ray, out NewRayInfo, 10, WaterDetection))
        {
            EndDistance = Vector3.Distance(QuickFind.PlayerTrans.position, NewRayInfo.point);
            return NewRayInfo.point;
        }
        else { Debug.Log("Handle Error Case"); return Vector3.zero; }
    }
    //Launch Bobber
    bool isCastComplete()
    {
        BobberModel.gameObject.SetActive(true);

        CastTimer = CastTimer - Time.deltaTime;
        if (CastTimer < 0) CastTimer = 0;
        BobberModel.transform.position = CalculateBobberPosition(CastTimer);

        if (CastTimer == 0) //Bobber Reached Destination.
        { if (PrintDebug) Debug.Log("Cast Complete"); return true; }
        else
            return false;
    }
    Vector3 CalculateBobberPosition(float CastTime)
    {
        CastDetectionHelper.position = QuickFind.PlayerTrans.position;
        Vector3 LiftedPos = CastDetectionHelper.position; LiftedPos.y = LiftedPos.y + 1;
        CastDetectionHelper.position = LiftedPos;
        CastDetectionHelper.LookAt(EndPoint);
        CastDetectionHelper.position += CastDetectionHelper.forward * (EndDistance * (1 - CastTime));

        return CastDetectionHelper.position;
    }
    void CalculateNextFishEvent()
    {
        if (QuickCatchFish) AwaitingFishTimer = 1;
        else
        {
            AwaitingFishTimer = 2;
            Debug.Log("TODO - Load Next Fish to be caught, and time till bite");
        }
    }
    bool DoWeTriggerFishEvent()
    {
        AwaitingFishTimer = AwaitingFishTimer - Time.deltaTime;
        if (AwaitingFishTimer < 0)
            { if (PrintDebug) Debug.Log("Activeate Fish Trigger Window"); HookedEventTimer = HookedEventTime; return true; }
        else return false;
    }
    bool FishEventTimerRanTooLong()
    {
        HookedEventTimer = HookedEventTimer - Time.deltaTime;
        if (HookedEventTimer < 0)
        { if (PrintDebug) Debug.Log("Failed to Hit Activation!"); return true; }
        else return false;
    }
    bool HitDisplayFinished()
    {
        HookedSuccessfulDisplayTimer = HookedSuccessfulDisplayTimer - Time.deltaTime;
        if (HookedSuccessfulDisplayTimer < 0)
        { if (PrintDebug) Debug.Log("Hit Display Ended."); return true; }
        else return false;
    }
    void OpenFishingGUI()
    {
        Debug.Log("TODO - Get Fishing Strength Based on Fishing Level");
        float FishingStrength = 200;
        QuickFind.FishingGUI.SetCaptureZoneSize(FishingStrength);

        MaxFishHeight = QuickFind.FishingGUI.FishRegionMain.rect.height;
        MaxRegionHeight = MaxFishHeight - FishingStrength;

        QuickFind.FishingGUI.EnableFishingGUI();
    }
    float ReelProgressBar()
    {
        CurrentReelValue = GetReelValueFactoringinReelHeight(ReelingHeld);
        bool WithinReelBounds = FishWithinReelbarBounds(ReelingHeld);
        if (ReelingHeld) ReelingHeld = false;

        if(WithinReelBounds) CurrentFishCaughtValue = CurrentFishCaughtValue + (FishCaughtSpeed * Time.deltaTime);
            else CurrentFishCaughtValue = CurrentFishCaughtValue - (FishCaughtSpeed * Time.deltaTime);

        QuickFind.FishingGUI.SpinReel(WithinReelBounds);
        QuickFind.FishingGUI.SetFishCaughtProgress(CurrentFishCaughtValue);

        return CurrentFishCaughtValue;
    }
    float GetReelValueFactoringinReelHeight(bool isAdd)
    {
        if (isAdd)
        {
            if (PrintDebug) Debug.Log("Raise Reel Bar.");
            return CurrentReelValue + (ReelMoveSpeed * Time.deltaTime);
        }
        else
        {
            if (PrintDebug) Debug.Log("Lower Reel Bar.");
            return CurrentReelValue - (ReelMoveSpeed * Time.deltaTime);
        }
    }
    bool FishWithinReelbarBounds(bool isHeld)
    {
        float FishPosition = QuickFind.FishingGUI.Fish.anchoredPosition.y;

        //Debug Move Fish
        FishPosition += DebugFishMoveUpSpeed * Time.deltaTime;
        QuickFind.FishingGUI.Fish.anchoredPosition = new Vector3(0, FishPosition, 0);
        //

        float SafeRegionAdditive = QuickFind.FishingGUI.CapturePosHeight.rect.height - 50;
        float CurrentFishRegionPosition = QuickFind.FishingGUI.CaptureZonePos.anchoredPosition.y;
        if (isHeld)
        {
            //Todo Factor In Gravity Inertia
            CurrentFishRegionPosition += RegionMoveSpeed * Time.deltaTime;
            if (CurrentFishRegionPosition > MaxRegionHeight) CurrentFishRegionPosition = MaxRegionHeight;
        }
        else
        {
            CurrentFishRegionPosition -= RegionMoveSpeed * Time.deltaTime;
            if (CurrentFishRegionPosition < 0) CurrentFishRegionPosition = 0;
        }

        QuickFind.FishingGUI.CaptureZonePos.anchoredPosition = new Vector3(0, CurrentFishRegionPosition, 0);

        if (FishPosition >= CurrentFishRegionPosition && FishPosition <= (CurrentFishRegionPosition + SafeRegionAdditive))
            return true;
        else
            return false;
    }
    bool BobberPulledIn()
    {
        PullInTimer = PullInTimer - Time.deltaTime;
        if (PullInTimer < 0)
        { if (PrintDebug) Debug.Log("Bobber Pulled In."); if (CurrentFishCaughtValue > 1) RewardPlayerWithCaughtFish(); return true; }
        else return false;
    }
    void RewardPlayerWithCaughtFish()
    {
        Debug.Log("Reward Player With fish");
    }





    //public bool LureIsIn = true;
    //public bool FishHooked = false;
    //public bool isDay = false;
    //public string CurrentStateDebug = "";

    //[Space(10)]
    //public FishingCastingData CastingData = new FishingCastingData();
    //[System.Serializable]
    //public class FishingCastingData
    //{
    //    [Header("Casting Reference")]
    //    public GameObject CastGauge = null;
    //    public RectTransform ScalingGauge = null;
    //    public float GaugeHighPoint = 0;
    //    public float GaugeLowPoint = -200;
    //
    //    [Header("Casting Values")]
    //    public float LowToHighCastTime = 2f;
    //    public float HoldTimeOut = 2.5f;
    //
    //    [Header("Cast Arc Data")]
    //    public int CastArcResolution = 15;
    //    public float CastDelay = .2f;
    //
    //    [Header("Debug")]
    //    public bool ShowLandingPointSphere = false;
    //    public GameObject CastLandPointSphere = null;
    //
    //    public bool ShowCastSpheres = false;
    //    public Transform ArcSphereContainer = null;
    //    public GameObject CastArcProjectionSphere = null;
    //
    //    public bool ShowUpperArcSphere = false;
    //    public GameObject CastUpperAngleArcSphere = null;
    //}

    //public FishingBobberData BobberData = new FishingBobberData();
    //[System.Serializable]
    //public class FishingBobberData
    //{
    //    [Header("Bobber")]
    //    public Transform Bobber = null;
    //    public float BobberFlySpeed = 15f;
    //    public float BobberBobSpeed = 10f;
    //    public float BobberHeight = .06f;
    //    public LayerMask BobberInteraction;
    //    public Transform BobberOffsetHelperTransform;
    //
    //    [Header("Debug")]
    //    public bool PrintBobberRelativeDirection = false;
    //}

    //public FishingReelData ReelData = new FishingReelData();
    //[System.Serializable]
    //public class FishingReelData
    //{
    //    [Header("Reeling")]
    //    public float ReelUpwardsDistance = 3f;
    //    public float BobberIsFullyInDistance = .2f;
    //}

    //[Space(10)]
    //public FishingGUIData GUI = new FishingGUIData();
    //[System.Serializable]
    //public class FishingGUIData
    //{
    //    public GameObject FishingCanvas = null;
    //    [Header("Reeling GUI")]
    //    public Text LineDistanceText = null;
    //    public Text LineTensionText = null;
    //    public Slider TensionSlider = null;
    //    public Text ReelRemainingLineText = null;
    //    [Header("Event RELAY")]
    //    public GameObject FishCaughtRelay = null;
    //    public GameObject LineBreakRelay = null;
    //    public GameObject MissingGearRelay = null;
    //    [Header("Fish GUI")]
    //    public Text FishEnergyText = null;
    //    public Text FishCaughtText = null;
    //    public Text FishNameText = null;
    //    [Header("Fish Description")]
    //    public GameObject FishCaughtDescription = null;
    //    public Text FishCaughtTitle = null;
    //    public Text FishCaughtDescriptionText = null;
    //}

    //public FishingCamera Camera = new FishingCamera();
    //[System.Serializable]
    //public class FishingCamera
    //{
    //    public Transform BobberSmoothCam = null;
    //    public float FishingZoomLevel = 12;
    //    public float CastCameraAngleY = 3.5f;
    //}

    //[Header("Dynamic Data")]
    //public FishingWaitTimes WaitTimes = new FishingWaitTimes();
    //[System.Serializable]
    //public class FishingWaitTimes
    //{
    //    [Header("Pull Time values")]
    //    public float PullMinBase = 3f;
    //    public float PullMaxBase = 4f;
    //    [Header("Pull Wait Time values")]
    //    public float PullWaitMinBase = 2f;
    //    public float PullWaitMaxBase = 3f;
    //
    //    [Header("SideWays Pull")]
    //    public float SidePullMin = -1f;
    //    public float SidePullMax = 1f;
    //    [Header("SideWays Wait")]
    //    public float SidePullWaitMin = .1f;
    //    public float SidePullWaitMax = 1.1f;
    //
    //    [Header("Fish pull Wobble")]
    //    public float WobbleDistance = 15f;
    //    public float WobbleSpeed = 1f;
    //
    //    [Header("Broken Line")]
    //    public float PeelAwayTime = 5f;
    //}

    //public FishingGearData CurrentGear = new FishingGearData();
    //[System.Serializable]
    //public class FishingGearData
    //{
    //    [Header("Rod")]
    //    public float BobberPullRate = 5f;
    //    public float MaxCastForce = 40f;
    //    [Header("Line")]
    //    public float TensionRate = .2f;
    //    public float MaxTension = 10;
    //    public float TensionRestoreRate = .4f;
    //    public float MaxLineDistance = 60;
    //}

    //public FishBehaviourModule CurrentFish = new FishBehaviourModule();
    //[System.Serializable]
    //public class FishBehaviourModule
    //{
    //    [Header("Fish Object")]
    //    public GameObject FishCaught = null;
    //    public DG_ItemObject FishObjectScript = null;
    //
    //    [Header("Fish Stats")]
    //    public float FishWeight = 20f;
    //    public float FishTotalEnergy = 0;
    //    public float DrainRate = 1f;
    //
    //    [Header("Fish Pull Strength")]
    //    public float FishPullRate = 1f;
    //}


    //float Timer = 0;
    //float GaugeHeldTime = 0;
    //float StoredRPGCamZoom = 60;
    //float StoredRPGCamMax = 60;
    //float CastDistance;
    //bool UpdateCamZoom = false;
    //int UpdateCamState = 1;
    //float LastKnownDistance = 0;
    //float FishPullRate = 0;
    //bool FishOverRide;
    //float TensionMultiplier;
    //public Transform BobberModel = null;
    //Vector3 FinalCastPoint;
    //List<Vector3> GeneratedArcPoints;
    //int NextBobberArcPointGoal = 0;
    //bool WithinReelUpDistance = false;

    //int CurrentState = 0;
    //int FishingState = 0;
    //bool UpdateFishing = false;
    //float TimeRemaining = 0f;
    //bool waitForRelease = false;

    //CurrentValues

    //float Tension = 0;
    //float LineDistance = 0;
    //float SlackDistance = 0;
    //float TensionAmount = 0;

    //Fishing Values

    //bool Pulling = false;
    //float PullTimer = 0;
    //float TurnTimer = 0;
    //float TurnValue = 0;
    //float FishEnergy = 16f;
    //bool FishRotateRight = false;


    //public void ActivateFishing(bool isTrue)
    //{
    //    if (isTrue)
    //    {
    //        GUI.FishingCanvas.SetActive(true);
    //        CurrentState = 1;
    //        CurrentStateDebug = "Awaiting Cast";
    //        CastingData.CastGauge.SetActive(true);
    //        GUI.LineDistanceText.enabled = false;
    //        GUI.LineTensionText.enabled = false;
    //        GUI.ReelRemainingLineText.enabled = false;
    //        GUI.FishEnergyText.enabled = false;
    //        GUI.FishNameText.enabled = false;
    //
    //        FetchCurrentGearValues();
    //
    //        this.enabled = true;
    //    }
    //    else
    //        GUI.FishingCanvas.SetActive(false);
    //}








    //void Start()
    //{
    //    CastingData.CastLandPointSphere.SetActive(false);
    //    CastingData.CastUpperAngleArcSphere.SetActive(false);
    //    CastingData.CastArcProjectionSphere.SetActive(false);
    //    BobberData.BobberOffsetHelperTransform.gameObject.SetActive(false);
    //    GUI.TensionSlider.gameObject.SetActive(false);
    //    GUI.FishingCanvas.SetActive(false);
    //    CastingData.ScalingGauge.anchoredPosition = new Vector2(CastingData.ScalingGauge.anchoredPosition.x, CastingData.GaugeLowPoint);
    //    UpdateFishing = false;
    //    this.enabled = false;
    //
    //    if (CastingData.ShowCastSpheres)
    //    {
    //        for (int i = 0; i < CastingData.CastArcResolution; i++)
    //        {
    //            GameObject Marker = GameObject.Instantiate(CastingData.CastArcProjectionSphere);
    //            Marker.transform.SetParent(CastingData.ArcSphereContainer);
    //            Marker.SetActive(false);
    //        }
    //    }
    //}



    //private void Update()
    //{  
    //if (CurrentState == 1) // 1 - Waiting For Cast
    //{
    //    if (Input.GetKey(KeyCode.F)) //Context Key
    //    {
    //        if (waitForRelease)
    //            return;
    //
    //        Debug.Log("Charge Cast");
    //        Timer = 0;
    //        CurrentState = 2;
    //        CurrentStateDebug = "Charging Cast";
    //    }
    //    else
    //        waitForRelease = false;
    //}
    //if (CurrentState == 2) // 2 - Charging Cast
    //{
    //    if (Input.GetKey(KeyCode.F)) //Context Key
    //    {
    //        UpdateCastMeter();
    //        CalculateCastEndPoint();
    //    }
    //    else
    //    {
    //        CalculateCastEndPoint();
    //        Timer = CastingData.CastDelay;
    //        LureIsIn = false;
    //        CurrentState = 4;
    //        CurrentStateDebug = "Cast Launched";
    //        CastingData.CastGauge.SetActive(false);
    //        Debug.Log("Throw Cast");
    //    }
    //}
    //if (CurrentState == 3) //Cast Failed
    //{
    //    if (Input.GetKeyUp(KeyCode.F)) //Context Key
    //    {
    //        CurrentState = 1;
    //        CurrentStateDebug = "Awaiting Cast";
    //    }
    //}
    //if (CurrentState == 4) //Cast Launch
    //{
    //    LaunchBobber();
    //    UpdateCamAngle();
    //}
    //if (CurrentState == 5) //Reel Mechanincs
    //{
    //    bool ReelInBool = Input.GetKey(KeyCode.F);
    //    UpdatePlatformMovement(ReelInBool);
    //    UpdateFishPullMechanics(ReelInBool);
    //
    //    if (ReelInBool || FishOverRide) //Context Key
    //        ReelIn();
    //
    //    UpdateTensionSlider();
    //    UpdateBobberVertical();
    //    IsReeling(ReelInBool);
    //    CalcSlackRemaining();
    //    LastKnownDistance = LineDistance;
    //}
    //if (CurrentState == 6) //Reel is Finished
    //{
    //    if (GUI.FishCaughtDescription.activeInHierarchy)
    //        return;
    //
    //    if (Input.anyKey)
    //        return;
    //
    //    CurrentState = 1;
    //    CastingData.CastGauge.SetActive(true);
    //    CastingData.ScalingGauge.anchoredPosition = new Vector2(CastingData.ScalingGauge.anchoredPosition.x, CastingData.GaugeLowPoint);
    //    CurrentStateDebug = "Awaiting Cast";
    //    LureIsIn = true;
    //
    //}
    //if (CurrentState == 7) //FishingLineShootsOff
    //{
    //    BreakAwayLogic();
    //}
    //
    //FishingUpdate();
    //}

    //Casting

    //void UpdateCastMeter()
    //{
    //    Timer = Timer + Time.deltaTime;
    //
    //    if (Timer > CastingData.HoldTimeOut)
    //    {
    //        CastingData.ScalingGauge.anchoredPosition = new Vector2(CastingData.ScalingGauge.anchoredPosition.x, CastingData.GaugeLowPoint);
    //        CurrentState = 3;
    //        CurrentStateDebug = "Cast Failed";
    //        return;
    //    }
    //
    //    GaugeHeldTime = 0;
    //
    //    if (Timer > CastingData.LowToHighCastTime)
    //        GaugeHeldTime = CastingData.LowToHighCastTime;
    //    else
    //        GaugeHeldTime = Timer;
    //
    //    float Percent = GaugeHeldTime / CastingData.LowToHighCastTime;
    //    float newGaugeAdditive = Mathf.Abs(CastingData.GaugeLowPoint) * Percent;
    //    float newGaugePosition = CastingData.GaugeLowPoint + newGaugeAdditive;
    //
    //    CastingData.ScalingGauge.anchoredPosition = new Vector2(CastingData.ScalingGauge.anchoredPosition.x, newGaugePosition);
    //}

    //void CalculateCastEndPoint()
    //{
    //    float BaseStrength = 25;  //VC_QuickFind.MainPlayerData.StatContainer.Find("Strength").Value;
    //    float BonusStrength = 0; //VC_QuickFind.PlayerRig.BuffContainer.GetAttribute("Strength");
    //
    //    //Determine Cast End Point
    //
    //    CastDistance = (BaseStrength + BonusStrength) * (Timer / CastingData.LowToHighCastTime);
    //    if (CastDistance > CurrentGear.MaxCastForce)
    //        CastDistance = CurrentGear.MaxCastForce;
    //
    //    Vector3 CharacterPos = QuickFind.InputController.transform.position;
    //    Vector3 Direction = QuickFind.InputController.transform.forward;
    //    Vector3 NewPos = CharacterPos + (Direction * CastDistance);
    //
    //    float SuimonoHeight = QuickFind.WaterModule.SuimonoGetHeight(NewPos, "height");
    //    Vector3 WaterLevelPos = new Vector3(NewPos.x, SuimonoHeight, NewPos.z);
    //
    //    if (CastingData.ShowLandingPointSphere)
    //    {
    //        CastingData.CastLandPointSphere.SetActive(true);
    //        CastingData.CastLandPointSphere.transform.position = WaterLevelPos;
    //    }
    //
    //    FinalCastPoint = WaterLevelPos;
    //}

    //Cast Launch

    //void CalculateCastArc()
    //{
    //
    //    Vector3 CharacterPos = VC_QuickFind.PlayerRig.CharacterTransform.position;
    //    Vector3 Direction = VC_QuickFind.PlayerRig.CharacterTransform.forward;
    //    Vector3 CastPoint = VC_QuickFind.PlayerRig.CastPoint.position;
    //
    //    //Get Upper Angle Point
    //    Vector3 NewPosHalf = CharacterPos + (Direction * CastDistance / 2f);
    //    Vector3 midPointVertical = new Vector3(FinalCastPoint.x, CastPoint.y, FinalCastPoint.z) - FinalCastPoint;
    //    Vector3 NewUpper = new Vector3(NewPosHalf.x, CastPoint.y + (midPointVertical.y * (6f * (GaugeHeldTime / CastingData.LowToHighCastTime))), NewPosHalf.z);
    //
    //    if (CastingData.ShowUpperArcSphere)
    //    {
    //        CastingData.CastUpperAngleArcSphere.SetActive(true);
    //        CastingData.CastUpperAngleArcSphere.transform.position = NewUpper;
    //    }
    //
    //
    //    //Generate Arc
    //
    //    List<Vector3> ArcPath = new List<Vector3>();
    //    ArcPath.Add(CastPoint);
    //    ArcPath.Add(NewUpper);
    //    ArcPath.Add(FinalCastPoint);
    //
    //    GeneratedArcPoints = VC_HelperFunctions.LinearCurveGenerator.GenerateCurvePathFromStraightPath(ArcPath, 1, CastingData.CastArcResolution, 100);
    //
    //    if (CastingData.ShowCastSpheres)
    //    {
    //        for (int i = 0; i < CastingData.CastArcResolution; i++)
    //        {
    //            GameObject Marker = CastingData.ArcSphereContainer.GetChild(i).gameObject;
    //            Marker.transform.position = GeneratedArcPoints[i];
    //            Marker.SetActive(true);
    //        }
    //    }
    //
    //    //Turn On Bobber
    //
    //    BobberData.Bobber.transform.position = GeneratedArcPoints[0];
    //    NextBobberArcPointGoal = 1;
    //
    //    //Update Fishing Line to Use Segments By Distance
    //    VC_QuickFind.PlayerRig.FishingLineScript.m_UseSegmentsByDistance = true;
    //    VC_QuickFind.PlayerRig.FishingLineScript.m_SmoothFactor = 3;
    //
    //    VC_QuickFind.InventoryScreen.ShowFishingEquipment(false);
    //
    //    VC_QuickFind.PlayerRig.IKEnable(false, false, true);
    //
    //    WithinReelUpDistance = false;
    //}

    void LaunchBobber()
    {

    }

    //Moving Platform Adjustments

    //void UpdatePlatformMovement(bool ReelInBool)
    //{
    //    Vector3 CurrentPlatformPos = VC_QuickFind.CharacterController.CurrentPlatform.position;
    //    //Vector3 BoatTraveledDistance = CurrentPlatformPos - PlatformLastPosition;
    //    float distance = Vector3.Distance(PlatformLastPosition, CurrentPlatformPos);
    //    if (distance < .05f)
    //        return;
    //
    //    BobberData.BobberOffsetHelperTransform.LookAt(VC_QuickFind.PlayerRig.CharacterTransform);
    //    float combinedY = Mathf.Abs((BobberData.BobberOffsetHelperTransform.eulerAngles.y - 180f) - (VC_QuickFind.CharacterController.CurrentPlatform.eulerAngles.y - 180));
    //
    //    if (combinedY < 50 || combinedY > 310)
    //    {
    //        if (BobberData.PrintBobberRelativeDirection)
    //            Debug.Log("Bobber is behind the boat");
    //
    //        if (SlackDistance > 0)
    //        {
    //            CalcLineDistance();
    //            SlackDistance = SlackDistance + (LastKnownDistance - LineDistance);
    //            VC_QuickFind.PlayerRig.FishingLineScript.m_Tension = getFishingLineSlack();
    //        }
    //        else //If there is no Slack, Drag Bobber with Boat.
    //        {
    //            BobberData.Bobber.LookAt(VC_QuickFind.PlayerRig.FishingRodTip);
    //            Vector3 Direction = BobberData.Bobber.forward;
    //            Vector3 NewPos = VC_QuickFind.PlayerRig.FishingRodTip.position + (-Direction * LineDistance);
    //
    //            BobberData.Bobber.position = NewPos;
    //        }
    //
    //    }
    //    else //Boat is Behind Bobber.
    //    {
    //        if (combinedY > 50 && combinedY < 130)
    //        {
    //            if (BobberData.PrintBobberRelativeDirection)
    //                Debug.Log("Left Side of boat");
    //        }
    //        else if (combinedY > 230)
    //        {
    //            if (BobberData.PrintBobberRelativeDirection)
    //                Debug.Log("Right Side of boat");
    //        }
    //        else
    //        {
    //            if (ReelInBool)
    //                return;
    //
    //            CalcLineDistance();
    //
    //            if (BobberData.PrintBobberRelativeDirection)
    //                Debug.Log("Bobber is in Front of the boat");
    //            else
    //            {
    //                SlackDistance = SlackDistance + (LastKnownDistance - LineDistance);
    //                VC_QuickFind.PlayerRig.FishingLineScript.m_Tension = getFishingLineSlack();
    //            }
    //        }
    //    }
    //}

    //Reeling

    //void ReelIn()
    //{
    //    bool OverRide = FishOverRide;
    //
    //    if (LineDistance < ReelData.ReelUpwardsDistance && !WithinReelUpDistance && !OverRide)
    //    {
    //        WithinReelUpDistance = true;
    //        VC_QuickFind.SimpleRPGCam.target = VC_QuickFind.PlayerRig.CharacterTransform;
    //        if (VC_QuickFind.EnviroSky != null)
    //        {
    //            VC_QuickFind.EnviroSky.Player = VC_QuickFind.PlayerRig.gameObject;
    //            VC_QuickFind.RainDropHandler.Player = VC_QuickFind.PlayerRig.transform;
    //        }
    //        VC_QuickFind.PlayerRig.PlayerCam.enabled = false;
    //        FishHooked = false;
    //        FishOverRide = false;
    //        VC_QuickFind.PlayerRig.FakeLight.SetActive(false);
    //    }
    //
    //    if (WithinReelUpDistance && !OverRide)
    //    {
    //        if (LineDistance < ReelData.BobberIsFullyInDistance)
    //            BobberFullyIn();
    //    }
    //
    //    bool UsingSlack = false;
    //    if (SlackDistance > 0 && !OverRide) //Use Up spare Slack
    //    {
    //        SlackDistance = SlackDistance - (CurrentGear.BobberPullRate * Time.deltaTime);
    //        VC_QuickFind.PlayerRig.FishingLineScript.m_Tension = getFishingLineSlack();
    //        UsingSlack = true;
    //    }
    //
    //    if (!UsingSlack || Pulling) //If there is no Slack, then start Realing towards fishing tip.
    //    {
    //        BobberData.Bobber.LookAt(VC_QuickFind.PlayerRig.FishingRodTip);
    //        Vector3 Direction = BobberData.Bobber.forward;
    //
    //        //Apply Fishing Pull Direction
    //        if (FishingState == 2)
    //            Direction.z = Direction.z + TurnValue * Time.deltaTime;
    //
    //        //Vector3 NewPos = Bobber.position + (Direction * BobberPullRate * Time.deltaTime);
    //
    //        if (!UsingSlack && !OverRide)
    //            LineDistance = LineDistance - ((CurrentGear.BobberPullRate - FishPullRate) * Time.deltaTime);
    //        else
    //            LineDistance = LineDistance + (FishPullRate * Time.deltaTime);
    //
    //        GUI.LineDistanceText.text = "Remaining Distance     " + (SlackDistance + LineDistance).ToString("F1");
    //        LastKnownDistance = LineDistance;
    //        Vector3 NewPos = VC_QuickFind.PlayerRig.FishingRodTip.position + (-Direction * LineDistance);
    //
    //        BobberData.Bobber.position = NewPos;
    //        if (WithinReelUpDistance)
    //        {
    //            Vector3 newVec = BobberData.Bobber.position - new Vector3(VC_QuickFind.PlayerRig.FishingRodTip.position.x, BobberData.Bobber.position.y, VC_QuickFind.PlayerRig.FishingRodTip.position.z);
    //            newVec.Normalize();
    //            NewPos = BobberData.Bobber.position - (newVec * CurrentGear.BobberPullRate * Time.deltaTime);
    //            BobberData.Bobber.position = NewPos;
    //        }
    //    }
    //}

    //void UpdateBobberVertical()
    //{
    //    if (WithinReelUpDistance)
    //    {
    //        VC_QuickFind.PlayerRig.FishingLineEndPoint.position = BobberModel.position;
    //        return;
    //    }
    //
    //    RaycastHit NewRayInfo = new RaycastHit();
    //    Ray Ray = new Ray(BobberData.Bobber.position + new Vector3(0, 100, 0), Vector3.down);
    //
    //    if (Physics.Raycast(Ray, out NewRayInfo))
    //    {
    //        if (NewRayInfo.collider.gameObject.layer != BobberData.BobberInteraction) //Still Over Water
    //            SetOnWater();
    //        else
    //        {
    //            float SuimonoHeight = VC_QuickFind.PlayerRig.SuimonoModule.SuimonoGetHeight(BobberData.Bobber.position, "height");
    //
    //            if (NewRayInfo.point.y < SuimonoHeight)
    //                SetOnWater();
    //            else
    //                SetOnLand(NewRayInfo);
    //        }
    //    }
    //}

    //void SetOnWater()
    //{
    //    //Set Proper Vertical with Water Height.
    //    Vector3 NewPos = new Vector3(BobberData.Bobber.position.x, (VC_QuickFind.PlayerRig.SuimonoModule.SuimonoGetHeight(BobberData.Bobber.position, "height") - BobberData.BobberHeight), BobberData.Bobber.position.z);
    //    Vector3 SlerpPoint = Vector3.Slerp(BobberData.Bobber.position, NewPos, BobberData.BobberBobSpeed * Time.deltaTime);
    //    BobberData.Bobber.position = SlerpPoint;
    //    VC_QuickFind.PlayerRig.FishingLineEndPoint.position = BobberModel.position;
    //    BobberData.Bobber.LookAt(VC_QuickFind.PlayerRig.FishingRodTip);
    //}

    //void SetOnLand(RaycastHit NewRayInfo)
    //{
    //    //Set Proper Vertical with Land Height.
    //    Vector3 NewPos = new Vector3(BobberData.Bobber.position.x, NewRayInfo.point.y + BobberData.BobberHeight, BobberData.Bobber.position.z);
    //    BobberData.Bobber.position = NewPos;
    //    VC_QuickFind.PlayerRig.FishingLineEndPoint.position = BobberModel.position;
    //    BobberData.Bobber.LookAt(VC_QuickFind.PlayerRig.FishingRodTip);
    //
    //    VC_QuickFind.PlayerRig.FishingLineScript.m_UseSegmentsByDistance = true;
    //}

    //Camera

    //void UpdateCamZoomPoint()
    //{
    //    if (UpdateCamState == 1)
    //    {
    //        if (VC_QuickFind.SimpleRPGCam.distance < Camera.FishingZoomLevel)
    //        {
    //            VC_QuickFind.SimpleRPGCam.distance = VC_QuickFind.SimpleRPGCam.distance + .6f;
    //        }
    //        else
    //        {
    //            VC_QuickFind.SimpleRPGCam.distance = Camera.FishingZoomLevel;
    //            UpdateCamZoom = false;
    //        }
    //    }
    //    if (UpdateCamState == 2)
    //    {
    //        if (VC_QuickFind.SimpleRPGCam.distance > StoredRPGCamZoom)
    //        {
    //            VC_QuickFind.SimpleRPGCam.distance = VC_QuickFind.SimpleRPGCam.distance - .6f;
    //        }
    //        else
    //        {
    //            VC_QuickFind.SimpleRPGCam.distance = StoredRPGCamZoom;
    //            UpdateCamZoom = false;
    //            VC_QuickFind.SimpleRPGCam.allowZoom = true;
    //            this.enabled = false;
    //        }
    //    }
    //}

    //void UpdateCamAngle()
    //{
    //    Vector2 CamAngle = VC_QuickFind.SimpleRPGCam._angle;
    //
    //    if (CamAngle.y == Camera.CastCameraAngleY)
    //        return;
    //
    //    if (CamAngle.y > Camera.CastCameraAngleY)
    //    {
    //        CamAngle.y = CamAngle.y - .3f;
    //
    //        if (CamAngle.y < Camera.CastCameraAngleY)
    //            CamAngle.y = Camera.CastCameraAngleY;
    //    }
    //    else
    //    {
    //        CamAngle.y = CamAngle.y + .3f;
    //
    //        if (CamAngle.y > Camera.CastCameraAngleY)
    //            CamAngle.y = Camera.CastCameraAngleY;
    //    }
    //
    //    VC_QuickFind.SimpleRPGCam._angle = CamAngle;
    //
    //}

    //Util

    //void CalcLineDistance()
    //{
    //    LineDistance = Vector3.Distance(VC_QuickFind.PlayerRig.FishingRodTip.position, BobberData.Bobber.transform.position);
    //    GUI.LineDistanceText.text = "Remaining Distance     " + (SlackDistance + LineDistance).ToString("F1");
    //}

    //void CalcSlackRemaining()
    //{
    //    float SlackNum = CurrentGear.MaxLineDistance - (SlackDistance + LineDistance);
    //
    //    if (SlackNum < 0)
    //    {
    //        LineDistance = CurrentGear.MaxLineDistance;
    //        GUI.ReelRemainingLineText.text = "Reel     " + "0";
    //        ZeroLineLeft();
    //        return;
    //    }
    //
    //    GUI.ReelRemainingLineText.text = "Reel     " + (CurrentGear.MaxLineDistance - (SlackDistance + LineDistance)).ToString("F1");
    //}

    //float getFishingLineSlack()
    //{
    //    float value = 0;
    //
    //    if (SlackDistance < 0)
    //        value = 1f;
    //    else if (SlackDistance > 1)
    //        value = 0f;
    //    else
    //        value = 1 - SlackDistance;
    //
    //    float SlackText = SlackDistance;
    //    if (SlackText < 0)
    //        SlackText = 0;
    //
    //    return value;
    //}

    //void UpdateTensionSlider()
    //{
    //    if (!FishHooked)
    //        return;
    //
    //    if (TensionMultiplier == 0)
    //    {
    //        Tension = Tension - (CurrentGear.TensionRestoreRate * Time.deltaTime);
    //        if (Tension < 0)
    //            Tension = 0;
    //        if (Tension > CurrentGear.MaxTension)
    //        {
    //            Tension = CurrentGear.MaxTension;
    //            TensionOverload();
    //        }
    //        GUI.TensionSlider.value = Tension / CurrentGear.MaxTension;
    //
    //        GUI.LineTensionText.text = "Line Tension     " + Tension.ToString("F1");
    //    }
    //    else
    //    {
    //        Tension = Tension + ((CurrentGear.TensionRate * TensionMultiplier) * Time.deltaTime);
    //        if (Tension > CurrentGear.MaxTension)
    //        {
    //            Tension = CurrentGear.MaxTension;
    //            TensionOverload();
    //        }
    //        GUI.TensionSlider.value = Tension / CurrentGear.MaxTension;
    //
    //        GUI.LineTensionText.text = "Line Tension     " + Tension.ToString("F1");
    //    }
    //}

    //DynamicData

    //void FetchCurrentGearValues()
    //{
    //    CurrentGear.BobberPullRate = VC_QuickFind.MainPlayerData.MainHandSlot.UnityReference.GetComponent<VC_Inventory.Inventory_FishingRod>().PullRate;
    //    CurrentGear.MaxCastForce = VC_QuickFind.MainPlayerData.MainHandSlot.UnityReference.GetComponent<VC_Inventory.Inventory_FishingRod>().MaxCastForce;
    //
    //    CurrentGear.TensionRate = VC_QuickFind.MainPlayerData.OffHandSlot.UnityReference.GetComponent<VC_Inventory.Inventory_Line>().TensionRate;
    //    CurrentGear.MaxTension = VC_QuickFind.MainPlayerData.OffHandSlot.UnityReference.GetComponent<VC_Inventory.Inventory_Line>().MaxTension;
    //    CurrentGear.TensionRestoreRate = VC_QuickFind.MainPlayerData.OffHandSlot.UnityReference.GetComponent<VC_Inventory.Inventory_Line>().TensionRestoreRate;
    //    CurrentGear.MaxLineDistance = VC_QuickFind.MainPlayerData.OffHandSlot.UnityReference.GetComponent<VC_Inventory.Inventory_Line>().MaxLineDistance;
    //}

    //void FetchFishValues()
    //{
    //    CurrentFish.FishWeight = CurrentFish.FishCaught.GetComponent<VC_Inventory.Inventory_Fish>().CaptureData.FishWeight;
    //    CurrentFish.FishTotalEnergy = CurrentFish.FishCaught.GetComponent<VC_Inventory.Inventory_Fish>().CaptureData.FishTotalEnergy;
    //    CurrentFish.DrainRate = CurrentFish.FishCaught.GetComponent<VC_Inventory.Inventory_Fish>().CaptureData.EnergyDrainRate;
    //    CurrentFish.FishPullRate = CurrentFish.FishCaught.GetComponent<VC_Inventory.Inventory_Fish>().CaptureData.FishPullRate;
    //}

    //Fishing

    //void FishingUpdate()
    //{
    //    if (FishingState == 0)
    //        return;
    //    if (!UpdateFishing)
    //        return;
    //
    //    if (FishingState == 1)
    //    {
    //        if (TimeRemaining > 0)
    //            TimeRemaining = TimeRemaining - Time.deltaTime;
    //        else
    //        {
    //            CurrentFish.FishCaught = DiceRollScript.FishTypeRoll(isDay);
    //            CurrentFish.FishObjectScript = CurrentFish.FishCaught.GetComponent<VC_Inventory.Inventory_Object>();
    //            Debug.Log("FISH TRIGGERED! " + CurrentFish.FishObjectScript.ObjectName);
    //
    //            FetchFishValues();
    //
    //            VC_Inventory.Inventory_Fish FishScript = CurrentFish.FishCaught.GetComponent<VC_Inventory.Inventory_Fish>();
    //            GameObject FishPrefab = GameObject.Instantiate(FishScript.ModelReference);
    //            FishPrefab.transform.SetParent(VC_QuickFind.PlayerRig.FishContainer);
    //            FishPrefab.transform.localPosition = Vector3.zero;
    //            FishPrefab.transform.localEulerAngles = Vector3.zero;
    //
    //
    //            FishEnergy = CurrentFish.FishTotalEnergy;
    //            GUI.FishEnergyText.text = "Fish Energy     " + FishEnergy.ToString("F1");
    //            FishHooked = true;
    //
    //            GUI.FishCaughtRelay.SetActive(true);
    //
    //            if (VC_QuickFind.MainPlayerData.FishIsKnown(CurrentFish.FishCaught))
    //                GUI.FishNameText.text = CurrentFish.FishObjectScript.ObjectName;
    //            else
    //                GUI.FishNameText.text = "Unknown fish ?????";
    //            GUI.FishNameText.enabled = true;
    //
    //            FishingState = 2;
    //        }
    //    }
    //    if (FishingState == 2)
    //    {
    //        if (Pulling) //Fish is Pulling.
    //        {
    //            if (PullTimer > 0)
    //                PullTimer = PullTimer - Time.deltaTime;
    //            else
    //            {
    //                Pulling = false;
    //                PullTimer = DiceRollScript.FishPullTimeRoll(WaitTimes.PullMinBase, WaitTimes.PullMaxBase, false);
    //            }
    //
    //            if (TurnTimer > 0)
    //                TurnTimer = TurnTimer - Time.deltaTime;
    //            else
    //            {
    //                bool EnergyLeft = true;
    //                if (FishEnergy < 0)
    //                    EnergyLeft = false;
    //
    //                TurnValue = DiceRollScript.FishPullTimeRoll(WaitTimes.SidePullMin, WaitTimes.SidePullMax, false);
    //
    //                float Mult = 1;
    //                if (TurnValue > WaitTimes.SidePullMax / 2 || TurnValue < WaitTimes.SidePullMin / 2)  //if Fish is pulling harder, go for shorter time period.
    //                    Mult = .5f;
    //
    //                if (!EnergyLeft)
    //                    Mult = Mult * .8f;
    //
    //                TurnTimer = DiceRollScript.FishPullTimeRoll(WaitTimes.SidePullWaitMin * Mult, WaitTimes.SidePullWaitMax * Mult, false);
    //
    //            }
    //        }
    //        else //Fish is Waiting.
    //        {
    //            if (PullTimer > 0)
    //                PullTimer = PullTimer - Time.deltaTime;
    //            else
    //            {
    //                Pulling = true;
    //                PullTimer = DiceRollScript.FishPullTimeRoll(WaitTimes.PullMinBase, WaitTimes.PullMaxBase, true);
    //            }
    //        }
    //    }
    //}

    //void ActivateFishing()
    //{
    //    Debug.Log("Fishing MiniGameActivated - NOTE.  Using 2 second wait time.");
    //
    //    float minTime = 2f;
    //    float MaxTime = 2f;
    //
    //
    //    FishingState = 1;
    //    TimeRemaining = DiceRollScript.FishTimerWaitRoll(minTime, MaxTime);
    //
    //    UpdateFishing = true;
    //}

    //void IsReeling(bool isTrue)
    //{
    //    if (FishingState != 1)
    //        return;
    //
    //    if (isTrue)
    //        UpdateFishing = false;
    //    else
    //        UpdateFishing = true;
    //}

    //void UpdateFishPullMechanics(bool ReelInBool)
    //{
    //    if (!FishHooked)
    //        return;
    //
    //    FishOverRide = false;
    //    if (ReelInBool && FishEnergy > 0)
    //        TensionMultiplier = .3f * CurrentFish.FishWeight / 10;
    //    else if (ReelInBool)
    //        TensionMultiplier = .1f * CurrentFish.FishWeight / 10;
    //    else
    //        TensionMultiplier = 0;
    //
    //    if (FishEnergy < 0) // Fish has No Energy left.
    //    {
    //        GUI.FishEnergyText.text = "Fish Energy     " + "0";
    //        FishPullRate = (CurrentFish.FishWeight / 10) / 2;
    //        UpdateFishWobble(.3f);
    //        return;
    //    }
    //
    //    if (!Pulling) // Fish is resting.
    //    {
    //        FishPullRate = CurrentFish.FishWeight / 10;
    //        if (ReelInBool)
    //            TensionMultiplier = 1f * CurrentFish.FishWeight / 10;
    //        UpdateFishWobble(1.2f);
    //    }
    //    else
    //    {
    //        if (ReelInBool)
    //        {
    //            FishEnergy = FishEnergy - (CurrentFish.DrainRate * Time.deltaTime);
    //            FishPullRate = CurrentFish.FishPullRate + (CurrentFish.FishWeight / 20);
    //            TensionMultiplier = 2f * CurrentFish.FishWeight / 10;
    //
    //            GUI.FishEnergyText.text = "Fish Energy     " + FishEnergy.ToString("F1");
    //            UpdateFishWobble(2f);
    //        }
    //        else
    //        {
    //            FishEnergy = FishEnergy - (CurrentFish.DrainRate * Time.deltaTime);
    //            FishPullRate = CurrentFish.FishPullRate;
    //            FishOverRide = true;
    //            TensionMultiplier = .3f * CurrentFish.FishWeight / 10;
    //
    //            GUI.FishEnergyText.text = "Fish Energy     " + FishEnergy.ToString("F1");
    //            UpdateFishWobble(1.5f);
    //        }
    //
    //        if (FishEnergy < 0)
    //            TurnValue = TurnValue * .8f;
    //
    //
    //    }
    //}

    //void UpdateFishWobble(float Multiplier)
    //{
    //    Transform T = VC_QuickFind.PlayerRig.FishContainer.transform;
    //    float Value = WaitTimes.WobbleSpeed * Multiplier * Time.deltaTime;
    //    float YVal = T.localEulerAngles.y;
    //    if (YVal > 180)
    //        YVal = YVal - 360;
    //
    //    if (FishRotateRight)
    //    {
    //        Value = YVal + Value;
    //        if (Value > WaitTimes.WobbleDistance)
    //            FishRotateRight = false;
    //    }
    //    else
    //    {
    //        Value = YVal - Value;
    //        if (Value < -WaitTimes.WobbleDistance)
    //            FishRotateRight = true;
    //    }
    //
    //    Vector3 NewRotation = new Vector3(0, Value, 0);
    //    VC_QuickFind.PlayerRig.FishContainer.transform.localEulerAngles = NewRotation;
    //}

    //Failure Events

    //void ZeroLineLeft()
    //{
    //    Tension = Tension + (1 * Time.deltaTime);
    //}

    //void TensionOverload()
    //{
    //    Debug.Log("Tension Overload");
    //
    //    VC_QuickFind.PlayerRig.FishingLineScript.m_Tension = 0;
    //    CurrentState = 7;
    //    VC_QuickFind.SimpleRPGCam.target = null;
    //    Timer = WaitTimes.PeelAwayTime;
    //    GUI.LineBreakRelay.SetActive(true);
    //
    //    BreakGear();
    //}

    //void BreakAwayLogic()
    //{
    //    Timer = Timer - Time.deltaTime;
    //    if (Timer > 0)
    //    {
    //        BobberData.Bobber.LookAt(VC_QuickFind.PlayerRig.FishingRodTip);
    //        Vector3 Direction = BobberData.Bobber.forward;
    //
    //        LineDistance = LineDistance + (FishPullRate * Time.deltaTime);
    //
    //        LastKnownDistance = LineDistance;
    //        Vector3 NewPos = VC_QuickFind.PlayerRig.FishingRodTip.position + (-Direction * LineDistance);
    //        BobberData.Bobber.position = NewPos;
    //
    //        VC_QuickFind.PlayerRig.FishingLineEndPoint.position = BobberModel.position;
    //        BobberData.Bobber.LookAt(VC_QuickFind.PlayerRig.FishingRodTip);
    //    }
    //    else //Breakaway Finished.
    //    {
    //        WithinReelUpDistance = true;
    //        VC_QuickFind.SimpleRPGCam.target = VC_QuickFind.PlayerRig.CharacterTransform;
    //        if (VC_QuickFind.EnviroSky != null)
    //        {
    //            VC_QuickFind.EnviroSky.Player = VC_QuickFind.PlayerRig.gameObject;
    //            VC_QuickFind.RainDropHandler.Player = VC_QuickFind.PlayerRig.transform;
    //        }
    //        VC_QuickFind.PlayerRig.PlayerCam.enabled = false;
    //        FishHooked = false;
    //        FishOverRide = false;
    //
    //        CurrentFish.FishCaught = null;
    //
    //        BobberFullyIn();
    //    }
    //}


    //End Fishing

    //void BobberFullyIn()
    //{
    //    Debug.Log("Bobber is In");
    //
    //    VC_QuickFind.PlayerRig.CharacterAnimator.SetBool("Fishing Reel", false);
    //    VC_QuickFind.PlayerRig.FishingLineScript.gameObject.SetActive(false);
    //    VC_QuickFind.PlayerRig.IKEnable(true, false, true);
    //
    //    GUI.LineDistanceText.enabled = false;
    //    GUI.LineTensionText.enabled = false;
    //    GUI.ReelRemainingLineText.enabled = false;
    //    GUI.FishEnergyText.enabled = false;
    //
    //
    //    BobberModel.gameObject.SetActive(false);
    //    GUI.TensionSlider.gameObject.SetActive(false);
    //    GUI.FishNameText.enabled = false;
    //
    //    FishPullRate = 0;
    //    CurrentState = 6;
    //
    //    if (CurrentFish.FishCaught != null)
    //        AddFishToInventory();
    //}

    //void AddFishToInventory()
    //{
    //    GUI.FishCaughtText.gameObject.SetActive(true);
    //
    //    LowerBaitCount();
    //
    //    VC_Inventory.Inventory_Object InventoryObjectScript = CurrentFish.FishCaught.GetComponent<VC_Inventory.Inventory_Object>(); //Master Item Database Reference
    //
    //    GUI.CaughtGridSlot.SlotDisplayImage.sprite = InventoryObjectScript.ObjectSpriteRef;
    //    GUI.FishCaughtTitle.text = InventoryObjectScript.ObjectName;
    //    GUI.FishCaughtDescriptionText.text = InventoryObjectScript.ObjectDescription;
    //
    //    GUI.FishCaughtDescription.SetActive(true);
    //
    //    VC_QuickFind.MainPlayerData.MarkAsKnownFish();
    //    VC_QuickFind.ItemDropHandler.AddItemToInventory(CurrentFish.FishCaught);
    //    Debug.Log("Add Fish to Inventory");
    //    CurrentFish.FishCaught = null;
    //}

    //Gear

    //void BreakGear()
    //{
    //    VC_Inventory.Inventory_Slot OffHandSlotRef = VC_QuickFind.MainPlayerData.OffHandSlot;
    //    VC_Inventory.Inventory_Slot AccessorySlotRef = VC_QuickFind.MainPlayerData.AccessorySlot;
    //
    //    VC_QuickFind.InventoryScreen.ClearSlot(OffHandSlotRef);
    //    VC_QuickFind.InventoryScreen.ClearGUISlot(VC_QuickFind.InventoryScreen.OffHandSlot);
    //
    //    if (AccessorySlotRef.StackCount > 1)
    //        LowerBaitCount();
    //    else
    //    {
    //        VC_QuickFind.InventoryScreen.ClearSlot(AccessorySlotRef);
    //        VC_QuickFind.InventoryScreen.ClearGUISlot(VC_QuickFind.InventoryScreen.AccessorySlot);
    //    }
    //}

    //void LowerBaitCount()
    //{
    //    VC_Inventory.Inventory_Slot AccessorySlotRef = VC_QuickFind.MainPlayerData.AccessorySlot;
    //
    //    if (!AccessorySlotRef.isStack)
    //        return;
    //
    //    AccessorySlotRef.StackCount--;
    //
    //    if (AccessorySlotRef.StackCount == 0)
    //    {
    //        VC_QuickFind.InventoryScreen.ClearSlot(AccessorySlotRef);
    //        VC_QuickFind.InventoryScreen.ClearGUISlot(VC_QuickFind.InventoryScreen.AccessorySlot);
    //    }
    //}
}

