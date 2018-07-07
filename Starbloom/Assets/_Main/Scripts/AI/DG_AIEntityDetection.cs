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
        public float SightAngle = 45;
        public float SightRange = 10;
        public float HearingRadius = 5;
        public float DetectionUpdateRate = .5f;
    }


    //Detection
    public enum DetectionBehaviour
    {
        Detect_NotDetecting,
        Detect_VisualOnly,
        Detect_HearingOnly,
        Detect_Either,
        Detect_Global,

        Detect_ObjectHasLeftAggroRange
    }


    DG_AIEntity Entity;

    [Header("Detection")]
    public DetectionBehaviour StartDetectionBehaviour;

    [ReadOnly] public DetectionBehaviour CurrentDetectionBehaviour;
    [ReadOnly] public Transform DetectedTarget;

    [System.NonSerialized] public DetectionOptions DetectionSettings;
    float ScanTimer;

    Transform _T;

    #endregion

    private void InitialLoad()
    {
        _T = transform;
        Entity = transform.GetComponent<DG_AIEntity>();
        CurrentDetectionBehaviour = StartDetectionBehaviour;
        ScanTimer = Random.Range(0, DetectionSettings.DetectionUpdateRate);
    }

    #region Detection


    private void Update()
    {
        if (_T == null) InitialLoad();
        if (!Entity.CheckIfYouAreOwner()) return;
        if (CurrentDetectionBehaviour != DetectionBehaviour.Detect_NotDetecting) { ScanTimer -= Time.deltaTime; if (ScanTimer < 0) { GetNextDetection(); } }
    }

    void GetNextDetection()
    {
        if (DetectedTarget == null)
        {
            bool NeedTarget = true;
            if (CurrentDetectionBehaviour == DetectionBehaviour.Detect_ObjectHasLeftAggroRange) NeedTarget = false;

            if (NeedTarget) NeedTarget = ScanThroughPlayers();
            ScanTimer = DetectionSettings.DetectionUpdateRate;
        }
        //drop target if out of range?
    }
    bool ScanThroughPlayers()
    {
        for (int i = 0; i < QuickFind.NetworkSync.UserList.Count; i++)
        {
            Transform PlayerChar = QuickFind.NetworkSync.UserList[i].CharacterLink._T;
            if (CurrentDetectionBehaviour != DetectionBehaviour.Detect_VisualOnly)
            { if (QuickFind.WithinDistance(_T, PlayerChar, DetectionSettings.HearingRadius)) { SetDetectedTarget(PlayerChar); return false; } }
            if (CurrentDetectionBehaviour != DetectionBehaviour.Detect_HearingOnly)
            {
                Vector3 targetDir = PlayerChar.position - _T.position;
                float angle = Vector3.Angle(targetDir, _T.forward);
                if (angle < DetectionSettings.SightAngle)
                    if (QuickFind.WithinDistance(_T, PlayerChar, DetectionSettings.SightRange)) { SetDetectedTarget(PlayerChar); return false; }
            }
        }
        return true;
    }

    void SetDetectedTarget(Transform NewTarget)
    {
        DetectedTarget = NewTarget;
        Debug.Log("Player Detected");
    }


    #endregion

    #region Set Values
    public void SetDetectionSettings(DetectionOptions Options) { DetectionSettings = Options; }
    void ChangeDetectionBehaviourState(DetectionBehaviour Behaviour) { CurrentDetectionBehaviour = Behaviour; GetNextDetection(); }

    [Button(ButtonSizes.Small)] public void DebugShiftDetectionState() { ChangeDetectionBehaviourState(StartDetectionBehaviour); }

    #endregion
}
