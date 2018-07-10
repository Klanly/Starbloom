using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class DG_AIEntityDetection : MonoBehaviour
{

    #region Variables


    [System.Serializable]
    public class DetectionOptions
    {
        public DetectionBehaviour DefaultDetectionBehaviour;
        public DetectionBehaviour DebugDetectionBehaviour;

        public float SightAngle = 45;
        public float SightRange = 10;
        public float HearingRadius = 5;
        public float DetectionUpdateRate = .5f;
    }


    //Detection
    public enum DetectionBehaviour
    {
        InitialLoad,

        Detect_NotDetecting,
        Detect_VisualOnly,
        Detect_HearingOnly,
        Detect_Either,
        Detect_Global,

        Detect_ObjectHasLeftAggroRange
    }


    //Detection
    [ReadOnly] public DetectionBehaviour CurrentDetectionBehaviour;
    [ReadOnly] public Transform DetectedTarget;

    [System.NonSerialized] public DG_AIEntity Entity;
    [System.NonSerialized] public DetectionOptions DetectionSettings;
    float ScanTimer;

    #endregion

    private void Awake()
    {
        this.enabled = false;
    }

    private void InitialLoad()
    {
        CurrentDetectionBehaviour = DetectionSettings.DefaultDetectionBehaviour;
        ScanTimer = Random.Range(0, DetectionSettings.DetectionUpdateRate);
    }

    #region Detection


    private void Update()
    {
        if (CurrentDetectionBehaviour == DetectionBehaviour.InitialLoad) InitialLoad();
        if (!Entity.CheckIfYouAreOwner()) return;
        if (CurrentDetectionBehaviour != DetectionBehaviour.Detect_NotDetecting) { ScanTimer -= Time.deltaTime; if (ScanTimer < 0) { GetNextDetection(); } }
    }

    void GetNextDetection()
    {
        if (CurrentDetectionBehaviour == DetectionBehaviour.Detect_NotDetecting) return;
        if (DetectedTarget == null)
        {
            bool TargetNotFound = true;
            if (CurrentDetectionBehaviour == DetectionBehaviour.Detect_ObjectHasLeftAggroRange) TargetNotFound = false;

            if (TargetNotFound) TargetNotFound = ScanThroughPlayers();
            ScanTimer = DetectionSettings.DetectionUpdateRate;
        }
        //drop target if out of range?
    }
    bool ScanThroughPlayers()
    {
        if (CurrentDetectionBehaviour == DetectionBehaviour.Detect_Global) { SetDetectedTarget(QuickFind.GetClosestCharacter(Entity._T.position, float.MaxValue)); return false; }

        for (int i = 0; i < QuickFind.NetworkSync.UserList.Count; i++) 
        {
            if (QuickFind.NetworkSync.UserList[i].CharacterLink == null) continue;
            Transform PlayerChar = QuickFind.NetworkSync.UserList[i].CharacterLink._T;
            if (CurrentDetectionBehaviour != DetectionBehaviour.Detect_VisualOnly)
            { if (QuickFind.WithinDistance(Entity._T, PlayerChar, DetectionSettings.HearingRadius)) { SetDetectedTarget(PlayerChar); return false; } }
            if (CurrentDetectionBehaviour != DetectionBehaviour.Detect_HearingOnly)
            { if (QuickFind.TargetCanSeeOtherTarget(Entity._T, PlayerChar, DetectionSettings.SightAngle, DetectionSettings.SightRange)) { SetDetectedTarget(PlayerChar); return false; } }
        }
        return true;
    }

    void SetDetectedTarget(Transform NewTarget)
    {
        DetectedTarget = NewTarget;
    }


    #endregion

    #region Set Values
    public void SetDetectionSettings(DetectionOptions Options) { DetectionSettings = Options; }
    void ChangeDetectionBehaviourState(DetectionBehaviour Behaviour) { CurrentDetectionBehaviour = Behaviour; GetNextDetection(); }

    [Button(ButtonSizes.Small)] [HideInEditorMode] public void SetToDebugDetectionState() { ChangeDetectionBehaviourState(DetectionSettings.DebugDetectionBehaviour); }

    #endregion
}
