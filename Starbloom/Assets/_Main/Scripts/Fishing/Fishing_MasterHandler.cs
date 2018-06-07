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
        WaitForUpTrigger,
    }

    public enum WaterTypes
    {
        River,
        Lake,
        Ocean,
        All
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
    public bool QuickHookFish;
    public bool QuickCatchFish; 
    [Header("Temp Fish Movement")]
    public float MoveUpFish;


    [Button(ButtonSizes.Small)] public void ResetFishingState() { CurrentState = FishingStates.Idle; }
    float CameraInitialPosition;

    //ActiveFish
    DG_FishingRoller.FishRollValues ActiveFishReference;




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
        if (CurrentState == FishingStates.Idle && !isUp) { SetAnimationState("Casting"); CurrentState = FishingStates.SearchingForCast; QuickFind.InputController.InputState = DG_PlayerInput.CurrentInputState.PerformingAction; }
        if (CurrentState == FishingStates.SearchingForCast) { if (FishableAreaIsPossible()) { AdjustCamera(true); CastPower = 0; CurrentState = FishingStates.CastCharging; } else if (isUp) { SetAnimationState("Idle"); CurrentState = FishingStates.Idle; QuickFind.InputController.InputState = DG_PlayerInput.CurrentInputState.Default; } }
        if (CurrentState == FishingStates.CastCharging) { CastPower = CalculateCastPower(); if (isUp) { PlayCastEffect(); EndPoint = CalcEndPoint();  CastTimer = CastTime; CurrentState = FishingStates.Casting; } }
        if (CurrentState == FishingStates.AwaitingFish) { SetAnimationState("PullingInBobber"); CurrentState = FishingStates.PullingInBobber; AdjustCamera(false); }
        if (CurrentState == FishingStates.FishEventTriggered) { if (PrintDebug) Debug.Log("Fish Hook Successful"); SetAnimationState("Reeling"); HookedSuccessfulDisplayTimer = HookedSuccessfulDisplayTime; QuickFind.FishingGUI.DisplayHookedText(BobberModel.transform); CurrentState = FishingStates.HitDisplay; }
        if (CurrentState == FishingStates.Reeling) ReelingHeld = true;
        if (CurrentState == FishingStates.WaitForUpTrigger) { if (isUp) { CloseCaughtFishWindow(); CurrentState = FishingStates.Idle; } }
    }

    //Internal Responses
    private void Update()
    {
        if (CurrentState == FishingStates.Casting) { if (isCastComplete()) { SetAnimationState("AwaitingFish"); QuickFind.FishingGUI.DisableEnergyBar(); CalculateNextFishEvent(); CurrentState = FishingStates.AwaitingFish; } }
        if (CurrentState == FishingStates.AwaitingFish) { if (DoWeTriggerFishEvent()) { QuickFind.FishingGUI.DisplayFishEventAlert(); CurrentState = FishingStates.FishEventTriggered; } }
        if (CurrentState == FishingStates.FishEventTriggered) { if (FishEventTimerRanTooLong()) CurrentState = FishingStates.Casting; }
        if (CurrentState == FishingStates.HitDisplay) { if (HitDisplayFinished()) { OpenFishingGUI(); CurrentState = FishingStates.Reeling; } }
        if (CurrentState == FishingStates.Reeling) { float Progress = ReelProgressBar(); if (Progress < 0 || Progress > 1) { PullInTimer = PullInTime; SetAnimationState("PullingInBobber"); CurrentState = FishingStates.PullingInBobber; AdjustCamera(false); } }
        if (CurrentState == FishingStates.PullingInBobber) { if (BobberPulledIn()) { SetAnimationState("Idle"); CurrentState = FishingStates.WaitForUpTrigger; } }
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
        ActiveFishReference = QuickFind.FishingRoller.GetNewFishRoll(DetectedWater);
        AwaitingFishTimer = ActiveFishReference.WaitTime;
        if (QuickHookFish) AwaitingFishTimer = .5f;
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
        float FishingBarSize = 100 + (QuickFind.SkillTracker.GetMySkillLevel(DG_SkillTracker.SkillTags.Fishing) * 20);
        QuickFind.FishingGUI.SetCaptureZoneSize(FishingBarSize);

        MaxFishHeight = QuickFind.FishingGUI.FishRegionMain.rect.height;
        MaxRegionHeight = MaxFishHeight - FishingBarSize;

        QuickFind.FishingGUI.EnableFishingGUI();
    }
    float ReelProgressBar()
    {
        CurrentReelValue = GetReelValueFactoringinReelHeight(ReelingHeld);
        bool WithinReelBounds = FishWithinReelbarBounds(ReelingHeld);
        if (ReelingHeld) ReelingHeld = false;

        if(WithinReelBounds) CurrentFishCaughtValue = CurrentFishCaughtValue + (FishCaughtSpeed * Time.deltaTime);
            else CurrentFishCaughtValue = CurrentFishCaughtValue - (FishCaughtSpeed * Time.deltaTime);

        if (QuickCatchFish) CurrentFishCaughtValue = CurrentFishCaughtValue + .2f;

        QuickFind.FishingGUI.SpinReel(WithinReelBounds);
        QuickFind.FishingGUI.SetFishCaughtProgress(CurrentFishCaughtValue);

        return CurrentFishCaughtValue;
    }





    //Actual Fishing Mechanics
    //////////////////////////////////////////////////////////////////////////

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
        FishPosition += MoveUpFish * Time.deltaTime;
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

    //////////////////////////////////////////////////////////////////////////




    bool BobberPulledIn()
    {
        PullInTimer = PullInTimer - Time.deltaTime;
        if (PullInTimer < 0)
        { if (PrintDebug) Debug.Log("Bobber Pulled In."); if (CurrentFishCaughtValue > 1) RewardPlayerWithCaughtFish(); return true; }
        else { return false; }
    }
    void RewardPlayerWithCaughtFish()
    {
        QuickFind.FishingGUI.Fish.anchoredPosition = Vector3.zero;
        QuickFind.FishingGUI.CaptureZonePos.anchoredPosition = Vector3.zero;
        CurrentFishCaughtValue = 0;
        CurrentReelValue = 0;

        int PlayerCharID = QuickFind.NetworkSync.PlayerCharacterID;

        DG_ItemObject ItemRef = QuickFind.ItemDatabase.GetItemFromID(ActiveFishReference.AtlasObject.ItemObjectRefDatabaseID);
        QuickFind.InventoryManager.AddItemToRucksack(PlayerCharID, ItemRef.DatabaseID, ActiveFishReference.QualityLevel, true);
        Sprite FishSprite = ItemRef.Icon;
        float DisplayWeight = (float)ActiveFishReference.Weight / 10;
        QuickFind.FishingGUI.OpenObjectCaughtGUI(FishSprite, QuickFind.WordDatabase.GetWordFromID(ItemRef.ToolTipType.MainLocalizationID), DisplayWeight.ToString());

        //Update Trackers
        DG_PlayerCharacters.RucksackSlot CurrentRucksackSlot = QuickFind.ItemActivateableHandler.CurrentRucksackSlot;
        QuickFind.SkillTracker.IncreaseFishingLevel(ActiveFishReference, (DG_ItemObject.ItemQualityLevels)CurrentRucksackSlot.CurrentStackActive);
        QuickFind.AcheivementTracker.CheckFishAchevement(ActiveFishReference);

        ActiveFishReference = null;
    }


    void CloseCaughtFishWindow()
    {
        QuickFind.InputController.InputState = DG_PlayerInput.CurrentInputState.Default;
        QuickFind.FishingGUI.CloseCaughtGui();
    }
}

