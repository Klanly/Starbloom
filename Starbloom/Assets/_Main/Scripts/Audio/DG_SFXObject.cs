using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class DG_SFXObject : MonoBehaviour {

    public enum GreaterThan1SFXItemLogic
    {
        PlayRandom,
        PlaySequential,
        PlayAllAtOnce,
        PlayAllInOrder
    }

    [HideInInspector] public int DatabaseID;
    [HideInInspector] public bool LockItem;
    public string Name;
    public DG_AudioManager.AudioFXZoneTypes[] AllowedFXZones;

    [Header("If More than 1 Piece")]
    public GreaterThan1SFXItemLogic ItemLogic;

    [Header("Pieces")]
    public SFXItem[] SFXPieces;

    [HideInInspector] public int Index = 0;


    [System.Serializable]
    public class SFXItem
    {
        public string SFXName;
        public int PrefabID;
        public float Volume = 1;
        [Header("Pitch Options")]
        public float PitchValue = 1;
        public bool RandomizePitchWithinRangeOfValue;
        public float RandomPitchRange;
    }


    [Button()] public void TriggerSFX() { if (!Application.isPlaying) return; QuickFind.AudioManager.PlaySoundEffect(DatabaseID, transform.position); }
}
